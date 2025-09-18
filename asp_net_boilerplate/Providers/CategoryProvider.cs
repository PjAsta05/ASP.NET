using asp_net_boilerplate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace asp_net_boilerplate.Provider
{
    public class CategoryProvider
    {
        string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;
        public List<tb_m_category_model> getCategory(int offset, int fetch, string search = "")
        {
            var sql = "SELECT category_id,name, description FROM TB_M_CATEGORY WHERE is_deleted = 0 AND (@search = '' OR name LIKE '%' + @search + '%' OR description LIKE '%' + @search + '%') ORDER BY created_at DESC OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY";
            List<tb_m_category_model> list = new List<tb_m_category_model>();
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
                        var category = new tb_m_category_model
                        {
                            category_id = (Guid)rdr["category_id"],
                            name = rdr["name"].ToString(),
                            description = rdr["description"].ToString(),
                            action = "<button type='button' onclick='ShowData(\"" + rdr["category_id"].ToString() + "\")'>Show</button> " +
                                     "<button type='button' onclick='EditData(\"" + rdr["category_id"].ToString() + "\")'>Edit</button> " +
                                     "<button type='button' onclick='DeleteData(\"" + rdr["category_id"].ToString() + "\")'>Delete</button>"
                        };
                        list.Add(category);
                    }
                    con.Close();
                }
            }
            return list;
        }

        public List<category_model> getAllCategory()
        {
            var sql = "SELECT category_id,name FROM TB_M_CATEGORY WHERE is_deleted = 0";
            List<category_model> list = new List<category_model>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var category = new category_model
                        {
                            category_id = (Guid)rdr["category_id"],
                            name = rdr["name"].ToString(),
                        };
                        list.Add(category);
                    }
                    con.Close();
                }
            }
            return list;
        }

        public int getCategoryCount()
        {
            var sql = "SELECT COUNT(*) AS total FROM TB_M_CATEGORY WHERE is_deleted = 0";
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

        public tb_m_category_model getCategory(Guid category_id)
        {
            var sql = "SELECT * FROM TB_M_CATEGORY WHERE category_id = @category_id AND is_deleted = 0";
            tb_m_category_model category = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    cmd.Parameters.Add("@category_id", System.Data.SqlDbType.UniqueIdentifier).Value = category_id;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        category = new tb_m_category_model
                        {
                            category_id = (Guid)rdr["category_id"],
                            name = rdr["name"].ToString(),
                            description = rdr["description"].ToString()
                        };
                    }
                    con.Close();
                }
            }
            return category;
        }

        public string storeCategory(tb_m_category_model category)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = @"IF EXISTS(SELECT 1 FROM TB_M_CATEGORY WHERE category_id = @category_id) UPDATE TB_M_CATEGORY SET name = @name, description = @description, updated_at = GETDATE() WHERE category_id = @category_id
                        ELSE INSERT INTO TB_M_CATEGORY (category_id, name, description) VALUES (@category_id, @name, @description)";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@category_id", SqlDbType.UniqueIdentifier).Value = category.category_id == Guid.Empty ? Guid.NewGuid() : category.category_id;
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = category.name;
                        cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = category.description;

                        int rows = cmd.ExecuteNonQuery();
                        status = rows > 0 ? "success" : "failed";
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    status = "error: " + ex.Message;
                }
            }
            return status;
        }

        public string deleteCategory(Guid category_id)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = "UPDATE TB_M_CATEGORY SET is_deleted = 1, deleted_at = GETDATE() WHERE category_id = @category_id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@category_id", SqlDbType.UniqueIdentifier).Value = category_id;
                        int rows = cmd.ExecuteNonQuery();
                        status = rows > 0 ? "success" : "failed";
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    status = "error: " + ex.Message;
                }
            }
            return status;
        }
    }
}
