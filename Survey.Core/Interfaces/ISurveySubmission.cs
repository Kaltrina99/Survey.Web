using Survey.Core.HelperClasses;
using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface ISurveySubmission
    {
        Task<ServiceResponse<bool>> saveSubmission(SurveySubmission model);
        Task<ServiceResponse<bool>> removeSubmission(int id);
        Task<ServiceResponse<List<SurveySubmission>>> getSubmissions(int id);
        Task<ServiceResponse<SurveySubmission>> getOneSubmission(int id);
        Task<ServiceResponse<bool>> updateSubmission(SurveySubmission model);

    }
}
