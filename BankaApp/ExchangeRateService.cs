using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace BankaApp
{
    public class ExchangeRateService
    {
        public List<decimal> LoadRates(int fromCurrencyId, int toCurrencyId, int maxRows = 6)
        {
            string query = @"
                SELECT Rate
                FROM (
                    SELECT Rate
                    FROM Exchange_Rate
                    WHERE Currency_type_from = :fromId
                      AND Currency_type_to = :toId
                    ORDER BY Rate_Date DESC
                )
                WHERE ROWNUM <= :maxRows";

            DataTable dt = DatabaseHelper.ExecuteDataTable(
                query,
                new OracleParameter("fromId", OracleDbType.Int32) { Value = fromCurrencyId },
                new OracleParameter("toId", OracleDbType.Int32) { Value = toCurrencyId },
                new OracleParameter("maxRows", OracleDbType.Int32) { Value = maxRows }
            );

            List<decimal> rates = new List<decimal>();

            foreach (DataRow row in dt.Rows)
                rates.Add(Convert.ToDecimal(row[0]));

            rates.Reverse();
            return rates;
        }
    }
}