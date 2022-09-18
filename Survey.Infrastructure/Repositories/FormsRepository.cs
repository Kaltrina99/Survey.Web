using Survey.Core.HelperClasses;
using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Infrastructure.Data;
using Survey.Infrastructure.Utility;
using Survey.Core.Filter;
using Survey.Infrastructure.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using Survey.Core.ViewModels;

namespace Survey.Infrastructure.Repositories
{
    public class FormsRepository : Repository<Forms>, IForms
    {
        private readonly ApplicationDbContext _dbContext;

        public FormsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Forms form)
        {
            _dbContext.Forms.Update(form);
        }
        public Forms GetForm (int id) 
        {
            return _dbContext.Forms.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == WebConstants.Project)
            {
                return _dbContext.Projects.Select(proj => new SelectListItem
                {
                    Value = proj.Id.ToString(),
                    Text = proj.Title
                });
            }
            return null;
        }

        public bool UpdateForm(Forms form)
        {
            try
            {
             var oldform= _dbContext.Forms.FirstOrDefault(x => x.Id == form.Id);
                if (oldform is not null) 
                {
                    oldform.FormTitle = form.FormTitle;
                    oldform.Project_Id = form.Project_Id;
                    oldform.Description = form.Description;
                    _dbContext.Update(oldform);
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e) 
            {
                return false;
            }
            
        }
        public async Task<ServiceResponse<XSSFWorkbook>> CreateExcelFromForm(int id) 
        {
            
            var response = new ServiceResponse<XSSFWorkbook>();
            try
            {
                var fro = await _dbContext.Forms
                    .Include(x=>x.Questions)
                    .Include(x=>x.Questions)
                    .ThenInclude(x=>x.Options)
                    .Include(x=>x.Questions)
                    .ThenInclude(x=>x.skipChild).AsSplitQuery().FirstOrDefaultAsync(x => x.Id == id);
               fro.Questions= fro.Questions.OrderBy(x => x.QuestionOrder).ToList();
                fro.Questions.ForEach(x => x.Options = x.Options.OrderBy(x => x.OrderNumber).ToList());
               
                if (fro != null)
                {

                    FormToExcel excel = new FormToExcel(fro);
                    response.Data = excel._excel;
                    return response;

                }
            }
            catch (Exception e) 
            {
                response.Success = false;
            }
            response.Message = "Form Not Found";
            return response;
        }
        public async  Task<ServiceResponse<bool>> CreateFormFromExcel(IFormFile file,int projectid)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                ExcelToForm excform = new ExcelToForm();
                excform.MapObject(file);
                Forms form = excform.MainForm;
                form.Project_Id = projectid;
                form.Questions = new List<Questions>();
                await _dbContext.Forms.AddAsync(form);
                await _dbContext.SaveChangesAsync();
                form.Questions = excform.Formquestions.Select(x => x.Value.Item1).ToList();
                foreach (var question in form.Questions)
                {
                   
                    int optionorder = 0;
                    foreach (var opt in question.Options)
                    {
                        opt.OrderNumber = optionorder;
                        optionorder++;
                       
                    }
                  
                }
                _dbContext.Forms.Update(form);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;

            }
            return response;
        }
        public IPagedList<Forms> GetForms(FormFilter filter, List<string>? id, List<int>? proj,int status, int pagenumber, int pagesize)
        {
            if (id.Count==0) { 
            List<Forms> results = new List<Forms>();

            var formlist = _dbContext.Forms.ToPagedList(pagenumber, pagesize);
            return formlist;
            }
            else
            {
                List<Forms> results = new List<Forms>();

                foreach ( var nr in proj)
                {
                    List<Forms> tem = new List<Forms>();
                    if (status==2)
                    {
                        tem.AddRange( _dbContext.Forms.Where(x => x.Project_Id == nr && (int)x.Form_Status == status).ToList());
                    }
                    tem.AddRange(_dbContext.Forms.Where(x => x.Project_Id == nr && (int)x.Form_Status == 1).ToList());
                    results.AddRange(tem);
                }
                return results.ToPagedList(pagenumber, pagesize);
            }
            
        }

        public List<Projects> GetProjects()
        {
            List<Projects> results = new List<Projects>();

            return results;
        }
    } 
}
