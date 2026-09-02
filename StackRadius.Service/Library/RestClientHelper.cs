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
using System.Net;
using System.Net.Mime;
using System.Security.Authentication;
using System.Text;
public enum HttpVerb
{
    GET,
    POST,
    PUT,
    DELETE
}

namespace StackRadius.Service
{
    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        //public const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
        //public const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;

        public RestClient()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
            Headers = new Dictionary<string, string>();
        }

        public RestClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
            Headers = new Dictionary<string, string>();
        }

        public RestClient(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = "";
            Headers = new Dictionary<string, string>();
        }

        public RestClient(string endpoint, HttpVerb method, string contentType)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = contentType;
            PostData = "";
            Headers = new Dictionary<string, string>();
        }

        public RestClient(string endpoint, HttpVerb method, string contentType, string postData)
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
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | Tls12 ;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

                request.KeepAlive = true;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Accept = "*/*";
                request.ProtocolVersion = HttpVersion.Version10;
                request.ServicePoint.ConnectionLimit = 1;
                request.Method = Method.ToString();
                request.ContentLength = 0;
                request.ContentType = ContentType;

                // Add header that is sent with constructor to the web request...
                if (Headers != null && Headers.Count > 0)
                {
                    foreach (KeyValuePair<string, string> headerItem in Headers)
                    {
                        request.Headers.Add(headerItem.Key, headerItem.Value);
                    }
                }

                // Add header sent as MakeRequest() parameter to the Web request...
                if (headers != null && headers.Count > 0)
                {
                    foreach (KeyValuePair<string, string> headerItem in headers)
                    {
                        request.Headers.Add(headerItem.Key, headerItem.Value);
                    }
                }

                if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
                {
                    Logger.LogDebug("PostData: " + PostData);
                    var encoding = new UTF8Encoding();
                    var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                    request.ContentLength = bytes.Length;
                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                try
                {
                    Logger.LogDebug("Make Request: " + request.Method + " " + EndPoint + parameters);

                    var response = (HttpWebResponse)request.GetResponse();
                    statusCode = (int)response.StatusCode;
                    Logger.LogDebug(String.Format("MakeRequest(). Received HTTP {0}", (int)response.StatusCode));

                    //// Normal handling for 200 OK responses
                    //if (response.StatusCode != HttpStatusCode.OK)
                    //{
                    //    statusCode = (int)response.StatusCode;
                    //    var message = String.Format("Request failed. Received HTTP {0}", (int)response.StatusCode);
                    //    Logger.LogDebug(string.Concat("Error in MakeRequest(). Error: ", message));
                    //    throw new ApplicationException(message);
                    //}
                    //else
                    //{
                    //    statusCode = (int)response.StatusCode;
                    //    Logger.LogDebug(String.Format("MakeRequest(). Received HTTP {0}", (int)response.StatusCode));
                    //}

                    // grab the response
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                        }
                    }

                    string contentType = response.ContentType;
                    string fileName = "";

                    Logger.LogDebug(String.Format("MakeRequest(). Content Type: {0}", contentType));

                    // Check if the type points to something other than HTML/Text
                    if (!contentType.StartsWith("text/html") && 
                        !contentType.StartsWith("application/json"))
                    {
                        // Try extracting from Content-Disposition
                        string dispositionHeader = response.Headers["Content-Disposition"];
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

                        // 3. Fallback to the final URL path if header is empty
                        if (string.IsNullOrEmpty(fileName))
                        {
                            fileName = Path.GetFileName(response.ResponseUri.LocalPath);
                        }
                    }

                    retval.Result = responseValue;
                    retval.HasError = false;
                    retval.Message = string.Empty;
                    retval.StatusCode = statusCode;
                    retval.FileName = fileName;
                }
                catch (WebException ex)
                {
                    // Verify that a response object actually exists
                    if (ex.Response is HttpWebResponse errorResponse)
                    {
                        statusCode = (int)errorResponse.StatusCode;
                        var message = String.Format("Request failed. Received HTTP {0}", (int)errorResponse.StatusCode);// 400
                        Logger.LogDebug(string.Concat("Error in MakeRequest(). Error: ", message));

                        // Extract the error stream from the exception response object
                        var stream = errorResponse.GetResponseStream();
                        if (stream != null)
                        {
                            var reader = new StreamReader(stream);
                            string errorResponseBody = reader.ReadToEnd();
                            Logger.LogDebug($"Error Details: {errorResponseBody}");
                            responseValue = errorResponseBody;
                        }
                    }
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

            Logger.LogDebug("Return StatusCode : " + (int) retval.StatusCode);
            Logger.LogDebug("Return Result     : " + retval.Result);
            Logger.LogDebug("Return Message    : " + retval.Message);
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
