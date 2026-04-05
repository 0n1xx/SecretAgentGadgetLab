using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
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
        private readonly Cloudinary _cloudinary;

        public GadgetsController(ApplicationDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Gadgets.Include(g => g.Agent);
            return View(await applicationDbContext.OrderBy(g => g.Name).ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id, int? agentId)
        {
            ViewBag.AgentId = agentId;
            if (id == null) return NotFound();

            var gadget = await _context.Gadgets
                .Include(g => g.Agent)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gadget == null) return NotFound();

            return View(gadget);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["AgentId"] = new SelectList(_context.Agents.OrderBy(c => c.CodeName), "Id", "CodeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,AgentId")] Gadget gadget, IFormFile Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    using var stream = Photo.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(Photo.FileName, stream),
                        Folder = "secret-agent-gadgets"
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    gadget.Photo = uploadResult.SecureUrl.ToString();
                }

                _context.Add(gadget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AgentId"] = new SelectList(_context.Agents, "Id", "CodeName", gadget.AgentId);
            return View(gadget);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gadget = await _context.Gadgets.FindAsync(id);
            if (gadget == null) return NotFound();

            ViewData["AgentId"] = new SelectList(_context.Agents, "Id", "CodeName", gadget.AgentId);
            return View(gadget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,AgentId")] Gadget gadget, IFormFile Photo)
        {
            if (id != gadget.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    using var stream = Photo.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(Photo.FileName, stream),
                        Folder = "secret-agent-gadgets"
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    gadget.Photo = uploadResult.SecureUrl.ToString();
                }

                try
                {
                    _context.Update(gadget);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GadgetExists(gadget.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["AgentId"] = new SelectList(_context.Agents, "Id", "CodeName", gadget.AgentId);
            return View(gadget);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gadget = await _context.Gadgets
                .Include(g => g.Agent)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gadget == null) return NotFound();

            return View(gadget);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gadget = await _context.Gadgets.FindAsync(id);
            if (gadget != null) _context.Gadgets.Remove(gadget);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GadgetExists(int id)
        {
            return _context.Gadgets.Any(e => e.Id == id);
        }
    }
}