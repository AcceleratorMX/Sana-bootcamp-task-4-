using Dapper;
using MyTodoList.Data;
using MyTodoList.Data.Models;
using MyTodoList.Interfaces;

namespace MyTodoList.Repositories;

public class CategoryRepository(DatabaseService databaseService) : ICategoryRepository
{
    private readonly DatabaseService _databaseService = databaseService;

    public async Task<IEnumerable<Category>> GetCategories()
    {
        using var db = _databaseService.OpenConnection();
        return await db.QueryAsync<Category>("SELECT Id, Name FROM Categories");
    }
}