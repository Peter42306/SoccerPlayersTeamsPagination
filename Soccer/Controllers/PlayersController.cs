﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Soccer.Models;

namespace Soccer.Controllers
{
    public class PlayersController : Controller
    {
        private readonly SoccerContext _context;

        public PlayersController(SoccerContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 11;   // количество элементов на странице

            IQueryable<Players> players = _context.Players.Include(x => x.Team);
            var count = await players.CountAsync();
            var items = await players.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel(items, pageViewModel);
            return View(viewModel);
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var players = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (players == null)
            {
                return NotFound();
            }

            return View(players);
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name");
            return View();
        }

        // POST: Players/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Age,Position,TeamId")] Players players)
        {
            _context.Add(players);
             await _context.SaveChangesAsync();
             return RedirectToAction(nameof(Index));
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var players = await _context.Players.FindAsync(id);
            if (players == null)
            {
                return NotFound();
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", players.TeamId);
            return View(players);
        }

        // POST: Players/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Age,Position,TeamId")] Players players)
        {
            if (id != players.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(players);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayersExists(players.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var players = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (players == null)
            {
                return NotFound();
            }

            return View(players);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("Entity set 'SoccerContext.Players'  is null.");
            }
            var players = await _context.Players.FindAsync(id);
            if (players != null)
            {
                _context.Players.Remove(players);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlayersExists(int id)
        {
          return (_context.Players?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
