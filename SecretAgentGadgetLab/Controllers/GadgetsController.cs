using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecretAgentGadgetLab.Data;
using SecretAgentGadgetLab.Models;

namespace SecretAgentGadgetLab.Controllers
{
    public class GadgetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GadgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Gadgets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Gadgets.Include(g => g.Agent);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Gadgets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gadget = await _context.Gadgets
                .Include(g => g.Agent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gadget == null)
            {
                return NotFound();
            }

            return View(gadget);
        }

        // GET: Gadgets/Create
        public IActionResult Create()
        {
            ViewData["AgentId"] = new SelectList(_context.Agents, "Id", "CodeName");
            return View();
        }

        // POST: Gadgets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Photo,AgentId")] Gadget gadget)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gadget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgentId"] = new SelectList(_context.Agents, "Id", "CodeName", gadget.AgentId);
            return View(gadget);
        }

        // GET: Gadgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gadget = await _context.Gadgets.FindAsync(id);
            if (gadget == null)
            {
                return NotFound();
            }
            ViewData["AgentId"] = new SelectList(_context.Agents, "Id", "CodeName", gadget.AgentId);
            return View(gadget);
        }

        // POST: Gadgets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Photo,AgentId")] Gadget gadget)
        {
            if (id != gadget.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gadget);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GadgetExists(gadget.Id))
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
            ViewData["AgentId"] = new SelectList(_context.Agents, "Id", "CodeName", gadget.AgentId);
            return View(gadget);
        }

        // GET: Gadgets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gadget = await _context.Gadgets
                .Include(g => g.Agent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gadget == null)
            {
                return NotFound();
            }

            return View(gadget);
        }

        // POST: Gadgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gadget = await _context.Gadgets.FindAsync(id);
            if (gadget != null)
            {
                _context.Gadgets.Remove(gadget);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GadgetExists(int id)
        {
            return _context.Gadgets.Any(e => e.Id == id);
        }
    }
}
