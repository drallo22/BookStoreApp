using BookStoreApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Controllers
{
    
        public class AccountController : Controller
        {
            private UserManager<User> userManager;
            private SignInManager<User> signInManager;

		private BookstoreContext context { get; set; }

		public AccountController(UserManager<User> userMngr,
                SignInManager<User> signInMngr, BookstoreContext cxt)
            {
                userManager = userMngr;
                signInManager = signInMngr;
                context = cxt;
            }

        // The Register(), LogIn(), and LogOut()methods go here
        [HttpGet]
        public IActionResult LogIn(string returnURL = "")
        {
            var model = new LoginViewModel { ReturnUrl = returnURL };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.Username, model.Password,
                    isPersistent: model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) &&
                        Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid username/password.");
            return View(model);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Username };
                var result = await userManager.CreateAsync(user,model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user,
                        isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Book");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(
        String username, String newPassword)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound();
            }

            // Generate a password reset token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password using the token
            var result = await userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                // Password reset successful
                return RedirectToAction("LogIn");
            }
            else
            {
                // Handle the case where the password reset fails
                return View("Error");
            }
        }




    }


}
