
using System.Collections.Generic;
using Newtonsoft.Json;
using StackRadius.Service;
using System.Linq;

namespace StackRadius.Service.NSE
{
    public class KYC
    {
        public static ReturnMessageWrapper<KYCResponse> FreshRegister(KYCRequest request)
        {
            ReturnMessage<string> returnMessage = new ReturnMessage<string>();
            ReturnMessageWrapper<KYCResponse> response = new ReturnMessageWrapper<KYCResponse>();

            RequestParameters para = Common.GetRequestParameters();
            string endPoint = string.Concat(para.baseUrl, Constants.NSEAPIUrls.KYCRegistration);
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
                KYCResponse responseObj = JsonConvert.DeserializeObject<KYCResponse>(returnMessage.Result);
                response.Result = responseObj;
                response.Error = null;
            }
            else if (returnMessage.StatusCode == 403)   // 403 Forbidden
            {
                ErrorResponse errorObj = JsonConvert.DeserializeObject<ErrorResponse>(returnMessage.Message);
                response.Result = null;
                response.Error = errorObj;
            }
            else  // Handle Other status codes
            {
                KYCResponse responseObj = JsonConvert.DeserializeObject<KYCResponse>(returnMessage.Message);
                response.Result = responseObj;
                response.Error = null;
                response.HasError = true;
            }
            return response;
        }
    }
}

