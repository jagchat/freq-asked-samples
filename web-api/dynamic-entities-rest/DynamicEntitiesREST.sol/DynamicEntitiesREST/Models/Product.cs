﻿using DynamicEntitiesREST.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicEntitiesREST.Models
{
    public class Product : BaseEntity //TODO: just for testing..can be removed
    {
        public Product()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}