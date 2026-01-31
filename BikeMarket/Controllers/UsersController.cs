using DTO.User;
using Business.Interface;
using Business.Models;
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BikeMarket.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _userService.GetAllAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRegisterDTO registerDto)    
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterAsync(registerDto);
                if (!result.Success)
                {
                    ModelState.AddModelError("Email", result.ErrorMessage ?? "Đăng ký thất bại");
                    return View(registerDto);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(registerDto);
        }

        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDTO loginDto, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var result = await _userService.AuthenticateAsync(loginDto);
            if (!result.Success || result.User == null)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Email hoặc mật khẩu không đúng");
                return View(loginDto);
            }

            var user = result.User;

            // ✅ THÊM DÒNG NÀY: Lưu UserId vào Session
            HttpContext.Session.SetString("UserId", user.Id.ToString());

            // Tạo authentication cookie
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Remember me
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (string.Equals(user.Role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "HomeAdmin");
            }

            // Redirect về trang ban đầu hoặc Home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            //chuyển qua trang view Index
            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,EmailVerified,PasswordHash,Phone,Role,RatingAvg,Status,CreatedAt")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateAsync(user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


      
        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // ✅ THÊM DÒNG NÀY: Xóa session khi logout
            HttpContext.Session.Remove("UserId");
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        private Task<bool> UserExists(int id)
        {
            return _userService.ExistsAsync(id);
        }



    }
}
