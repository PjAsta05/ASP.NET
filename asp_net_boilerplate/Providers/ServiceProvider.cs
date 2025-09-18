using asp_net_boilerplate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace asp_net_boilerplate.Provider
{
    public class ServiceProvider
    {
        string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

        public List<tb_m_service_type_model> getService(int offset, int fetch, string search = "")
        {
            var sql = "SELECT service_type_id, name, description FROM TB_M_SERVICETYPE WHERE is_deleted = 0 AND (@search = '' OR name LIKE '%' + @search + '%' OR description LIKE '%' + @search + '%') ORDER BY created_at DESC OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY";
            List<tb_m_service_type_model> list = new List<tb_m_service_type_model>();
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
                        var service = new tb_m_service_type_model
                        {
                            service_type_id = (Guid)rdr["service_type_id"],
                            name = rdr["name"].ToString(),
                            description = rdr["description"].ToString(),
                            action = "<button type='button' onclick='ShowData(\"" + rdr["service_type_id"].ToString() + "\")'>Show</button> " +
                                     "<button type='button' onclick='EditData(\"" + rdr["service_type_id"].ToString() + "\")'>Edit</button> " +
                                     "<button type='button' onclick='DeleteData(\"" + rdr["service_type_id"].ToString() + "\")'>Delete</button>"
                        };
                        list.Add(service);
                    }
                    con.Close();
                }
            }
            return list;
        }

        public int getServiceCount()
        {
            var sql = "SELECT COUNT(*) AS total FROM TB_M_SERVICETYPE WHERE is_deleted = 0";
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

        public tb_m_service_type_model getService(Guid service_type_id)
        {
            var sql = "SELECT * FROM TB_M_SERVICETYPE WHERE service_type_id = @service_type_id AND is_deleted = 0";
            tb_m_service_type_model service = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    cmd.Parameters.Add("@service_type_id", System.Data.SqlDbType.UniqueIdentifier).Value = service_type_id;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        service = new tb_m_service_type_model
                        {
                            service_type_id = (Guid)rdr["service_type_id"],
                            name = rdr["name"].ToString(),
                            description = rdr["description"].ToString()
                        };
                    }
                    con.Close();
                }
            }
            return service;
        }

        public string storeService(tb_m_service_type_model service)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = @"IF EXISTS(SELECT 1 FROM TB_M_SERVICETYPE WHERE service_type_id = @service_type_id) UPDATE TB_M_SERVICETYPE SET name = @name, description = @description, updated_at = GETDATE() WHERE service_type_id = @service_type_id
                        ELSE INSERT INTO TB_M_SERVICETYPE (service_type_id, name, description) VALUES (@service_type_id, @name, @description)";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@service_type_id", SqlDbType.UniqueIdentifier).Value = service.service_type_id == Guid.Empty ? Guid.NewGuid() : service.service_type_id;
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = service.name;
                        cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = service.description;

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

        public string deleteService(Guid service_type_id)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = "UPDATE TB_M_SERVICETYPE SET is_deleted = 1, deleted_at = GETDATE() WHERE service_type_id = @service_type_id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@service_type_id", SqlDbType.UniqueIdentifier).Value = service_type_id;
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
