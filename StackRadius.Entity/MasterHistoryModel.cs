using System;

namespace StackRadius.Entity
{
    public class MasterHistoryModel : JSONBaseClass
    {
        public int? Id { get; set; }
        public DateTime DownloadOn { get; set; }         
        public string Filename { get; set; }           
        public string NoOfRecords { get; set; }       
        public DateTime UpdatedOn { get; set; }      
    }
}


