namespace AcxiomCRM.Models
{
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }

        public int TotalLeads { get; set; }

        public int TotalOpportunities { get; set; }

        public int TotalFollowUps { get; set; }

        public int TotalActivities { get; set; }

        public int OpenOpportunities { get; set; }

        public int WonOpportunities { get; set; }

        public decimal TotalOpportunityValue { get; set; }

        // Lead Status

        public int NewLeads { get; set; }

        public int ContactedLeads { get; set; }

        public int QualifiedLeads { get; set; }

        public int ConvertedLeads { get; set; }

        public int LostLeads { get; set; }

        // Opportunity Stages

        public int QualificationOpportunities { get; set; }

        public int ProposalOpportunities { get; set; }

        public int NegotiationOpportunities { get; set; }

        public int WonOpportunitiesCount { get; set; }

        public int LostOpportunitiesCount { get; set; }
    }
}