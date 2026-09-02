using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using static StackRadius.Service.NSE.Constants.NSEAPIUrls;
using StackRadius.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StackRadius.Service.NSE
{
    public class Common
    {
        public static RequestParameters GetRequestParameters()
        {
            RequestParameters parameters = new RequestParameters();

            parameters.env = ConfigurationManager.AppSettings["env"].ToString();

            if (parameters.env.ToUpper() == "PRD")
            {
                Logger.LogDebug("Environment connected : **** PRD *****");

                parameters.baseUrl = ConfigurationManager.AppSettings["nseinvestPrd"].ToString();
                parameters.memberId = ConfigurationManager.AppSettings["NSEMemberIdPrd"].ToString();
                parameters.userId = ConfigurationManager.AppSettings["NSEUserIdPrd"].ToString();
                parameters.encPassword = ConfigurationManager.AppSettings["valuePrd"].ToString();
            }
            else
            {
                Logger.LogDebug("Environment connected : **** UAT *****");

                parameters.baseUrl = ConfigurationManager.AppSettings["nseinvestUat"].ToString();
                parameters.memberId = ConfigurationManager.AppSettings["NSEMemberIdUat"].ToString();
                parameters.userId = ConfigurationManager.AppSettings["NSEUserIdUat"].ToString();
                parameters.encPassword = ConfigurationManager.AppSettings["valueUat"].ToString();
            }
            return parameters;
        }

        public static Dictionary<string, string> GetBasicAuthorizationHeader(string userId, string encPassword)
        {
            // Authorization  BASIC  base64(Login User ID: Encrypted Password)
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string authData = string.Format("{0}:{1}", userId, encPassword.TrimEnd('='));
            string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            headers.Add("Authorization", string.Concat("Basic ", authHeaderValue));
            return headers;
        }

        public static Dictionary<string, string> AddAdditionalHeaders(Dictionary<string, string> headers)
        {
            headers.Add("Accept", "*/*");
            headers.Add("User-Agent", "PostmanRuntime/7.34.0");   // IMPORTANT. Else forbidden 403 message will be thrown
            headers.Add("Accept-Language", "en-US");
            headers.Add("Accept-Encoding", "gzip, deflate, br");
            headers.Add("Connection", "keep-alive");
            return headers;
        }

        public static List<SelectListItem> GetAMCCodeList()
        {
            List<SelectListItem> AMCCodeList = new List<SelectListItem>();

            // Loops through the actual enum constants
            foreach (AMCCodes amcCode in Enum.GetValues<AMCCodes>())
            {
                SelectListItem item = new SelectListItem();
                item.Text = amcCode.ToString();
                item.Value = amcCode.ToString();
                AMCCodeList.Add(item);
            }
            return AMCCodeList;
        }
    }
}

