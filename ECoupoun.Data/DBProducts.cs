using ECoupoun.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECoupoun.Data
{
    public class DBProducts : DBHelper
    {
        public List<Products> GetAllProducts()
        {
            List<Products> productList = new List<Products>();
            try
            {
                OpenConnection();
                objCommand = new SqlCommand("GetAllProducts", objConnection);
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;

                objReader = objCommand.ExecuteReader();
                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {
                        Products product = new Products();
                        product.Sku = Convert.ToInt32(objReader["Sku"].ToString());
                        product.Name = objReader["Name"].ToString();
                        product.Image = objReader["Image"].ToString();
                        product.ModelNumber = objReader["ModelNumber"].ToString();
                        product.RegularPrice = Convert.ToDouble(objReader["RegularPrice"]);
                        product.SalePrice = Convert.ToDouble(objReader["SalePrice"]);
                        productList.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (objConnection != null)
                {
                    objConnection.Close();
                }
            }

            return productList;
        }

        public bool InsertProduct(Products products)
        {
            try
            {
                OpenConnection();
                objCommand = new SqlCommand("InsertProduct", objConnection);
                objCommand.CommandType = System.Data.CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("Sku", products.Sku);
                objCommand.Parameters.AddWithValue("Name", products.Name);
                objCommand.Parameters.AddWithValue("Image", products.Image);
                objCommand.Parameters.AddWithValue("ModelNumber", products.ModelNumber);
                objCommand.Parameters.AddWithValue("RegularPrice", products.RegularPrice);
                objCommand.Parameters.AddWithValue("SalePrice", products.SalePrice);

                int data = objCommand.ExecuteNonQuery();
                if (data > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (objConnection != null)
                {
                    objConnection.Close();
                }
            }
            return false;
        }
    }
}
