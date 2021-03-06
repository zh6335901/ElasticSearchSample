﻿using ElasticSearchSample.Models;
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
        private ProductSearchService _productService = new ProductSearchService("elasticsearch-sample");

        public IActionResult Index()
        {
            ViewBag.SortFields = new string[] { "memberPrice", "marketPrice", "publishedTime" };

            return View();
        }

        public async Task<IActionResult> SearchProducts(ProductSearchRequest request)
        {
            var products = await _productService.SearchProductsWithWeightAsync(
                request.Keyword, 
                request.Terms, 
                request.SortField, 
                request.SortDescending, 
                request.From, 
                request.Size);

            var suggest = await _productService.GetSearchSuggestionAsync(request.Keyword);

            return Json(new { Products = products, Suggest = suggest });
        }

        public async Task<IActionResult> SearchPrompts(string keyword, int size = 10)
        {
            var prompts = await _promptService.GetProductSearchPromptsAsync(keyword, size);
            return Json(prompts);
        }

        [HttpGet]
        public IActionResult AddPrompt()
        {
            return View(new ProductSearchPrompt());
        }

        [HttpPost]
        public async Task<IActionResult> AddPrompt(ProductSearchPrompt prompt)
        {
            if (ModelState.IsValid)
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
            }

            return View(prompt);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View(new Product());
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.IndexAsync(product);

                if (response.ServerError == null)
                {
                    TempData["Message"] = "success";
                }
                else
                {
                    TempData["Message"] = "error:" + response.ServerError.Error;
                }
            }

            return View("AddProduct", product);
        }
    }
}
