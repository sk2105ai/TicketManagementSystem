using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketMonitoringSystem.Models;
using TicketMonitoringSystem.Services;

namespace TicketMonitoringSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        // Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login Post Action
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            var user = _userService.Authenticate(username, password);
            if (user != null)
            {
                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                // Create identity
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create authentication properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                };

                // Sign in
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Dashboard", "Ticket");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }

        // Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Register Post Action
        [HttpPost]
        public IActionResult Register(User user)
        {
            // Clear any existing ModelState errors for TicketId as it's auto-generated
            ModelState.Remove("Salt");
            ModelState.Remove("PasswordHash");

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // Check if username or email is already taken
            if (_userService.IsUsernameTaken(user.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(user);
            }

            if (_userService.IsEmailTaken(user.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(user);
            }

            // Attempt to register user
            bool registrationSuccess = _userService.RegisterUser(user);
            if (registrationSuccess)
            {
                TempData["SuccessMessage"] = "Registration successful. Please log in.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed. Please try again.");
            return View(user);
        }

        // Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Access Denied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
