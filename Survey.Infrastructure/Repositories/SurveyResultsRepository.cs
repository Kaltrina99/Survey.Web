using Survey.Core.DTOmodels;
using Survey.Core.Extensions;
using Survey.Core.HelperClasses;
using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Survey.Infrastructure.Repositories
{
    public class SurveyResultsRepository : ISurveyResults
    {
        private readonly ApplicationDbContext _db;

        public SurveyResultsRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<ServiceResponse<SurveyResultsViewModel>> getDashbord(int id)
        {
            ServiceResponse<SurveyResultsViewModel> response = new();
            SurveyResultsViewModel model = new SurveyResultsViewModel();
            model.FormId = id;
            try
            {
                var date = DateTime.Now;
                var submissions = await _db.SurveySubmissions
                    .Include(x => x.Answers)
                    .ThenInclude(x => x.Option)
                    .Where(x => x.EndTime != null && x.FormId == id)
                    .ToListAsync();
                var grouped = submissions.GroupBy(x => x.StartTime.ToLongDateString()).Select(x => Tuple.Create(x.Key, x.Count())).ToList();
                int MonthSub = submissions.Where(x => x.StartTime.Year == date.Year && x.StartTime.Month == date.Month).Count();
                int WeekSub = submissions.Where(x => x.StartTime.DatesInSameWeek(date, DayOfWeek.Monday)).Count();
                var TotalSubmissions = submissions.Count();
                model.RecentSubmits = grouped;
                model.TotalSubmissions = TotalSubmissions;
                model.ThisMonthSubs = MonthSub;
                model.ThisWeekSubs = WeekSub;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            response.Data = model;
            return response;
        }
       

        public async Task<ServiceResponse<SurveyResultsViewModel>> getMedia(int id)
        {
            ServiceResponse<SurveyResultsViewModel> response = new();
            SurveyResultsViewModel model = new SurveyResultsViewModel();
            model.FormId = id;
            try
            {
                var medias = await _db.Answers.Where(x => x.Form_Id == id && (int)x.question.Field_Type == 12).Select(x => x.Answer).ToListAsync();
                model.Images = medias;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            response.Data = model;
            return response;
        }

        public async Task<ServiceResponse<SurveyResultsViewModel>> getTable(int id, int pageNumber, int pageSize)
        {
            ServiceResponse<SurveyResultsViewModel> response = new();
            SurveyResultsViewModel model = new SurveyResultsViewModel();
            model.FormId = id;
            try
            {
                var questions = await _db.Questions
                     .Where(x => x.Form_Id == id)
                     .Include(x => x.Options)
                     .ToListAsync();
                questions = questions.OrderBy(x => x.QuestionOrder).ToList();
                var submissions =await _db.SurveySubmissions.Select(x => new SurveySubmission
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    AgentUsername = x.Agent.UserName,
                    SubmitFromIP = x.SubmitFromIP,
                    Answers = x.Answers.ToList(),
                    FormId = x.FormId
                }).Where(x => x.EndTime != null && x.FormId == id).ToPagedListAsync(pageNumber, pageSize);
                model.Questions = questions;
                model.Submissions = submissions;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            response.Data = model;
            return response;
        }

        public async Task<ServiceResponse<SurveyResultsViewModel>> getReports(int id)
        {
            ServiceResponse<SurveyResultsViewModel> response = new();
            SurveyResultsViewModel model = new SurveyResultsViewModel();
            model.FormId = id;
            try
            {
                var form = await _db.Forms
                        .Include(x => x.Submissions.Where(x => x.EndTime != null))
                        .ThenInclude(x => x.Answers)
                        .Include(x => x.Questions)
                        .ThenInclude(x => x.Options)
                        .Where(x => x.Id == id).AsSplitQuery().FirstOrDefaultAsync();
                form.Questions = form.Questions.OrderBy(X => X.QuestionOrder).ToList();
                FormReport report = new FormReport(form);
                model.Report = report;

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            response.Data = model;
            return response;
        }

    }
}
