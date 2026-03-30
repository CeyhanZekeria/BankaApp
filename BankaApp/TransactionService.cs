using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace BankaApp
{
    public class TransactionService
    {
        public DataTable LoadFilteredTransactions(
            int clientId,
            DateTime dateFrom,
            DateTime dateTo,
            string type,
            int? accountId)
        {
            string query = @"
                SELECT 
                    tp.Type AS Type,
                    t.Sum_Amount AS Amount,
                    c.Currency_type_Name AS Currency,
                    t.Description AS Description,
                    t.Date_Tran AS Tran_Date
                FROM Transactions t
                JOIN Account a ON t.ID_Account = a.ID_Account
                JOIN Type tp ON t.ID_Type = tp.ID_Type
                JOIN Currency_type c ON t.ID_Currency_Type = c.ID_Currency_type
                WHERE a.ID_Client = :clientId
                  AND t.Date_Tran >= :dateFrom
                  AND t.Date_Tran < :dateTo";

            if (type != "All")
                query += " AND tp.Type = :type";

            if (accountId.HasValue)
                query += " AND a.ID_Account = :accountId";

            query += " ORDER BY t.Date_Tran DESC";

            if (type != "All" && accountId.HasValue)
            {
                return DatabaseHelper.ExecuteDataTable(
                    query,
                    new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId },
                    new OracleParameter("dateFrom", OracleDbType.Date) { Value = dateFrom.Date },
                    new OracleParameter("dateTo", OracleDbType.Date) { Value = dateTo.Date.AddDays(1) },
                    new OracleParameter("type", OracleDbType.Varchar2) { Value = type },
                    new OracleParameter("accountId", OracleDbType.Int32) { Value = accountId.Value }
                );
            }

            if (type != "All")
            {
                return DatabaseHelper.ExecuteDataTable(
                    query,
                    new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId },
                    new OracleParameter("dateFrom", OracleDbType.Date) { Value = dateFrom.Date },
                    new OracleParameter("dateTo", OracleDbType.Date) { Value = dateTo.Date.AddDays(1) },
                    new OracleParameter("type", OracleDbType.Varchar2) { Value = type }
                );
            }

            if (accountId.HasValue)
            {
                return DatabaseHelper.ExecuteDataTable(
                    query,
                    new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId },
                    new OracleParameter("dateFrom", OracleDbType.Date) { Value = dateFrom.Date },
                    new OracleParameter("dateTo", OracleDbType.Date) { Value = dateTo.Date.AddDays(1) },
                    new OracleParameter("accountId", OracleDbType.Int32) { Value = accountId.Value }
                );
            }

            return DatabaseHelper.ExecuteDataTable(
                query,
                new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId },
                new OracleParameter("dateFrom", OracleDbType.Date) { Value = dateFrom.Date },
                new OracleParameter("dateTo", OracleDbType.Date) { Value = dateTo.Date.AddDays(1) }
            );
        }
        public DataTable LoadRecentTransactions(int clientId, int? accountId)
        {
            string query = @"
        SELECT *
        FROM (
            SELECT 
                tp.Type AS Type,
                t.Sum_Amount AS Amount,
                c.Currency_type_Name AS Currency,
                t.Description AS Description,
                t.Date_Tran AS Tran_Date
            FROM Transactions t
            JOIN Account a ON t.ID_Account = a.ID_Account
            JOIN Type tp ON t.ID_Type = tp.ID_Type
            JOIN Currency_type c ON t.ID_Currency_Type = c.ID_Currency_type
            WHERE a.ID_Client = :clientId";

            if (accountId.HasValue)
                query += " AND a.ID_Account = :accountId";

            query += @"
            ORDER BY t.Date_Tran DESC
        )
        WHERE ROWNUM <= 10";

            if (accountId.HasValue)
            {
                return DatabaseHelper.ExecuteDataTable(
                    query,
                    new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId },
                    new OracleParameter("accountId", OracleDbType.Int32) { Value = accountId.Value }
                );
            }

            return DatabaseHelper.ExecuteDataTable(
                query,
                new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId }
            );
        }
    }

}