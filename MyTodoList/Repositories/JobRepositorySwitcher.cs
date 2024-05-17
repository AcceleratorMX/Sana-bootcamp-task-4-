using MyTodoList.Data.Models;
using MyTodoList.Data.Service;
using MyTodoList.Interfaces;

namespace MyTodoList.Repositories;

public class JobRepositorySwitcher(
    IJobRepository sqlRepository,
    IJobRepository xmlRepository,
    RepositoryTypeService repositoryTypeService,
    ILogger<JobRepositorySwitcher> logger)
    : IJobRepository
{
    private readonly ILogger<JobRepositorySwitcher> _logger = logger;

    private const string SqlRepositoryType = "sql";
    private const string XmlRepositoryType = "xml";

    private IJobRepository CurrentRepository
    {
        get
        {
            return repositoryTypeService.CurrentRepositoryType switch
            {
                "xml" => xmlRepository,
                _ => sqlRepository,
            };
        }
    }

    public void SwitchToSql() => repositoryTypeService.CurrentRepositoryType = SqlRepositoryType;
    public void SwitchToXml() => repositoryTypeService.CurrentRepositoryType = XmlRepositoryType;

    public Task<int> AddJob(Job job) => CurrentRepository.AddJob(job);
    public Task<IEnumerable<Job>> GetJobs() => CurrentRepository.GetJobs();
    public Task<Job> GetJob(int id) => CurrentRepository.GetJob(id);
    public Task<int> UpdateJob(Job job) => CurrentRepository.UpdateJob(job);
    public Task<int> DeleteJob(int id) => CurrentRepository.DeleteJob(id);
    public Task<IEnumerable<Category>> GetCategories() => CurrentRepository.GetCategories();
}
