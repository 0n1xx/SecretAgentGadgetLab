using Microsoft.AspNetCore.Mvc;
using SecretAgentGadgetLab.Data;

namespace SecretAgentGadgetLab.Controllers
{
    public class ViewAgentsController : Controller
    {
        // dB connection 
        private readonly ApplicationDbContext _context;
        // Connecting to the database through the constructor
        public ViewAgentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Get all agents from the database, sort them by CodeName and pass to the view
            var agents = _context.Agents.OrderBy(a => a.CodeName).ToList();
            return View(agents);
        }
    }
}
