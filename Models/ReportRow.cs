namespace AcxiomCRM.Models
{
    public class ReportRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? Source { get; set; }
        public string? Owner { get; set; }
        public string? Stage { get; set; }

        public decimal Amount { get; set; }
        public int Probability { get; set; }

        public DateTime? ExpectedCloseDate { get; set; }
        public DateTime? FollowUpDate { get; set; }

        public string? Action { get; set; }
        public string? EntityName { get; set; }
        public int? EntityId { get; set; }

        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? RelatedName { get; set; }

        public decimal WeightedAmount { get; set; }
    }
}
