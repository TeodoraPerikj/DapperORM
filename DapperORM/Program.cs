using System;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace DapperORM
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbConnectionString = "Data Source=(local);Integrated Security=True";

            using (IDbConnection db = new SQLiteConnection(dbConnectionString))
            {
                db.Open();

                db.Execute(@"CREATE TABLE IF NOT EXISTS Categories(
                CategoryID INTEGER PRIMARY KEY AUTOINCREMENT,
                CategoryName TEXT NOT NULL,
                Description TEXT,
                Picture BLOB)");

                db.Execute(@"CREATE TABLE IF NOT EXISTS Products(
                ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductName TEXT NOT NULL,
                SupplierID INTEGER,
                CategoryID INTEGER,
                QuantityPerUnit INTEGER,
                UnitPrice INTEGER,
                UnitInStock INTEGER, 
                UnitOnOrder INTEGER,
                ReorderLevel INTEGER,
                Discontinued BIT,
                LastUserId INTEGER, 
                LastDateUpdated DATE,
                FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID))");

                db.Execute(@"CREATE TABLE IF NOT EXISTS Orders(
                OrderID INTEGER PRIMARY KEY AUTOINCREMENT,
                CustomerID INTEGER, 
                EmployeeID INTEGER,
                OrderDate DATE,
                RequiredDate DATE,
                ShippedDate DATE,
                ShipVia TEXT,
                Freight BIT,
                ShipName TEXT,
                ShipAddress TEXT,
                ShipCity TEXT,
                ShipRegion TEXT,
                ShipPostalCode INTEGER,
                ShipCountry TEXT)");

                db.Execute(@"CREATE TABLE IF NOT EXISTS OrderDetails(
                OrderID INTEGER,
                ProductID INTEGER,
                UnitPrice INTEGER,
                Quantity INTEGER,
                Discount FLOAT,
                PRIMARY KEY (OrderID, ProductID),
                FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
                FOREIGN KEY (ProductID) REFERENCES Products(ProductID))");

                // Getting a list of Orders sorted by Date
                var orders = db.Query<Order>("SELECT * FROM Orders ORDER BY OrderDate");

                //Getting a list of all products sorted by most sold products
                var products = db.Query<Product>("SELECT p.* FROM Products p JOIN OrderDetails o ON p.ProductID= o.ProductID " +
                                        "GROUP BY p.ProductID ORDER BY SUM(o.Quantity) DESC");

                //Getting a list of all categories sorted by most sold products
                var categories = db.Query<Category>("SELECT c.* FROM Categories c JOIN Products p ON c.CategoryID = p.CategoryID " +
                                        "JOIN OrderDetails o ON p.ProductID = o.ProductID GROUP BY c.CategoryID " +
                                        "ORDER BY SUM(o.Quantity) DESC");
            }
        }
    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
    }
    
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int SupplierID { get; set; }
        public int CategoryID { get; set; }
        public int QuantityPerUnit { get; set; }
        public int UnitPrice { get; set; }
        public int UnitInStock { get; set; }
        public int UnitOnOrder { get; set; }
        public int ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public int LastUserId { get; set; }
        public DateTime LastDateUpdated { get; set; }
    }

    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public string ShipVia { get; set; }
        public bool Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public int ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
    }

    public class OrderDetail
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public float Discount { get; set; }
    }
}