using System.Xml.Linq;
using MyTodoList.Data.Models;
using MyTodoList.Data.Service;
using MyTodoList.Interfaces;

namespace MyTodoList.Repositories;

public class JobRepositoryXml(XmlStorageService xmlStorageService, ILogger<JobRepositoryXml> logger) : IJobRepository
{
    private readonly XmlStorageService _xmlStorageService = xmlStorageService;
    private readonly ILogger<JobRepositoryXml> _logger = logger;
    
    public async Task<int> AddJob(Job job)
    {
        var jobsElement = _xmlStorageService.LoadJobs().Element("Jobs") ?? 
                          throw new Exception("Jobs element not found!");

        var lastJobId = jobsElement.Descendants("Job").Select(j => (int?)j.Attribute("id")).Max() ?? 0;
        var newJobId = lastJobId + 1;

        var newJob = new XElement("Job",
            new XAttribute("id", newJobId),
            new XAttribute("categoryId", job.CategoryId ?? throw new Exception("Category id not found!")),
            new XAttribute("isDone", job.IsDone),
            job.Name
        );

        jobsElement.Add(newJob);
        _xmlStorageService.SaveJobs(jobsElement.Document);

        return await Task.FromResult(newJobId);
    }
    
    public async Task<IEnumerable<Job>> GetJobs()
    {
        var categories = await GetCategories();

        var query = from job in _xmlStorageService.LoadJobs().Descendants("Job")
            select new Job
            {
                Id = int.Parse(job.Attribute("id")?.Value ?? throw new Exception("Job id not found!")),
                Name = job.Value,
                CategoryId = int.Parse(job.Attribute("categoryId")?.Value ?? "0"),
                IsDone = bool.Parse(job.Attribute("isDone")?.Value ?? "false"),
                Category = categories.FirstOrDefault(c => c.Id == int.Parse(job.Attribute("categoryId")?.Value ?? "0"))
            };

        var jobs = query.ToList();
        
        return await Task.FromResult(jobs);
    }

    public async Task<Job> GetJob(int id)
    {
        var document = _xmlStorageService.LoadJobs();
        var jobElement = document.Root?.Elements("Job")
            .FirstOrDefault(e => int.Parse(e.Attribute("id")?.Value ?? "0") == id);

        if (jobElement == null)
            throw new Exception($"Job with id {id} not found!");

        var job = new Job
        {
            Id = id,
            Name = jobElement.Value,
            CategoryId = int.Parse(jobElement.Attribute("categoryId")?.Value ?? "0"),
            IsDone = bool.Parse(jobElement.Attribute("isDone")?.Value ?? "false")
        };

        var categories = await GetCategories();
        job.Category = categories.FirstOrDefault(c => c.Id == job.CategoryId) ??
                       throw new Exception($"Category with id {job.CategoryId} not found!");

        return job;
    }

    public async Task<int> UpdateJob(Job job)
    {
        var jobElement = _xmlStorageService.LoadJobs()
                             .Descendants("Job")
                             .FirstOrDefault(e => (int)e.Attribute("id")! == job.Id)
                         ?? throw new Exception($"Job with id {job.Id} not found!");

        jobElement.Attribute("isDone")?.SetValue(true);

        _xmlStorageService.SaveJobs(jobElement.Document);
        return await Task.FromResult(job.Id);
    }


    public Task<int> DeleteJob(int id)
    {
        var document = _xmlStorageService.LoadJobs();
        var jobElement = document.Descendants("Job")
            .FirstOrDefault(e => (int)e.Attribute("id")! == id);

        jobElement?.Remove();
        _xmlStorageService.SaveJobs(document);

        return Task.FromResult(id);
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