using System;

namespace StackRadius.Entity
{
    public class SchemaMasterModel : JSONBaseClass
    {
        public int unique_sr_no { get; set; }                   //UNIQUE SR NO	-	38352
        public string scheme_code { get; set; }                 //SCHEME CODE	-	IIIBHRG-GR
        public string rta_scheme_code { get; set; }             //RTA SCHEME CODE	-	IBHRG
        public string amc_scheme_code { get; set; }             //AMC SCHEME CODE	-	IBHRG
        public string isin { get; set; }                        //ISIN	-	INF579M01AV5
        public string amc_code { get; set; }                    //AMC CODE	-	360_ONE_MUTUALFUND_MF
        public string scheme_type { get; set; }                 //SCHEME TYPE	-	DEBT
        public string plan_type { get; set; }                   //PLAN TYPE	-	NORMAL
        public string scheme_name { get; set; }                 //SCHEME NAME	-	360 ONE BALANCED HYBRID FUND - REGULAR PLAN - GROWTH
        public string purchase_allowed { get; set; }            //PURCHASE ALLOWED	-	Y
        public string purchase_transaction_mode { get; set; }   //PURCHASE TRANSACTION MODE	-	DP
        public double new_purchase_min_amount { get; set; }     //NEW PURCHASE MIN AMOUNT	-	1000
        public double additional_purchase_min_amount { get; set; }  //ADDITIONAL PURCHASE MIN AMOUNT	-	1000
        public int additional_purchase_max_amount { get; set; }     //ADDITIONAL PURCHASE MAX AMOUNT	-	0
        public double purchase_amount_multiplier { get; set; }      //PURCHASE AMOUNT MULTIPLIER	-	1
        public TimeSpan purchase_cutoff_time { get; set; }          //PURCHASE CUTOFF TIME	-	14:30:00 'time without time zone	
        public string redemption_allowed { get; set; }              //REDEMPTION ALLOWED	-	Y
        public string redemption_transaction_mode { get; set; }     //REDEMPTION TRANSACTION MODE	-	DP
        public double redemption_min_qty { get; set; }              //REDEMPTION MIN QTY	-	0.001
        public double redemption_qty_multiplier { get; set; }       //REDEMPTION QTY MULTIPLIER	-	0.001
        public int redemption_max_qty { get; set; }                 //REDEMPTION MAX QTY	-	0
        public double redemption_min_amount { get; set; }           //REDEMPTION MIN AMOUNT	-	1
        public int redemption_max_amount { get; set; }              //REDEMPTION MAX AMOUNT	-	0
        public double redemption_amount_multiplier { get; set; }    //REDEMPTION AMOUNT MULTIPLIER	-	0.01
        public TimeSpan redemption_cutoff_time { get; set; }        //REDEMPTION CUTOFF TIME	-	15:00:00 -time without time zone	
        public string rta_agent_code { get; set; }                  //RTA AGENT CODE	-	CAMS
        public string amc_active_flag { get; set; }                 //AMC ACTIVE FLAG	-	Y
        public string div_reinvest_flag { get; set; }               //DIV REINVEST FLAG	-	Z
        public string sip_allowed { get; set; }                     //SIP ALLOWED	-	Y
        public string stp_enabled { get; set; }                     //STP ENABLED	-	Y
        public string swp_enabled { get; set; }                     //SWP ENABLED	-	Y
        public string switch_allowed { get; set; }                  //SWITCH ALLOWED	-	Y
        public string settlement_type { get; set; }                 //SETTLEMENT TYPE	-	T2
        public string amc_ind { get; set; }                         //AMC IND	-	
        public int face_value { get; set; }                         //FACE VALUE	-	0
        public DateTime scheme_start_date { get; set; }             //SCHEME START DATE	-	01-01-2010
        public DateTime maturity_date { get; set; }             //MATURITY DATE	-	31-12-2099
        public string exit_load_flag { get; set; }              //EXIT LOAD FLAG	-	N
        public string exit_load { get; set; }                   //EXIT LOAD	-	
        public string lock_in_period_flag { get; set; }         //LOCK IN PERIOD_FLAG	-	N
        public int lock_in_period { get; set; }                 //LOCK IN PERIOD	-	0
        public string channel_partner_code { get; set; }        //CHANNEL PARTNER CODE	-	IFIBHRG
        public DateTime reopening_date { get; set; }            //REOPENING DATE	-	01-01-2026
        public string open_close_ended_scheme { get; set; }     //OPEN/CLOSE ENDED SCHEME	
    }
}