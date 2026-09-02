namespace StackRadius.Entity
{
    public class FundAllocationModel : JSONBaseClass
    {
        public string Mode { get; set; }
        public string cfppgbankrefno { get; set; } // "2026020900004",
        public string order_number { get; set; } // "460400000296",
        public string duplicatepgbank { get; set; } // " ",
        public string orderprocessduplicatepgbank { get; set; } // " ",
        public string importedfilename { get; set; } // "NCL_RZP_ND_09022026_04.txt",
        public string errorremarks { get; set; } // " ",
        public string id { get; set; } // "670",
        public string utrno { get; set; } // "UTR2026020900004",
        public string membercode { get; set; } // "99915",
        public string clientcode { get; set; } // "TESTT01",
        public string paymentdate { get; set; } // "09-02-2026 02:03:12",
        public string totalamount { get; set; } // "200000",
        public string totalallocatedamount { get; set; } // "200000",
        public string remainingamount { get; set; } // "0",
        public string totalsettlementamount { get; set; } // "0",
        public string remainingamountsettlement { get; set; } // "200000",
        public string totalallotmentamount { get; set; } // "0",
        public string remainingaamountallotment { get; set; } // "200000",
        public string settlement_number { get; set; } // "2026028",
        public string settled_flag { get; set; } // "N",
        public string allotment_flag { get; set; } // "N",
        public string clientname { get; set; } // "BULBUL BULBUL BHAN",
        public string bankname { get; set; } // "HDFC BANK",
        public string ifsccode { get; set; } // "HDFC0000060",
        public string remitteraccountno { get; set; } // "1234567890",
        public string taxstatus { get; set; } // "01",
        public string accounttype { get; set; } // "SB",
        public string refundamount { get; set; } // " ",
        public string refunddate { get; set; } // " ",
        public string refundutr { get; set; } // " ",
        public string payment_mode { get; set; } // "NODL"        
    }
}

