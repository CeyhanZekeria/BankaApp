using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace BankaApp
{
    public class SendMoneyService
    {
        public int? GetAccountIdByAccountNo(string accountNo)
        {
            string query = @"
                SELECT ID_Account
                FROM Account
                WHERE TRIM(UPPER(Account_NO)) = TRIM(UPPER(:accountNo))";

            object result = DatabaseHelper.ExecuteScalar(
                query,
                new OracleParameter("accountNo", OracleDbType.Varchar2) { Value = accountNo.Trim() }
            );

            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToInt32(result);
        }

        public void SendMoneyWithFx(int fromAccountId, int toAccountId, decimal amount, int selectedCurrencyId, string description, int employeeId = 1)
        {
            using (OracleConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (OracleTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        int withdrawTypeId = GetTypeIdByName("Withdrawal", conn, tx);
                        int depositTypeId = GetTypeIdByName("Deposit", conn, tx);

                        int withdrawTransactionId = ExecuteAddTransactionWithFx(
                            withdrawTypeId,
                            employeeId,
                            fromAccountId,
                            amount,
                            selectedCurrencyId,
                            string.IsNullOrWhiteSpace(description) ? "Send money - outgoing" : description + " - outgoing",
                            conn,
                            tx);

                        int depositTransactionId = ExecuteAddTransactionWithFx(
                            depositTypeId,
                            employeeId,
                            toAccountId,
                            amount,
                            selectedCurrencyId,
                            string.IsNullOrWhiteSpace(description) ? "Send money - incoming" : description + " - incoming",
                            conn,
                            tx);

                        UpdateTransferLinks(withdrawTransactionId, fromAccountId, toAccountId, conn, tx);
                        UpdateTransferLinks(depositTransactionId, fromAccountId, toAccountId, conn, tx);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        private int GetTypeIdByName(string typeName, OracleConnection conn, OracleTransaction tx)
        {
            string query = @"SELECT ID_Type FROM Type WHERE UPPER(Type) = UPPER(:typeName)";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Transaction = tx;
                cmd.BindByName = true;
                cmd.Parameters.Add("typeName", OracleDbType.Varchar2).Value = typeName;

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new Exception("Transaction type not found: " + typeName);

                return Convert.ToInt32(result);
            }
        }

        private int ExecuteAddTransactionWithFx(
            int typeId,
            int employeeId,
            int accountId,
            decimal amount,
            int currencyId,
            string description,
            OracleConnection conn,
            OracleTransaction tx)
        {
            decimal amountBefore = GetAccountBalance(accountId, conn, tx);

            using (OracleCommand cmd = new OracleCommand("Add_Transaction_With_FX", conn))
            {
                cmd.Transaction = tx;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("p_type_id", OracleDbType.Int32).Value = typeId;
                cmd.Parameters.Add("p_emp_id", OracleDbType.Int32).Value = employeeId;
                cmd.Parameters.Add("p_acc_id", OracleDbType.Int32).Value = accountId;
                cmd.Parameters.Add("p_amount", OracleDbType.Decimal).Value = amount;
                cmd.Parameters.Add("p_curr_id", OracleDbType.Int32).Value = currencyId;
                cmd.Parameters.Add("p_desc", OracleDbType.Varchar2).Value = description;

                cmd.ExecuteNonQuery();
            }

            decimal amountAfter = GetAccountBalance(accountId, conn, tx);

            return GetLastTransactionIdForAccount(accountId, amountAfter, conn, tx);
        }

        private decimal GetAccountBalance(int accountId, OracleConnection conn, OracleTransaction tx)
        {
            string query = @"SELECT Availibility FROM Account WHERE ID_Account = :accountId";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Transaction = tx;
                cmd.BindByName = true;
                cmd.Parameters.Add("accountId", OracleDbType.Int32).Value = accountId;

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new Exception("Account not found.");

                return Convert.ToDecimal(result);
            }
        }

        private int GetLastTransactionIdForAccount(int accountId, decimal balanceAfter, OracleConnection conn, OracleTransaction tx)
        {
            string query = @"
                SELECT MAX(ID_Transactions)
                FROM Transactions
                WHERE ID_Account = :accountId";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Transaction = tx;
                cmd.BindByName = true;
                cmd.Parameters.Add("accountId", OracleDbType.Int32).Value = accountId;

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new Exception("Transaction was not created.");

                return Convert.ToInt32(result);
            }
        }

        private void UpdateTransferLinks(int transactionId, int fromAccountId, int toAccountId, OracleConnection conn, OracleTransaction tx)
        {
            string query = @"
                UPDATE Transactions
                SET FROM_ACCOUNT_ID = :fromAccountId,
                    TO_ACCOUNT_ID = :toAccountId
                WHERE ID_Transactions = :transactionId";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Transaction = tx;
                cmd.BindByName = true;

                cmd.Parameters.Add("fromAccountId", OracleDbType.Int32).Value = fromAccountId;
                cmd.Parameters.Add("toAccountId", OracleDbType.Int32).Value = toAccountId;
                cmd.Parameters.Add("transactionId", OracleDbType.Int32).Value = transactionId;

                cmd.ExecuteNonQuery();
            }
        }
    }
}