using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StokTakipUygulamasi.Entity;
using System.Data;
using System.Data.SqlClient;

namespace StokTakipUygulamasi.ORM
{
    public class ProductORM : ORMBase<Products>
    {
        public DataTable ProductInCategory(int categoryID)
        {
            SqlDataAdapter adap = new SqlDataAdapter("pr_ProductInCategory_List", Tools.Connection);
            adap.SelectCommand.CommandType = CommandType.StoredProcedure;
            adap.SelectCommand.Parameters.AddWithValue("@CategoryID", categoryID);
            DataTable dt = new DataTable();
            adap.Fill(dt);
            return dt;
        }
        public DataTable ProductSearch(int categoryID, string searchText)
        {
            SqlDataAdapter adap = new SqlDataAdapter("pr_Product_Search", Tools.Connection);
            adap.SelectCommand.CommandType = CommandType.StoredProcedure;
            adap.SelectCommand.Parameters.AddWithValue("@CategoryID", categoryID);
            adap.SelectCommand.Parameters.AddWithValue("@text", searchText);
            DataTable dt = new DataTable();
            adap.Fill(dt);
            return dt;
        }
        public bool StockDecrease(int productID, int productQuantity)
        {
            SqlCommand cmd = new SqlCommand("pr_ProductStockDecrease", Tools.Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productID);
            cmd.Parameters.AddWithValue("@ProductQuantity", productQuantity);
            return Tools.ExecuteNonQuery(cmd);
        }
    }
}
