using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if (_context.Users.Any(u => u.Email == HttpContext.Session.GetString("UserEmail")))
            {
                User _User = _context.Users.FirstOrDefault(d => d.Email == HttpContext.Session.GetString("UserEmail"));
                return Redirect($"Dashboard/{_User.UserId}");
            }
            return View("Index");
        }
        [HttpGet("Dashboard/{uid}")]
        public IActionResult Dashboard(int uid)
        {
            if (_context.Users.Any(u => u.Email == HttpContext.Session.GetString("UserEmail")))
            {
                ViewBag.AllWeddings = _context.Weddings.ToList();
                ViewBag.WeddingCounts = _context.Attendances.ToList();
                ViewBag.Attendees = _context.Users.Include(s => s.Weddings).ThenInclude(d => d.Wedding).FirstOrDefault(a => a.UserId == uid);
                ViewBag.HasAttendees = _context.Attendances.Any(c=>c.UserId == uid);
                ViewBag.CurrentUserId = uid;
                return View();
            }
            return View("Index");
        }
        [HttpGet("Dashboard/WeddingDetails/{wid}")]
        public IActionResult WeddingDetails(int wid)
        {
            if (_context.Users.Any(u => u.Email == HttpContext.Session.GetString("UserEmail"))){
                ViewBag.Wedding = _context.Weddings.Include(s=>s.Guests).ThenInclude(p=>p.User).FirstOrDefault(w=>w.WeddingId == wid);
                return View();
            }
            return View("Index");
        }
        [HttpGet("Dashboard/newWedding")]
        public IActionResult newWedding()
        {
            if (_context.Users.Any(u => u.Email == HttpContext.Session.GetString("UserEmail"))){
                User _User = _context.Users.FirstOrDefault(d => d.Email == HttpContext.Session.GetString("UserEmail"));
                ViewBag.CurrentUser = _User.UserId;
                return View();
            }
            return View("Index");
        }
        [HttpPost("Dashboard/makeWedding")]
        public IActionResult makeWedding(Wedding newWedding)
        {
            User _User = _context.Users.FirstOrDefault(d => d.Email == HttpContext.Session.GetString("UserEmail"));
            ViewBag.CurrentUser = _User.UserId;
            if (ModelState.IsValid)
            {
                int current = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                int date = int.Parse(newWedding.Date.ToString("yyyyMMdd"));
                if (date < current)
                {
                    ModelState.AddModelError("Date", "Date cannot have already passed");
                    return View("newWedding");
                }
                _context.Weddings.Add(newWedding);
                _context.SaveChanges();
                return Redirect($"{newWedding.CreatorId}");
            }
            
            return View("newWedding");
        }
        [HttpPost("Dashboard/RSVP")]
        public IActionResult RSVP(Attendance newAttendance)
        {
            _context.Attendances.Add(newAttendance);
            _context.SaveChanges();
            return Redirect($"{newAttendance.UserId}");
        }
        [HttpPost("Dashboard/unRSVP")]
        public IActionResult unRSVP(Attendance deleteAttendance)
        {
            var userRSVP = _context.Attendances.FirstOrDefault(a => a.UserId == deleteAttendance.UserId && a.WeddingId == deleteAttendance.WeddingId);
            _context.Attendances.Remove(userRSVP);
            _context.SaveChanges();
            return Redirect($"{deleteAttendance.UserId}");
        }
        [HttpPost("Dashboard/Delete/{wid}")]
        public IActionResult Delete(int wid)
        {
            var wedding = _context.Weddings.FirstOrDefault(t => t.WeddingId == wid);
            var attendances = _context.Attendances.FirstOrDefault(f => f.WeddingId == wid);
            var userId = wedding.CreatorId;
            _context.Weddings.Remove(wedding);
            if (attendances != null)
            {
                _context.Attendances.Remove(attendances);
            }
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpPost("Attempting")]
        public IActionResult Attempting(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Users.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetString("UserEmail", newUser.Email);
                return Redirect($"Dashboard/{newUser.UserId}");
            }
            return View("Index");
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("Logging")]
        public IActionResult Logging(LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                User _User = _context.Users.FirstOrDefault(u => u.Email == loginUser.LoginEmail);
                if (_User == null)
                {
                    ModelState.AddModelError("LoginEmail", "Incorrect Email/Password");
                    return View("Login");
                }
                PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                PasswordVerificationResult result = Hasher.VerifyHashedPassword(loginUser, _User.Password, loginUser.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Incorrect Email/Password");
                    return View("Login");
                }
                HttpContext.Session.SetString("UserEmail", _User.Email);
                var uid = _User.UserId;
                return Redirect($"Dashboard/{uid}");
            }
            return View("Login");
        }
        [HttpPost("Dashboard/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
