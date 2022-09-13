using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.ViewModels
{
    public class UserProjectViewModel
    {
        public SelectList Users { get; set; }
        public SelectList Project { get; set; }
    }
}
