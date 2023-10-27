using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.Data;
using WebProject.Data.Roles;
using WebProject.Data.Services;
using WebProject.Models;

namespace WebProject.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class CinemasController : Controller
    {
        private readonly ICinemasService _service;
        public CinemasController(ICinemasService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allProducers = await _service.GetAllAsync();
            return View(allProducers);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var cinemaDetails = await _service.GetByIdAsync(id);
            if (cinemaDetails == null) return View("Empty");
            return View(cinemaDetails);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Logo,Name,Description")] Cinema cinema)
        {
            if (!ModelState.IsValid) return View(cinema);
            await _service.AddAsync(cinema);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var cinemaDetails = await _service.GetByIdAsync(id);
            if (cinemaDetails == null) return View("Empty");
            return View(cinemaDetails);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Logo,Name,Description")] Cinema cinema)
        {
            if (!ModelState.IsValid) return View(cinema);
            if (id == cinema.Id)
            {
                await _service.UpdateAsync(id, cinema);
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cinemaDetails = await _service.GetByIdAsync(id);
            if (cinemaDetails == null) return View("Empty");
            return View(cinemaDetails);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinemaDetails = await _service.GetByIdAsync(id);
            if (cinemaDetails == null) return View("Empty");
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
