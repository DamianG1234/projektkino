using AplikacjaKino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AplikacjaKino.Controllers
{
    public class SeansController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SeansController(DatabaseContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var seanse = _context.Seanse
                .Include(s => s.Film)
                .Include(s => s.Sala)
                .OrderBy(s => s.DataGodzina)
                .ToList();
            return View(seanse);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Rezerwuj(int id)
        {
            var seans = _context.Seanse
                .Include(s => s.Film)
                .Include(s => s.Sala)
                .Include(s => s.Rezerwacje)
                .FirstOrDefault(s => s.Id == id);

            if (seans == null) return NotFound();
            return View(seans);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RezerwujWiele(RezerwacjaWieleModel model)
        {
            if (model.Miejsca == null || model.Miejsca.Count == 0)
            {
                TempData["Blad"] = "Nie wybrano ¿adnego miejsca.";
                return RedirectToAction(nameof(Rezerwuj), new { id = model.SeansId });
            }

            if (model.Miejsca.Count > 10)
            {
                TempData["Blad"] = "Mo¿esz zarezerwowaæ maksymalnie 10 miejsc naraz.";
                return RedirectToAction(nameof(Rezerwuj), new { id = model.SeansId });
            }

            foreach (var miejsce in model.Miejsca)
            {
                var zajete = _context.Rezerwacje.Any(r =>
                    r.SeansId == model.SeansId &&
                    r.NumerRzedu == miejsce.NumerRzedu &&
                    r.NumerMiejsca == miejsce.NumerMiejsca);

                if (zajete)
                {
                    TempData["Blad"] = $"Miejsce rz¹d {miejsce.NumerRzedu}, miejsce {miejsce.NumerMiejsca} jest ju¿ zajête.";
                    return RedirectToAction(nameof(Rezerwuj), new { id = model.SeansId });
                }
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? pierwszaRezerwacjaId = null;

            foreach (var miejsce in model.Miejsca)
            {
                var rezerwacja = new Rezerwacja
                {
                    Id = 0,
                    SeansId = model.SeansId,
                    NumerRzedu = miejsce.NumerRzedu,
                    NumerMiejsca = miejsce.NumerMiejsca,
                    UserId = userId
                };
                _context.Rezerwacje.Add(rezerwacja);
                _context.SaveChanges();
                if (pierwszaRezerwacjaId == null) pierwszaRezerwacjaId = rezerwacja.Id;
            }

            return RedirectToAction(nameof(Potwierdzenie), new { id = pierwszaRezerwacjaId });
        }

        public IActionResult Potwierdzenie(int id)
        {
            var rezerwacja = _context.Rezerwacje
                .Include(r => r.Seans)
                .ThenInclude(s => s.Film)
                .Include(r => r.Seans)
                .ThenInclude(s => s.Sala)
                .FirstOrDefault(r => r.Id == id);

            if (rezerwacja == null) return NotFound();
            return View(rezerwacja);
        }

        [Authorize]
        public IActionResult MojeRezerwacje()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rezerwacje = _context.Rezerwacje
                .Include(r => r.Seans)
                .ThenInclude(s => s.Film)
                .Include(r => r.Seans)
                .ThenInclude(s => s.Sala)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Seans.DataGodzina)
                .ToList();
            return View(rezerwacje);
        }

        [Authorize]
        public IActionResult Manage()
        {
            var seanse = _context.Seanse
                .Include(s => s.Film)
                .Include(s => s.Sala)
                .OrderBy(s => s.DataGodzina)
                .ToList();
            return View(seanse);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Filmy = _context.Filmy.ToList();
            ViewBag.Sale = _context.Sale.ToList();
            return View(new Seans());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Seans seans)
        {
            ModelState.Remove("Seans");
            ModelState.Remove("Film");
            ModelState.Remove("Sala");

            if (ModelState.IsValid)
            {
                var film = _context.Filmy.FirstOrDefault(f => f.Id == seans.FilmId);

                if (film == null || film.CzasTrwaniaMin < 0)
                {
                    TempData["Blad"] = "Wybrany film ma niepoprawny czas trwania.";
                    ViewBag.Filmy = _context.Filmy.ToList();
                    ViewBag.Sale = _context.Sale.ToList();
                    return View(seans);
                }

                var koniecNowego = seans.DataGodzina.AddMinutes(film.CzasTrwaniaMin);

                var kolizja = _context.Seanse
                    .Include(s => s.Film)
                    .Where(s => s.SalaId == seans.SalaId)
                    .AsEnumerable()
                    .Any(s =>
                    {
                        var koniecIstniejacego = s.DataGodzina.AddMinutes(s.Film.CzasTrwaniaMin);
                        return seans.DataGodzina < koniecIstniejacego && koniecNowego > s.DataGodzina;
                    });

                if (kolizja)
                {
                    TempData["Blad"] = "Ta sala jest ju¿ zajêta w wybranym terminie.";
                    ViewBag.Filmy = _context.Filmy.ToList();
                    ViewBag.Sale = _context.Sale.ToList();
                    return View(seans);
                }

                _context.Seanse.Add(seans);
                _context.SaveChanges();
                return RedirectToAction(nameof(Manage));
            }

            ViewBag.Filmy = _context.Filmy.ToList();
            ViewBag.Sale = _context.Sale.ToList();
            return View(seans);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var seans = _context.Seanse
                .Include(s => s.Film)
                .Include(s => s.Sala)
                .FirstOrDefault(s => s.Id == id);
            if (seans == null) return NotFound();
            return View(seans);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var seans = _context.Seanse.FirstOrDefault(s => s.Id == id);
            if (seans != null)
            {
                _context.Seanse.Remove(seans);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Manage));
        }

        [Authorize]
        public IActionResult Rezerwacje(int id)
        {
            var seans = _context.Seanse
                .Include(s => s.Film)
                .Include(s => s.Sala)
                .Include(s => s.Rezerwacje)
                .FirstOrDefault(s => s.Id == id);
            if (seans == null) return NotFound();

            var userIds = seans.Rezerwacje
                .Where(r => r.UserId != null)
                .Select(r => r.UserId)
                .Distinct()
                .ToList();

            var users = _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.Email);

            ViewBag.UserEmails = users;
            return View(seans);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UsunRezerwacje(int id, int seansId)
        {
            var rezerwacja = _context.Rezerwacje.FirstOrDefault(r => r.Id == id);
            if (rezerwacja != null)
            {
                _context.Rezerwacje.Remove(rezerwacja);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Rezerwacje), new { id = seansId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UsunMojaRezerwacje(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rezerwacja = _context.Rezerwacje.FirstOrDefault(r => r.Id == id && r.UserId == userId);
            if (rezerwacja != null)
            {
                _context.Rezerwacje.Remove(rezerwacja);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(MojeRezerwacje));
        }

        [Authorize]
        public IActionResult ZajeteSale(int salaId, string data)
        {
            if (!DateTime.TryParse(data, out DateTime wybranaData))
                return Json(new List<object>());

            var seanse = _context.Seanse
                .Include(s => s.Film)
                .Where(s => s.SalaId == salaId && s.DataGodzina.Date == wybranaData.Date)
                .OrderBy(s => s.DataGodzina)
                .Select(s => new
                {
                    film = s.Film.Tytul,
                    od = s.DataGodzina.ToString("HH:mm"),
                    do_ = s.DataGodzina.AddMinutes(s.Film.CzasTrwaniaMin).ToString("HH:mm")
                })
                .ToList();

            return Json(seanse);
        }
    }
}