using System;

namespace StackRadius.App
{
   public class ErrorLogModel
    {
        public string UserName { get; set; }
        public DateTime DateTime { get; set; }
        public string ErrorLocation { get; set; }
        public string ErrorType { get; set; }
        public string ErrorDescription { get; set; }
    }

}