using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ODataWebAPI.Models
{
    public class ProductsContext : DbContext
    {
        public ProductsContext(): base("name=ProductsContext")
        {
        }
        public DbSet<Products> products { get; set; }
    }
}