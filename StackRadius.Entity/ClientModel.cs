using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace StackRadius.Entity
{
    public class ClientModel : JSONBaseClass
    {
        public string Mode { get; set; }
        public int? ClientId { get; set; }
        public string amcCode { get; set; }         // B, K
        public string panNo { get; set; }           // "BVYPD3825K"
        public string mobileNo { get; set; }        // "9748975222"
        public string invEmail { get; set; }        // "abcdef@gmail.com"
        public string link { get; set; }            // "https://www.nseinvest.com/nsemfdesk/ekycVerifyByUser/5962AFCE5AFA82BCE0635E28A8C0DC95"
        public string message { get; set; }         // "EKYC FRESH REGISTRATION REQUEST RECEVIED"
    
        public List<SelectListItem> AMCCodeList { get; set; }
        public List<SelectListItem> ClientList { get; set; }
        
    }
}