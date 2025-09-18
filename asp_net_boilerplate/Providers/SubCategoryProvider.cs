using asp_net_boilerplate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace asp_net_boilerplate.Provider
{
    public class SubCategoryProvider
    {
        string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

        public List<tb_m_sub_category_model> getSubCategory(int offset, int fetch, string search = "")
        {
            var sql = @"SELECT s.sub_category_id, s.category_id, s.name AS sub_category_name, s.description, c.name AS category_name FROM TB_M_SUBCATEGORY s INNER JOIN TB_M_CATEGORY c ON s.category_id = c.category_id WHERE s.is_deleted = 0 AND (@search = '' OR s.name LIKE '%' + @search + '%' OR s.description LIKE '%' + @search + '%' OR c.name LIKE '%' + @search + '%') ORDER BY s.created_at DESC OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY";
            List<tb_m_sub_category_model> list = new List<tb_m_sub_category_model>();
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
                        var subCategory = new tb_m_sub_category_model
                        {
                            sub_category_id = (Guid)rdr["sub_category_id"],
                            category_id = (Guid)rdr["category_id"],
                            sub_category_name = rdr["sub_category_name"].ToString(),
                            category_name = rdr["category_name"].ToString(),
                            description = rdr["description"].ToString(),
                            action = "<button type='button' onclick='ShowData(\"" + rdr["sub_category_id"].ToString() + "\")'>Show</button> " +
                                     "<button type='button' onclick='EditData(\"" + rdr["sub_category_id"].ToString() + "\")'>Edit</button> " +
                                     "<button type='button' onclick='DeleteData(\"" + rdr["sub_category_id"].ToString() + "\")'>Delete</button>"
                         };

                        list.Add(subCategory);
                    }
                    con.Close();
                }
            }
            return list;
        }

        public List<sub_category_model> getAllSubCategory()
        {
            var sql = "SELECT sub_category_id,name FROM TB_M_SUBCATEGORY WHERE is_deleted = 0";
            List<sub_category_model> list = new List<sub_category_model>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var subCategory = new sub_category_model
                        {
                            sub_category_id = (Guid)rdr["sub_category_id"],
                            name = rdr["name"].ToString(),
                        };
                        list.Add(subCategory);
                    }
                    con.Close();
                }
            }
            return list;
        }

        public int getSubCategoryCount()
        {
            var sql = "SELECT COUNT(*) AS total FROM TB_M_SUBCATEGORY WHERE is_deleted = 0";
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

        public tb_m_sub_category_model getSubCategory(Guid sub_category_id)
        {
            var sql = @"SELECT s.sub_category_id, s.category_id, s.name AS sub_category_name, s.description, c.name AS category_name FROM TB_M_SUBCATEGORY s INNER JOIN TB_M_CATEGORY c ON s.category_id = c.category_id WHERE s.sub_category_id = @sub_category_id AND s.is_deleted = 0";
            tb_m_sub_category_model subCategory = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    cmd.Parameters.Add("@sub_category_id", System.Data.SqlDbType.UniqueIdentifier).Value = sub_category_id;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        subCategory = new tb_m_sub_category_model
                        {
                            sub_category_id = (Guid)rdr["sub_category_id"],
                            category_id = (Guid)rdr["category_id"],
                            sub_category_name = rdr["sub_category_name"].ToString(),
                            category_name = rdr["category_name"].ToString(),
                            description = rdr["description"].ToString(),
                        };
                    }
                    con.Close();
                }
            }
            return subCategory;
        }

        public string storeSubCategory(tb_m_sub_category_model subCategory)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = @"IF EXISTS(SELECT 1 FROM TB_M_SUBCATEGORY WHERE sub_category_id = @sub_category_id) UPDATE TB_M_SUBCATEGORY SET name = @name, description = @description, category_id = @category_id, updated_at = GETDATE() WHERE sub_category_id = @sub_category_id 
                        ELSE INSERT INTO TB_M_SUBCATEGORY (sub_category_id, name, description, category_id) VALUES (@sub_category_id, @name, @description, @category_id)";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@sub_category_id", SqlDbType.UniqueIdentifier).Value = subCategory.sub_category_id == Guid.Empty ? Guid.NewGuid() : subCategory.sub_category_id; ;
                        cmd.Parameters.Add("@category_id", SqlDbType.UniqueIdentifier).Value = subCategory.category_id;
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = subCategory.sub_category_name;
                        cmd.Parameters.Add("@description", SqlDbType.NVarChar, 1000).Value = subCategory.description;

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

        public string deleteSubCategory(Guid sub_category_id)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = "UPDATE TB_M_SUBCATEGORY SET is_deleted = 1, deleted_at = GETDATE() WHERE sub_category_id = @sub_category_id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@sub_category_id", SqlDbType.UniqueIdentifier).Value = sub_category_id;
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
