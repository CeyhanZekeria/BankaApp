using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace BankaApp
{
    public class AccountService
    {

        public AccountBalanceInfo GetAccountBalanceInfo(int accountId)
        {
            AccountBalanceInfo info = null;

            using (OracleConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT 
                a.ID_Account AS ID_Account,
                a.Account_NO AS Account_NO,
                a.Availibility AS Availibility,
                a.ID_Currency_type AS CurrencyId
            FROM Account a
            WHERE a.ID_Account = :accountId";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("accountId", OracleDbType.Int32).Value = accountId;

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            info = new AccountBalanceInfo
                            {
                                AccountId = Convert.ToInt32(reader["ID_Account"]),
                                AccountNo = reader["Account_NO"].ToString(),
                                Balance = Convert.ToDecimal(reader["Availibility"]),
                                CurrencyId = Convert.ToInt32(reader["CurrencyId"])
                            };
                        }
                    }
                }
            }

            return info;
        }

        public string LoadUserCVV(int appUserId)
        {
            string query = "SELECT CVV FROM App_User WHERE ID_User = :userId";

            object result = DatabaseHelper.ExecuteScalar(
                query,
                new OracleParameter("userId", OracleDbType.Int32) { Value = appUserId }
            );

            return result != null && result != DBNull.Value
                ? result.ToString()
                : "";
        }

        public DataTable LoadUserAccounts(int clientId)
        {
            string query = @"
                SELECT ID_Account, Account_NO
                FROM Account
                WHERE ID_Client = :clientId
                ORDER BY ID_Account";

            return DatabaseHelper.ExecuteDataTable(
                query,
                new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId }
            );
        }

        public string LoadCardHolderName(int clientId)
        {
            string query = "SELECT Name FROM Client WHERE Client_ID = :clientId";

            object result = DatabaseHelper.ExecuteScalar(
                query,
                new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId }
            );

            return result != null && result != DBNull.Value
                ? result.ToString().Trim()
                : "";
        }

        public DataTable LoadCardData(int appUserId)
        {
            string query = @"
        SELECT CARD_NUMBER, VALID_THRU
        FROM App_User
        WHERE ID_User = :appUserId";

            return DatabaseHelper.ExecuteDataTable(
                query,
                new OracleParameter("appUserId", OracleDbType.Int32) { Value = appUserId }
            );
        }
    }
}