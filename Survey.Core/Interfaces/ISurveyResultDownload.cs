
using Survey.Core.HelperClasses;
using Survey.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface ISurveyResultDownload
    {
        Task<ServiceResponse<string>> DownloadFile(int id,string path);
        Task<ServiceResponse<SurveyResultsViewModel>> getDownloads(int id);
        Task<ServiceResponse<SurveyResultsViewModel>> NewDownload(int id,int optionId,string path,string userid);
    
    }
}
