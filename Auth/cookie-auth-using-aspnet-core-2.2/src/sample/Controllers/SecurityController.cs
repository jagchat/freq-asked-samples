using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using sample.Models;

namespace sample.Controllers
{
    public class SecurityController : Controller
    {
        public IActionResult Login(string returnUrl)
        {
            var m = new LoginViewModel() { ReturnUrl = returnUrl };
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel m)
        {
            if (m.Email != "jag@email.com" &&
                m.Email != "scott@email.com")
            {
                var nm = new LoginViewModel() { Email = m.Email, ReturnUrl = m.ReturnUrl, ErrorInfo = "Invalid UserId/Password" };
                return View(nm);
            }

            List<Claim> claims = null;
            if (m.Email == "jag@email.com") //admin claims
            {
                claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, m.Email),
                    new Claim("FullName", "Jag"),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };
            }
            else //member claims
            {
                claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, m.Email),
                    new Claim("FullName", "Scott"),
                    new Claim(ClaimTypes.Role, "Member"),
                    new Claim("MemberId", "scott-01-abc"),
                };
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. 

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect(m.ReturnUrl);
        }

        public async Task<IActionResult> Logout(string returnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(returnUrl);
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }
    }
}