using ElasticSearchSample.Models;
using ElasticSearchSample.Services;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchSample.Controllers
{
    public class LocationController : Controller
    {
        //just create a specific service instance
        private LocationSearchService _locationSearchService = new LocationSearchService("elasticsearch-sample");

        public async Task<IActionResult> Index(LocationSearchRequest searchModel)
        {
            var locations = await _locationSearchService.SearchAndSortByGeoAsync(
                searchModel.Keyword,
                new GeoPoint(searchModel.Longitude, searchModel.Latitude));

            ViewBag.SearchModel = searchModel;

            return View(locations);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var location = new Location();
            return View(location);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Location location)
        {
            if (ModelState.IsValid)
            {
                var response = await _locationSearchService.IndexAsync(location);

                if (response.ServerError == null)
                {
                    return RedirectToAction("Index");
                }

                return View("Error", response.ServerError.Error);
            }

            return View(location);
        }
    }
}
