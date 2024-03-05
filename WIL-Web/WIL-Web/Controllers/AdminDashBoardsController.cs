using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WIL_Web.Data;
using WIL_Web.Models;

namespace WIL_Web.Controllers
{
    public class AdminDashBoardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashBoardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminDashBoards
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdminDashBoards.ToListAsync());
        }

        // GET: AdminDashBoards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminDashBoard = await _context.AdminDashBoards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminDashBoard == null)
            {
                return NotFound();
            }

            return View(adminDashBoard);
        }

        // GET: AdminDashBoards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminDashBoards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] AdminDashBoard adminDashBoard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminDashBoard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adminDashBoard);
        }

        // GET: AdminDashBoards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminDashBoard = await _context.AdminDashBoards.FindAsync(id);
            if (adminDashBoard == null)
            {
                return NotFound();
            }
            return View(adminDashBoard);
        }

        // POST: AdminDashBoards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] AdminDashBoard adminDashBoard)
        {
            if (id != adminDashBoard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminDashBoard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminDashBoardExists(adminDashBoard.Id))
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
            return View(adminDashBoard);
        }

        // GET: AdminDashBoards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminDashBoard = await _context.AdminDashBoards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminDashBoard == null)
            {
                return NotFound();
            }

            return View(adminDashBoard);
        }

        // POST: AdminDashBoards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminDashBoard = await _context.AdminDashBoards.FindAsync(id);
            if (adminDashBoard != null)
            {
                _context.AdminDashBoards.Remove(adminDashBoard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminDashBoardExists(int id)
        {
            return _context.AdminDashBoards.Any(e => e.Id == id);
        }
    }
}
