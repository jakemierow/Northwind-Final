using System.Data.Entity;

namespace NorthwindConsole.Models
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext() : base("name=NorthwindContext") { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public void addCategory(Category category)
        {
            this.Categories.Add(category);
            this.SaveChanges();
        }

        public void addProduct(Product product)
        {
            this.Products.Add(product);
            this.SaveChanges();
        }


    }
}
