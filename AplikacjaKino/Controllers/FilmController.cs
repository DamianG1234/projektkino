using AplikacjaKino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplikacjaKino.Controllers
{
    [Authorize]
    public class FilmController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _env;

        public FilmController(DatabaseContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        public IActionResult Index()
        {
            var filmy = _context.Filmy.ToList();
            return View(filmy);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Film());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Film film, IFormFile? okladka)
        {
            if (okladka != null && okladka.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "okladki");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var nazwaPliku = Guid.NewGuid().ToString() + Path.GetExtension(okladka.FileName);
                var sciezka = Path.Combine(folder, nazwaPliku);

                using (var stream = new FileStream(sciezka, FileMode.Create))
                    await okladka.CopyToAsync(stream);

                film.Okladka = "/okladki/" + nazwaPliku;
            }

            ModelState.Remove("Okladka");
            if (ModelState.IsValid)
            {
                _context.Filmy.Add(film);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var film = _context.Filmy.FirstOrDefault(x => x.Id == id);
            if (film == null) return NotFound();
            return View(film);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Film film, IFormFile? okladka)
        {
            if (film.CzasTrwaniaMin < 0)
            {
                ModelState.AddModelError("CzasTrwaniaMin", "Czas trwania filmu nie mo¿e byæ ujemny.");
            }
            if (okladka != null && okladka.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "okladki");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var nazwaPliku = Guid.NewGuid().ToString() + Path.GetExtension(okladka.FileName);
                var sciezka = Path.Combine(folder, nazwaPliku);

                using (var stream = new FileStream(sciezka, FileMode.Create))
                    await okladka.CopyToAsync(stream);

                film.Okladka = "/okladki/" + nazwaPliku;
            }
            else
            {
                var istniejacy = _context.Filmy.AsNoTracking().FirstOrDefault(f => f.Id == film.Id);
                film.Okladka = istniejacy?.Okladka;
            }

            ModelState.Remove("Okladka");
            if (!ModelState.IsValid) return View(film);
            _context.Filmy.Update(film);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var film = _context.Filmy.FirstOrDefault(x => x.Id == id);
            if (film == null) return NotFound();
            return View(film);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var film = _context.Filmy.FirstOrDefault(x => x.Id == id);
            if (film != null)
            {
                _context.Filmy.Remove(film);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}