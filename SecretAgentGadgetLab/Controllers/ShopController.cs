using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecretAgentGadgetLab.Data;

namespace SecretAgentGadgetLab.Controllers
{
    // Only authenticated users can access the shop
    [Authorize]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Agents.ToListAsync());
        }

        public async Task<IActionResult> Browse(int agentId)
        {
            var gadgets = await _context.Gadgets
                .Where(g => g.AgentId == agentId)
                .ToListAsync();

            return View(gadgets);
        }
    }
}