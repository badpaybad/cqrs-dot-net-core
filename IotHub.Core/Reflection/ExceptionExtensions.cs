using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.Core.Reflection
{
  public static  class ExceptionExtensions
    {
        public static string GetMessages(this Exception ex)
        {
            if (ex == null) return string.Empty;
            string msg = string.Empty;

            while (ex!=null)
            {
                msg += ex.Message +"\r\n";
                ex = ex.InnerException;
            }

            return msg;
        }
    }
}
