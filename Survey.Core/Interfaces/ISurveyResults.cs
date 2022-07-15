using Survey.Core.HelperClasses;
using Survey.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface ISurveyResults
    {
        Task<ServiceResponse<SurveyResultsViewModel>> getDashbord(int id);
        Task<ServiceResponse<SurveyResultsViewModel>> getTable(int id,int pageNumber,int pageSize);
        Task<ServiceResponse<SurveyResultsViewModel>> getMedia(int id);
        Task<ServiceResponse<SurveyResultsViewModel>> getMap(int id);
        Task<ServiceResponse<SurveyResultsViewModel>> getReports(int id);

    }
}
