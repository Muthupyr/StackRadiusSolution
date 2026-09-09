using StackRadius.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StackRadius.Services;

/// <summary>
/// In-memory sitemap for the Master app. In production this reads from
/// sr.SiteMap + sr.SiteMapArea + sr.SiteMapGroup + sr.SiteMapSubArea rows
/// in the Postgres metadata DB. Same shape either way.
///
/// Every SubArea's Key becomes the LAST segment of the URL:
///   /master/{area}/{subarea}
/// e.g. /master/dashboard/md-home  →  Pages/Master/Dashboard/MdHome.cshtml
/// </summary>
public sealed class SitemapService
{
    public IReadOnlyList<AreaVm> MasterAppSitemap { get; } = new AreaVm[]
    {
        new(
            Key:    "dashboard",
            Label:  "Dashboard",
            IconId: "i-dashboard",
            Phase:  "live",
            Groups: new GroupVm[]
            {
                new(
                    Label: "Overview",
                    SubAreas: new SubAreaVm[]
                    {
                        new("md-home", "Master Data home", "i-dashboard", "live", "Cross-exchange reference data + counts + freshness")
                    }),
            }),
        new(
            Key:    "nse",
            Label:  "NSE",
            IconId: "i-trending-up",
            Phase:  "live",
            Groups: new GroupVm[]
            {
                new(
                    Label: "Catalogue",
                    SubAreas: new SubAreaVm[]
                    {
                        new("nse-scheme", "Scheme master", "i-book",     "live", "NSE MFSS scheme catalogue — 13,393 rows"),
                        new("nse-amc",    "AMCs",          "i-building", "stub", "Asset Management Companies"),
                    }),
                new(
                    Label: "Feeds",
                    SubAreas: new SubAreaVm[]
                    {
                        new("nse-nav",    "NAV history",   "i-trending-down", "stub", "Daily NAV time-series"),
                    }),
            }),
        new(
            Key:    "bse",
            Label:  "BSE",
            IconId: "i-bar-chart",
            Phase:  "stub",
            Groups: new GroupVm[]
            {
                new(
                    Label: "Catalogue",
                    SubAreas: new SubAreaVm[]
                    {
                        new("bse-scheme", "Scheme master", "i-book",     "stub"),
                        new("bse-amc",    "AMCs",          "i-building", "stub"),
                    }),
            }),
        new(
            Key:    "common",
            Label:  "Common",
            IconId: "i-layers",
            Phase:  "live",
            Groups: new GroupVm[]
            {
                new(
                    Label: "Taxonomy",
                    SubAreas: new SubAreaVm[]
                    {
                        new("md-category",    "Categories",    "i-layers", "live"),
                        new("md-asset-class", "Asset classes", "i-coins",  "live"),
                        new("md-sector",      "Sectors",       "i-branch", "live"),
                        new("md-benchmark",   "Benchmarks",    "i-award",  "live"),
                    }),
                new(
                    Label: "Vocabulary",
                    SubAreas: new SubAreaVm[]
                    {
                        new("vocab-browser",  "Vocabulary browser", "i-book",  "live", "Canonical srVocabulary members"),
                        new("vocab-compare",  "Rail comparison",    "i-arrows","live", "NSE ↔ BSE ↔ MFU code compare"),
                    }),
            }),
        new(
            Key:    "config",
            Label:  "Configuration",
            IconId: "i-settings",
            Phase:  "live",
            Groups: new GroupVm[]
            {
                new(
                    Label: "Runtime",
                    SubAreas: new SubAreaVm[]
                    {
                        new("cfg-settings",     "Settings",         "i-settings",     "live"),
                        new("cfg-integrations", "Integrations",     "i-layers",       "live"),
                        new("cfg-audit",        "Audit events",     "i-shield-alert", "live"),
                    }),
            }),
    };

    public SessionVm Session { get; } = new(
        SignedInName:  "Muthukumar J",
        SignedInEmail: "muthupyr@yahoo.com",
        Location:      "Chennai, IN",
        Aal:           "",
        SessionId:     "",
        Provider:      "");

    /// <summary>Find the (Area, SubArea) tuple that matches the current URL segments — used by the SideMap partial.</summary>
    public (AreaVm? Area, SubAreaVm? SubArea) FindByKeys(string areaKey, string? subAreaKey)
    {
        var area = MasterAppSitemap.FirstOrDefault(a => a.Key.Equals(areaKey, StringComparison.OrdinalIgnoreCase));
        if (area is null) return (null, null);
        if (subAreaKey is null) return (area, null);
        var sub = area.Groups
            .SelectMany(g => g.SubAreas)
            .FirstOrDefault(s => s.Key.Equals(subAreaKey, StringComparison.OrdinalIgnoreCase));
        return (area, sub);
    }
}
