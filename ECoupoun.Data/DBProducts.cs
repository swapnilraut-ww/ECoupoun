using ECoupoun.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
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

        public int InsertProduct(int categoryId, int providerId, List<Products> productList, ECoupounEntities db)
        {
            int i = 0;
            foreach (var product in productList)
            {
                string table = string.Empty;
                try
                {
                    ProductMaster productMaster = db.ProductMasters.Where(x => x.ModelNumber == product.ModelNumber).SingleOrDefault();
                    if (productMaster == null)
                    {
                        productMaster = new ProductMaster();

                        Manufacturer manufacturer = db.Manufacturers.Where(x => x.Name == product.Manufacturer).SingleOrDefault();
                        if (manufacturer != null)
                        {
                            productMaster.ManufacturerId = manufacturer.ManufacturerId;
                        }
                        else if (manufacturer == null && product.Manufacturer != null)
                        {
                            manufacturer = new Manufacturer();
                            manufacturer.Name = product.Manufacturer;
                            manufacturer.CreatedOn = System.DateTime.Now;
                            manufacturer.IsActive = true;
                            db.Manufacturers.Add(manufacturer);

                            productMaster.ManufacturerId = manufacturer.ManufacturerId;
                        }
                        table = "manufacturer";

                        if (product.CategoryPath != null && product.CategoryPath.Count > 3)
                        {
                            Category subCategory = db.Categories.Where(x => x.IsActive == true).ToList().Where(x => x.Name == product.CategoryPath[3].Name.ToString()).SingleOrDefault();
                            if (subCategory != null)
                                productMaster.SubCategoryId = subCategory.CategoryId;
                        }

                        productMaster.CategoryId = categoryId;
                        productMaster.Name = product.Name;
                        productMaster.LongDescription = product.ShortDescription;
                        productMaster.ModelNumber = product.ModelNumber;
                        productMaster.Image = product.Image;
                        productMaster.Color = product.Color;
                        productMaster.Size = string.IsNullOrWhiteSpace(product.ScreenSizeIn) ? 0 : Convert.ToDouble(product.ScreenSizeIn);
                        productMaster.CreatedOn = System.DateTime.Now;
                        db.ProductMasters.Add(productMaster);
                        table = "ProductMasters";

                        ProductLink productLink = new ProductLink();
                        productLink.ProductId = productMaster.ProductId;
                        productLink.ProviderId = providerId;
                        productLink.SoruceUrl = product.Url;
                        productLink.MobileUrl = product.MobileUrl;
                        productLink.CreatedOn = System.DateTime.Now;
                        productLink.IsActive = true;
                        db.ProductLinks.Add(productLink);
                        table = "ProductLink";

                        ProductPricing productPricing = new ProductPricing();
                        productPricing.ProductId = productMaster.ProductId;
                        productPricing.ProviderId = providerId;
                        productPricing.SKU = product.Sku;
                        productPricing.RegularPrice = product.RegularPrice;
                        productPricing.SalePrice = product.SalePrice;
                        productPricing.AsofDate = System.DateTime.Now;
                        db.ProductPricings.Add(productPricing);
                        table = "ProductPricing";

                        db.SaveChanges();

                        i++;
                    }

                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    return i;
                }
            }

            return i;
        }
    }
}
