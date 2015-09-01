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
    public class DBProducts : DBData
    {
        public List<Products> GetAllProducts()
        {
            List<Products> productList = new List<Products>();
            try
            {
                var a = db.APIDetails.Where(x => x.CategoryId == 3).FirstOrDefault().ServiceUrl;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {

            }

            return productList;
        }

        public bool InsertProduct(int categoryId, int providerId, Products product)
        {
            try
            {
                ProductMaster productMaster = db.ProductMasters.Where(x => x.ModelNumber == product.ModelNumber).SingleOrDefault();
                if (productMaster == null)
                {
                    Manufacturer manufacturer = db.Manufacturers.Where(x => x.Name == product.Manufacturer).SingleOrDefault();
                    if (manufacturer == null)
                    {
                        manufacturer = new Manufacturer();
                        manufacturer.Name = product.Manufacturer;
                        manufacturer.CreatedOn = System.DateTime.Now;
                        manufacturer.IsActive = true;
                        db.Manufacturers.Add(manufacturer);
                    }

                    productMaster = new ProductMaster();

                    if (product.CategoryPath != null && product.CategoryPath.Count > 3)
                    {
                        Category subCategory =  db.Categories.ToList().Where(x => x.Name == product.CategoryPath[3].Name.ToString()).SingleOrDefault();
                        if (subCategory != null)
                            productMaster.SubCategoryId = subCategory.CategoryId;
                    }
                   
                    productMaster.CategoryId = categoryId;
                    productMaster.ManufacturerId = manufacturer.ManufacturerId;
                   
                    productMaster.Name = product.Name;
                    productMaster.LongDescription = product.ShortDescription;
                    productMaster.ModelNumber = product.ModelNumber;
                    productMaster.Image = product.Image;
                    productMaster.CreatedOn = System.DateTime.Now;
                    db.ProductMasters.Add(productMaster);

                    ProductLink productLink = new ProductLink();
                    productLink.ProductId = productMaster.ProductId;
                    productLink.ProviderId = providerId;
                    productLink.SoruceUrl = product.Url;
                    productLink.MobileUrl = product.MobileUrl;
                    productLink.CreatedOn = System.DateTime.Now;
                    productLink.IsActive = true;
                    db.ProductLinks.Add(productLink);

                    ProductPricing productPricing = new ProductPricing();
                    productPricing.ProductId = productMaster.ProductId;
                    productPricing.ProviderId = providerId;
                    productPricing.SKU = product.Sku;
                    productPricing.RegularPrice = product.RegularPrice;
                    productPricing.SalePrice = product.SalePrice;
                    productPricing.AsofDate = System.DateTime.Now;
                    db.ProductPricings.Add(productPricing);

                    db.SaveChanges();                                       
                }

                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
