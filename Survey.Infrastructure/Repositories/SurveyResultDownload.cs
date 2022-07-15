using Survey.Core.Constants;
using Survey.Core.HelperClasses;
using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using Survey.Infrastructure.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Repositories
{
    public class SurveyResultDownload : ISurveyResultDownload
    {
        private readonly ApplicationDbContext _db;

        public SurveyResultDownload(ApplicationDbContext db)
        {
           _db = db;
        }
       
        public async Task<ServiceResponse<string>> DownloadFile(int id,string path)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            SurveyResultsViewModel model = new SurveyResultsViewModel();
            try
            {
                var record = await _db.SurveyDownloads.FirstOrDefaultAsync(x => x.Id == id);
                
                if (record is null)
                {
                    response.Success = false;
                    response.Message = "Record Not Found";
                    return response;

                }
                response.Data = DownloadResults.GetFilePath(record,path);
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Success = false;
                return response;
            }
            return response;
        }


        public async Task<ServiceResponse<SurveyResultsViewModel>> getDownloads(int id)
        {
            ServiceResponse<SurveyResultsViewModel> response = new ServiceResponse<SurveyResultsViewModel>();
            SurveyResultsViewModel model = new SurveyResultsViewModel();
            try
            {
                var data = await _db.SurveyDownloads.Where(x => x.FormId == id).Select(x=> new { x,x.User.UserName}).ToListAsync();
                foreach (var record in data) 
                {
                    var download = new SurveyDownload 
                    { 
                        Id = record.x.Id,
                        User = new ApplicationUser { UserName = record.UserName } ,
                        FormId= record.x.FormId,
                        Date = record.x.Date,
                        DownloadType=record.x.DownloadType ,
                        FileName= record.x.FileName
                    };
                    model.Downloads.Add(download);
                }

            }
            catch (Exception e) 
            {
                response.Message = e.Message;
                response.Success = false;
                return response;
            }
            response.Data = model;
            return response;
        }

        public async Task<ServiceResponse<SurveyResultsViewModel>> NewDownload(int id, int optionId,string path,string userid)
        {
            ServiceResponse<SurveyResultsViewModel> response = new ServiceResponse<SurveyResultsViewModel>();
            SurveyResultsViewModel model = new SurveyResultsViewModel();
            SurveyDownload download = new();
            string filename="";
            try
            {
                if (DownloadTypes.JSON == (DownloadTypes)optionId || DownloadTypes.Excel == (DownloadTypes)optionId)
                {
                    var records = await _db.Forms
                       .Where(x => x.Id == id).Include(x=>x.Questions).ThenInclude(x=>x.Options).FirstOrDefaultAsync();
                    var submissions = await _db.SurveySubmissions.Select(x => new SurveySubmission
                    {
                        Id = x.Id,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        AgentUsername =x.AgentUsername,
                        SubmitFromIP = x.SubmitFromIP,
                        Answers = x.Answers,
                        FormId = x.FormId
                    }).Where(x => x.EndTime != null && x.FormId == id).AsNoTracking().ToListAsync();
                   

                    records.Submissions = submissions;
                    records.Questions = records.Questions.OrderBy(x => x.QuestionOrder).ToList();
                    records.Questions.ForEach(x =>x.Options= x.Options.OrderBy(x => x.OrderNumber).ToList());
                    records.Submissions.RemoveAll(x => x.EndTime == null);
                    foreach(var sub in submissions) 
                    {
                        _db.Entry(sub).State = EntityState.Detached;

                    }
                    _db.Entry(records).State = EntityState.Detached;
                    if (DownloadTypes.JSON == (DownloadTypes)optionId)
                    { 
                        filename = DownloadResults.CreateDownloadJson(records, path);
                        download.DownloadType = DownloadTypes.JSON;
                    }
                    else 
                    {
                        download.DownloadType = DownloadTypes.Excel;
                        SurveyResultsExcel excel = new SurveyResultsExcel(records,path);
                        filename= excel.CreateDownloadExcel();
                        
                    }
                }
                
               
                download.FileName = filename;
                download.Date = DateTime.Now;
                download.UserId = userid;
                download.FormId = id;
                await _db.SurveyDownloads.AddAsync(download);
                await _db.SaveChangesAsync();
                model.Download = download;
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Success = false;
                return response;
            }
            response.Data = model;
            return response;
        }
    }
}
