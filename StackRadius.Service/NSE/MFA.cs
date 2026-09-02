using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using StackRadius.Service;

namespace StackRadius.Service.NSE
{ 
    public class MFA
    {
        public static ReturnMessageWrapper<MFAResponse> OrderWiseReport(MFARequest request)
        {          
            ReturnMessage<string> returnMessage = new ReturnMessage<string>();
            ReturnMessageWrapper<MFAResponse> response = new ReturnMessageWrapper<MFAResponse>();

            RequestParameters para = Common.GetRequestParameters();
            string endPoint = string.Concat(para.baseUrl, Constants.NSEAPIUrls.MFAOrderwise);
            string jsonString = JsonConvert.SerializeObject(request);
            string postData = jsonString;
            var client = new RestClient(endPoint, HttpVerb.POST, "application/json", postData);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers = Common.GetBasicAuthorizationHeader(para.userId, para.encPassword);
            headers.Add("memberId", para.memberId);
            headers = Common.AddAdditionalHeaders(headers);
            client.AddHeaders(headers);

            // Make the request
            returnMessage = client.MakeRequest();

            response.HasError = returnMessage.HasError;
            response.StatusCode = returnMessage.StatusCode;

            if (!returnMessage.HasError && returnMessage.StatusCode == 200) //OK
            {
                MFAResponse responseObj = JsonConvert.DeserializeObject<MFAResponse>(returnMessage.Result);
                response.Result = responseObj;
                response.Error = null;
            }
            else if (returnMessage.StatusCode == 403)   // 403 Forbidden
            {
                ErrorResponse errorObj = JsonConvert.DeserializeObject<ErrorResponse>(returnMessage.Message);
                response.HasError = true;
                response.Result = null;
                response.Error = errorObj;
            }
            else  // Handle Other status codes 404
            {
                MFAResponse responseObj = JsonConvert.DeserializeObject<MFAResponse>(returnMessage.Message);
                response.HasError = true;
                response.Result = responseObj;
                response.Error = null;
            }
            return response;
        }      
    
    }
}