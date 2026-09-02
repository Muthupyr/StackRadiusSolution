using System;
using StackRadius.Service.NSE;
using StackRadius.Common;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            

            Console.WriteLine("-------------------- MasterDownloadReport --------------------");
            MasterDownloadReport();
            Console.WriteLine("-------------------------------------------------------------");


            //Console.WriteLine("-------------------- ClientDetailReport --------------------");
            //ClientDetailReport();
            //Console.WriteLine("-------------------------------------------------------------");

            //Console.WriteLine("---------------- KYC Fresh Register -----------------------");
            //KYCFreshRegister();
            //Console.WriteLine("-----------------------------------------------------------");

            //Console.WriteLine("--------------Member Fund Allocation ----------------------");
            //MFAOrderwiseReport();
            //Console.WriteLine("-----------------------------------------------------------");

            //Console.WriteLine(string.Concat("retValue: " + retValue));

            Console.WriteLine("Press a key to continue...");
            Console.ReadLine();
        }
        static void MasterDownloadReport()
        {
            Logger.LogDebug("********* MasterDownload() ********");

            MasterDownloadRequest request = new MasterDownloadRequest("SCH");
            ReturnMessageWrapper<MasterDownloadResponse> response = MasterDownload.MasterDownloadReport(request);

            if (!response.HasError && response.StatusCode == 200)
            {
                Console.WriteLine($"response_status    : {response.Result.response_status}");
                if (response.Result.response_status == "S")
                {
                    Console.WriteLine($"filename           : {response.Result.filename}");
                    Console.WriteLine($"Content Length     : {response.Result.filecontent.Length}");

                    if (response.Result.filecontent.Length > 0)
                    {
                        // save the content to file or db
                        StackRadius.Common.Common.SaveToFile(response.Result.filecontent, response.Result.filename);
                    }
                }
            }
            else if (response.HasError && response.StatusCode == 403)   // 403 Forbidden
            {
                Console.WriteLine($"Status      : {response.Error.status}");
                Console.WriteLine($"Error_type  : {response.Error.error_type}");
                Console.WriteLine($"Message     : {response.Error.message}");
            }
            else // 404 
            {
                Console.WriteLine($"Status      : {response.Error.status}");
                Console.WriteLine($"Error_type  : {response.Error.error_type}");
                Console.WriteLine($"Message     : {response.Error.message}");
            }
        }

        static void ClientDetailReport()
        {
            Logger.LogDebug("********* ClientDetailReport() ********");

            ClientDetailRequest request = new ClientDetailRequest("01-03-2026", "07-03-2026", "", "", "");
            ReturnMessageWrapper<ClientDetailResponse> response = ClientDetail.ClientDetailReport(request);

            if (!response.HasError && response.StatusCode == 200)
            {
                Console.WriteLine($"response_status     : {response.Result.response_status}");
                Console.WriteLine($"Total Records       : {response.Result.report_data_total}");
            }
            else if (response.HasError && response.StatusCode == 403)   // 403 Forbidden
            {
                Console.WriteLine($"Status      : {response.Error.status}");
                Console.WriteLine($"Error_type  : {response.Error.error_type}");
                Console.WriteLine($"Message     : {response.Error.message}");
            }
            else
            {
                Console.WriteLine($"Has Error     : {response.HasError}");
                Console.WriteLine($"Error remark  : {response.Result.error_remark}");
            }
        }

        static void MFAOrderwiseReport()
        {
            MFARequest request = new MFARequest();
            request.from_date = "01-02-2026";
            request.to_date = "28-02-2026";
            request.client_code = "";
            request.pg_bank_refno = "";

            ReturnMessageWrapper<MFAResponse> response = MFA.OrderWiseReport(request);

            if (!response.HasError && response.StatusCode == 200)
            {
                Console.WriteLine($"response_status    : {response.Result.response_status}");
                Console.WriteLine($"report_data_total  : {response.Result.report_data_total}");
                Console.WriteLine($"report_data        : {string.Join(", ", response.Result.report_data)}");
                Console.WriteLine($"error_remark       : {response.Result.error_remark}");
            }
            else if (response.HasError && response.StatusCode == 403)   // 403 Forbidden
            {
                Console.WriteLine($"Status      : {response.Error.status}");
                Console.WriteLine($"Error_type  : {response.Error.error_type}");
                Console.WriteLine($"Message     : {response.Error.message}");
            }
            else
            {
                Console.WriteLine($"Has Error          : {response.HasError}");
                Console.WriteLine($"response_status    : {response.Result.response_status}");
                Console.WriteLine($"report_data_total  : {response.Result.report_data_total}");
                Console.WriteLine($"report_data        : {string.Join(", ", response.Result.report_data)}");
                Console.WriteLine($"error_remark       : {response.Result.error_remark}");
            }
        }

        static void KYCFreshRegister()
        {
            KYCRequest request = new KYCRequest("AXF", "BVYPD3825K", "9748975222", "abcdef@gmail.com");
            ReturnMessageWrapper<KYCResponse> response = KYC.FreshRegister(request);

            if (!response.HasError && response.StatusCode == 200)
            {
                Console.WriteLine($"Message  : {response.Result.message}");
                Console.WriteLine($"Link     : {response.Result.link}");
            }
            else if (response.HasError && response.StatusCode == 403)   // 403 Forbidden
            {
                Console.WriteLine($"Status      : {response.Error.status}");
                Console.WriteLine($"Error_type  : {response.Error.error_type}");
                Console.WriteLine($"Message     : {response.Error.message}");
            }
            else
            {
                Console.WriteLine($"Has Error: {response.HasError}");
                Console.WriteLine($"Link     : {response.Result.link}");
                Console.WriteLine($"Message  : {response.Result.message}");
            }
        }

    }
}
