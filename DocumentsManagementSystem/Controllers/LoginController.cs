using DocumentsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        private LoginLogoutExampleContext _context;
        public LoginController(LoginLogoutExampleContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel login)
        {
            var user = _context.Userdetails.SingleOrDefault(u => u.Email == login.Email && u.Password == login.Password);
            if (user == null)
            {
                ViewBag.LoginFailed = "Login failed, please try again";
                return View();
            }
            else
            {
                HttpContext.Session.SetString("userId", user.Id.ToString());
            }
            return RedirectToAction("index", "home");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                Userdetail u = new Userdetail
                {
                    Name = register.Name,
                    Email = register.Email,
                    Password = register.Password,
                    Mobile = register.Mobile,
                };
                _context.Userdetails.Add(u);
                _context.SaveChanges();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}
