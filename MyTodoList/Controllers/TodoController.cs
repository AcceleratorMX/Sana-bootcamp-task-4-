using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyTodoList.Data.Models;
using MyTodoList.Interfaces;
using MyTodoList.ViewModels;

namespace MyTodoList.Controllers;

public class TodoController(IJobRepository jobRepository, ICategoryRepository categoryRepository)
    : Controller
{
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
        var categories = await categoryRepository.GetCategories();
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
}