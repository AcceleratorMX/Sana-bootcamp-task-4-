using MyTodoList.Data.Models;

namespace MyTodoList.Interfaces;

public interface IJobRepository
{
    public Task<int> AddJob(Job job);
    public Task<IEnumerable<Job>> GetJobs();
    public Task<Job> GetJob(int id);
    public Task<int> UpdateJob(Job job);
    public Task<int> DeleteJob(int id);
}