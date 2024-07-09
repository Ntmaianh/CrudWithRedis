using Redis.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.DAL
{
    public interface IRepository
    {
        public List<Product> GetProducts();
        public Product GetProductById(Guid id);
        public bool Create(Product product);
        public bool Update(Product product);
        public bool Delete(Guid id);
    }
}
