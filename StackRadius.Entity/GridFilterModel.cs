using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace StackRadius.Entity
{
    public class GridSingleFilterModel
    {
        // { "amcCode":{ "filterType":"text","type":"contains","filter":"B"} }
        public string ColumnName { get; set; } // "amcCode"
        public FilterCondition filterCondition { get; set; }

        GridSingleFilterModel()
        {
            ColumnName = "";
            filterCondition = new FilterCondition();
        }

        // Constructor to initialize the GridFilterModel from a JSON string
        public GridSingleFilterModel(string jsonData)
        {
            jsonData = jsonData.Trim('{');
            int index = jsonData.LastIndexOf('}');  // Find the index of the last occurrence
            if (index != -1)
            {
                jsonData = jsonData.Remove(index, 1);
            }

            if (String.IsNullOrEmpty(jsonData))
            {
                ColumnName = "";
                filterCondition = new FilterCondition();
                return;
            }

            int pos1 = jsonData.IndexOf(':');
            int pos2 = jsonData.IndexOf('}', pos1);

            string part1 = jsonData.Substring(0, pos1);
            string part2 = jsonData.Substring(pos1 + 1, pos2 - pos1).Trim(); // dont remove "}"
            ColumnName = part1;
            filterCondition = new FilterCondition();

            if (!String.IsNullOrEmpty(part2))
            {
                //{ "filterType":"text","type":"contains","filter":"B"}
                if (part2.Contains("\"type\":\"contains\""))
                {
                    var model = System.Text.Json.JsonSerializer.Deserialize<FilterCondition>(part2);
                    this.filterCondition = model;
                }
                // { "filterType":"number","type":"equals","filter":5}
                else if (part2.Contains("\"type\":\"equals\""))
                {
                    string[] parts = part2.Split(",");  
                    this.filterCondition.FilterType = "number";
                    this.filterCondition.Type = "equals";
                    //"filter":5
                    this.filterCondition.FilterValue = parts[2].Split(':')[1].Trim().Trim('}');

                }

            }
        }
    }

    //=========================================================================================
    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public class GridMultipleFilterModel
    {
        public string ColumnName { get; set; }
        public ColumnFilter ColumnFilter { get; set; }

        GridMultipleFilterModel()
        {
            ColumnName = "";
            ColumnFilter = new ColumnFilter();
        }

        // Constructor to initialize the GridFilterModel from a JSON string
        public GridMultipleFilterModel(string jsonData)
        {
            jsonData = jsonData.Trim('{');
            int index = jsonData.LastIndexOf('}');  // Find the index of the last occurrence
            if (index != -1)
            {
                jsonData = jsonData.Remove(index, 1);
            }

            if (String.IsNullOrEmpty(jsonData))
            {
                ColumnName = "";
                ColumnFilter = new ColumnFilter();
                return;
            }

            int pos = jsonData.IndexOf(':');
            string part1 = jsonData.Substring(0, pos);
            string part2 = jsonData.Substring(pos + 1).Trim();
            ColumnName = part1;
            ColumnFilter = new ColumnFilter();
            if (!String.IsNullOrEmpty(part2))
            {
                //{ "filterType":"text","type":"contains","filter":"B"}
                if (part2.Contains("\"type\":\"contains\""))
                {
                    var model = System.Text.Json.JsonSerializer.Deserialize<ColumnFilter>(part2);
                    this.ColumnFilter = model;
                }
                else // { "filterType":"number","type":"equals","filter":5}
                {
                    //var model = System.Text.Json.JsonSerializer.Deserialize<ColumnFilter>(part2);
                    //this.ColumnFilter = model;
                }
            }
        }
    }

    public class ColumnFilter
    {
        [JsonPropertyName("filterType")]
        public string FilterType { get; set; }

        [JsonPropertyName("operator")]
        public string Operator { get; set; } // Can be "OR" or "AND"

        [JsonPropertyName("conditions")]
        public List<FilterCondition> Conditions { get; set; }
    }

    public class FilterCondition
    {
        [JsonPropertyName("filterType")]
        public string FilterType { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("filter")]
        public string FilterValue { get; set; }
    }
}



// { "amcCode":{ "filterType":"text",  "type":"contains","filter":"B"} }
//             { "filterType":"number","type":"equals",  "filter":5}"
// { "panNo"  :{ "filterType":"text","operator":"OR","conditions":[{"filterType":"text","type":"contains","filter":"b"},{"filterType":"text","type":"contains","filter":"C"}]}}

// {
//      "panNo":
//      {
//          "filterType":"text",
//          "operator":"OR",
//          "conditions":[
//              {"filterType":"text","type":"contains","filter":"b"},
//              {"filterType":"text","type":"contains","filter":"C"}
//              ]
//       }
// }
