
using Newtonsoft.Json;
using StackRadius.Service;
using System;
using System.Collections.Generic;

namespace StackRadius.Service.NSE
{
    public class MasterDownload
    {
        public static ReturnMessageWrapper<MasterDownloadResponse> MasterDownloadReport(MasterDownloadRequest request)
        {
            ReturnMessage<string> returnMessage = new ReturnMessage<string>();
            ReturnMessageWrapper<MasterDownloadResponse> response = new ReturnMessageWrapper<MasterDownloadResponse>();

            RequestParameters para = Common.GetRequestParameters();
            string endPoint = string.Concat(para.baseUrl, Constants.NSEAPIUrls.MasterDownload);
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

            response.Result = new MasterDownloadResponse();
            response.Error = new ErrorResponse();
            response.HasError = returnMessage.HasError;
            response.StatusCode = returnMessage.StatusCode;

            if (!returnMessage.HasError && returnMessage.StatusCode == 200) //OK
            {
                response.Result.response_status = "S";
                response.Result.filename = returnMessage.FileName;
                response.Result.filecontent = returnMessage.Result; // String data with | delemeted.
                response.Error = null;
            }
            else  // Handle Other status codes like 404
            {
                ErrorResponseDownload downloadErrorObj = JsonConvert.DeserializeObject<ErrorResponseDownload>(returnMessage.Message);
                response.Error = new ErrorResponse();

                response.HasError = true;
                response.Result.response_status = "F";
                response.Result.response_remark = downloadErrorObj.response_remark;

                response.Error.status = response.StatusCode.ToString();
                response.Error.error_type = "";
                response.Error.message = downloadErrorObj.response_remark;
            }
            return response;         
        }
    }
}

