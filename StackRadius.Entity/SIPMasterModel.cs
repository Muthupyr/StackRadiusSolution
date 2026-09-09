using System;

namespace StackRadius.Entity
{
    public class SIPMasterModel : JSONBaseClass
    {
        public String amc_code { get; set; }
        public String amc_name { get; set; }
        public String scheme_code { get; set; }
        public String scheme_name { get; set; }
        public String sip_transaction_mode { get; set; }
        public String sip_frequency { get; set; }
        public String sip_dates { get; set; }
        public int sip_minimum_gap { get; set; }
        public int sip_maximum_gap { get; set; }
        public int sip_installment_gap { get; set; }
        public int sip_status { get; set; }
        public int sip_minimum_installment_amount { get; set; }
        public double sip_maximum_installment_amount { get; set; }   // "9999999999"
        public int sip_multiplier_amount { get; set; }
        public int sip_minimum_installment_numbers { get; set; }
        public int sip_maximum_installment_numbers { get; set; }
        public String scheme_isin { get; set; }
        public String scheme_type { get; set; }
        public String pause_flag { get; set; }
        public int pause_minimum_installments { get; set; }
        public int pause_maximum_installments { get; set; }
        public int pause_modification_count { get; set; }
        public String filler_1 { get; set; }
        public String filler_2 { get; set; }
        public String filler_3 { get; set; }
        public String filler_4 { get; set; }
        public String filler_5 { get; set; }
    }
}
