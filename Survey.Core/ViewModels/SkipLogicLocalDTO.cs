using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.ViewModels
{
    public class SkipLogicLocalDTO
    {
        public int parentquestion { get; set; }
        public int childquestion { get; set; }
        public string Operator { get; set; }
        public double? value { get; set; }
        public int? Optionid { get; set; }
        public bool isChecked { get; set; } = false;


    }
}
