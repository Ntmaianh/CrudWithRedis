using Redis.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.DAL
{
    public class Repository : IRepository
    {
        DemoRedisContext _context;
        public Repository()
        {
            _context = new DemoRedisContext();
        }
        public Product GetProductById(Guid id)
        {
            try
            {
                Product product = _context.Products.Find(id);
                if (product == null)
                {
                    throw new Exception("Not found");
                }
                return product;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Product> GetProducts()
        {
            try
            {
                List<Product> lstProduct = _context.Products.ToList();
                return lstProduct;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool Create(Product product)
        {
            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public bool Delete(Guid id)
        {
            try
            {

                Product productDelete = _context.Products.Find(id);
                if (productDelete == null)
                {
                    return false;
                }
                _context.Products.Remove(productDelete);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        public bool Update(Product product)
        {
            try
            {
                var productUpdate = _context.Products.Find(product.Id);
                if (productUpdate != null)
                {
                    productUpdate.Name = product.Name;
                    productUpdate.Description = product.Description;
                    _context.Products.Update(productUpdate);
                    _context.SaveChanges();

                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
