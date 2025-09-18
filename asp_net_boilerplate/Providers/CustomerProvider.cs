using asp_net_boilerplate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace asp_net_boilerplate.Provider
{
    public class CustomerProvider
    {
        string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

        public List<tb_m_customer_model> getCustomerList(int offset, int fetch, string search = "")
        {
            var sql = @"SELECT customer_id, name, address, telephone, handphone, email FROM TB_M_CUSTOMER WHERE is_deleted = 0 AND (@search = '' OR name LIKE '%' + @search + '%' OR address LIKE '%' + @search + '%' OR telephone LIKE '%' + @search + '%' OR handphone LIKE '%' + @search + '%') ORDER BY created_at DESC OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY";
            List<tb_m_customer_model> list = new List<tb_m_customer_model>();
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
                        var customer = new tb_m_customer_model
                        {
                            customer_id = (Guid)rdr["customer_id"],
                            name = rdr["name"].ToString(),
                            address = rdr["address"].ToString(),
                            telephone = rdr["telephone"].ToString(),
                            handphone = rdr["handphone"].ToString(),
                            email = rdr["email"].ToString(),
                            action = "<button type='button' onclick='ShowData(\"" + rdr["customer_id"].ToString() + "\")'>Show</button> " +
                                     "<button type='button' onclick='EditData(\"" + rdr["customer_id"].ToString() + "\")'>Edit</button> " +
                                     "<button type='button' onclick='DeleteData(\"" + rdr["customer_id"].ToString() + "\")'>Delete</button>"
                        };
                        list.Add(customer);
                    }
                    con.Close();
                }
            }
            return list;
        }

        public int getCustomerCount()
        {
            var sql = "SELECT COUNT(*) AS total FROM TB_M_CUSTOMER WHERE is_deleted = 0";
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
            }
            return total;
        }

        public tb_m_customer_model getCustomer(Guid customer_id)
        {
            var sql = "SELECT * FROM TB_M_CUSTOMER WHERE customer_id = @customer_id AND is_deleted = 0";
            tb_m_customer_model customer = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    cmd.Parameters.Add("@customer_id", System.Data.SqlDbType.UniqueIdentifier).Value = customer_id;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        customer = new tb_m_customer_model
                        {
                            customer_id = (Guid)rdr["customer_id"],
                            name = rdr["name"].ToString(),
                            address = rdr["address"].ToString(),
                            telephone = rdr["telephone"].ToString(),
                            handphone = rdr["handphone"].ToString(),
                            email = rdr["email"].ToString()
                        };
                    }
                    con.Close();
                }
            }
            return customer;
        }

        public string storeCustomer(tb_m_customer_model customer)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = @"IF EXISTS(SELECT 1 FROM TB_M_CUSTOMER WHERE customer_id = @customer_id) UPDATE TB_M_CUSTOMER SET name=@name, address=@address, telephone=@telephone, handphone=@handphone, email=@email, updated_at=GETDATE() WHERE customer_id=@customer_id
                        ELSE INSERT INTO TB_M_CUSTOMER(customer_id, name, address, telephone, handphone, email) VALUES(@customer_id, @name, @address, @telephone, @handphone, @email)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@customer_id", SqlDbType.UniqueIdentifier).Value = customer.customer_id == Guid.Empty ? Guid.NewGuid() : customer.customer_id;
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = customer.name;
                        cmd.Parameters.Add("@address", SqlDbType.NVarChar).Value = customer.address ?? "";
                        cmd.Parameters.Add("@telephone", SqlDbType.NVarChar).Value = customer.telephone ?? "";
                        cmd.Parameters.Add("@handphone", SqlDbType.NVarChar).Value = customer.handphone ?? "";
                        cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = customer.email ?? "";

                        int rows = cmd.ExecuteNonQuery();
                        status = rows > 0 ? "success" : "failed";
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

        public string deleteCustomer(Guid customer_id)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "UPDATE TB_M_CUSTOMER SET is_deleted=1, deleted_at=GETDATE() WHERE customer_id=@customer_id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@customer_id", SqlDbType.UniqueIdentifier).Value = customer_id;
                        int rows = cmd.ExecuteNonQuery();
                        status = rows > 0 ? "success" : "failed";
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