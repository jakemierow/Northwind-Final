using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using NLog;
using NorthwindConsole.Models;

namespace NorthwindConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Edit Category");
                    Console.WriteLine("6) Delete Category");
                    Console.WriteLine("7) Add Product");
                    Console.WriteLine("8) Edit Product");
                    Console.WriteLine("9) Display a specified Product");
                    Console.WriteLine("10) Display Products");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                    }
                    else if (choice == "2")
                    {
                        Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.addCategory(category);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }

                    }
                    else if (choice == "5")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category ID you want to edit:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);

                        Console.WriteLine("Enter the new category name:");
                        var name = Console.ReadLine();
                        logger.Info($"Category Name {name} entered");

                        category.CategoryName = name;
                        db.SaveChanges();
                        logger.Info($"Category Name {name} updated");
                    }
                    else if (choice == "6")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category ID you want to delete:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);

                        db.Categories.Remove(category);
                        db.SaveChanges();
                        logger.Info($"Category Id {id} deleted");
                    }
                    else if (choice == "7")
                    {
                        Product product = new Product();
                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);

                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                var queryC = db.Categories.OrderBy(p => p.CategoryId);

                                foreach (var item in queryC)
                                {
                                    Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                                }

                                try
                                {
                                    int value = 0;
                                    Console.WriteLine("Please enter the desired category id: ");
                                    bool valid = int.TryParse(Console.ReadLine(), out value);
                                    bool existing = db.Categories.Any(c => c.CategoryId == value);

                                    if (valid && existing)
                                    {
                                        product.CategoryId = value;
                                    }
                                    else
                                    {
                                        logger.Info("Category ID must be a number value that is listed");
                                    }

                                }
                                catch (Exception e)
                                {
                                    logger.Info("Category ID error : " + e);
                                }

                                var queryS = db.Suppliers.OrderBy(s => s.SupplierId);

                                foreach (var item in queryS)
                                {
                                    Console.WriteLine($"{item.SupplierId}) {item.CompanyName}");
                                }

                                try
                                {
                                    int value = 0;
                                    Console.WriteLine("Please select the product Supplier ID: ");
                                    bool valid = int.TryParse(Console.ReadLine(), out value);
                                    bool existing = db.Suppliers.Any(s => s.SupplierId == value);

                                    if (valid && existing)
                                    {
                                        product.SupplierId = value;
                                    }
                                    else
                                    {
                                        logger.Info("Supplier ID must be a number value that is listed");
                                    }

                                }
                                catch (Exception e)
                                {
                                    logger.Info("Supplier ID error: " + e);
                                }



                                Console.WriteLine("Enter a product name:");
                                product.ProductName = Console.ReadLine();

                                Console.WriteLine("Enter Quantity per unit:");
                                product.QuantityPerUnit = Console.ReadLine();

                                Console.WriteLine("Enter unit price:");
                                try
                                {
                                    decimal value = 0;
                                    Console.WriteLine("What is the unit price?");
                                    bool valid = decimal.TryParse(Console.ReadLine(), out value);
                                    if (valid)
                                    {
                                        product.UnitPrice = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Unit price must be a numeric value");
                                    }
                                    Console.Clear();
                                }
                                catch (Exception e)

                                {
                                    logger.Info("Unit Price error: " + e);
                                }


                                try
                                {
                                    Int16 value = 0;
                                    Console.WriteLine("Enter units in stock:");
                                    bool valid = Int16.TryParse(Console.ReadLine(), out value);

                                    if (valid)
                                    {
                                        product.UnitsInStock = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Units in stock must be a numeric value");
                                    }

                                    Console.Clear();
                                }
                                catch (Exception e)
                                {
                                    logger.Info("Units in stock error: " + e);
                                }



                                try
                                {
                                    Int16 value = 0;
                                    Console.WriteLine("What are the units on order?");
                                    bool valid = Int16.TryParse(Console.ReadLine(), out value);

                                    if (valid)
                                    {
                                        product.UnitsOnOrder = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Units on order must be a numeric value");
                                    }

                                    Console.Clear();
                                }
                                catch (Exception e)
                                {
                                    logger.Info("Units on order error " + e);
                                }


                                try
                                {
                                    Int16 value = 0;
                                    Console.WriteLine("Enter reorder level");
                                    bool valid = Int16.TryParse(Console.ReadLine(), out value);

                                    if (valid)
                                    {
                                        product.ReorderLevel = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Reorder level must be a numeric value");
                                    }

                                    Console.Clear();
                                }
                                catch (Exception e)
                                {
                                    logger.Info("Reorder level error: " + e);
                                }

                                db.addProduct(product);
                                db.SaveChanges();
                                logger.Info("Product successfully added");


                            }
                        }
                    }
                    else if (choice == "8")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                 
                        Console.WriteLine("Categories:");

                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }

                        try
                        {
                            int value = 0;
                            Console.WriteLine("Enter the Category ID for the Product you wish to edit: ");
                            bool valid = int.TryParse(Console.ReadLine(), out value);
                            bool existing = db.Categories.Any(c => c.CategoryId == value);

                            if (valid && existing)
                            {
                                var productQuery = db.Products.Where(p => p.CategoryId == value).OrderBy(p => p.ProductID);
                                foreach (var item in productQuery)
                                {
                                    Console.WriteLine($"{item.ProductID}) {item.ProductName}");
                                }

                                int productId = 0;
                                Console.WriteLine("Enter the Product ID to edit: ");
                                bool validProdID = int.TryParse(Console.ReadLine(), out productId);
                                bool existingProdID = db.Products.Any(p => p.ProductID == productId);

                                if (validProdID && existingProdID)
                                {
                                    var editProduct = db.Products.First(p => p.ProductID == productId);
                                    int entry;

                                    do
                                    {
                                        ProductDetails(editProduct, db);
                                        Console.WriteLine("Enter the field that you wish to change :");
                                        entry = int.Parse(Console.ReadLine());
                                        Console.Clear();

                                        switch (entry)
                                        {
                                            case 1:
                                                Console.WriteLine($"1) Product name: {editProduct.ProductName}");
                                                Console.WriteLine("Enter new Product name");
                                                editProduct.ProductName = Console.ReadLine();
                                                break;

                                            case 2:
                                                var queryCat = db.Categories.First(c => c.CategoryId == editProduct.CategoryId);
                                                Console.WriteLine($"2) Category {editProduct.CategoryId} {queryCat.CategoryName}");

                                                var cQuery = db.Categories.OrderBy(p => p.CategoryId);

                                                foreach (var item in cQuery)
                                                {
                                                    Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                                                }

                                                Console.WriteLine("Please enter the new category that the product belongs to: ");
                                                valid = int.TryParse(Console.ReadLine(), out value);
                                                existing = db.Categories.Any(c => c.CategoryId == value);

                                                if (valid && existing)
                                                {
                                                    editProduct.CategoryId = value;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Category ID must be a numeric value listed");
                                                }
                                                break;

                                            case 3:
                                                var querySup = db.Suppliers.First(s => s.SupplierId == editProduct.SupplierId);
                                                Console.WriteLine($"3) Supplier {editProduct.SupplierId} {querySup.CompanyName}");

                                                var queryS = db.Suppliers.OrderBy(s => s.SupplierId);

                                                foreach (var item in queryS)
                                                {
                                                    Console.WriteLine($"{item.SupplierId}) {item.CompanyName}");
                                                }

                                                Console.WriteLine("Please enter the new Supplier id: ");
                                                valid = int.TryParse(Console.ReadLine(), out value);
                                                existing = db.Suppliers.Any(s => s.SupplierId == value);

                                                if (valid && existing)
                                                {
                                                    editProduct.SupplierId = value;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Category ID must be a numeric value listed.");
                                                }

                                                break;

                                            case 4:
                                                Console.WriteLine($"4) Quantity per Unit: {editProduct.QuantityPerUnit}");
                                                Console.WriteLine("Enter new Quantity per Unit: ");
                                                editProduct.QuantityPerUnit = Console.ReadLine();
                                                break;

                                            case 5:
                                                Console.WriteLine($"5) Unit Price: {editProduct.UnitPrice}");
                                                Console.WriteLine("Enter new Unit Price: ");
                                                editProduct.UnitPrice = decimal.Parse(Console.ReadLine());
                                                break;

                                            case 6:
                                                Console.WriteLine($"6) Units in Stock: {editProduct.UnitsInStock}");
                                                Console.WriteLine("Enter new Units in Stock: ");
                                                editProduct.UnitsInStock = Int16.Parse(Console.ReadLine());
                                                break;

                                            case 7:
                                                Console.WriteLine($"7) Units On order: {editProduct.UnitsOnOrder}");
                                                Console.WriteLine("Enter new Units on Order: ");
                                                editProduct.UnitsOnOrder = Int16.Parse(Console.ReadLine());
                                                break;

                                            case 8:
                                                Console.WriteLine($"8) Reorder Level: {editProduct.ReorderLevel}");
                                                Console.WriteLine("Enter new Reorder Level: ");
                                                editProduct.ReorderLevel = Int16.Parse(Console.ReadLine());
                                                break;

                                            case 9:
                                                Console.WriteLine($"9) Discontinued Status: {editProduct.Discontinued}");
                                                Console.WriteLine("Enter Status (true = Active / false = Discontinued : ");
                                                editProduct.Discontinued = Boolean.Parse(Console.ReadLine());
                                                break;
                                        }

                                    } while (entry != 0);

                                    db.Entry(editProduct).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                            }
                            else
                            {
                                logger.Info("Please enter a valid Category ID");
                            }

                        }
                        catch (Exception e)
                        {
                            logger.Info("Error : " + e);
                        }

                    }
                    else if (choice == "9")
                    {
                        int value = 0;
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }

                        Console.WriteLine("Enter the category id of the product you wish to display: ");

                        bool valid = int.TryParse(Console.ReadLine(), out value);
                        bool existing = db.Categories.Any(c => c.CategoryId == value);

                        if (valid && existing)
                        {
                            var productQuery = db.Products.Where(p => p.CategoryId == value).OrderBy(p => p.ProductID);
                            foreach (var item in productQuery)
                            {
                                Console.WriteLine($"{item.ProductID}) {item.ProductName}");
                            }
                            int ProdId = 0;
                            Console.WriteLine("Enter the Product ID of desired product: ");
                            bool validProdId = int.TryParse(Console.ReadLine(), out ProdId);
                            bool existingProdId = db.Products.Any(p => p.ProductID == ProdId);

                            if(validProdId && existingProdId)
                            {
                                ProductDetails(db.Products.First(p => p.ProductID == ProdId), db);
                            }

                            else
                            {
                                logger.Info("Please enter a valid Product ID value");
                            }

                        }
                        else
                        {
                            logger.Info("Please enter a valid Category ID value");
                        }




                    }

                    else if(choice == "10")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);
                        foreach(var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.WriteLine("Enter the category id of the products you wish to display: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"Category ID {id} selected");


                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");

                        Console.WriteLine("please select one of the following:");
                        Console.WriteLine("A) Display All Products");
                        Console.WriteLine("B) Display Active Products");
                        Console.WriteLine("C) Display Discontinued products");
                        var entry = Console.ReadLine().ToUpper();

                        logger.Info("Option " + entry + " selected");


                        try
                        {
                            switch(entry)
                            {
                                case "A":
                                    Console.WriteLine("All Products:");
                                    foreach(Product p in category.Products)
                                    {
                                        Console.WriteLine(p.ProductName);
                                    }
                                    break;

                                case "B":
                                    Console.WriteLine("Active Products:");
                                    foreach (Product p in category.Products.Where(p => p.Discontinued.Equals(false)))
                                    {
                                        Console.WriteLine(p.ProductName);
                                    }
                                    break;

                                case "C":
                                    Console.WriteLine("Discontinued Products:");
                                    foreach (Product p in category.Products.Where(p => p.Discontinued.Equals(true)))
                                    {
                                        Console.WriteLine(p.ProductName);
                                    }
                                    break;

                            }

                        }

                        catch (Exception e)
                        {
                            logger.Info("product information not found" + e);
                        }

    
                        
                    }



                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }

        public static void ProductDetails(Product product, NorthwindContext context)
        {
            Console.WriteLine($"Product ID: {product.ProductID}");
            Console.WriteLine($"1) Product name: {product.ProductName}");
            var catQuery = context.Categories.First(c => c.CategoryId == product.CategoryId);
            Console.WriteLine($"2) Category {product.CategoryId} {catQuery.CategoryName}");
            var SupplierQuery = context.Suppliers.First(s => s.SupplierId == product.SupplierId);
            Console.WriteLine($"3) Supplier {product.SupplierId} {SupplierQuery.CompanyName}");
            Console.WriteLine($"4) Quantity per Unit: {product.QuantityPerUnit}");
            Console.WriteLine($"5) Unit Price: {product.UnitPrice}");
            Console.WriteLine($"6) Units in Stock: {product.UnitsInStock}");
            Console.WriteLine($"7) Units On order: {product.UnitsOnOrder}");
            Console.WriteLine($"8) Reorder Level: {product.ReorderLevel}");
            Console.WriteLine($"9) Discontinued Status: {product.Discontinued}");
            Console.WriteLine("0) Save Product and return to main menu");
        }




    }
}

