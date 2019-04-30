using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreStudy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CoreStudy.Controllers
{
    public class AccountController : Controller
    {
        #region DI
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        #endregion


        // GET: Account/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }


        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Email", "Password", "ConfirmPassword")] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser() { Email = model.Email, UserName = model.Email };

                //trying to create and save new IdentityUser inside "Identity" DB
                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError(String.Empty, err.Description);
                    }
                }
            }

            return View(model);
        }


        // GET: Account/Login
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }


        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email", "Password", "RememberMe")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                SignInResult result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Invalid Login/Password");
                }
            }

            return View(model);
        }


        // GET: Account/LogOff
        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            //clear authentication cookies for current IdentityUser
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}