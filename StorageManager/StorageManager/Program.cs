using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Xml;


class ProductTypes
{
    public int Id { get; set; }
    public string Type { get; set; }
    public override string ToString()
    {
        return $"Id: {Id}\tType: {Type}";
    }
}
class Suppliers
{
    public int Id { get; set; }
    public string Name { get; set; }
    public override string ToString()
    {
        return $"Id: {Id}\tName: {Name}";
    }
}
class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public int TypeId { get; set; }
    public int SupplierId { get; set; }
    public int Quantity { get; set; }
    public float Cost { get; set; }
    public DateTime SupplyDate { get; set; }
    public override string ToString()
    {
        return $"Id: {Id,-4} Product: {ProductName,-15} TypeId: {TypeId,-3} SupplierId: {SupplierId,-4} " +
            $"Quantity: {Quantity,-4} Cost: {Cost,-5} Date: {SupplyDate.ToString("yyyy.MM.dd"),-5}";
    }
}

class StorageDb

{
    private SqlConnection connection;
    private SqlCommand command;

    public StorageDb(string connString)
    {
        connection = new SqlConnection(connString);
        connection.Open();
        //Console.WriteLine("true");
    }
    // TASK 2
    public void AddProduct(Product product)
    {
        string query = @"insert into Products
                         values(@name, @type, @supplier, @quantity, @cost, @supplyDate)";
        command = new SqlCommand(query, connection);
        SetProductParams(product);
        command.ExecuteNonQuery();
    }

    public void AddProdType(ProductTypes pt)
    {
        string query = @"insert into ProductTypes values(@type)";
        command = new SqlCommand(query, connection);
        command.Parameters.Add("@type", SqlDbType.NVarChar).Value = pt.Type;
        command.ExecuteNonQuery();
    }
    public void AddSupplier(Suppliers supplier)
    {
        string query = "insert into Suppliers values (@name)";
        command = new SqlCommand(query, connection);
        command.Parameters.Add("@name", SqlDbType.NVarChar).Value = supplier.Name;
        command.ExecuteNonQuery();
    }
    private void SetProductParams(Product product)
    {
        command.Parameters.Add("@name", SqlDbType.NVarChar).Value = product.ProductName;
        command.Parameters.Add("@type", SqlDbType.Int).Value = product.TypeId;
        command.Parameters.Add("@supplier", SqlDbType.Int).Value = product.SupplierId;
        command.Parameters.Add("@quantity", SqlDbType.Int).Value = product.Quantity;
        command.Parameters.Add("@cost", SqlDbType.Money).Value = product.Cost;
        command.Parameters.Add("@supplyDate", SqlDbType.Date).Value = product.SupplyDate;
    }
    // READ DATA
    public List<Product> GetAllProducts()
    {
        string query = "select * from Products";
        command = new SqlCommand(query, connection);
        var reader = command.ExecuteReader();
        var products = new List<Product>();

        while (reader.Read())
        {
            products.Add(new Product()
            {
                Id = (int)reader[0],
                ProductName = (string)reader[1],
                TypeId = (int)reader[2],
                SupplierId = (int)reader[3],
                Quantity = (int)reader[4],
                Cost = (float)(decimal)reader[5],
                SupplyDate = (DateTime)reader[6]
            });
        }
        reader.Close();
        return products;
    }
    public List<ProductTypes> GetAllProdTypes()
    {
        string query = "select * from ProductTypes";
        command = new SqlCommand(query, connection);
        var reader = command.ExecuteReader();
        var types = new List<ProductTypes>();
        while (reader.Read())
        {
            types.Add(new ProductTypes()
            {
                Id = (int)reader[0],
                Type = (string)reader[1],
            });
        }
        reader.Close();
        return types;
    }
    public List<Suppliers> GetAllSuppliers()
    {
        string query = "select * from Suppliers";
        command = new SqlCommand(query, connection);
        var reader = command.ExecuteReader();
        var suppliers = new List<Suppliers>();
        while (reader.Read())
        {
            suppliers.Add(new Suppliers()
            {
                Id = (int)reader[0],
                Name = (string)reader[1],
            });
        }
        reader.Close();
        return suppliers;
    }
    // TASK 3
    public void UpdateProduct(Product product)
    {
        string cmdText = @"update Products
                           set
                                ProductName = @name,
                                TypeId = @type,
                                SupplierID = @supplier, 
                                Quantity = @quantity,
                                Cost = @cost,
                                SupplyDate = @supplyDate
                            where Id = @id";

        command = new SqlCommand(cmdText, connection);
        SetProductParams(product);
        command.Parameters.Add("@id", SqlDbType.Int).Value = product.Id;
        command.ExecuteNonQuery();
    }
    public void UpdateProdType(ProductTypes type)
    {

        string query = @"update ProductTypes
                        set Type = @type
                        where Id = @id";
        command = new SqlCommand(query, connection);
        command.Parameters.Add("@id", SqlDbType.Int).Value = type.Id;
        command.Parameters.Add("@type", SqlDbType.NVarChar).Value = type.Type;
        command.ExecuteNonQuery();
    }

    public void UpdateSupplier(Suppliers supplier)
    {
        string query = @"update Suppliers
                        set Name = @name
                        where Id = @id";
        command = new SqlCommand(query, connection);
        command.Parameters.Add("@id", SqlDbType.Int).Value = supplier.Id;
        command.Parameters.Add("@name", SqlDbType.NVarChar).Value = supplier.Name;
        command.ExecuteNonQuery();
    }
    // TASK 4
    public void DeleteProduct(int id)
    {
        string query = @"delete Products
                        where Id = @id";
        command = new SqlCommand(query, connection);
        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
        command.ExecuteNonQuery();
    }

    public void DeleteProductType(int id)
    {
        string query = @"delete ProductTypes
                         where Id = @id";
        command = new SqlCommand(query, connection);
        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
        command.ExecuteNonQuery();
    }

    public void DeleteSupplier(int id)
    {
        string query = @"delete Suppliers 
                        where Id = @id";
        command = new SqlCommand(query, connection);
        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
        command.ExecuteNonQuery();
    }
    ~StorageDb() { connection.Close(); }
}
internal class Program
{
    private static void Main(string[] args)
    {
        StorageDb storage = new StorageDb(ConfigurationManager.ConnectionStrings["Storage"].ConnectionString);

        // ADD
        //ProductTypes pt1 = new ProductTypes() { Type = "Electronics" };
        //ProductTypes pt2 = new ProductTypes() { Type = "Clothing" };
        //ProductTypes pt3 = new ProductTypes() { Type = "Food" };
        //ProductTypes pt4 = new ProductTypes() { Type = "Cosmetics" };
        //storage.AddProdType(pt1);
        //storage.AddProdType(pt2);
        //storage.AddProdType(pt3);
        //storage.AddProdType(pt4);


        //Suppliers supp1 = new Suppliers() { Name = "Samsung" };
        //Suppliers supp2 = new Suppliers() { Name = "Nike" };
        //Suppliers supp3 = new Suppliers() { Name = "Metro" };
        //Suppliers supp4 = new Suppliers() { Name = "Loreal" };
        //storage.AddSupplier(supp1);
        //storage.AddSupplier(supp2);
        //storage.AddSupplier(supp3);
        //storage.AddSupplier(supp4);


        //Product p1 = new Product(){ProductName = "Laptop", TypeId = 1, SupplierId = 1,
        //    Quantity = 10, Cost = 1500, SupplyDate = new DateTime(2024, 3, 10)};

        //Product p2 = new Product(){ProductName = "Smartphone", TypeId = 1, SupplierId = 1,
        //    Quantity = 10, Cost = 1200, SupplyDate = new DateTime(2024, 3, 12)};

        //Product p3 = new Product(){ProductName = "T-shirt", TypeId = 2, SupplierId = 2,
        //    Quantity = 30, Cost = 50, SupplyDate = new DateTime(2024, 3, 5)};

        //Product p4 = new Product(){ProductName = "Meat", TypeId = 3, SupplierId = 3,
        //    Quantity = 50, Cost = 300, SupplyDate = new DateTime(2024, 3, 12)};

        //Product p5 = new Product(){ProductName = "Face Cream", TypeId = 4, SupplierId = 4,
        //    Quantity = 25, Cost = 150, SupplyDate = new DateTime(2024, 3, 11)};

        //storage.AddProduct(p1);
        //storage.AddProduct(p2);
        //storage.AddProduct(p3);
        //storage.AddProduct(p4);
        //storage.AddProduct(p5);

        // UPDATE
        //Product up = new Product()
        //{ Id = 6, ProductName = "Cream", TypeId = 4, SupplierId = 4,
        //    Quantity = 20, Cost = 210, SupplyDate = new DateTime(2024, 2, 15)};
        //storage.UpdateProduct(up);

        //ProductTypes up1 = new ProductTypes()
        //{ Id = 3, Type = "Grocery"};
        //storage.UpdateProdType(up1);

        //Suppliers up2 = new Suppliers()
        //{ Id = 4, Name = "Lancome"};
        //storage.UpdateSupplier(up2);

        // DELETE
        //storage.DeleteProduct(6);
        //storage.DeleteProductType(4);
        //storage.DeleteSupplier(4);

        // SELECT * FROM...
        Console.WriteLine("\n");
        var prods = storage.GetAllProducts();
        prods.ForEach(p => Console.WriteLine(p.ToString()));
        Console.WriteLine();
        var types = storage.GetAllProdTypes();
        types.ForEach(p => Console.WriteLine(p.ToString()));
        Console.WriteLine();
        var supps = storage.GetAllSuppliers();
        supps.ForEach(p => Console.WriteLine(p.ToString()));
    }
}