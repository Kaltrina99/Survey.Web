using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.HelperClasses
{
    public class ServiceResponse<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; } = true;
        public T Data { get; set; } 
    }
}
