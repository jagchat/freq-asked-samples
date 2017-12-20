using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicEntitiesREST.Models;

namespace DynamicEntitiesREST.Repositories
{
    public class ProductRepository //TODO: just for testing..can be removed
    {
        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        public Product[] GetAll()
        {
            return products;
        }

        public Product GetById(string id)
        {
            var product = products.FirstOrDefault((p) => p.Id == Convert.ToInt16(id));
            return product;
        }
    }
}