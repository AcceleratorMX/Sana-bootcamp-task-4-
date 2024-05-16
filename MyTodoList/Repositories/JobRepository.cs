using Dapper;
using MyTodoList.Data;
using MyTodoList.Data.Models;
using MyTodoList.Interfaces;

namespace MyTodoList.Repositories;

public class JobRepository(DatabaseService databaseService) : IJobRepository
{
    private readonly DatabaseService _databaseService = databaseService;

    public async Task<int> AddJob(Job job)
    {
        using var db = _databaseService.OpenConnection();
        const string query = "INSERT INTO Jobs (Name, CategoryId, IsDone) VALUES (@Name, @CategoryId, @IsDone)";

        return await db.ExecuteAsync(query, job);
    }


    public async Task<Job> GetJob(int id)
    {
        using var db = _databaseService.OpenConnection();
        const string query = "SELECT Id, Name, CategoryId, IsDone FROM Jobs WHERE Id = @Id ";
        return await db.QueryFirstOrDefaultAsync<Job>(query, new { Id = id }) ?? 
               throw new InvalidOperationException($"Job with id {id} not found!");
    }


    public async Task<IEnumerable<Job>> GetJobs()
    {
        using var connection = _databaseService.OpenConnection();
    
        var jobs = (await connection.QueryAsync<Job>("SELECT Id, Name, CategoryId, IsDone FROM Jobs")).ToList();
        var categories = (await connection.QueryAsync<Category>("SELECT Id, Name FROM Categories")).ToList();
    
        foreach (var job in jobs)
        {
            job.Category = categories.FirstOrDefault(c => c.Id == job.CategoryId) ??
                           throw new InvalidOperationException();
        }
    
        return jobs;
    }


    public async Task<int> UpdateJob(Job job)
    {
        using var db = _databaseService.OpenConnection();
        return await db.ExecuteAsync(
            "UPDATE Jobs SET Name = @Name, CategoryId = @CategoryId, IsDone = @IsDone WHERE Id = @Id",
            job
        );
    }


    public async Task<int> DeleteJob(int id)
    {
        using var db = _databaseService.OpenConnection();
        return await db.ExecuteAsync("DELETE FROM Jobs WHERE Id = @Id", new { Id = id });
    }
}