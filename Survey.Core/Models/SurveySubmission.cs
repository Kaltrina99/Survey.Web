using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class SurveySubmission
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime? EndTime { get; set; }
        [NotMapped]
        public string AgentUsername { get; set; }
        public int FormId { get; set; }
        public string AgentId { get; set; }
        public ApplicationUser Agent { get; set; }
        public string SubmitFromIP { get; set; }
        public virtual Forms Form { get; set; }
        public int? CaseId { get; set; }
        public virtual Cases Case { get; set; }
        public virtual List<Answers> Answers { get; set; } = new List<Answers>();
    }
}
