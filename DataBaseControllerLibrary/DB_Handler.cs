using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;

namespace DataBaseControllerLibrary
{
    public class DB_Handler
    {
        string ConnectionString;
        public DB_Handler(string conString)
        {
            ConnectionString = $@"Server={Environment.MachineName};Database=CarRepairDB;Trusted_Connection=True;TrustServerCertificate=true;";//conString;

        }
        public DB_Handler()
        {
            ConnectionString = $@"Server={Environment.MachineName};Database=CarRepairDB;Trusted_Connection=True;TrustServerCertificate=true;";
        }

        public (string, string, User) ExecLogin(string Login, string Password)
        {
            string querystr = $"select login, password, type, phone, userID, fio from Users where login = '{Login}' " +
                $"and Password = '{Password}'"; // выбор пользователей, которые уже есть в бд
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var comm = new SqlCommand(querystr, conn))
                {
                    using (var adapter = new SqlDataAdapter(comm))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count != 1)
                        {

                            var query = @"DECLARE @userID INT;
                                -- Check if login exists in the Users table
                                SELECT @userID = [userID]
                                FROM [CarRepairDB].[dbo].[Users]
                                WHERE [login] = @login;

                                -- If a matching login is found, insert a new record into the Login_logs table
                                IF @userID IS NOT NULL
                                BEGIN
                                    INSERT INTO [CarRepairDB].[dbo].[Login_logs] ([userID], [LoginTime], [LoginStatus])
                                    VALUES (@userID, GETDATE(), @loginStatus);
                                END";
                            ExecuteWriteQuery(query, [new SqlParameter("@login", Login), new SqlParameter("loginStatus", "Неудачный вход")]);
                            return ("Неправильное имя пользователя или пароль!", "Ошибка", null);
                        }
                        else
                        {
                            var LoginQueryRes = dataTable.Rows[0];
                            string security = (string)LoginQueryRes["type"];
                            int pn = (int)LoginQueryRes["userID"];
                            string fio = LoginQueryRes["fio"].ToString();
                            string phone = LoginQueryRes["phone"].ToString();

                            string ToolTip = "";
                            switch (security)
                            {
                                case "Менеджер":
                                    ToolTip = "Авторизация менеджера успешно пройдена!";
                                    break;
                                case "Оператор":
                                    ToolTip = "Авторизация оператора успешно пройдена!";
                                    break;
                                case "Автомеханик":
                                    ToolTip = "Авторизация автомеханика успешно пройдена!";
                                    break;
                                case "Заказчик":
                                    ToolTip = "Авторизация заказчика успешно пройдена!";
                                    break;
                                default:
                                    return ("Неправильное имя пользователя или пароль!", "Ошибка", null);
                            }
                            var query = @"INSERT INTO [CarRepairDB].[dbo].[Login_logs] ([userID], [LoginTime], [LoginStatus])
                                VALUES (@userID, GETDATE(), @loginStatus);";
                            ExecuteWriteQuery(query, [new SqlParameter("@userID", pn), new SqlParameter("loginStatus", "Вход")]);
                            return (ToolTip, "Успешно", new User(fio, pn, phone, security));
                        }
                    }
                }
            }
        }

        public List<dynamic> ExecuteReadQuery(string sqlQuery, SqlParameter[] parameters = null)
        {
            List<dynamic> result = new List<dynamic>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic item = new ExpandoObject();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    ((IDictionary<string, object>)item).Add(reader.GetName(i), reader[i]);
                                }

                                result.Add(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new List<dynamic>();
                    }
                }
            }

            return result;
        }

        public bool ExecuteWriteQuery(string sqlquery, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlquery, connection);
                try
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    SqlDataReader dr = command.ExecuteReader();
                    dr.Close();
                    return true;
                }
                catch (Exception e)
                {
                    //MessageBox.Show("Ошибка в выполнении запроса");
                    return false;
                }
            }

        }

    }

    public class User
    {
        public string FName = "";
        public string SName = "";
        public string TName = "";
        public int Personnel_Num;
        public string Phone = "";
        public string Security = "";

        public User(string FullName, int personnel_Num, string phoneNum, string security)
        {
            if (FullName != "")
            {
                var splito = FullName.Split(' ');
                FName = splito[1];
                SName = splito[0];
                TName = splito[2];
            }
            Phone = phoneNum;
            Personnel_Num = personnel_Num;
            Security = security;
        }
    }
}

