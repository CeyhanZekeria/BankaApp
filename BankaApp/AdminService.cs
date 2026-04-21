using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace BankaApp
{
    public class AdminService
    {
        public DataTable SearchClients(string searchBy, string searchText)
        {
            string selected = (searchBy ?? "").Trim().ToLower();
            string value = "%" + searchText.Trim().ToLower() + "%";

            string query = @"
        SELECT DISTINCT
            c.Client_ID,
            c.Name,
            c.EGN,
            c.Identity_Type,
            c.Email,
            c.Phone_Number,
            c.Country,
            c.Is_Active
        FROM Client c
        LEFT JOIN App_User au
            ON LOWER(TRIM(c.Email)) = LOWER(TRIM(au.Email))
        LEFT JOIN Account a
            ON a.ID_Client = c.Client_ID
        WHERE 1 = 1 ";

            OracleParameter param = new OracleParameter("searchText", OracleDbType.Varchar2)
            {
                Value = value
            };

            if (selected == "name")
            {
                query += " AND LOWER(c.Name) LIKE :searchText ";
            }
            else if (selected == "egn/lnc")
            {
                query += " AND LOWER(c.EGN) LIKE :searchText ";
            }
            else if (selected == "email")
            {
                query += " AND (LOWER(c.Email) LIKE :searchText OR LOWER(au.Email) LIKE :searchText) ";
            }
            else if (selected == "phone")
            {
                query += " AND LOWER(c.Phone_Number) LIKE :searchText ";
            }
            else if (selected == "username")
            {
                query += " AND LOWER(au.Username) LIKE :searchText ";
            }
            else if (selected == "account no")
            {
                query += " AND LOWER(a.Account_NO) LIKE :searchText ";
            }
            else
            {
                query += " AND LOWER(c.Name) LIKE :searchText ";
            }

            query += " ORDER BY c.Name";

            return DatabaseHelper.ExecuteDataTable(query, param);
        }

        public DataTable GetClientById(int clientId)
        {
            string query = @"
                SELECT
                    Client_ID,
                    Name,
                    EGN,
                    Identity_Type,
                    Email,
                    Phone_Number,
                    Country,
                    Is_Active
                FROM Client
                WHERE Client_ID = :clientId";

            return DatabaseHelper.ExecuteDataTable(
                query,
                new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId }
            );
        }

        public DataTable GetClientAccounts(int clientId)
        {
            string query = @"
                SELECT
                    a.ID_Account,
                    a.Account_NO,
                    ct.Currency_Type_Name,
                    a.Interest,
                    a.Availibility
                FROM Account a
                JOIN Currency_Type ct ON a.ID_Currency_Type = ct.ID_Currency_Type
                WHERE a.ID_Client = :clientId
                ORDER BY a.ID_Account";

            return DatabaseHelper.ExecuteDataTable(
                query,
                new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId }
            );
        }

        public void BlockClient(int clientId)
        {
            string query = @"
                UPDATE Client
                SET Is_Active = 0
                WHERE Client_ID = :clientId";

            DatabaseHelper.ExecuteNonQuery(
                query,
                new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId }
            );
        }

        public void UnblockClient(int clientId)
        {
            string query = @"
                UPDATE Client
                SET Is_Active = 1
                WHERE Client_ID = :clientId";

            DatabaseHelper.ExecuteNonQuery(
                query,
                new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId }
            );
        }
        public DataTable GetAccountHistory(int accountId)
        {
            using (OracleConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("Get_Account_History", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_account_id", OracleDbType.Int32).Value = accountId;
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}