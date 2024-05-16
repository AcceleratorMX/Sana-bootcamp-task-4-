using MyTodoList.Data.Models;
using MyTodoList.Interfaces;
namespace MyTodoList.Repositories;

public class JobRepositorySwitcher : IJobRepository
{
    private readonly IJobRepository _sqlRepository;
    private readonly IJobRepository _xmlRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<JobRepositorySwitcher> _logger;

    private const string RepositoryKey = "CurrentRepository";

    public JobRepositorySwitcher(IJobRepository sqlRepository, IJobRepository xmlRepository, IHttpContextAccessor httpContextAccessor, ILogger<JobRepositorySwitcher> logger)
    {
        _sqlRepository = sqlRepository;
        _xmlRepository = xmlRepository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private IJobRepository CurrentRepository
    {
        get
        {
            var repositoryType = _httpContextAccessor.HttpContext!.Session.GetString(RepositoryKey);
            return repositoryType switch
            {
                "xml" => _xmlRepository,
                _ => _sqlRepository,
            };
        }
        set
        {
            var repositoryType = value == _xmlRepository ? "xml" : "sql";
            _httpContextAccessor.HttpContext!.Session.SetString(RepositoryKey, repositoryType);
            _logger.LogInformation($"Current repository set to: {value.GetType().Name}");
        }
    }

    public void SwitchToSql()
    {
        _logger.LogInformation("Switching to SQL repository.");
        CurrentRepository = _sqlRepository;
    }

    public void SwitchToXml()
    {
        _logger.LogInformation("Switching to XML repository.");
        CurrentRepository = _xmlRepository;
    }

    public Task<int> AddJob(Job job) => CurrentRepository.AddJob(job);
    public Task<IEnumerable<Job>> GetJobs() => CurrentRepository.GetJobs();
    public Task<Job> GetJob(int id) => CurrentRepository.GetJob(id);
    public Task<int> UpdateJob(Job job) => CurrentRepository.UpdateJob(job);
    public Task<int> DeleteJob(int id) => CurrentRepository.DeleteJob(id);
    public Task<IEnumerable<Category>> GetCategories() => CurrentRepository.GetCategories();
}
