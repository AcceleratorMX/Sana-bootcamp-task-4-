using System.Xml.Linq;
using MyTodoList.Data.Models;
using MyTodoList.Data.Service;
using MyTodoList.Interfaces;

namespace MyTodoList.Repositories;

public class JobRepositoryXml(XmlStorageService xmlStorageService, ILogger<JobRepositoryXml> logger) : IJobRepository
{
    private readonly XmlStorageService _xmlStorageService = xmlStorageService;
    private readonly ILogger<JobRepositoryXml> _logger = logger;
    
    // public async Task<int> AddJob(Job job)
    // {
    //
    // }


    public Task<int> AddJob(Job job)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Job>> GetJobs()
    {
        _logger.LogInformation("Fetching jobs from XML repository.");
        var jobs = from job in _xmlStorageService.LoadJobs().Descendants("Job")
            select new Job
            {
                Id = int.Parse(job.Attribute("id")?.Value ?? 
                               throw new InvalidOperationException("Job id not found!")),
                Name = job.Element("Name")?.Value,
                CategoryId = int.Parse(job.Element("Category")?.Value ?? 
                                       throw new InvalidOperationException("Category id not found!")),
                IsDone = bool.Parse(job.Element("IsDone")?.Value ?? "false")
            };
    
        return await Task.FromResult(jobs.ToList());
    }


    public Task<Job> GetJob(int id)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateJob(Job job)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteJob(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        var query = from category in _xmlStorageService.LoadCategories().Descendants("Category")
            select new Category
            {
                Id = int.Parse(category.Attribute("id")?.Value ?? 
                               throw new InvalidOperationException("Category id not found!")),
                Name = category.Value
            };
        
        return await Task.FromResult(query.ToList());
    }
}