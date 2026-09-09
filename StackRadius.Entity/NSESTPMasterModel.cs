using System;

namespace StackRadius.Entity
{
    public class NSESTPMasterModel : JSONBaseClass
    {
        public String amc_code { get; set; }
        public String amc_name { get; set; }
        public String nse_scheme_code { get; set; }
        public String scheme_name { get; set; }
        public String scheme_isin { get; set; }
        public String scheme_type { get; set; }
        public String astp_transaction_mode { get; set; }
        public int astp_in_minimum_installment_amount { get; set; }
        public long astp_in_maximum_installment_amount { get; set; }
        public double astp_in_multiplier_amount { get; set; }
        public int astp_out_minimum_installment_amount { get; set; }
        public long astp_out_maximum_installment_amount { get; set; }
        public double astp_out_multiplier_amount { get; set; }
        public double astp_minimum_installment_units { get; set; }
        public int astp_maximum_installment_units { get; set; }
        public double astp_multiplier_units { get; set; }
        public int astp_minimum_installment_numbers { get; set; }
        public int astp_maximum_installment_numbers { get; set; }
        public int astp_reg_in { get; set; }
        public int astp_reg_out { get; set; }
        public String astp_frequency { get; set; }
        public String astp_dates { get; set; }
        public int astp_minimum_gap { get; set; }
        public int astp_maximum_gap { get; set; }
        public int astp_installment_gap { get; set; }
        public String astp_status { get; set; }

    }
}
