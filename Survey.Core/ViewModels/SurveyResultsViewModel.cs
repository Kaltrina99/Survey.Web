using Survey.Core.Constants;
using Survey.Core.DTOmodels;
using Survey.Core.HelperClasses;
using Survey.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using X.PagedList;

namespace Survey.Core.ViewModels
{
    public class SurveyResultsViewModel
    {
        public int TotalSubmissions { get; set; }
        public IPagedList<SurveySubmission> Submissions { get; set; }
        public List<Questions> Questions { get; set; }
        public List<Tuple<string, int>> RecentSubmits { get; set; }
        public int ThisMonthSubs { get; set; }
        public int ThisWeekSubs { get; set; }
        public int FormId { get; set; }
        public List<string> Images { get; set; }

        public FormReport Report { get; set; }

        #region download
        public List<SurveyDownload> Downloads { get; set; } = new List<SurveyDownload>();
        public SurveyDownload Download { get; set; }
        public DownloadTypes DownloadTypes { get; set; }
        public IFormFile File { get; set; }
        #endregion


    }

}
