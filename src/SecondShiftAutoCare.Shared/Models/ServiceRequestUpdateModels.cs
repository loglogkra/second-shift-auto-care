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
}

public sealed class ServiceRequestNotesUpdateModel
{
    [StringLength(2000)]
    public string? InternalNotes { get; set; }
}

public static class ServiceRequestStatuses
{
    public const string New = "New";
    public const string Contacted = "Contacted";
    public const string Quoted = "Quoted";
    public const string Scheduled = "Scheduled";
    public const string Completed = "Completed";
    public const string Canceled = "Canceled";

    public static readonly string[] All = [New, Contacted, Quoted, Scheduled, Completed, Canceled];
}
