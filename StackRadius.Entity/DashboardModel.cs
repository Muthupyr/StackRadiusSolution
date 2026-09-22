using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StackRadius.Entity
{
    public class DashboardModel : JSONBaseClass
    {
        // Same shape a StackRadius runtime would return. Numbers here are fixture-realistic
        // per solutions/finserve/mint/apps/master's real NSE scheme master fixture.    




        public IReadOnlyList<StatTileVm> Tiles { get; private set; }

        public DashboardModel()
        {
            //Tiles = System.Array.Empty<StatTileVm>();

            Tiles = new StatTileVm[]
                {
               // new (Label: "Schemes",          Value: "13,393", Sub: "across 66 SEBI-registered AMCs", IconId: "i-book",           Tone: "indigo"),
                new (Label: "SIP-eligible",     Value: "12,182", Sub: "91% of the universe",            IconId: "i-refresh",        Tone: "emerald"),
                new (Label: "Purchase-active",  Value: "11,116", Sub: "83% currently accepting orders", IconId: "i-check-circle",   Tone: "blue"),
                new (Label: "AMCs",             Value: "66",     Sub: "SEBI-registered mutual funds",   IconId: "i-building",       Tone: "purple"),
                new (Label: "Scheme types",     Value: "5",      Sub: "Debt · Equity · ETF · Liquid · Overnight", IconId: "i-layers", Tone: "amber"),
                new (Label: "Settlement cycles", Value: "10",    Sub: "L0 · L1 · MF · T1..T7",          IconId: "i-file-chart",     Tone: "sky"),
                };
        }


        // Sectio 1 Cross-exchange reference data.
        // NSE + BSE + MFU scheme masters, ISIN registry, AMC catalogue, categories,
        // benchmarks, calendars. Read-mostly; refreshed daily from source files.

        public string SchemesCount { get; set; }                   // Across 66 SEBI-registered AMCs - 13,393
        public string SIPEligibleCount { get; set; }               // 91% of the universe - 12,182
        public string PurchaseActiveCount { get; set; }            // 83% currently accepting orders - 11,116
        public string AMCCount { get; set; }                       // SEBI-registered mutual funds - 66
        public string SchemeTypesCount { get; set; }            // Debt,Equity,ETF,Liquid,Overnight - 6
        public string SettlementCyclesCount { get; set; }       // L0, L1, MF, T1, T7 - 5


        // Section 2  - How many schemes each rail sees in its master file

        public string NSEMFSSCount { get; set; }                // 13,393
        public string BSEStARMFCount { get; set; }              // nnnn - phase 2 — pending
        public string MFUIndiaCount { get; set; }               // nnnn - phase 3 — pending
        public string CAMSRTACount { get; set; }                // reconciliation only
        public string KFinRTACount { get; set; }                // reconciliation only

    }
}