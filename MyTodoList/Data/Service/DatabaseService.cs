using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace MyTodoList.Data;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
        InitializeDatabaseExists();
    }

    public IDbConnection OpenConnection() => new SqlConnection(_connectionString);

    private void InitializeDatabaseExists()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        var databaseName = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        using var connection = new SqlConnection(builder.ConnectionString);
        connection.Open();

        var exists = connection.ExecuteScalar<bool>(
            "SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM sys.databases WHERE name = @name) THEN 1 ELSE 0 END AS BIT)",
            new { name = databaseName });

        if (!exists)
        {
            connection.Execute($"CREATE DATABASE {databaseName}");
            AddInitialTables();
            AddInitialCategories();
        }
    }

    private void AddInitialTables()
    {
        using var connection = OpenConnection();

        const string query = """
                             USE MyTodoListDB;

                             IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'Categories')
                             CREATE TABLE Categories
                             (
                                Id INT PRIMARY KEY IDENTITY,
                                Name NVARCHAR(MAX) NOT NULL
                             );

                             IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'Jobs')
                             CREATE TABLE Jobs
                             (
                                Id INT PRIMARY KEY IDENTITY,
                                Name NVARCHAR(MAX) NOT NULL,
                                CategoryId INT,
                                IsDone BIT NOT NULL,
                                CONSTRAINT FK_Jobs_Categories FOREIGN KEY (CategoryId)
                                REFERENCES Categories(Id)
                             );
                             """;

        connection.Execute(query);
    }

    private void AddInitialCategories()
    {
        using var connection = OpenConnection();

        var categories = new List<string> { "Дім", "Робота", "Навчання" };

        foreach (var category in categories)
        {
            const string query = "INSERT INTO Categories (Name) VALUES (@Name)";
            connection.Execute(query, new { Name = category });
        }
    }

}