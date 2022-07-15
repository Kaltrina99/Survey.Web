using Survey.Core.DTOmodels;
using Survey.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.ViewModels
{
    public class TakeSurveyViewModel
    {
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
        public DateTime EndTime { get; set; }
        public string SAgTRid { get; set; }
        public int caseId { get; set; }
        public int formid { get; set; }
        public List<SkipLogicLocalDTO> SkipLogicLocalDTOs { get; set; } = new();
        public bool isTest { get; set; } = false;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public int SubmitionId { get; set; }
        public Forms Form { get; set; }
        public string SkipLogicJson { get; set; }

        public void JsonSkipDest() 
        {
             SkipLogicJson = JsonConvert.SerializeObject(SkipLogicLocalDTOs, Formatting.Indented,
                   new JsonSerializerSettings
                   {
                       PreserveReferencesHandling = PreserveReferencesHandling.Objects
                   });
        }
    }
}
