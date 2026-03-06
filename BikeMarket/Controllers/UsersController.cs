using DTO.User;
using Business.Interface;
using Business.Models;
using BikeMarket.Models;
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
        private readonly IVehicleService _vehicleService;
        private readonly IUserRatingService _userRatingService;
        private readonly IOrderService _orderService;

        public UsersController(IUserService userService, IVehicleService vehicleService, IUserRatingService userRatingService, IOrderService orderService)
        {
            _userService = userService;
            _vehicleService = vehicleService;
            _userRatingService = userRatingService;
            _orderService = orderService;
        }

        // GET: Users
        public async Task<IActionResult> Index(
            string? search = null,
            decimal ratingAvg = 0,
            string? role = null,
            string? sortBy = "email",
            string? sortOrder = "asc")
        {
            var users = await _userService.GetAllUserAsync(search, ratingAvg, role, sortBy, sortOrder);
            
            // Pass filter values to view for form state
            ViewBag.Search = search;
            ViewBag.RatingAvg = ratingAvg;
            ViewBag.Role = role;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            
            return View(users);
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

                TempData["SuccessMessage"] = "Tạo người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(registerDto);
        }

        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterDTO registerDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterAsync(registerDto);
                if (!result.Success)
                {
                    ModelState.AddModelError("Email", result.ErrorMessage ?? "Đăng ký thất bại");
                    return View(registerDto);
                }

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(Login));
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
                    TempData["SuccessMessage"] = "User updated successfully!";
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
            try
            {
                await _userService.DeleteAsync(id);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
            }
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

        // GET: Users/Owner/5
        public async Task<IActionResult> Owner(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _userService.GetByIdAsync(id.Value);
            if (seller == null)
            {
                return NotFound();
            }

            var summary = await _vehicleService.GetMyPostSummaryAsync(id.Value);
            var vehicles = summary.Vehicles
                .Where(v => string.IsNullOrEmpty(v.Status) || v.Status.Equals("available", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var ratings = await _userRatingService.GetByRatedUserAsync(id.Value);
            var tagCounts = ratings
                .GroupBy(r => r.Rating)
                .OrderByDescending(g => g.Key)
                .ToDictionary(g => $"{g.Key} sao", g => g.Count());

            var canRateSeller = false;
            int? paidOrderId = null;
            var sessionUserId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(sessionUserId) && int.TryParse(sessionUserId, out var buyerId) && buyerId != seller.Id)
            {
                var paidOrder = await _orderService.GetLatestPaidOrderAsync(buyerId, seller.Id);
                if (paidOrder != null)
                {
                    var existingRating = await _userRatingService.GetByOrderAndRaterAsync(paidOrder.Id, buyerId);
                    if (existingRating == null)
                    {
                        canRateSeller = true;
                        paidOrderId = paidOrder.Id;
                    }
                }
            }

            var viewModel = new OwnerProfileViewModel
            {
                Seller = seller,
                RatingAvg = seller.RatingAvg,
                RatingCount = ratings.Count,
                Ratings = ratings,
                TagCounts = tagCounts,
                Vehicles = vehicles,
                CanRateSeller = canRateSeller,
                PaidOrderId = paidOrderId
            };

            return View(viewModel);
        }

        private Task<bool> UserExists(int id)
        {
            return _userService.ExistsAsync(id);
        }

        // GET: Users/Profile/5
        public async Task<IActionResult> Profile(int? id)
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

        // GET: Users/EditProfile/5
        public async Task<IActionResult> EditProfile(int? id)
        {
            var sessionUserId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(sessionUserId))
            {
                return RedirectToAction(nameof(Login));
            }

            var currentUserId = int.Parse(sessionUserId);
            if (id == null)
            {
                id = currentUserId;
            }

            if (id.Value != currentUserId)
            {
                return Forbid();
            }

            var user = await _userService.GetByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserEditProfileViewModel
            {
                Id = user.Id,
                Name = user.Name ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.Phone
            };

            return View(viewModel);
        }

        // POST: Users/EditProfile/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserEditProfileViewModel model)
        {
            var sessionUserId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(sessionUserId))
            {
                return RedirectToAction(nameof(Login));
            }

            var currentUserId = int.Parse(sessionUserId);
            if (model.Id != currentUserId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Name;
            user.Phone = model.Phone;

            await _userService.UpdateAsync(user);
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";

            return RedirectToAction(nameof(Profile), new { id = model.Id });
        }
    }
}
