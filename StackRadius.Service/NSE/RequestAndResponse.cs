using StackRadius.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace StackRadius.Service.NSE
{
    #region Master Download
    //Sample Request Json:
    //{
    //"file_type": "SCH",
    //}
    //Possible Values:
    //SCH = Consolidated Scheme master
    //SIP = SIP Scheme Master,
    //STP = STP Scheme Master,
    //SWP = SWP Scheme Master,
    //NAV = NAV Download
    //SET = Settlement Calendar Download

    public class MasterDownloadRequest
    {
        public string file_type { get; set; }         // "SCH",SIP,STP,SWP,NAV,SET

        public MasterDownloadRequest()
        { }

        public MasterDownloadRequest(string file_type)
        {
            this.file_type = file_type;
        }
    }

    //Sample Response:
    //If, Success then
    //.txt file with pipe (|) seperated will be downloaded with data.File name wil l be as below
    //NSE_NSEINVEST_ALL_<SYSDATE(DDMMYYYY)>.txt (If, File Type = ‘SCH’)
    //NSE_NSEINVEST_SIP_<SYSDATE(DDMMYYYY)>.txt(If, File Type = ‘SIP’)
    //NSE_NSEINVEST_STP_<SYSDATE(DDMMYYYY)>.txt(If, File Type = ‘STP’)
    //NSE_NSEINVEST_SWP_<SYSDATE(DDMMYYYY)>.txt(If, File Type = ‘SWP’)
    //NSE_NSEINVEST_NAV_<SYSDATE(DDMMYYYY)>.txt(If, File Type = ‘NAV’)
    //NSE_NSEINVEST_SET_<SYSDATE(DDMMYYYY)>.txt(If, File Type = ‘SET’) //// NAV File will not have header

    //If, Failed then
    //{
    //"response_status": "Fail",
    //"response_remark": "Invalid File Type"
    //}

    public class MasterDownloadResponse
    {
        public string response_status { get; set; }  // "S" or "F"
        public string report_data_total { get; set; }
        public string filename { get; set; }
        public string filecontent { get; set; } // Text file content
        public string response_remark { get; set; }
    }

    #endregion Master Download 


    #region Client Details 

    public class ClientDetailRequest
    {
        // IMPORTANT: Date range should not exceed 7 days
        public string from_date { get; set; }         // "01-03-2026"  // dd-MM-yyyy
        public string to_date { get; set; }           // "07-03-2026"  // dd-MM-yyyy
        public string client_code { get; set; }       // ""
        public string auth_status { get; set; }       // ""
        public string date_type { get; set; }         // ""

        public ClientDetailRequest()
        { }

        public ClientDetailRequest
            (string from_date, string to_date,
             string client_code, string auth_status, string date_type)
        {
            this.from_date = from_date;
            this.to_date = to_date;
            this.client_code = client_code;
            this.auth_status = auth_status;
            this.date_type = date_type;
        }
    }

    public class ClientDetailResponse
    {
        public string response_status { get; set; }  // "S"
        public string report_data_total { get; set; }
        public List<ClientProfileModel> report_data { get; set; }
        public string error_remark { get; set; }
    }

    #endregion Client Details 

    #region MFA 
    public class MFARequest
    {
        public string from_date { get; set; }   // dd-MM-yyyy
        public string to_date { get; set; }     // dd-MM-yyyy
        public string client_code { get; set; }
        public string pg_bank_refno { get; set; }
    }

    public class MFAResponse
    {
        public string response_status { get; set; }
        public string report_data_total { get; set; }
        public List<FundAllocationModel> report_data { get; set; }
        public string error_remark { get; set; }
        public string cfppgbankrefno { get; set; }  //080420241306423594511

    }
    #endregion MFA 

    #region KYC

    public class KYCRequest
    {
        public string amcCode { get; set; }         //  "B",
        public string panNo { get; set; }           // "BVYPD3825K",
        public string mobileNo { get; set; }       // """9748975222",
        public string invEmail { get; set; }       //  "abcdef@gmail.com"

        public KYCRequest()
        { }

        public KYCRequest(string amcCode, string panNo, string mobileNo, string invEmail)
        {
            this.amcCode = amcCode;
            this.panNo = panNo;
            this.mobileNo = mobileNo;
            this.invEmail = invEmail;
        }
    }
    public class KYCResponse
    {
        public string response_status { get; set; }  // "S"
        public string message { get; set; } //"EKYC FRESH REGISTRATION REQUEST RECEVIED",
        public string link { get; set; }//"https://www.nseinvest.com/nsemfdesk/ekycVerifyByUser/59C5F260B58F4C6CE0635D28A8C07A75"
        public string error_remark { get; set; }
    }

    #endregion KYC
    public class RequestParameters
    {
        public string env { get; set; }
        public string baseUrl { get; set; }
        public string userId { get; set; }
        public string encPassword { get; set; }
        public string memberId { get; set; }
    }

    public class ReturnMessageWrapper<T>
    {
        public bool HasError { get; set; }
        public int StatusCode { get; set; } // 200/400/403
        public T Result { get; set; }
        public ErrorResponse Error { get; set; }
    }

    /// <summary>
    /// THe ReturnMessage result class definition
    /// </summary>
    /// <typeparam name="T">The generic parameter T</typeparam>
    public class ReturnMessage<T>
    {
        public bool HasError { get; set; } // True/False
        public int StatusCode { get; set; } // 200/400/403
        public T Result { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
    }

    /// <summary>
    /// THe ErrorResponse class definition
    /// This will be return when 403 Forbidden occurs
    /// </summary>
    public class ErrorResponse
    {
        public string status { get; set; } // 403 Forbidden
        public string error_type { get; set; }  //"unauthorized",
        public string message { get; set; } // "Invalid authorization header or IP Address not mapped with user."
    }

    public class ErrorResponseDownload
    {
        public string response_status { get; set; } // "response_status": "Fail",
        public string response_remark { get; set; } // "Invalid File Type"
    }
}