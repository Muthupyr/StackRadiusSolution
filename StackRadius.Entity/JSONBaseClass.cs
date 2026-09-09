using System;

namespace StackRadius.Entity
{
    public class JSONBaseClass
    {
        public string Mode { get; set; }
        public int RecordFrom { get; set; }
        public int RecordTo { get; set; }
        public string SortKey { get; set; }
        public string SortDir { get; set; }
        public string FilterCondition { get; set; }
        public int TotalRowCount { get; set; }

        //public string CreatedUser { get; set; }
        //public int? CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public string UpdatedUser { get; set; }
        //public int? UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
    }
}
