using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Models
{
    public class Products
    {
        public async Task<List<string>> GetProducts()
        {
            List<string> products = new List<string>();
            string productQuery =
                              "SELECT productType " +
                              "FROM Products";

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(productQuery, conn))
                {
                    await conn.OpenAsync();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        products.Add(rdr["productType"].ToString());
                    }
                }
            }
            return products;
        }
    }
}