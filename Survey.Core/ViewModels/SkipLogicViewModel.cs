using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.ViewModels
{
    public class SkipLogicViewModel
    {
        public List<FormViewModel> Model { get; set; } = new List<FormViewModel>();
        public int questionindex { get; set; }
        public string json { get; set; }
        public int formid { get; set; }
        public int SubmitionId { get; set; }
        public Forms Form { get; set; }
    }
}
