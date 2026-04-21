using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace BankaApp
{
    public class PasswordMigrationService
    {
        public int MigratePlainTextPasswordsToHash()
        {
            int updatedCount = 0;

            using (OracleConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (OracleTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        string selectQuery = @"
                            SELECT ID_User AS USER_ID, User_Password AS PWD
                            FROM App_User";

                        using (OracleCommand selectCmd = new OracleCommand(selectQuery, conn))
                        {
                            selectCmd.Transaction = tx;
                            selectCmd.BindByName = true;

                            using (OracleDataReader reader = selectCmd.ExecuteReader())
                            {
                                DataTable dt = new DataTable();
                                dt.Load(reader);

                                foreach (DataRow row in dt.Rows)
                                {
                                    int userId = Convert.ToInt32(row["USER_ID"]);
                                    string currentPassword = row["PWD"]?.ToString();

                                    if (string.IsNullOrWhiteSpace(currentPassword))
                                        continue;

                                    if (currentPassword.StartsWith("$2a$") ||
                                        currentPassword.StartsWith("$2b$") ||
                                        currentPassword.StartsWith("$2y$"))
                                    {
                                        continue;
                                    }

                                    string hashedPassword = PasswordHelper.HashPassword(currentPassword);

                                    string updateQuery = @"
                                        UPDATE App_User
                                        SET User_Password = :hashedPassword
                                        WHERE ID_User = :userId";

                                    using (OracleCommand updateCmd = new OracleCommand(updateQuery, conn))
                                    {
                                        updateCmd.Transaction = tx;
                                        updateCmd.BindByName = true;

                                        updateCmd.Parameters.Add("hashedPassword", OracleDbType.Varchar2).Value = hashedPassword;
                                        updateCmd.Parameters.Add("userId", OracleDbType.Int32).Value = userId;

                                        updatedCount += updateCmd.ExecuteNonQuery();
                                    }
                                }
                            }
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

            return updatedCount;
        }
    }
}