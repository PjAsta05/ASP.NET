using asp_net_boilerplate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace asp_net_boilerplate.Provider
{
    public class UserProvider
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

        public tb_m_user_model Authenticate(string username, string password)
        {
            tb_m_user_model user = null;

            var sql = @"SELECT * FROM TB_M_USER WHERE username = @username AND password = @password AND is_allow_login = 1 AND is_deleted = 0";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        user = new tb_m_user_model
                        {
                            user_id = (Guid)rdr["user_id"],
                            username = rdr["username"].ToString(),
                            name = rdr["name"].ToString(),
                            password = rdr["password"].ToString(),
                            is_allow_login = (bool)rdr["is_allow_login"],
                            is_deleted = (bool)rdr["is_deleted"]
                        };
                    }
                    con.Close();
                }
            }
            return user;
        }

        public List<tb_m_user_model> GetUserList(int offset, int fetch, string search = "")
        {
            var sql = "SELECT user_id, username, name, description FROM TB_M_USER WHERE is_deleted = 0 AND (@search = '' OR username LIKE '%' + @search + '%' OR name LIKE '%' + @search + '%' OR description LIKE '%' + @search + '%') ORDER BY created_at DESC OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY";
            List<tb_m_user_model> list = new List<tb_m_user_model>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@offset", offset);
                    cmd.Parameters.AddWithValue("@fetch", fetch);
                    cmd.Parameters.AddWithValue("@search", search ?? "");
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var user = new tb_m_user_model
                        {
                            user_id = (Guid)rdr["user_id"],
                            username = rdr["username"].ToString(),
                            name = rdr["name"].ToString(),
                            description = rdr["description"].ToString(),
                            action = "<button type='button' onclick='ShowData(\"" + rdr["user_id"].ToString() + "\")'>Show</button> " +
                                     "<button type='button' onclick='EditData(\"" + rdr["user_id"].ToString() + "\")'>Edit</button> " +
                                     "<button type='button' onclick='ChangePassword(\"" + rdr["user_id"].ToString() + "\")'>Change Password</button> " +
                                     "<button type='button' onclick='DeleteData(\"" + rdr["user_id"].ToString() + "\")'>Delete</button>"
                        };
                        list.Add(user);
                    }
                    con.Close();
                }
            }
            return list;
        }

        public int GetUserCount()
        {
            var sql = "SELECT COUNT(*) AS total FROM TB_M_USER WHERE is_deleted = 0";
            int total = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        total = (int)rdr["total"];
                    }
                    con.Close();
                }
                return total;
            }
        }

        public tb_m_user_model GetUser(Guid user_id)
        {
            var sql = "SELECT * FROM TB_M_USER WHERE user_id = @user_id AND is_deleted = 0";
            tb_m_user_model user = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        user = new tb_m_user_model
                        {
                            user_id = (Guid)rdr["user_id"],
                            username = rdr["username"].ToString(),
                            name = rdr["name"].ToString(),
                            description = rdr["description"].ToString(),
                            password = rdr["password"].ToString(),
                            is_allow_login = (bool)rdr["is_allow_login"],
                            is_deleted = (bool)rdr["is_deleted"]
                        };
                    }
                    con.Close();
                }
            }
            return user;
        }

        public string storeUser(tb_m_user_model user)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = @"IF EXISTS(SELECT 1 FROM TB_M_USER WHERE user_id = @user_id) UPDATE TB_M_USER SET username=@username, name=@name, description=@description, is_allow_login=@is_allow_login, updated_at=GETDATE() WHERE user_id=@user_id
                                ELSE INSERT INTO TB_M_USER(user_id, username, name, description, password, is_allow_login) VALUES(@user_id, @username, @name, @description, @password, @is_allow_login)";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@user_id", SqlDbType.UniqueIdentifier).Value = user.user_id == Guid.Empty ? Guid.NewGuid() : user.user_id;
                        cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = user.username;
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = user.name;
                        cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = user.description;
                        cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = user.password ?? "";
                        cmd.Parameters.Add("@is_allow_login", SqlDbType.Bit).Value = user.is_allow_login;

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0) status = "success";
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    status = ex.Message;
                }
            }
            return status;
        }

        public string changePassword(Guid user_id, string old_password, string new_password)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var checkSql = "SELECT password FROM TB_M_USER WHERE user_id = @user_id";
                    using (SqlCommand checkCmd = new SqlCommand(checkSql, con))
                    {
                        checkCmd.Parameters.Add("@user_id", SqlDbType.UniqueIdentifier).Value = user_id;
                        var currentPassword = checkCmd.ExecuteScalar() as string;

                        if (currentPassword == null)
                        {
                            status = "user not found";
                            return status;
                        }

                        if (currentPassword != old_password)
                        {
                            status = "wrong password";
                            return status;
                        }
                    }
                    var sql = @"UPDATE TB_M_USER SET password = @password, updated_at = GETDATE() WHERE user_id = @user_id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@user_id", SqlDbType.UniqueIdentifier).Value = user_id;
                        cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = new_password;
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0) status = "success";
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    status = ex.Message;
                }
            }
            return status;
        }

        public string deleteUser(Guid user_id)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = "UPDATE TB_M_USER SET is_deleted=1, deleted_at=GETDATE() WHERE user_id=@user_id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@user_id", SqlDbType.UniqueIdentifier).Value = user_id;
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0) status = "success";
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    status = ex.Message;
                }
            }
            return status;
        }
    }
}