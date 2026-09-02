using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackRadius.Common
{ 
    public class DataTable<T>
    {
        public DataTable()
        {
        }
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<T> data { get; set; }
    }
}