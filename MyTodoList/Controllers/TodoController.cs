using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyTodoList.Data.Models;
using MyTodoList.Interfaces;
using MyTodoList.Repositories;
using MyTodoList.ViewModels;
using Microsoft.Extensions.Logging;

namespace MyTodoList.Controllers
{
    public class TodoController : Controller
    {
        private readonly JobRepositorySwitcher _jobRepository;
        private readonly ILogger<TodoController> _logger;

        public TodoController(JobRepositorySwitcher jobRepository, ILogger<TodoController> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Todo()
        {
            _logger.LogInformation($"Current repository in use: {_jobRepository.GetType().Name}");
            var model = new JobViewModel
            {
                NewJob = new Job(),
                Jobs = (await _jobRepository.GetJobs())
                    .OrderByDescending(job => job.IsDone)
                    .ThenByDescending(job => job.Id)
            };

            ViewBag.Categories = await GetCategoriesSelectList();

            return View(model);
        }

        private async Task<SelectList> GetCategoriesSelectList()
        {
            var categories = await _jobRepository.GetCategories();
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
                model.Jobs = await _jobRepository.GetJobs();
                return View(model);
            }

            await _jobRepository.AddJob(job);

            return RedirectToAction("Todo");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProgress(int id)
        {
            var job = await _jobRepository.GetJob(id);
            if (job.IsDone) return RedirectToAction("Todo");
            job.IsDone = true;
            await _jobRepository.UpdateJob(job);
            return RedirectToAction("Todo");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _jobRepository.DeleteJob(id);
            return RedirectToAction("Todo");
        }

        [HttpPost]
        public IActionResult Switch(string repositoryType)
        {
            _logger.LogInformation($"Switching repository to: {repositoryType}");

            switch (repositoryType.ToLower())
            {
                case "sql":
                    _jobRepository.SwitchToSql();
                    break;
                case "xml":
                    _jobRepository.SwitchToXml();
                    break;
                default:
                    _logger.LogWarning($"Unknown repository type: {repositoryType}");
                    break;
            }

            _logger.LogInformation($"Repository switched to: {repositoryType}");

            return RedirectToAction("Todo");
        }

    }
}
