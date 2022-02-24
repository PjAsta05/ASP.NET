using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using asp_net_boilerplate.Models;
using asp_net_boilerplate.DataCenter;

namespace asp_net_boilerplate.DataCenter
{
    public class AssetDataCenter : Controller
    {
		// GET: AssetDataCenter
		public List<tb_m_asset_model> getAssetList(string entity_id, int page = 1, int display_row = 10, string tag_number = "")
		{
			string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

			var sql = "EXEC S_ASSET_LOAD @entity_id, @offset, @fetch, @tag_number";

			List<tb_m_asset_model> tb_m_asset_list = new List<tb_m_asset_model>();

			using (SqlConnection con = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					con.Open();
					cmd.Parameters.Add("@entity_id", SqlDbType.NVarChar).Value = entity_id;
					cmd.Parameters.Add("@offset", SqlDbType.NVarChar).Value = page;
					cmd.Parameters.Add("@fetch", SqlDbType.NVarChar).Value = display_row;
					cmd.Parameters.Add("@tag_number", SqlDbType.NVarChar).Value = tag_number;
					SqlDataReader rdr = cmd.ExecuteReader();
					while (rdr.Read())
					{
						tb_m_asset_model tb_m_asset = new tb_m_asset_model();

						tb_m_asset.equipment = (int)rdr["equipment"];
						tb_m_asset.tag_number = rdr["tag_number"].ToString();
						tb_m_asset.description = rdr["description"].ToString();
						tb_m_asset.cat = rdr["cat"].ToString();
						tb_m_asset.object_type = rdr["object_type"].ToString();
						tb_m_asset.entity_id = rdr["entity_id"].ToString();
						tb_m_asset.action =
							"<button type='button' onclick='EditData(" + ((int)rdr["equipment"]).ToString() + ")'>Edit</a>" +
							"<button type='button' onclick='DeleteData(" + ((int)rdr["equipment"]).ToString() + ")'>Delete</a>";

						tb_m_asset_list.Add(tb_m_asset);
					}
					con.Close();
				}
			}

			return tb_m_asset_list;
		}

		public tb_m_asset_model getAsset(int equipment, string entity_id)
		{
			string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

			var sql = "SELECT * FROM TB_M_ASSET WHERE equipment = @equipment AND entity_id = @entity_id";

			tb_m_asset_model tb_m_asset = new tb_m_asset_model();

			using (SqlConnection con = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					con.Open();
					cmd.Parameters.Add("@equipment", SqlDbType.Int).Value = equipment;
					cmd.Parameters.Add("@entity_id", SqlDbType.NVarChar).Value = entity_id;
					SqlDataReader rdr = cmd.ExecuteReader();
					if (rdr.Read())
					{
						tb_m_asset.equipment = (int)rdr["equipment"];
						tb_m_asset.tag_number = rdr["tag_number"].ToString();
						tb_m_asset.description = rdr["description"].ToString();
						tb_m_asset.cat = rdr["cat"].ToString();
						tb_m_asset.object_type = rdr["object_type"].ToString();
						tb_m_asset.entity_id = rdr["entity_id"].ToString();
					}
					con.Close();
				}
			}

			return tb_m_asset;
		}

		public string storeAsset(tb_m_asset_model tb_m_asset)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

            var status = "failed";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();
                    string query = "EXEC S_ASSET_SAVE @equipment, @tag_number, @description, @cat, @object_type, @entity_id";
                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {
                        cmd.Parameters.Add("@equipment", SqlDbType.Int).Value = tb_m_asset.equipment;
                        cmd.Parameters.Add("@tag_number", SqlDbType.NVarChar).Value = tb_m_asset.tag_number;
                        cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = tb_m_asset.description;
                        cmd.Parameters.Add("@cat", SqlDbType.NVarChar).Value = tb_m_asset.cat;
                        cmd.Parameters.Add("@object_type", SqlDbType.NVarChar).Value = tb_m_asset.object_type;
                        cmd.Parameters.Add("@entity_id", SqlDbType.NVarChar).Value = tb_m_asset.entity_id;

                        int rowsAdded = cmd.ExecuteNonQuery();

                        if (rowsAdded > 0) { status = "success"; } else { status = "failed"; }
                    }
                    cnn.Close();
                }
                catch (Exception ex)
                {
                    status = ex.Message;
                }
            }
            return status;
        }

        public string deleteAsset(int equipment, string entity_id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString;

            var status = "failed";

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    cnn.Open();
                    string query = "DELETE FROM TB_M_ASSET WHERE equipment = @equipment AND entity_id = @entity_id";
                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {
                        cmd.Parameters.Add("@equipment", SqlDbType.Int).Value = equipment;
                        cmd.Parameters.Add("@entity_id", SqlDbType.NVarChar).Value = entity_id;
						int deleteData = cmd.ExecuteNonQuery();
						if (deleteData > 0) { status = "success"; } else { status = "failed"; }
					}
					cnn.Close();
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