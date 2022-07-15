using Survey.Core.HelperClasses;
using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Repositories
{
    public class SubmissionRepository : ISurveySubmission
    {
        private readonly ApplicationDbContext _db;

        public SubmissionRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public  async Task<ServiceResponse<SurveySubmission>> getOneSubmission(int id)
        {
            ServiceResponse<SurveySubmission> response = new ServiceResponse<SurveySubmission>();
            try
            {
                var record = await _db.SurveySubmissions.FirstOrDefaultAsync(x => x.Id == id);
                if (record is null) 
                {
                    response.Success = false;
                    response.Message = "Record Not Found";
                    return response;
                }
                response.Data = record;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<SurveySubmission>>> getSubmissions(int id)
        {
            ServiceResponse<List<SurveySubmission>> response = new ServiceResponse<List<SurveySubmission>>();
            try
            {
                var records = await _db.SurveySubmissions.Where(x => x.FormId == id).ToListAsync();
                if (records.Count==0) 
                {
                    response.Success = false;
                    response.Message = "Records Not Found";
                    return response;
                }
                response.Data = records;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> removeSubmission(int id)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();
            try
            {
                var record = await _db.SurveySubmissions.Include(x=>x.Answers).FirstOrDefaultAsync(x => x.Id == id);
                if (record is null) 
                {
                    response.Message = "Record Not Found";
                    response.Success = false;
                    return response;
                }
                _db.Remove(record);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> saveSubmission(SurveySubmission model)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();
            try
            {
                _db.SurveySubmissions.Add(model);
               await  _db.SaveChangesAsync();
          
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> updateSubmission(SurveySubmission model)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();
            try
            {
                var record = await _db.SurveySubmissions.Include(x=>x.Answers).FirstOrDefaultAsync(x => x.Id == model.Id);
                if (record is null)
                {
                    response.Success = false;
                    response.Message = "Record Not Found";
                    return response;
                }
                record.Answers = model.Answers;
                _db.Update(record);
                await _db.SaveChangesAsync();
                return response;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
    }
}
