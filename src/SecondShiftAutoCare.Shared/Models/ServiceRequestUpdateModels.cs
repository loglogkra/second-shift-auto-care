using System.ComponentModel.DataAnnotations;

namespace SecondShiftAutoCare.Shared.Models;

public sealed class ServiceRequestStatusUpdateModel
{
    [Required, StringLength(50)]
    public string Status { get; set; } = string.Empty;
}

public sealed class ServiceRequestQuoteUpdateModel
{
    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? EstimateLow { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? EstimateHigh { get; set; }

    [StringLength(1000)]
    public string? PartsNeeded { get; set; }

    [StringLength(4000)]
    public string? InternalQuoteNotes { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? LaborAmount { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? PartsAmount { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? ShopSuppliesAmount { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? TotalEstimate { get; set; }

    [StringLength(100)]
    public string? QuoteTemplate { get; set; }

    [StringLength(2000)]
    public string? AssumptionDisclaimerText { get; set; }

    [StringLength(2000)]
    public string? GoodOption { get; set; }

    [StringLength(2000)]
    public string? BetterOption { get; set; }

    [StringLength(2000)]
    public string? BestOption { get; set; }

    public DateTime? QuoteExpirationDate { get; set; }

    [StringLength(50)]
    public string CustomerApprovalStatus { get; set; } = ServiceRequestApprovalStatuses.Pending;
}

public sealed class ServiceRequestNotesUpdateModel
{
    [StringLength(2000)]
    public string? InternalNotes { get; set; }
}

public static class ServiceRequestStatuses
{
    public const string New = "New";
    public const string NeedInfo = "Need Info";
    public const string Quoted = "Quoted";
    public const string Contacted = "Contacted";
    public const string Scheduled = "Scheduled";
    public const string PartsOrdered = "Parts Ordered";
    public const string InProgress = "In Progress";
    public const string Completed = "Completed";
    public const string Paid = "Paid";
    public const string Declined = "Declined";
    public const string Canceled = "Canceled";

    public static readonly string[] All = [New, Contacted, NeedInfo, Quoted, Scheduled, PartsOrdered, InProgress, Completed, Paid, Declined, Canceled];
}

public static class ServiceRequestUrgencyLevels
{
    public const string Routine = "Routine";
    public const string Soon = "Soon";
    public const string Urgent = "Urgent";

    public static readonly string[] All = [Routine, Soon, Urgent];
}

public static class ServiceRequestApprovalStatuses
{
    public const string Pending = "Pending";
    public const string Sent = "Sent";
    public const string Approved = "Approved";
    public const string Declined = "Declined";
    public const string Expired = "Expired";
    public const string Question = "Question";

    public static readonly string[] All = [Pending, Sent, Approved, Declined, Question, Expired];
}

public static class QuoteTemplateOptions
{
    public const string GeneralRepair = "General repair";
    public const string Diagnostics = "Diagnostics";
    public const string Brakes = "Brakes";
    public const string Maintenance = "Maintenance";
    public const string SuspensionSteering = "Suspension / steering";
    public const string StartingCharging = "Starting / charging";

    public static readonly string[] All = [GeneralRepair, Diagnostics, Brakes, Maintenance, SuspensionSteering, StartingCharging];
}
