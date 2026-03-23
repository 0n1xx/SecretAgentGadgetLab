using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecretAgentGadgetLab.Data;
using SecretAgentGadgetLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretAgentGadgetLab.Controllers
{
    /*
     * In this controller, I implemented the following features:
     * Create, edit and delete operations are only available to authorized users.
     * But all users, including unauthorized ones, can view the list of agents and their details.
     */
    public class GadgetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GadgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Gadgets.Include(g => g.Agent);
            return View(await applicationDbContext.OrderBy(g => g.Name).ToListAsync());
        }
        [AllowAnonymous]
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

        // 🔒 Только для авторизованных
        [Authorize]
        public IActionResult Create()
        {
            ViewData["AgentId"] = new SelectList(_context.Agents.OrderBy(c => c.CodeName), "Id", "CodeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,AgentId")] Gadget gadget, IFormFile Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    var extension = Path.GetExtension(Photo.FileName);
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "product-uploads", fileName);

                    using var stream = new FileStream(uploadPath, FileMode.Create);
                    await Photo.CopyToAsync(stream);

                    gadget.Photo = fileName;
                }

                _context.Add(gadget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AgentId"] = new SelectList(_context.Agents, "Id", "CodeName", gadget.AgentId);
            return View(gadget);
        }
        [Authorize]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,AgentId")] Gadget gadget, IFormFile Photo)
        {
            if (id != gadget.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    var extension = Path.GetExtension(Photo.FileName);
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "product-uploads", fileName);

                    using var stream = new FileStream(uploadPath, FileMode.Create);
                    await Photo.CopyToAsync(stream);

                    gadget.Photo = fileName;
                }

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

        [Authorize]
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
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