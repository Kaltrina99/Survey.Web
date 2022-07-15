using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Utility
{
   public class ExcelToFormException :Exception
    {
        public ExcelToFormException() { }
        public ExcelToFormException(string message):base(message)
        {

        }
        public ExcelToFormException(string message,Exception inner) : base(message,inner)
        {

        }
    }
}
