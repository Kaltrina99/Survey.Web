using Survey.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.ViewModels
{
    public class DatasetViewModel
    {
        public Dataset Dataset { get; set; }
        public EnrollDataset EnrollDataset { get; set; }
        public Cases Cases { get; set; }
        public CaseAssignedForms CaseAssignedForms { get; set; }
        public int caseNumber { get; set; }

        public IEnumerable<Dataset> Datasets { get; set; }
        public IEnumerable<CasesExcelHeaders> CasesExcelHeaders { get; set; }
        public List<CasesExcelData> CasesExcelData { get; set; }
        public IEnumerable<Cases> CaseList { get; set; }
        public IEnumerable<SelectListItem> FormsList { get; set; }
        public IEnumerable<CaseAssignedForms> AssignedFormsList { get; set; }


    }
}
