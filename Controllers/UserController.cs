using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using csbb0328.Models;
using csbb0328.ViewModels;
using csbb0328.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csbb0328.Controllers
{
    public class UserController : Controller
    {
        private readonly UserFactory userFactory;
 
        //use dependency injection on the HomeController constructor to get a UserFactory object
        public UserController(UserFactory user) {
            userFactory = user;
        }


        //get: send home route to login
        [HttpGetAttribute]
        [RouteAttribute("")]
        public IActionResult Index(){
            return RedirectToAction("ShowLogin");
        }
        // GET: /users/login
        [HttpGet]
        [Route("users/login")]
        public IActionResult ShowLogin(){
            return View("Login");
        }

        // GET: /users/login
        [HttpGet]
        [Route("logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("ShowLogin");
        }

        // GET: /users/login
        [HttpGet]
        [Route("users/register")]
        public IActionResult ShowRegister(){
            return View("Register");
        }
        
        // POST: /login
        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginViewModel loginModel){
            
            int? isInSession = HttpContext.Session.GetInt32("user");
            if(isInSession == null)
            {
                if (ModelState.IsValid)
                {
                    bool exists = userFactory.CheckUserInDB(loginModel.email);
                    if(!exists){
                        ViewBag.error = "This email isn't registered. Please register.";
                        return View("Register");
                    }
                    User user = userFactory.GetCurrentUser(loginModel.email);
                    HttpContext.Session.SetInt32("user", user.id);
                    return RedirectToAction("Success");
                        
                }
                return View("Login", loginModel);
            }
            return RedirectToAction("Success");
            
        }

        // POST: /register
        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterViewModel regModel){
            int? isInSession = HttpContext.Session.GetInt32("user");
            if(isInSession == null)
            {
                if (ModelState.IsValid)
                {
                    PasswordHasher<RegisterViewModel> Hasher = new PasswordHasher<RegisterViewModel>();
                    User user = new User{
                        name = regModel.name,
                        email = regModel.email,
                        description = regModel.description,
                        password = Hasher.HashPassword(regModel, regModel.password)
                    };
                    bool exists = userFactory.CheckUserInDB(user.email);
                    if(exists){
                        ViewBag.error = "This email is already registered. Please log in.";
                        return View("Login");
                    }
                    else{
                        userFactory.AddUser(user);
                        User newUser = userFactory.GetCurrentUser(user.email);
                        // ViewBag.user = newUser;
                    
                        HttpContext.Session.SetInt32("user", newUser.id);
                        return RedirectToAction("Success");
                    }
                    
                }
                return View("Register", regModel);
            }
            return RedirectToAction("Success");
        }

        [HttpGetAttribute]
        [RouteAttribute("success")]
        public IActionResult Success()
        {
            int? isInSession = HttpContext.Session.GetInt32("user");
            System.Console.WriteLine(isInSession);
            if(isInSession == null){
                return RedirectToAction("ShowLogin");
            }
            return RedirectToAction("ShowProfile");
        }

        [HttpGetAttribute]
        [RouteAttribute("showProfile")]
        public IActionResult ShowProfile()
        {
            int? isInSession = HttpContext.Session.GetInt32("user");
            System.Console.WriteLine(isInSession);
            if(isInSession == null){
                return RedirectToAction("ShowLogin");
            }
            
            //add current user to ViewBag
            ViewBag.currentUser = userFactory.GetUserByID((int)HttpContext.Session.GetInt32("user"));
            ViewBag.allUsers = userFactory.GetAllUsers();
            ViewBag.connectedUsers = userFactory.GetConnectedUsers((int)HttpContext.Session.GetInt32("user"));
            ViewBag.requestedUsers = userFactory.GetRequestedUsers((int)HttpContext.Session.GetInt32("user"));

            foreach(var user in ViewBag.requestedUsers)
            {
                System.Console.WriteLine(user);
            }

            return View("ProfessionalProfile");
        }

        [HttpGetAttribute]
        [RouteAttribute("user/{uid}")]
        public IActionResult ShowUser(int uid){
            int? isInSession = HttpContext.Session.GetInt32("user");
            System.Console.WriteLine(isInSession);
            if(isInSession == null){
                return RedirectToAction("ShowLogin");
            }

            ViewBag.user = userFactory.GetUserByID(uid);

            return View("UserProfile");
        }

        [HttpGet]
        [RouteAttribute("users")]
        public IActionResult ShowAllUsers()
        {
            int? isInSession = HttpContext.Session.GetInt32("user");
            System.Console.WriteLine(isInSession);
            if(isInSession == null){
                return RedirectToAction("ShowLogin");
            }

            User currentUser = userFactory.GetUserByID((int)HttpContext.Session.GetInt32("user"));
            List<User> allUsers = userFactory.GetAllUsers();
            List<User> connectedUsers = userFactory.GetAllConnectUsers((int)HttpContext.Session.GetInt32("user"));
            List<User> otherUsers = new List<User>();

            foreach(var user in allUsers)
            {
                if(user.id != currentUser.id)
                {
                    bool connected = false;
                    foreach(var cUser in connectedUsers)
                    {
                        if(user.id == cUser.id)
                        {
                            connected = true;
                        }
                    }
                    if(connected == false)
                    {
                        otherUsers.Add(user);
                    }
                }
            }

            ViewBag.currentUser = currentUser;
            ViewBag.otherUsers = otherUsers;

            return View("Users");
        }

        [HttpGet]
        [RouteAttribute("addConnection/{uid}")]
        public IActionResult AddConnection(int uid)
        {
            int? isInSession = HttpContext.Session.GetInt32("user");
            System.Console.WriteLine(isInSession);
            if(isInSession == null){
                return RedirectToAction("ShowLogin");
            }

            int currentID = (int)HttpContext.Session.GetInt32("user");
            userFactory.AddConnection(currentID, uid);

            return RedirectToAction("ShowProfile");
        }

        [HttpGet]
        [RouteAttribute("acceptConnection/{uid}")]
        public IActionResult AcceptConnection(int uid)
        {
            int? isInSession = HttpContext.Session.GetInt32("user");
            System.Console.WriteLine(isInSession);
            if(isInSession == null){
                return RedirectToAction("ShowLogin");
            }

            int currentID = (int)HttpContext.Session.GetInt32("user");
            userFactory.AcceptConnection(currentID, uid);

            return RedirectToAction("ShowProfile");
        }

        [HttpGet]
        [RouteAttribute("deleteConnection/{uid}")]
        public IActionResult DeleteConnection(int uid)
        {
            int? isInSession = HttpContext.Session.GetInt32("user");
            System.Console.WriteLine(isInSession);
            if(isInSession == null){
                return RedirectToAction("ShowLogin");
            }

            int currentID = (int)HttpContext.Session.GetInt32("user");
            userFactory.DeleteConnection(currentID, uid);

            return RedirectToAction("ShowProfile");
        }
    }
}
