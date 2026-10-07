using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string reportType = "Customers",
            string? search = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string sort = "date_desc",
            int page = 1,
            int pageSize = 10)
        {
            if (page < 1)
                page = 1;

            if (pageSize != 10 && pageSize != 25 && pageSize != 50)
                pageSize = 10;

            var rows = new List<ReportRow>();

            switch (reportType)
            {
                case "Customers":

                    var customers = _context.Customers
                        .AsNoTracking()
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        customers = customers.Where(c =>
                            c.FullName.Contains(search) ||
                            c.Email.Contains(search) ||
                            (c.Phone != null && c.Phone.Contains(search)) ||
                            (c.Company != null && c.Company.Contains(search)));
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                        customers = customers.Where(c => c.Status == status);

                    if (fromDate.HasValue)
                        customers = customers.Where(c => c.CreatedAt >= fromDate.Value);

                    if (toDate.HasValue)
                        customers = customers.Where(c =>
                            c.CreatedAt < toDate.Value.Date.AddDays(1));

                    rows = await customers
                        .Select(c => new ReportRow
                        {
                            Id = c.Id,
                            Name = c.FullName,
                            Email = c.Email,
                            Status = c.Status,
                            Owner = "N/A",
                            RelatedName = c.Company,
                            CreatedDate = c.CreatedAt
                        })
                        .ToListAsync();

                    break;

                case "Leads":

                    var leads = _context.Leads
                        .AsNoTracking()
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        leads = leads.Where(l =>
                            l.LeadName.Contains(search) ||
                            l.Email.Contains(search) ||
                            (l.CompanyName != null &&
                             l.CompanyName.Contains(search)));
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                        leads = leads.Where(l => l.Status == status);

                    if (fromDate.HasValue)
                        leads = leads.Where(l => l.CreatedDate >= fromDate.Value);

                    if (toDate.HasValue)
                        leads = leads.Where(l =>
                            l.CreatedDate < toDate.Value.Date.AddDays(1));

                    rows = await leads
                        .Select(l => new ReportRow
                        {
                            Id = l.Id,
                            Name = l.LeadName,
                            Email = l.Email,
                            Status = l.Status,
                            Source = l.Source,
                            Owner = l.AssignedTo,
                            Amount = l.ExpectedValue,
                            CreatedDate = l.CreatedDate
                        })
                        .ToListAsync();

                    break;

                case "FollowUps":

                    var followUps = _context.FollowUps
                        .AsNoTracking()
                        .Include(f => f.Customer)
                        .Include(f => f.Lead)
                        .Include(f => f.Opportunity)
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(search))
                        followUps = followUps.Where(f =>
                            f.Subject.Contains(search));

                    if (!string.IsNullOrWhiteSpace(status))
                        followUps = followUps.Where(f =>
                            f.Status == status);

                    if (fromDate.HasValue)
                        followUps = followUps.Where(f =>
                            f.FollowUpDate >= fromDate.Value);

                    if (toDate.HasValue)
                        followUps = followUps.Where(f =>
                            f.FollowUpDate < toDate.Value.Date.AddDays(1));

                    rows = await followUps
                        .Select(f => new ReportRow
                        {
                            Id = f.Id,
                            Name = f.Subject,
                            Status = f.Status,
                            FollowUpDate = f.FollowUpDate,
                            RelatedName =
                                f.Customer != null
                                    ? f.Customer.FullName
                                    : f.Lead != null
                                        ? f.Lead.LeadName
                                        : f.Opportunity != null
                                            ? f.Opportunity.OpportunityName
                                            : "N/A",
                            CreatedDate = f.CreatedDate
                        })
                        .ToListAsync();

                    break;

                case "Opportunities":

                    var opportunities = _context.Opportunities
                        .AsNoTracking()
                        .Include(o => o.Customer)
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(search))
                        opportunities = opportunities.Where(o =>
                            o.OpportunityName.Contains(search));

                    if (!string.IsNullOrWhiteSpace(status))
                        opportunities = opportunities.Where(o =>
                            o.Status == status);

                    if (fromDate.HasValue)
                        opportunities = opportunities.Where(o =>
                            o.CreatedDate >= fromDate.Value);

                    if (toDate.HasValue)
                        opportunities = opportunities.Where(o =>
                            o.CreatedDate < toDate.Value.Date.AddDays(1));

                    rows = await opportunities
                        .Select(o => new ReportRow
                        {
                            Id = o.Id,
                            Name = o.OpportunityName,
                            Status = o.Status,
                            Stage = o.Stage,
                            Owner = o.AssignedTo,
                            Amount = o.Amount,
                            Probability = o.Probability,
                            ExpectedCloseDate = o.ExpectedCloseDate,
                            RelatedName =
                                o.Customer != null
                                    ? o.Customer.FullName
                                    : "N/A",
                            CreatedDate = o.CreatedDate,
                            WeightedAmount =
                                o.Amount * o.Probability / 100m
                        })
                        .ToListAsync();

                    break;

                case "Pipeline":

                    rows = await _context.Opportunities
                        .AsNoTracking()
                        .GroupBy(o => o.Stage)
                        .Select(g => new ReportRow
                        {
                            Name = g.Key,
                            Amount = g.Sum(o => o.Amount),
                            Probability = g.Any()
                                ? (int)Math.Round(g.Average(o => o.Probability))
                                : 0,
                            WeightedAmount =
                                g.Sum(o => o.Amount * o.Probability / 100m)
                        })
                        .ToListAsync();

                    break;

                case "SalesConversion":

                    var totalLeads = await _context.Leads.CountAsync();

                    var convertedLeads =
                        await _context.Leads.CountAsync(
                            l => l.Status == "Converted");

                    var totalOpportunities =
                        await _context.Opportunities.CountAsync();

                    var wonOpportunities =
                        await _context.Opportunities.CountAsync(
                            o => o.Status == "Won");

                    rows = new List<ReportRow>
                    {
                        new ReportRow
                        {
                            Name = "Total Leads",
                            Amount = totalLeads
                        },
                        new ReportRow
                        {
                            Name = "Converted Leads",
                            Amount = convertedLeads
                        },
                        new ReportRow
                        {
                            Name = "Lead Conversion Rate",
                            Amount = totalLeads == 0
                                ? 0
                                : Math.Round(convertedLeads * 100m / totalLeads, 2)
                        },
                        new ReportRow
                        {
                            Name = "Total Opportunities",
                            Amount = totalOpportunities
                        },
                        new ReportRow
                        {
                            Name = "Won Opportunities",
                            Amount = wonOpportunities
                        },
                        new ReportRow
                        {
                            Name = "Opportunity Win Rate",
                            Amount = totalOpportunities == 0
                                ? 0
                                : Math.Round(wonOpportunities * 100m / totalOpportunities, 2)
                        }
                    };

                    break;

                case "UserActivity":

                    rows = await _context.AuditLogs
                        .AsNoTracking()
                        .GroupBy(a => a.UserName ?? "System")
                        .Select(g => new ReportRow
                        {
                            Name = g.Key,
                            Amount = g.Count()
                        })
                        .ToListAsync();

                    break;

                case "Audit":

                    var audit = _context.AuditLogs
                        .AsNoTracking()
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        audit = audit.Where(a =>
                            a.EntityName.Contains(search) ||
                            (a.UserName != null &&
                             a.UserName.Contains(search)) ||
                            (a.Details != null &&
                             a.Details.Contains(search)));
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                        audit = audit.Where(a => a.Action == status);

                    if (fromDate.HasValue)
                        audit = audit.Where(a => a.Timestamp >= fromDate.Value);

                    if (toDate.HasValue)
                        audit = audit.Where(a =>
                            a.Timestamp < toDate.Value.Date.AddDays(1));

                    rows = await audit
                        .Select(a => new ReportRow
                        {
                            Id = a.Id,
                            Name = a.UserName ?? "System",
                            Action = a.Action,
                            EntityName = a.EntityName,
                            EntityId = a.EntityId,
                            Timestamp = a.Timestamp,
                            Details = a.Details
                        })
                        .ToListAsync();

                    break;
            }

            // SORTING
            rows = sort switch
            {
                "name_asc" =>
                    rows.OrderBy(r => r.Name).ToList(),

                "name_desc" =>
                    rows.OrderByDescending(r => r.Name).ToList(),

                "amount_asc" =>
                    rows.OrderBy(r => r.Amount).ToList(),

                "amount_desc" =>
                    rows.OrderByDescending(r => r.Amount).ToList(),

                "date_asc" =>
                    rows.OrderBy(r =>
                        r.FollowUpDate ?? r.ExpectedCloseDate ??
                        r.CreatedDate).ToList(),

                _ =>
                    rows.OrderByDescending(r =>
                        r.Timestamp != default
                            ? r.Timestamp
                            : r.FollowUpDate ??
                              r.ExpectedCloseDate ??
                              r.CreatedDate).ToList()
            };

            var totalRecords = rows.Count;

            var totalPages =
                Math.Max(1, (int)Math.Ceiling(
                    totalRecords / (double)pageSize));

            if (page > totalPages)
                page = totalPages;

            rows = rows
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.ReportType = reportType;
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            return View(rows);
        }
    }
}
