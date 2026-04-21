using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecretAgentGadgetLab.Data;
using SecretAgentGadgetLab.Models;
/*
 * Controller responsible for managing gadgets in the system.
 * Users must be authenticated to access this controller,
 * while create, edit, and delete operations are restricted to administrators only.
 * Also handles image uploads via Cloudinary.
 */
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
        // Available to all authenticated users
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
        // Admin-only: show form for creating a gadget. Populating dropdown list of agents
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["AgentId"] = new SelectList(_context.Agents.OrderBy(c => c.CodeName), "Id", "CodeName");
            return View();
        }
        // Handle gadget creation + optional image upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,AgentId")] Gadget gadget, IFormFile Photo)
        {
            if (ModelState.IsValid)
            {
                // Upload image to Cloudinary if provided
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
            // Re-populate dropdown if validation fails
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
        // Handle edit + optional image replacement
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,AgentId")] Gadget gadget, IFormFile Photo)
        {
            if (id != gadget.Id) return NotFound();

            // Photo is optional on edit — remove it from validation
            ModelState.Remove("Photo");

            if (ModelState.IsValid)
            {
                // If new image uploaded → replace old one, otherwise keep existing photo
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
                else
                {
                    // No new photo selected — preserve the existing one from the database
                    var existingPhoto = await _context.Gadgets
                        .AsNoTracking()
                        .Where(g => g.Id == id)
                        .Select(g => g.Photo)
                        .FirstOrDefaultAsync();
                    gadget.Photo = existingPhoto;
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
        // Show delete confirmation page
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