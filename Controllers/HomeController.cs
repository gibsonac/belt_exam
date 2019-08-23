using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using belt_exam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace belt_exam.Controllers
{
    public class HomeController : Controller
    {
        public User loggedUser
        {
            get
            {
                // return dbContext.Users.FirstOrDefault(u => u.Userid == HttpContext.Session.GetInt32("Userid"));
                return dbContext.Users
            .Include(u => u.attendingActivites)
                .ThenInclude(list => list.AnActivity)
            .FirstOrDefault(u => u.Userid == HttpContext.Session.GetInt32("Userid"));
            }
        }
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                bool emailExists = dbContext.Users.Any(u => u.Email == user.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                User newUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
                HttpContext.Session.SetInt32("Userid", newUser.Userid);
                return RedirectToAction("Home");
            }
            return View("Index");
        }

        [HttpPost("submitlogin")]
        public IActionResult SubmitLogin(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LogEmail);
                if (userInDb == null)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LogPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email/Password");
                    return View("Index");
                }
                User newUser = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LogEmail);
                HttpContext.Session.SetInt32("Userid", newUser.Userid);
                return RedirectToAction("Home");
            }
            return View("Index");
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet("Home")]
        public IActionResult Home()
        {
            DateTime CurrentDate = DateTime.Now;
            if (HttpContext.Session.GetInt32("Userid") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.User = loggedUser;
            List<AnActivity> activityList = dbContext.Activities
            .OrderByDescending(a => a.Created_At)
            .Include(a => a.Creator)
            .Include(a => a.RSVPList)
                .ThenInclude(list => list.User)
            .Where(a => a.date > CurrentDate)
            .ToList();
            /////////if the date has expired, then it gets deleted from DataBase

            // foreach (var activity in activityList)
            // {
            //     if (activity.date < DateTime.Now)
            //     {
            //         AnActivity oldActivity = dbContext.Activities.FirstOrDefault(a => a.Activityid == activity.Activityid);
            //         dbContext.Remove(oldActivity);
            //         dbContext.SaveChanges();
            //     }
            // }
            // activityList = dbContext.Activities
            // .OrderByDescending(a => a.Created_At)
            // .Include(a => a.Creator)
            // .Include(a => a.RSVPList)
            //     .ThenInclude(list => list.User)
            // .ToList();
            return View(activityList);
        }
        [HttpGet("New")]
        public IActionResult NewActivity()
        {
            ViewBag.User = loggedUser;
            return View();
        }
        [HttpPost("SubmitActivity/{id}")]
        public IActionResult SubmitActivity(int id, AnActivity newActivity)
        {
            if (ModelState.IsValid)
            {
                newActivity.Creator = loggedUser;
                dbContext.Activities.Add(newActivity);
                dbContext.SaveChanges();
                return RedirectToAction("Home");
            }
            ViewBag.User = loggedUser;
            return View("NewActivity");
        }
        [HttpGet("activity/{id}")]
        public IActionResult Activity(int id)
        {
            AnActivity thisActivity = dbContext.Activities
            .Include(a => a.Creator)
            .Include(a => a.RSVPList)
                .ThenInclude(list => list.User)
            .FirstOrDefault(a => a.Activityid == id);
            ViewBag.Activity = thisActivity;
            @ViewBag.User = loggedUser;
            return View();
        }
        [HttpGet("Join/{id}")]
        public IActionResult Join(int id)
        {
            AttendeeList newAttendee = new AttendeeList();
            newAttendee.Userid = loggedUser.Userid;
            newAttendee.Activityid = id;
            // checking if something is already scheduled then
            // User thisUser = dbContext.Users
            // .Include(u => u.attendingActivites)
            //     .ThenInclude(list => list.AnActivity)
            // .FirstOrDefault(u => u.Userid == loggedUser.Userid);
            // foreach(var activity in loggedUser.attendingActivites)
            // {
            //     if(activity.AnActivity.date == newAttendee.AnActivity.date)
            //     {
            //         ModelState.AddModelError("LogEmail", "Invalid Email/Password");
            //         return View("Index");
            //     }
            // }
            // enter into database and return
            dbContext.AttendeeLists.Add(newAttendee);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }
        [HttpGet("unrsvp/{id}")]
        public IActionResult UNRSVP(int id)
        {
            AttendeeList theRSVP = dbContext.AttendeeLists.FirstOrDefault(List => List.Activityid == id && List.Userid == loggedUser.Userid);
            dbContext.Remove(theRSVP);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }
        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            AnActivity thisActivity = dbContext.Activities.FirstOrDefault(a => a.Activityid == id);
            dbContext.Remove(thisActivity);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }
    }
}
