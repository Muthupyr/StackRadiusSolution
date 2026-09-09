using System;
using System.Data;

namespace StackRadius.Entity
{
    public class NSESWPMasterModel : JSONBaseClass
    {
        public String amc_code { get; set; }
        public String amc_name { get; set; }
        public String nse_scheme_code { get; set; }
        public String scheme_name { get; set; }
        public String scheme_isin { get; set; }
        public String scheme_type { get; set; }
        public String aswp_transaction_mode { get; set; }
        public double aswp_minimum_installment_amount { get; set; }
        public double aswp_maximum_installment_amount { get; set; }
        public double aswp_multiplier_amount { get; set; }
        public double aswp_minimum_installment_units { get; set; }
        public double aswp_maximum_installment_units { get; set; }
        public double aswp_multiplier_units { get; set; }
        public int aswp_minimum_installment_numbers { get; set; }
        public int aswp_maximum_installment_numbers { get; set; }
        public String aswp_frequency { get; set; }
        public String aswp_dates { get; set; }
        public int aswp_minimum_gap { get; set; }
        public int aswp_maximum_gap { get; set; }
        public int aswp_installment_gap { get; set; }
        public String aswp_status { get; set; }     
    }
}
