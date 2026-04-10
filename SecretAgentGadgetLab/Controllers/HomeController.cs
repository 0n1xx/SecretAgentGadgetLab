using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SecretAgentGadgetLab.Models;
/*
 * Controls basic site pages like Home, About, Privacy, and Error.
 * No special logic here, just returning views.
 */
namespace SecretAgentGadgetLab.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Loading a Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Loading an About page
        public IActionResult About()
        {
            return View();
        }
    }
}