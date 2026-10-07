using AcxiomCRM.Data;
using AcxiomCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            try
            {
                model.CustomerCount = await _context.Customers.CountAsync();
            }
            catch
            {
                model.CustomerCount = 0;
            }

            try
            {
                model.LeadCount = await _context.Leads.CountAsync();
            }
            catch
            {
                model.LeadCount = 0;
            }

            try
            {
                model.OpportunityCount = await _context.Opportunities.CountAsync();
            }
            catch
            {
                model.OpportunityCount = 0;
            }

            try
            {
                model.FollowUpCount = await _context.FollowUps.CountAsync();
            }
            catch
            {
                model.FollowUpCount = 0;
            }

            try
            {
                model.ActivityCount = await _context.Activities.CountAsync();
            }
            catch
            {
                model.ActivityCount = 0;
            }

            try
            {
                model.PipelineValue = await _context.Opportunities
                    .Select(x => (decimal?)x.Amount)
                    .SumAsync() ?? 0;
            }
            catch
            {
                model.PipelineValue = 0;
            }

            try
            {
                model.RecentCustomers = await _context.Customers
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Take(5)
                    .Select(x => new RecentCustomerViewModel
                    {
                        Id = x.Id,
                        FullName = x.FullName,
                        Email = x.Email,
                        Company = x.Company,
                        Status = x.Status
                    })
                    .ToListAsync();
            }
            catch
            {
                model.RecentCustomers = new List<RecentCustomerViewModel>();
            }

            try
            {
                model.RecentLeads = await _context.Leads
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Take(5)
                    .Select(x => new RecentLeadViewModel
                    {
                        Id = x.Id,
                        LeadName = x.LeadName,
                        Email = x.Email,
                        CompanyName = x.CompanyName,
                        Status = x.Status
                    })
                    .ToListAsync();
            }
            catch
            {
                model.RecentLeads = new List<RecentLeadViewModel>();
            }

            return View(model);
        }
    }
}
