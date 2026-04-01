using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace BankaApp
{
    public class TransferRequestService
    {

        public int GetPendingRequestsCount(int fromClientId)
        {
            string query = @"
        SELECT COUNT(*)
        FROM Transfer_Request
        WHERE From_Client_ID = :fromClientId
          AND Status = 'Pending'";

            object result = DatabaseHelper.ExecuteScalar(
                query,
                new OracleParameter("fromClientId", OracleDbType.Int32) { Value = fromClientId }
            );

            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }
        public int GetCardOwnerUserId(string cardNumber, string validThru, string cvv)
        {
            string query = @"
                SELECT ID_User
                FROM App_User
                WHERE REPLACE(Card_Number, ' ', '') = :cardNumber
                  AND REPLACE(Valid_Thru, '.', '/') = :validThru
                  AND CVV = :cvv";

            object result = DatabaseHelper.ExecuteScalar(
                query,
                new OracleParameter("cardNumber", OracleDbType.Varchar2) { Value = cardNumber.Replace(" ", "").Trim() },
                new OracleParameter("validThru", OracleDbType.Varchar2) { Value = validThru.Trim() },
                new OracleParameter("cvv", OracleDbType.Varchar2) { Value = cvv.Trim() }
            );

            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }

        public int GetClientIdByUserId(int userId)
        {
            string query = @"
                SELECT c.Client_ID
                FROM Client c
                JOIN App_User u ON LOWER(c.Email) = LOWER(u.Email)
                WHERE u.ID_User = :userId";

            object result = DatabaseHelper.ExecuteScalar(
                query,
                new OracleParameter("userId", OracleDbType.Int32) { Value = userId }
            );

            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }

        public int GetClientIdByAccountId(int accountId)
        {
            string query = @"
                SELECT ID_Client
                FROM Account
                WHERE ID_Account = :accountId";

            object result = DatabaseHelper.ExecuteScalar(
                query,
                new OracleParameter("accountId", OracleDbType.Int32) { Value = accountId }
            );

            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }

        public int GetFirstAccountIdByClientId(int clientId)
        {
            string query = @"
                SELECT MIN(ID_Account)
                FROM Account
                WHERE ID_Client = :clientId";

            object result = DatabaseHelper.ExecuteScalar(
                query,
                new OracleParameter("clientId", OracleDbType.Int32) { Value = clientId }
            );

            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result)
                : 0;
        }

        public DataTable LoadPendingRequestsForClient(int fromClientId)
        {
            string query = @"
                SELECT
                    tr.ID_Request,
                    c_req.Name AS Requested_By,
                    tr.Amount,
                    cur.Currency_Type_Name AS Requested_Currency,
                    a_from.Account_NO AS From_IBAN,
                    a_to.Account_NO AS To_IBAN,
                    tr.Requested_At
                FROM Transfer_Request tr
                JOIN Client c_req ON tr.Requested_By_Client_ID = c_req.Client_ID
                JOIN Account a_from ON tr.From_Account_ID = a_from.ID_Account
                JOIN Account a_to ON tr.To_Account_ID = a_to.ID_Account
                JOIN Currency_Type cur ON tr.Requested_Currency_ID = cur.ID_Currency_Type
                WHERE tr.From_Client_ID = :fromClientId
                  AND tr.Status = 'Pending'
                ORDER BY tr.Requested_At DESC";

            return DatabaseHelper.ExecuteDataTable(
                query,
                new OracleParameter("fromClientId", OracleDbType.Int32) { Value = fromClientId }
            );
        }

        public void CreateTransferRequest(
            int requestedByClientId,
            int fromClientId,
            int toClientId,
            int fromAccountId,
            int toAccountId,
            decimal amount,
            int requestedCurrencyId,
            string note)
        {
            string query = @"
                INSERT INTO Transfer_Request
                (
                    ID_Request,
                    Requested_By_Client_ID,
                    From_Client_ID,
                    To_Client_ID,
                    From_Account_ID,
                    To_Account_ID,
                    Amount,
                    Requested_Currency_ID,
                    Status,
                    Requested_At,
                    Note
                )
                VALUES
                (
                    seq_transfer_request.NEXTVAL,
                    :requestedByClientId,
                    :fromClientId,
                    :toClientId,
                    :fromAccountId,
                    :toAccountId,
                    :amount,
                    :requestedCurrencyId,
                    'Pending',
                    SYSDATE,
                    :note
                )";

            DatabaseHelper.ExecuteNonQuery(
                query,
                new OracleParameter("requestedByClientId", OracleDbType.Int32) { Value = requestedByClientId },
                new OracleParameter("fromClientId", OracleDbType.Int32) { Value = fromClientId },
                new OracleParameter("toClientId", OracleDbType.Int32) { Value = toClientId },
                new OracleParameter("fromAccountId", OracleDbType.Int32) { Value = fromAccountId },
                new OracleParameter("toAccountId", OracleDbType.Int32) { Value = toAccountId },
                new OracleParameter("amount", OracleDbType.Decimal) { Value = amount },
                new OracleParameter("requestedCurrencyId", OracleDbType.Int32) { Value = requestedCurrencyId },
                new OracleParameter("note", OracleDbType.Varchar2) { Value = note }
            );
        }

        public DataTable LoadCurrencies()
        {
            string query = @"
                SELECT ID_Currency_Type, Currency_Type_Name
                FROM Currency_Type
                ORDER BY Currency_Type_Name";

            return DatabaseHelper.ExecuteDataTable(query);
        }

        public void ApproveRequest(int requestId)
        {
            using (OracleConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (OracleTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        string selectQuery = @"
                            SELECT 
                                From_Account_ID,
                                To_Account_ID,
                                Amount,
                                Requested_Currency_ID
                            FROM Transfer_Request
                            WHERE ID_Request = :requestId
                              AND Status = 'Pending'";

                        int fromAccountId = 0;
                        int toAccountId = 0;
                        int requestedCurrencyId = 0;
                        decimal requestedAmount = 0;

                        using (OracleCommand selectCmd = new OracleCommand(selectQuery, conn))
                        {
                            selectCmd.Transaction = tx;
                            selectCmd.BindByName = true;
                            selectCmd.Parameters.Add("requestId", OracleDbType.Int32).Value = requestId;

                            using (OracleDataReader reader = selectCmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception("Pending request not found.");

                                fromAccountId = Convert.ToInt32(reader["From_Account_ID"]);
                                toAccountId = Convert.ToInt32(reader["To_Account_ID"]);
                                requestedAmount = Convert.ToDecimal(reader["Amount"]);
                                requestedCurrencyId = Convert.ToInt32(reader["Requested_Currency_ID"]);
                            }
                        }

                        int fromAccountCurrencyId = GetAccountCurrencyId(fromAccountId, conn, tx);
                        int toAccountCurrencyId = GetAccountCurrencyId(toAccountId, conn, tx);

                        int withdrawTypeId = GetTypeIdByName("Withdrawal", conn, tx);
                        int depositTypeId = GetTypeIdByName("Deposit", conn, tx);

                        int? fxToSourceRateId;
                        int? fxToTargetRateId;

                        decimal sourceAmount = ConvertAmount(
                            requestedAmount,
                            requestedCurrencyId,
                            fromAccountCurrencyId,
                            conn,
                            tx,
                            out fxToSourceRateId
                        );

                        decimal targetAmount = ConvertAmount(
                            requestedAmount,
                            requestedCurrencyId,
                            toAccountCurrencyId,
                            conn,
                            tx,
                            out fxToTargetRateId
                        );

                        InsertTransaction(
                            withdrawTypeId,
                            1,
                            fromAccountId,
                            sourceAmount,
                            fromAccountCurrencyId,
                            fxToSourceRateId,
                            "Approved add money request - outgoing",
                            fromAccountId,
                            toAccountId,
                            conn,
                            tx
                        );

                        InsertTransaction(
                            depositTypeId,
                            1,
                            toAccountId,
                            targetAmount,
                            toAccountCurrencyId,
                            fxToTargetRateId,
                            "Approved add money request - incoming",
                            fromAccountId,
                            toAccountId,
                            conn,
                            tx
                        );

                        string updateQuery = @"
                            UPDATE Transfer_Request
                            SET Status = 'Approved',
                                Approved_At = SYSDATE
                            WHERE ID_Request = :requestId";

                        using (OracleCommand updateCmd = new OracleCommand(updateQuery, conn))
                        {
                            updateCmd.Transaction = tx;
                            updateCmd.BindByName = true;
                            updateCmd.Parameters.Add("requestId", OracleDbType.Int32).Value = requestId;
                            updateCmd.ExecuteNonQuery();
                        }

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

        public void RejectRequest(int requestId)
        {
            string query = @"
                UPDATE Transfer_Request
                SET Status = 'Rejected',
                    Rejected_At = SYSDATE
                WHERE ID_Request = :requestId
                  AND Status = 'Pending'";

            DatabaseHelper.ExecuteNonQuery(
                query,
                new OracleParameter("requestId", OracleDbType.Int32) { Value = requestId }
            );
        }

        private int GetAccountCurrencyId(int accountId, OracleConnection conn, OracleTransaction tx)
        {
            string query = @"
                SELECT ID_Currency_Type
                FROM Account
                WHERE ID_Account = :accountId";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Transaction = tx;
                cmd.BindByName = true;
                cmd.Parameters.Add("accountId", OracleDbType.Int32).Value = accountId;

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new Exception("Account currency not found.");

                return Convert.ToInt32(result);
            }
        }

        private int GetTypeIdByName(string typeName, OracleConnection conn, OracleTransaction tx)
        {
            string query = @"
                SELECT ID_Type
                FROM Type
                WHERE UPPER(Type) = UPPER(:typeName)";

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

        private decimal ConvertAmount(
       decimal amount,
       int fromCurrencyId,
       int toCurrencyId,
       OracleConnection conn,
       OracleTransaction tx,
       out int? exchangeRateId)
        {
            exchangeRateId = null;

            if (fromCurrencyId == toCurrencyId)
                return amount;

            // 1) direct rate
            decimal? direct = TryGetRate(fromCurrencyId, toCurrencyId, conn, tx, out int? directRateId);
            if (direct.HasValue)
            {
                exchangeRateId = directRateId;
                return decimal.Round(amount * direct.Value, 4);
            }

            // 2) inverse rate
            decimal? inverse = TryGetRate(toCurrencyId, fromCurrencyId, conn, tx, out int? inverseRateId);
            if (inverse.HasValue)
            {
                if (inverse.Value <= 0)
                    throw new Exception("Invalid inverse exchange rate.");

                exchangeRateId = inverseRateId;
                return decimal.Round(amount / inverse.Value, 4);
            }

            // 3) fallback through Lei (ID = 1)
            const int baseCurrencyId = 1;

            if (fromCurrencyId != baseCurrencyId && toCurrencyId != baseCurrencyId)
            {
                int? step1RateId;
                int? step2RateId;

                decimal amountInLei = ConvertAmount(
                    amount,
                    fromCurrencyId,
                    baseCurrencyId,
                    conn,
                    tx,
                    out step1RateId
                );

                decimal finalAmount = ConvertAmount(
                    amountInLei,
                    baseCurrencyId,
                    toCurrencyId,
                    conn,
                    tx,
                    out step2RateId
                );

                // keep final step rate id if available, otherwise first
                exchangeRateId = step2RateId ?? step1RateId;

                return decimal.Round(finalAmount, 4);
            }

            throw new Exception($"Exchange rate not found for currencies {fromCurrencyId} -> {toCurrencyId}.");
        }

        private decimal? TryGetRate(
    int fromCurrencyId,
    int toCurrencyId,
    OracleConnection conn,
    OracleTransaction tx,
    out int? exchangeRateId)
        {
            exchangeRateId = null;

            string query = @"
        SELECT ID_EXCHANGE_RATE, RATE
        FROM EXCHANGE_RATE
        WHERE CURRENCY_TYPE_FROM = :fromCurrencyId
          AND CURRENCY_TYPE_TO = :toCurrencyId
          AND RATE_DATE = (
              SELECT MAX(RATE_DATE)
              FROM EXCHANGE_RATE
              WHERE CURRENCY_TYPE_FROM = :fromCurrencyId
                AND CURRENCY_TYPE_TO = :toCurrencyId
          )";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Transaction = tx;
                cmd.BindByName = true;
                cmd.Parameters.Add("fromCurrencyId", OracleDbType.Int32).Value = fromCurrencyId;
                cmd.Parameters.Add("toCurrencyId", OracleDbType.Int32).Value = toCurrencyId;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        exchangeRateId = Convert.ToInt32(reader["ID_EXCHANGE_RATE"]);
                        return Convert.ToDecimal(reader["RATE"]);
                    }
                }
            }

            return null;
        }


        private void InsertTransaction(
            int idType,
            int idEmployee,
            int idAccount,
            decimal amount,
            int currencyId,
            int? exchangeRateId,
            string description,
            int fromAccountId,
            int toAccountId,
            OracleConnection conn,
            OracleTransaction tx)
        {
            string query = @"
                INSERT INTO Transactions
                (
                    ID_Transactions,
                    ID_Type,
                    ID_Employee,
                    ID_Account,
                    Sum_Amount,
                    Date_Tran,
                    ID_Currency_Type,
                    ID_Exchange_Rate,
                    Description,
                    FROM_ACCOUNT_ID,
                    TO_ACCOUNT_ID
                )
                VALUES
                (
                    seq_transactions.NEXTVAL,
                    :idType,
                    :idEmployee,
                    :idAccount,
                    :amount,
                    SYSDATE,
                    :currencyId,
                    :exchangeRateId,
                    :description,
                    :fromAccountId,
                    :toAccountId
                )";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Transaction = tx;
                cmd.BindByName = true;

                cmd.Parameters.Add("idType", OracleDbType.Int32).Value = idType;
                cmd.Parameters.Add("idEmployee", OracleDbType.Int32).Value = idEmployee;
                cmd.Parameters.Add("idAccount", OracleDbType.Int32).Value = idAccount;
                cmd.Parameters.Add("amount", OracleDbType.Decimal).Value = amount;
                cmd.Parameters.Add("currencyId", OracleDbType.Int32).Value = currencyId;

                if (exchangeRateId.HasValue)
                    cmd.Parameters.Add("exchangeRateId", OracleDbType.Int32).Value = exchangeRateId.Value;
                else
                    cmd.Parameters.Add("exchangeRateId", OracleDbType.Int32).Value = DBNull.Value;

                cmd.Parameters.Add("description", OracleDbType.Varchar2).Value = description;
                cmd.Parameters.Add("fromAccountId", OracleDbType.Int32).Value = fromAccountId;
                cmd.Parameters.Add("toAccountId", OracleDbType.Int32).Value = toAccountId;

                cmd.ExecuteNonQuery();
            }
        }
    }
}