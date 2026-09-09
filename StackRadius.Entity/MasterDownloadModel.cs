using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace StackRadius.Entity
{
    public class MasterDownloadModel : JSONBaseClass
    {
        public string FileType { get; set; }         // SCH, SIP, STP, SWP, NAV, SET

        public List<SelectListItem> MasterFileTypeList { get; set; }
        
    }
}