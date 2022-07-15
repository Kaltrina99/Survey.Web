using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Filter
{
    public class FormFilter
    {
        public int? FilterProjectId { get; set; }
        public int? FilterClientId { get; set; }
        public string SearchingWord { get; set; }

    }
}
