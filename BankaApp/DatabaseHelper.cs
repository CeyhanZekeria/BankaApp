using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BankaApp
{
    public static class DatabaseHelper
    {
        private static readonly string connStr =
            "User Id=banka;Password=1234;Data Source=localhost:1521/XE;";

        public static int ExecuteNonQuery(string query, params OracleParameter[] parameters)
        {
            using (OracleConnection conn = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                conn.Open();
                cmd.BindByName = true;

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string query, params OracleParameter[] parameters)
        {
            using (OracleConnection conn = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                conn.Open();
                cmd.BindByName = true;

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }

        public static DataTable ExecuteDataTable(string query, params OracleParameter[] parameters)
        {
            using (OracleConnection conn = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(query, conn))
            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
            {
                cmd.BindByName = true;

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public static OracleConnection GetConnection()
        {
            return new OracleConnection(connStr);
        }
    }
}