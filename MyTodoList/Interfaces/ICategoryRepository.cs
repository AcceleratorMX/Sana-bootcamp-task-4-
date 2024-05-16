using MyTodoList.Data.Models;

namespace MyTodoList.Interfaces;

public interface ICategoryRepository
{
    public Task<IEnumerable<Category>> GetCategories();
}