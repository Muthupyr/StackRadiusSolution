using System.Collections.Generic;

// Models for the sitemap tree + dashboard fixtures.
// These are plain POCOs a real .NET app would replace with EF-mapped
// entities backed by sr.* Postgres tables. Kept simple + serialisable
// so the developer sees the shape once and can extend without ceremony.

namespace StackRadius.Entity
{
    public sealed record AreaVm(
        string Key,           // 'md-home'            — URL slug (last segment)
        string Label,         // 'Master Data home'   — sidebar row text
        string IconId,        // 'i-dashboard'
        string Phase,         // 'live' | 'stub'
        IReadOnlyList<GroupVm> Groups);

    public sealed record GroupVm(
        string Label,         // 'Overview'           — small caps label above SubArea list
        IReadOnlyList<SubAreaVm> SubAreas);

    public sealed record SubAreaVm(
        string Key,           // 'md-home'            — URL slug (last segment)
        string Label,         // 'Master Data home'   — sidebar row text
        string IconId,        // 'i-dashboard'
        string Phase,         // 'live' | 'stub'
        string? Hint = null); // Tooltip, optional

    public sealed record BreadcrumbItem(string Label, string? Href = null);

    public sealed record StatTileVm(
        string Label,         // 'Schemes'            — small-caps top-left
        string Value,         // '13,393'             — big number
        string Sub,           // 'across 66 AMCs'     — thin caption
        string IconId,        // 'i-database'
        string Tone);         // 'indigo' | 'emerald' | 'amber' | 'purple' | 'blue' | 'red' | 'sky'

    public sealed record SessionVm(
        string SignedInName,
        string SignedInEmail,
        string Location,
        string Aal,
        string SessionId,
        string Provider);
}