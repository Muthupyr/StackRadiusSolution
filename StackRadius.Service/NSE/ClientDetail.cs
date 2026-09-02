
using Newtonsoft.Json;
using StackRadius.Service;
using System;
using System.Collections.Generic;

namespace StackRadius.Service.NSE
{
    public class ClientDetail
    {
        public static ReturnMessageWrapper<ClientDetailResponse> ClientDetailReport(ClientDetailRequest request)
        {
            ReturnMessage<string> returnMessage = new ReturnMessage<string>();
            ReturnMessageWrapper<ClientDetailResponse> response = new ReturnMessageWrapper<ClientDetailResponse>();

            RequestParameters para = Common.GetRequestParameters();
            string endPoint = string.Concat(para.baseUrl, Constants.NSEAPIUrls.ClientDetailReport);
            string jsonString = JsonConvert.SerializeObject(request);
            string postData = jsonString;
            var client = new RestClient(endPoint, HttpVerb.POST, "application/json", postData);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers = Common.GetBasicAuthorizationHeader(para.userId, para.encPassword);
            headers.Add("memberId", para.memberId);
            headers = Common.AddAdditionalHeaders(headers);
            client.AddHeaders(headers);

            // Make the request and return the result
            returnMessage = client.MakeRequest();

            response.HasError = returnMessage.HasError;
            response.StatusCode = returnMessage.StatusCode;

            if (!returnMessage.HasError && returnMessage.StatusCode == 200) //OK
            {
                ClientDetailResponse responseObj = JsonConvert.DeserializeObject<ClientDetailResponse>(returnMessage.Result);
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
            else  // Handle Other status codes
            {
                ClientDetailResponse responseObj = JsonConvert.DeserializeObject<ClientDetailResponse>(returnMessage.Message);
                response.Result = responseObj;
                response.Error = null;
                response.HasError = true;
            }
            return response;
        }
    }
}

