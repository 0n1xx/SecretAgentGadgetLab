using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecretAgentGadgetLab.Data;
using SecretAgentGadgetLab.Models;
using Stripe;
using Stripe.Checkout;

namespace SecretAgentGadgetLab.Controllers
{
    // Only authenticated users can access the shop
    [Authorize]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Neeed configuration to read Stripe keys from appsettings.json
        private IConfiguration _configuration;

        public ShopController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
        [HttpPost]
        public IActionResult AddToCart(int GadgetId, int Quantity)
        {
            // Get current gadget price
            var price = _context.Gadgets.Find(GadgetId).Price;

            // Identify the customer
            var customerId = User.Identity.Name;

            // Check if gadget already exists in the cart
            var cartItem = _context.Carts.SingleOrDefault(c => c.GadgetId == GadgetId && c.CustomerId == customerId);

            if (cartItem != null)
            {
                // Gadget already in cart, update quantity
                cartItem.Quantity += Quantity;
                _context.Update(cartItem);
                _context.SaveChanges();
            }
            else
            {
                // Create a new Cart object
                var cart = new Cart
                {
                    GadgetId = GadgetId,
                    Quantity = Quantity,
                    Price = (double)price,
                    CustomerId = customerId,
                    DateCreated = DateTime.Now
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            return RedirectToAction("Cart");
        }
        public IActionResult Cart()
        {
            // Get the logged-in user's email as their ID
            var customerId = User.Identity.Name;

            // Get items in the customer's cart with gadget details
            var cartItems = _context.Carts
                .Include(c => c.Gadget)
                .Where(c => c.CustomerId == customerId)
                .ToList();

            // Count total quantity for display in navbar
            var itemCount = _context.Carts
                .Where(c => c.CustomerId == customerId)
                .Sum(c => c.Quantity);

            ViewBag.ItemCount = itemCount;

            return View(cartItems);
        }

        // POST: /Shop/RemoveFromCart/12
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int id)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.Id == id);

            if (cartItem == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cartItem);
            _context.SaveChanges();

            return RedirectToAction(nameof(Cart));
        }
        // Get: /Shop/Checkout
        public IActionResult Checkout()
        {
            return View();
        }
        // POST: /Shop/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("FirstName,LastName,Address,City,Province,PostalCode,Phone")] Order order)
        {
            // Auto-fill the fields not in the form
            order.OrderDate = DateTime.Now;
            order.CustomerId = User.Identity.Name;
            order.OrderTotal = _context.Carts
                .Where(c => c.CustomerId == User.Identity.Name)
                .Sum(c => c.Quantity * c.Price);

            // Save order to DB
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Move cart items into OrderDetails
            var cartItems = _context.Carts
                .Where(c => c.CustomerId == User.Identity.Name)
                .ToList();

            foreach (var item in cartItems)
            {
                var detail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    GadgetId = item.GadgetId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(detail);
            }

            // Clear the cart
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("Payment", new { id = order.OrderId });
        }   

        public IActionResult Payment(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Gadget)
                .FirstOrDefault(o => o.OrderId == id && o.CustomerId == User.Identity.Name);

            if (order == null) return NotFound();

            ViewBag.PublishableKey = _configuration.GetSection("Stripe")["PublishableKey"];

            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessPayment(int orderId)
        {
            StripeConfiguration.ApiKey = _configuration.GetSection("Stripe")["SecretKey"];

            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Gadget)
                .FirstOrDefault(o => o.OrderId == orderId);

            var lineItems = order.OrderDetails.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "cad",
                    UnitAmount = (long)(item.Price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Gadget.Name
                    }
                },
                Quantity = item.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Shop/OrderConfirmation?id={orderId}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Shop/Payment/{orderId}"
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        // GET: /Shop/OrderConfirmation
        public IActionResult OrderConfirmation(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Gadget)
                .FirstOrDefault(o => o.OrderId == id && o.CustomerId == User.Identity.Name);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}