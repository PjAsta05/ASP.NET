using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using asp_net_boilerplate.Models;
using asp_net_boilerplate.Provider;

namespace asp_net_boilerplate.Provider
{
    public class FunctionalProvider : Controller
    {
		// GET: FunctionalProvider

		public List<tag_number_model> getTagNumberList(string entity_id)
		{
			string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

			var sql = "SELECT tag_number FROM TB_M_ASSET WHERE entity_id = @entity_id";

			List<tag_number_model> tag_number_list = new List<tag_number_model>();

			using (SqlConnection con = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					con.Open();
					cmd.Parameters.Add("@entity_id", SqlDbType.NVarChar).Value = entity_id;
					SqlDataReader rdr = cmd.ExecuteReader();
					while (rdr.Read())
					{
						tag_number_model tag_number = new tag_number_model();

						tag_number.tag_number = rdr["tag_number"].ToString();

						tag_number_list.Add(tag_number);
					}
					con.Close();
				}
			}

			return tag_number_list;
		}

		public List<object_type_model> getObjectTypeList(string entity_id)
		{
			string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

			var sql = "SELECT DISTINCT object_type FROM TB_M_ASSET WHERE entity_id = @entity_id";

			List<object_type_model> object_type_list = new List<object_type_model>();

			using (SqlConnection con = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					con.Open();
					cmd.Parameters.Add("@entity_id", SqlDbType.NVarChar).Value = entity_id;
					SqlDataReader rdr = cmd.ExecuteReader();
					while (rdr.Read())
					{
						object_type_model object_type = new object_type_model();

						object_type.object_type = rdr["object_type"].ToString();

						object_type_list.Add(object_type);
					}
					con.Close();
				}
			}

			return object_type_list;
		}

		public List<cat_model> getCatList(string entity_id)
		{
			string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

			var sql = "SELECT DISTINCT cat FROM TB_M_ASSET WHERE entity_id = @entity_id";

			List<cat_model> cat_list = new List<cat_model>();

			using (SqlConnection con = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					con.Open();
					cmd.Parameters.Add("@entity_id", SqlDbType.NVarChar).Value = entity_id;
					SqlDataReader rdr = cmd.ExecuteReader();
					while (rdr.Read())
					{
						cat_model cat = new cat_model();

						cat.cat = rdr["cat"].ToString();

						cat_list.Add(cat);
					}
					con.Close();
				}
			}

			return cat_list;
		}
	}
}