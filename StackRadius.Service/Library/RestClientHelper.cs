// -----------------------------------------------------------------------
// <copyright file="RestClient.cs" company="StackRadius">
//  Copyright StackRadius 2026.
// </copyright>
// -----------------------------------------------------------------------
using StackRadius.Common;
using StackRadius.Service.NSE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace StackRadius.Service
{
    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpMethod Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        //public const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
        //public const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;

        // Rule: Reuse your HttpClient instance across the application to prevent socket exhaustion
        //ServicePointManager.DefaultConnectionLimit = 1;
        private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            MaxConnectionsPerServer = 1,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        public RestClient()
        {
            EndPoint = "";
            Method = HttpMethod.Get;
            ContentType = "text/xml";
            PostData = "";
            Headers = new Dictionary<string, string>();
        }

        public RestClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpMethod.Get;
            ContentType = "text/xml";
            PostData = "";
            Headers = new Dictionary<string, string>();
        }
        public RestClient(string endpoint, HttpMethod method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = "";
            Headers = new Dictionary<string, string>();
        }

        public RestClient(string endpoint, HttpMethod method, string contentType)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = contentType;
            PostData = "";
            Headers = new Dictionary<string, string>();
        }

        public RestClient(string endpoint, HttpMethod method, string contentType, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = contentType;
            PostData = postData;
            Headers = new Dictionary<string, string>();
        }

        public ReturnMessage<string> MakeRequest()
        {
            return MakeRequest(string.Empty, null);
        }

        public ReturnMessage<string> MakeRequest(string parameters, Dictionary<string, string> headers)
        {
            ReturnMessage<string> retval = new ReturnMessage<string>();
            retval.Result = string.Empty;
            retval.HasError = true;
            retval.Message = string.Empty;
            retval.StatusCode = 0;
            retval.FileName = "";

            int statusCode = 0;
            var responseValue = string.Empty;

            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

                // 1. Create the request message
                var request = new HttpRequestMessage(Method, EndPoint + parameters);

                request.Headers.ConnectionClose = false;
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                request.Version = new Version(1, 0);        // Set the HTTP version to 1.0
                request.Headers.Add("ContentLength", "0");

                // Add header that is sent with constructor to the request...
                if (Headers != null && Headers.Count > 0)
                {
                    foreach (KeyValuePair<string, string> headerItem in Headers)
                    {
                        request.Headers.Add(headerItem.Key, headerItem.Value);
                    }
                }

                // Add header sent as MakeRequest() parameter to the request...
                if (headers != null && headers.Count > 0)
                {
                    foreach (KeyValuePair<string, string> headerItem in headers)
                    {
                        request.Headers.Add(headerItem.Key, headerItem.Value);
                    }
                }

                if (!string.IsNullOrEmpty(PostData) && Method == HttpMethod.Post)
                {
                    var content = new StringContent(PostData);
                    request.Content = content;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
                }

                try
                {
                    Logger.LogDebug("Make Request: " + request.Method + " " + EndPoint + parameters);

                    var response = httpClient.Send(request);

                    statusCode = (response?.StatusCode == null) ? 0 : (int)response?.StatusCode;
                    using (var responseStream = response.Content.ReadAsStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                        }
                    }

                    var message = "";
                    if (statusCode == 0) // null response or no status code, likely a network error
                    {
                        // This happens if the request never reached the server (e.g., DNS error)
                        message = String.Format("Request failed. Network level error.");
                        Logger.LogDebug(string.Concat("Error in MakeRequest(). Error: ", message));
                    }
                    else if (statusCode == (int)System.Net.HttpStatusCode.NotFound)
                    {
                        message = String.Format("Request failed. Received HTTP {0}.", statusCode);
                        Logger.LogDebug(string.Concat("Error in MakeRequest(). Error: ", message));
                        retval.Result = string.Empty;
                        retval.HasError = true;
                        retval.Message = responseValue;
                        retval.StatusCode = statusCode;
                    }
                    else if (statusCode == (int)System.Net.HttpStatusCode.InternalServerError)
                    {
                        message = String.Format("Request failed. Received HTTP {0}.", statusCode);
                        Logger.LogDebug(string.Concat("Error in MakeRequest(). Error: ", message));
                        retval.Result = string.Empty;
                        retval.HasError = true;
                        retval.Message = responseValue;
                        retval.StatusCode = statusCode;
                    }
                    else  // response is OK (200) or other success code
                    {
                        string fileName = "";
                        string contentType = response.Content.Headers.ContentType?.ToString();
                        if (contentType != null)
                        {
                            Logger.LogDebug(String.Format("MakeRequest(). Content Type: {0}", contentType));
                            // Check if the type points to something other than HTML/Text
                            if (!contentType.StartsWith("text/html") && !contentType.StartsWith("application/json"))
                            {
                                // Try extracting from Content-Disposition
                                string dispositionHeader = response.Content.Headers.GetValues("Content-Disposition").FirstOrDefault();
                                Logger.LogDebug(String.Format("MakeRequest(). Content Disposition: {0}", dispositionHeader));
                                if (!string.IsNullOrEmpty(dispositionHeader))
                                {
                                    try
                                    {
                                        ContentDisposition cd = new ContentDisposition(dispositionHeader);
                                        fileName = cd.FileName;
                                    }
                                    catch
                                    {
                                        // Fallback manual parse if ContentDisposition parsing fails
                                        int index = dispositionHeader.IndexOf("filename=");
                                        if (index >= 0)
                                        {
                                            fileName = dispositionHeader.Substring(index + 9).Replace("\"", "").Trim();
                                        }
                                    }
                                }
                            }
                        }
                        retval.Result = responseValue;
                        retval.HasError = false;
                        retval.Message = string.Empty;
                        retval.StatusCode = statusCode;
                        retval.FileName = fileName;
                    }
                }
                catch (HttpRequestException ex)
                {
                    statusCode = (int)ex.StatusCode;
                    var message = String.Format("Request failed. Received HTTP {0}.", statusCode);
                    Logger.LogDebug(string.Concat("Error in MakeRequest(). Error: ", message));
                    responseValue = ex.InnerException?.Message ?? string.Empty;
                    retval.Result = string.Empty;
                    retval.HasError = true;
                    retval.Message = responseValue;
                    retval.StatusCode = statusCode;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug(string.Concat("RestClient.MakeRequest(). Exception: ", ex.Message));
                if (ex.InnerException != null)
                    Logger.LogDebug(string.Concat("Inner Exception: ", ex.InnerException.Message));
                responseValue = ex.Message;

                retval.Result = string.Empty;
                retval.HasError = true;
                retval.Message = responseValue;
                retval.StatusCode = 0;
            }

            Logger.LogDebug("Return Status Code    : " + (int)retval.StatusCode);
            Logger.LogDebug("Return Result Length  : " + retval.Result.Length);
            if (retval.Result.Length <= 500)
                Logger.LogDebug("Return Result     : " + retval.Result);
            Logger.LogDebug("Return Message        : " + retval.Message);
            Logger.LogDebug("--------------------------------------------------------------");

            return retval;
        }

        public void AddHeaders(Dictionary<string, string> headers)
        {
            try
            {
                if (headers != null && headers.Count > 0)
                {
                    foreach (KeyValuePair<string, string> headerItem in headers)
                    {
                        Headers.Add(headerItem.Key, headerItem.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug(string.Concat("AddHeaders(): Exception: ", ex.Message));
            }
            return;
        }
    }
}
