// Constants.cs

namespace StackRadius.Service.NSE
{
    public class Constants
    {
        public class NSEAPIUrls
        {
            /// <summary>
            /// Master Download
            /// SCH
            /// </summary>
            public const string MasterDownload = "/nsemfdesk/api/v2/reports/MASTER_DOWNLOAD";

            /// <summary>
            /// CLIENT_DETAIL_REPORT API link
            /// </summary>
            public const string ClientDetailReport = "/nsemfdesk/api/v2/reports/CLIENT_DETAIL_REPORT";

            /// <summary>
            /// KYC Registration API link
            /// </summary>
            public const string KYCRegistration = "/nsemfdesk/api/v1/EKYC/EKYCREG";

            /// <summary>
            /// Member Fund Allocation Order wise API link
            /// </summary>
            public const string MFAOrderwise = "/nsemfdesk/api/v2/reports/MEMBER_FUND_ALLOCATION/ORDER_WISE";


            public enum AMCCodes
            {
                B,
                K,
                H,
                G,
                CR,
                O,
                UK
            }
        }
    }
}

