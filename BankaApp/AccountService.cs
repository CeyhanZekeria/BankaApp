using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace BankaApp
{
    public class AccountService
    {
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