using System.Configuration;
using System.Data.SqlClient;

class StorageDb
{
    private SqlConnection connection;
    public StorageDb(string connString)
    {
        connection = new SqlConnection(connString);
        connection.Open();s
        Console.WriteLine("true");
    }
}
internal class Program
{
    private static void Main(string[] args)
    {
        StorageDb s = new StorageDb(ConfigurationManager.ConnectionStrings["StorageDb"].ConnectionString);
    }
}