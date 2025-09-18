using asp_net_boilerplate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace asp_net_boilerplate.Provider
{
    public class StockProvider
    {
        string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

        public List<tb_m_stock_model> getStock(int offset, int fetch, string search = "")
        {
            var sql = @"SELECT 
                            s.stock_id,
                            s.barcode,
                            s.name,
                            s.sub_category_id,
                            sc.name AS sub_category_name,
                            s.category_id,
                            c.name AS category_name
                        FROM TB_M_STOCK s
                        INNER JOIN TB_M_SUBCATEGORY sc ON s.sub_category_id = sc.sub_category_id
                        INNER JOIN TB_M_CATEGORY c ON s.category_id = c.category_id
                        WHERE s.is_deleted = 0
                          AND (@search = '' 
                               OR s.name LIKE '%' + @search + '%'
                               OR sc.name LIKE '%' + @search + '%'
                               OR c.name LIKE '%' + @search + '%')
                        ORDER BY s.created_at DESC
                        OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY";
            List<tb_m_stock_model> list = new List<tb_m_stock_model>();
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
                        var stock = new tb_m_stock_model
                        {
                            stock_id = (Guid)rdr["stock_id"],
                            sub_category_id = (Guid)rdr["sub_category_id"],
                            category_id = (Guid)rdr["category_id"],
                            barcode = rdr["barcode"].ToString(),
                            name = rdr["name"].ToString(),
                            sub_category_name = rdr["sub_category_name"].ToString(),
                            category_name = rdr["category_name"].ToString(),
                            action = "<button type='button' onclick='ShowData(\"" + rdr["stock_id"].ToString() + "\")'>Show</button> " +
                                     "<button type='button' onclick='EditData(\"" + rdr["stock_id"].ToString() + "\")'>Edit</button> " +
                                     "<button type='button' onclick='DeleteData(\"" + rdr["stock_id"].ToString() + "\")'>Delete</button>"
                        };

                        list.Add(stock);
                    }
                    con.Close();
                }
            }
            return list;
        }

        public int getStockCount()
        {
            var sql = "SELECT COUNT(*) AS total FROM TB_M_STOCK WHERE is_deleted = 0";
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

        public tb_m_stock_model getStock(Guid stock_id)
        {
            var sql = @"SELECT 
                            s.stock_id,
                            s.name,
                            s.barcode,
                            s.sub_category_id,
                            sc.name AS sub_category_name,
                            s.category_id,
                            c.name AS category_name,
                            s.price,
                            s.notes
                        FROM TB_M_STOCK s
                        INNER JOIN TB_M_SUBCATEGORY sc ON s.sub_category_id = sc.sub_category_id
                        INNER JOIN TB_M_CATEGORY c ON s.category_id = c.category_id
                        WHERE s.is_deleted = 0 AND s.stock_id = @stock_id";
            tb_m_stock_model stock = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    cmd.Parameters.Add("@stock_id", System.Data.SqlDbType.UniqueIdentifier).Value = stock_id;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        stock = new tb_m_stock_model
                        {
                            stock_id = (Guid)rdr["stock_id"],
                            sub_category_id = (Guid)rdr["sub_category_id"],
                            category_id = (Guid)rdr["category_id"],
                            barcode = rdr["barcode"].ToString(),
                            name = rdr["name"].ToString(),
                            sub_category_name = rdr["sub_category_name"].ToString(),
                            category_name = rdr["category_name"].ToString(),
                            price = rdr["price"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr["price"]),
                            notes = rdr["notes"].ToString()
                        };
                    }
                    con.Close();
                }
            }
            return stock;
        }

        public string storeStock(tb_m_stock_model stock)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = @"IF EXISTS(SELECT 1 FROM TB_M_STOCK WHERE stock_id = @stock_id) UPDATE TB_M_STOCK SET name = @name, sub_category_id = @sub_category_id, category_id = @category_id, barcode = @barcode, price = @price, notes = @notes, updated_at = GETDATE() WHERE stock_id = @stock_id
                             ELSE INSERT INTO TB_M_STOCK (stock_id, name, sub_category_id, category_id, barcode, price, notes, created_at) VALUES (@stock_id, @name, @sub_category_id, @category_id, @barcode, @price, @notes, GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@stock_id", SqlDbType.UniqueIdentifier).Value = stock.stock_id == Guid.Empty ? Guid.NewGuid() : stock.stock_id; ;
                        cmd.Parameters.Add("@sub_category_id", SqlDbType.UniqueIdentifier).Value = stock.sub_category_id;
                        cmd.Parameters.Add("@category_id", SqlDbType.UniqueIdentifier).Value = stock.category_id;
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = stock.name;
                        cmd.Parameters.Add("@barcode", SqlDbType.NVarChar, 100).Value = stock.barcode;
                        cmd.Parameters.Add("@price", SqlDbType.Decimal).Value = stock.price;
                        cmd.Parameters.Add("@notes", SqlDbType.NVarChar, 1000).Value = stock.notes;

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

        public string deleteStock(Guid stock_id)
        {
            string status = "failed";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var sql = "UPDATE TB_M_STOCK SET is_deleted = 1, deleted_at = GETDATE() WHERE stock_id = @stock_id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@stock_id", SqlDbType.UniqueIdentifier).Value = stock_id;
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
