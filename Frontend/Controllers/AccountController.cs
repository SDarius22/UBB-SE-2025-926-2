using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Frontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/AccountModels/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var client = _clientFactory.CreateClient();

            var loginUrl = $"http://localhost:5035/api/user/login?username={model.Username}&password={model.Password}";

            var response = await client.GetAsync(loginUrl);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid username or password";
                return View("~/Views/AccountModels/Login.cshtml", model);
            }

            var token = await response.Content.ReadAsStringAsync();

            HttpContext.Session.SetString("JWToken", token);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
