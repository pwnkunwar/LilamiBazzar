﻿using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LilamiBazzar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="ADMIN")]
    public class AdminPanelController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public AdminPanelController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            int totalUsers = _dbContext.Users.Count();
            int totalListings = _dbContext.Products.Where(p => p.ProductRoles == "APPROVED").Count();
            int totalOngoingAuction = _dbContext.Auctions.Where(a=>a.EndDate > DateTime.UtcNow).Count();
            AdminPanel adminPanel =new AdminPanel
            {
                TotalUsers = totalUsers,
                TotalListings = totalListings,
                TotalOngoingAuction = totalOngoingAuction

            };
            return View(adminPanel);

        }
        public IActionResult LogOut()
        {
            if (Request.Cookies["Authorization"] != null)
            {
                Response.Cookies.Delete("Authorization");
                TempData["success"] = "Logged out successfully!";
            }
            return RedirectToAction("Index", "Home", new { area = "Users" });
        }
        public IActionResult Chart()
        {
            var allMonths = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Month = new DateTime(1, m, 1).ToString("MMM"),
                    MonthNumber = m
                })
                .ToList();

            var totalAuctionsData = _dbContext.Auctions
                .AsNoTracking()
                .GroupBy(a => a.EndDate.Month)
                .Select(g => new
                {
                    MonthNumber = g.Key,
                    TotalCount = g.Count()
                })
                .ToList();

            var ongoingAuctionsData = _dbContext.Auctions
                .AsNoTracking()
                .Where(a => a.EndDate > DateTime.UtcNow)
                .GroupBy(a => a.EndDate.Month)
                .Select(g => new
                {
                    MonthNumber = g.Key,
                    OngoingCount = g.Count()
                })
                .ToList();

            var combinedData = allMonths
                .GroupJoin(
                    totalAuctionsData,
                    m => m.MonthNumber,
                    t => t.MonthNumber,
                    (m, t) => new
                    {
                        Month = m.Month,
                        TotalCount = t.FirstOrDefault()?.TotalCount ?? 0
                    }
                )
                .GroupJoin(
                    ongoingAuctionsData,
                    mt => mt.Month,
                    o => allMonths.FirstOrDefault(am => am.MonthNumber == o.MonthNumber)?.Month,
                    (mt, o) => new
                    {
                        mt.Month,
                        mt.TotalCount,
                        OngoingCount = o.FirstOrDefault()?.OngoingCount ?? 0
                    }
                )
                .OrderBy(d => DateTime.ParseExact(d.Month, "MMM", null))
                .ToList();

            return Json(combinedData);
        }



    }
}
