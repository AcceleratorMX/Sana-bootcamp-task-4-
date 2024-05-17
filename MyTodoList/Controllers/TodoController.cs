using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyTodoList.Data.Models;
using MyTodoList.Repositories;
using MyTodoList.ViewModels;

namespace MyTodoList.Controllers;

public class TodoController(JobRepositorySwitcher jobRepository, ILogger<TodoController> logger)
    : Controller
{
    private readonly ILogger<TodoController> _logger = logger;

    public async Task<IActionResult> Todo()
    {
        var model = new JobViewModel
        {
            NewJob = new Job(),
            Jobs = (await jobRepository.GetJobs())
                .OrderByDescending(job => job.IsDone)
                .ThenByDescending(job => job.Id)
        };

        ViewBag.Categories = await GetCategoriesSelectList();

        return View(model);
    }

    private async Task<SelectList> GetCategoriesSelectList()
    {
        var categories = await jobRepository.GetCategories();
        return new SelectList(categories, "Id", "Name");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Todo(JobViewModel model)
    {
        var job = model.NewJob;

        if (string.IsNullOrEmpty(job.Name) || !job.CategoryId.HasValue || job.CategoryId == 0)
        {
            ViewBag.Categories = await GetCategoriesSelectList();
            model.Jobs = await jobRepository.GetJobs();
            return View(model);
        }

        await jobRepository.AddJob(job);

        return RedirectToAction("Todo");
    }

    [HttpPost]
    public async Task<IActionResult> ChangeProgress(int id)
    {
        var job = await jobRepository.GetJob(id);
        if (job.IsDone) return RedirectToAction("Todo");
        job.IsDone = true;
        await jobRepository.UpdateJob(job);
        return RedirectToAction("Todo");
    }

    public async Task<IActionResult> Delete(int id)
    {
        await jobRepository.DeleteJob(id);
        return RedirectToAction("Todo");
    }

    [HttpPost]
    public IActionResult Switch(string repositoryType)
    {
        switch (repositoryType.ToLower())
        {
            case "sql":
                jobRepository.SwitchToSql();
                break;
            case "xml":
                jobRepository.SwitchToXml();
                break;
        }
        
        return RedirectToAction("Todo");
    }
}