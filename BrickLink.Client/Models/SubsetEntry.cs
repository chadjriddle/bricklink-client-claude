using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents a subset entry containing a list of items included in the base set.
/// </summary>
public class SubsetEntry
{
    /// <summary>
    /// An ID for grouping alternative parts.
    /// </summary>
    [JsonPropertyName("match_no")]
    public int MatchNo { get; set; }

    /// <summary>
    /// A list of items included in the base set.
    /// </summary>
    [JsonPropertyName("entries")]
    public List<SubsetItem> Entries { get; set; } = new();
}
