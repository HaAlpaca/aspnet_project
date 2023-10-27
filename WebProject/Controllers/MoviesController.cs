using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using WebProject.Data;
using WebProject.Data.Roles;
using WebProject.Data.Services;
using WebProject.Data.ViewModel;
using WebProject.Models;

namespace WebProject.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class MoviesController: Controller
    {
        private readonly IMoviesService _service;
        public MoviesController(IMoviesService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            var allMovies = await _service.GetAllAsync(n => n.Cinema);

            int totalItems = allMovies.Count();
            int pageSize = (int)Math.Ceiling((double)totalItems / 3);
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var paginatedData = allMovies.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PartialIndex", paginatedData);
            }

            return View(paginatedData);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var movieDetail = await _service.GetMovieByIdAsync(id);
            return View(movieDetail);
        }

        public async Task<IActionResult> Create()
        {
            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(NewMovieVM movie)
        {
            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

                return View(movie);
            }

            await _service.AddNewMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movieDetails = await _service.GetMovieByIdAsync(id);
            if (movieDetails == null) return View("Empty");

            var response = new NewMovieVM()
            {
                Id = movieDetails.Id,
                Name = movieDetails.Name,
                Description = movieDetails.Description,
                Price = movieDetails.Price,
                StartDate = movieDetails.StartDate,
                EndDate = movieDetails.EndDate,
                ImageURL = movieDetails.ImageURL,
                MovieCategory = movieDetails.MovieCategory,
                CinemaId = movieDetails.CinemaId,
                ProducerId = movieDetails.ProducerId,
                ActorIds = movieDetails.Actors_Movies.Select(n => n.ActorId).ToList(),
            };

            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();
            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewMovieVM movie)
        {
            if (id != movie.Id) return View("Empty");

            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

                return View(movie);
            }

            await _service.UpdateMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }
        // search
        [AllowAnonymous]
        public async Task<IActionResult> Filter(string SearchString)
        {
            var allMovies = await _service.GetAllAsync(n => n.Cinema);

            if (!string.IsNullOrEmpty(SearchString))
            {
                //var filteredResult = allMovies.Where(n => n.Name.ToLower().Contains(searchString.ToLower()) || n.Description.ToLower().Contains(searchString.ToLower())).ToList();

                var filteredResultNew = allMovies.Where(n => string.Equals(n.Name, SearchString, StringComparison.CurrentCultureIgnoreCase) || string.Equals(n.Description, SearchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (filteredResultNew.Count() == 0) {
                    return View("Index", allMovies);
                }
                return View("_PartialIndex", filteredResultNew);
            }
            return View("Index", allMovies);
        }



        [AllowAnonymous]

        public async Task<IActionResult> FormSelectFillter(string ItemSelected)
        {

            var allMovies = await _service.GetAllAsync(n => n.Cinema);
            if (ItemSelected == "1")
            {
                var filterResult = allMovies.Where(n => DateTime.Now >= n.StartDate && DateTime.Now <= n.EndDate).ToList();
                return View("Index", filterResult);
            }
            else if (ItemSelected == "2")
            {
                var filterResult = allMovies.Where(n => DateTime.Now > n.EndDate).ToList();
                return View("Index", filterResult);
            }
            else if (ItemSelected == "3")
            {
                var filterResult = allMovies.Where(n => DateTime.Now > n.StartDate ).ToList();
                return View("Index", filterResult);
            }
            return View("Index", allMovies);
        }
    }
}
