using System.Diagnostics;

namespace SK.GraphRag.Application.Settings;

[DebuggerDisplay($"DocumentFileName = {{{nameof(DocumentFileName)}}}, DocumentUri = {{{nameof(DocumentUri)}}}")]
public record EinsteinQuerySettings()
{
    public const string SectionName = "EinsteinQuery";

    public required Uri DocumentUri { get; init; }

    public required string DocumentFileName { get; init; }
}