using AplikacjaKino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AplikacjaKino.Controllers
{
    [Authorize]
    public class SalaController : Controller
    {
        private readonly DatabaseContext _context;

        public SalaController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var sale = _context.Sale.ToList();
            return View(sale);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Sala());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Sala sala)
        {
            if (ModelState.IsValid)
            {
                _context.Sale.Add(sala);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(sala);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var sala = _context.Sale.FirstOrDefault(x => x.Id == id);
            if (sala == null) return NotFound();
            return View(sala);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Sala sala)
        {
            if (!ModelState.IsValid) return View(sala);
            _context.Sale.Update(sala);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var sala = _context.Sale.FirstOrDefault(x => x.Id == id);
            if (sala == null) return NotFound();
            return View(sala);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var sala = _context.Sale.FirstOrDefault(x => x.Id == id);
            if (sala != null)
            {
                _context.Sale.Remove(sala);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}