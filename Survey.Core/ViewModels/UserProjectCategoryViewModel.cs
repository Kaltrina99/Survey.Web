using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Survey.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Survey.Core.ViewModels
{
    public class UserProjectCategoryViewModel
    {
        public SelectList Users { get; set; }
        public SelectList Team { get; set; }
    }
}
