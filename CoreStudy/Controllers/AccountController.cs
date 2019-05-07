using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreStudy.Models;
using CoreStudy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CoreStudy.Controllers
{
    public class AccountController : Controller
    {
        #region DI
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }
        #endregion


        // Get: Account
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Users() => View(userManager.Users.ToList());

        // GET: Account/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            //initialize Identity DB by Roles
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            return View();
        }


        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Email", "Password", "ConfirmPassword", "UserRoles")] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser() { Email = model.Email, UserName = model.Email };

                //trying to create and save new IdentityUser inside "Identity" DB
                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, model.UserRoles);
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


        // GET: Account/ForgetPassword
        [HttpGet]
        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }


        // POST: Account/ForgetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword([Bind("Email")] ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByNameAsync(model.Email);

                //we have such user in Identity DB
                if (user != null)
                {
                    string callbackUrl = Url.Action("ResetPassword", "Account", new { Id = user.Id, Email = model.Email }, protocol: HttpContext.Request.Scheme);

                    await emailSender.SendAsync(model.Email, "Reset password", $"Link to {callbackUrl}. Sent on {DateTime.Now}");

                    ViewBag.Address = model.Email;
                    return View("ForgetPasswordInforming");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "There is no such email in a system. Register first");
                }
            }

            return View(model);
        }


        // GET: Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string Id = null, string Email = null)
        {
            if (Id == null || Email == null)
            {
                return BadRequest();
            }

            return View();
        }


        // POST: Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([Bind("Id", "Email", "OldPassword", "NewPassword", "ConfirmNewPassword")] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByIdAsync(model.Id);

                //we have such user in Identity DB
                if (user != null)
                {
                    IdentityResult result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        return View("ResetPasswordInforming");
                    }
                    else
                    {
                        foreach (IdentityError err in result.Errors)
                        {
                            ModelState.AddModelError(String.Empty, err.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "There is no such email in a system. Register first");
                }
            }

            return View(model);
        }
    }
}