using Survey.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Models
{
    public class SurveyDownload
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DownloadTypes DownloadType { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int  FormId { get; set; }
        public Forms Form { get; set; }
    }
}
