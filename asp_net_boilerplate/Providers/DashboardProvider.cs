using asp_net_boilerplate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace asp_net_boilerplate.Provider
{
    public class DashboardProvider
    {
        public List<ChartModel> GetMonthlySales()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

            string sql = @"SELECT YEAR(transaction_date) AS Year, MONTH(transaction_date) AS Month, SUM(grand_total) AS Total FROM TB_T_SALES WHERE is_deleted = 0 GROUP BY YEAR(transaction_date), MONTH(transaction_date) ORDER BY Year, Month";

            List<ChartModel> salesList = new List<ChartModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ChartModel chart = new ChartModel();
                        chart.Year = Convert.ToInt32(rdr["Year"]);
                        chart.Month = Convert.ToInt32(rdr["Month"]);
                        chart.Total = Convert.ToDecimal(rdr["Total"]);
                        salesList.Add(chart);
                    }
                    con.Close();
                }
            }

            return salesList;
        }

        public List<TopCustomerModel> GetTop10SalesByCustomer()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

            string sql = @"SELECT TOP 10 c.name AS CustomerName, COUNT(DISTINCT s.sales_id) AS TotalSales, SUM(si.qty) AS TotalQty, SUM(s.grand_total) AS GrandTotal FROM TB_M_CUSTOMER c INNER JOIN TB_T_SALES s ON c.customer_id = s.customer_id INNER JOIN TB_T_SALESITEM si ON s.sales_id = si.sales_id WHERE c.is_deleted = 0 AND s.is_deleted = 0 AND si.is_deleted = 0 GROUP BY c.name ORDER BY SUM(s.grand_total) DESC";

            List<TopCustomerModel> list = new List<TopCustomerModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new TopCustomerModel
                    {
                        CustomerName = rdr["CustomerName"].ToString(),
                        TotalSales = Convert.ToInt32(rdr["TotalSales"]),
                        TotalQty = Convert.ToDecimal(rdr["TotalQty"]),
                        GrandTotal = Convert.ToDecimal(rdr["GrandTotal"])
                    });
                }
            }

            return list;
        }
    }
}