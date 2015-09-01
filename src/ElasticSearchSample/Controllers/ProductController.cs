using ElasticSearchSample.Models;
using ElasticSearchSample.Services;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Controllers
{
    public class ProductController : Controller
    {
        private ProductSearchPromptService _promptService = new ProductSearchPromptService("elasticsearch-sample");

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SearchPrompts(string keyword, int size = 10)
        {
            var prompts = await _promptService.GetProductSearchPrompts(keyword, size);
            return Json(prompts);
        }

        public IActionResult AddPrompt()
        {
            return View(new ProductSearchPrompt());
        }

        [HttpPost]
        public async Task<IActionResult> AddPrompt(ProductSearchPrompt prompt)
        {
            var response = await _promptService.IndexAsync(prompt);

            if (response.ServerError == null)
            {
                TempData["Message"] = "success";
            }
            else
            {
                TempData["Message"] = "error:" + response.ServerError.Error;
            }

            return View(prompt);
        }
    }
}
