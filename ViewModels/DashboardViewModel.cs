namespace AcxiomCRM.ViewModels
{
    public class DashboardViewModel
    {
        public int CustomerCount { get; set; }
        public int LeadCount { get; set; }
        public int OpportunityCount { get; set; }
        public int FollowUpCount { get; set; }

        public int ActivityCount { get; set; }

        public decimal PipelineValue { get; set; }

        public List<RecentCustomerViewModel> RecentCustomers { get; set; }
            = new List<RecentCustomerViewModel>();

        public List<RecentLeadViewModel> RecentLeads { get; set; }
            = new List<RecentLeadViewModel>();
    }

    public class RecentCustomerViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Company { get; set; }
        public string? Status { get; set; }
    }

    public class RecentLeadViewModel
    {
        public int Id { get; set; }
        public string LeadName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? CompanyName { get; set; }
        public string Status { get; set; } = "";
    }
}
