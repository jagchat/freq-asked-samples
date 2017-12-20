using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicEntitiesREST.Repositories;

namespace DynamicEntitiesREST.Infrastructure
{
    public class EntityRepository //default Repository, but invokes others (TODO: invoke dynamically using DI/Config)
    {
        private string entityType { get; set; }

        public EntityRepository(string entityTypeName)
        {
            entityType = entityTypeName;
        }

        public object GetAll()
        {
            //if (entityType == "products") //TODO: remove this..just for testing
            //{
            //    return (new ProductRepository()).GetAll();
            //}
            //return null;

            //TODO: use DI / config to figure out Repository dynamically
            return (new DbRepository()).GetAll(entityType);
        }

        public object GetById(string id)
        {
            //if (entityType == "products")//TODO: remove this..just for testing
            //{
            //    return (new ProductRepository()).GetById(id);
            //}
            //return null;

            //TODO: use DI / config to figure out Repository dynamically
            return (new DbRepository()).GetById(entityType, id);
        }
    }

}