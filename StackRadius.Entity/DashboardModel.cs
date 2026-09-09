using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                new (Label: "Schemes",          Value: "13,393", Sub: "across 66 SEBI-registered AMCs", IconId: "i-book",           Tone: "indigo"),
                new (Label: "SIP-eligible",     Value: "12,182", Sub: "91% of the universe",            IconId: "i-refresh",        Tone: "emerald"),
                new (Label: "Purchase-active",  Value: "11,116", Sub: "83% currently accepting orders", IconId: "i-check-circle",   Tone: "blue"),
                new (Label: "AMCs",             Value: "66",     Sub: "SEBI-registered mutual funds",   IconId: "i-building",       Tone: "purple"),
                new (Label: "Scheme types",     Value: "5",      Sub: "Debt · Equity · ETF · Liquid · Overnight", IconId: "i-layers", Tone: "amber"),
                new (Label: "Settlement cycles", Value: "10",    Sub: "L0 · L1 · MF · T1..T7",          IconId: "i-file-chart",     Tone: "sky"),
                };
        }
    }
}