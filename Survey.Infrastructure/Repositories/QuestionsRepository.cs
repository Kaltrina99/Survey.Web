using Survey.Core.DTOmodels;
using Survey.Core.HelperClasses;
using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Repositories
{
    public class QuestionsRepository : Repository<Questions>, IQuestions
    {
        private readonly ApplicationDbContext _dbContext;

        public QuestionsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Questions objQuestion)
        {
            _dbContext.Questions.Update(objQuestion);
        }

        public List<Questions> GetQuestions(int obj)
        {
            return _dbContext.Questions.Include(x=>x.Options).Where(x => x.Form_Id == obj).OrderBy(x=>x.QuestionOrder).ToList();
        }

        public IEnumerable<Questions> AllQuestions()
        {
            return _dbContext.Questions.Include(f => f.form).Include(o => o.Options);

            /*response.Data.Questions = await _db.Questions
                   .Where(x => x.SectionId == id && !x.skipChild.Any())
                   .Include(x => x.translations.Take(1))
                   .Include(x => x.Options)
                   .ThenInclude(x => x.translation.Take(1))
                   .Select(x => new ParentQuestionDto
                   {
                       isParent = x.skipParent.Any(),
                       question = x,
                       options = x.Options.Select(y => new OptionDtoParent
                       {
                           Skip = y.ChildQuestions.Any(),
                           option = y
                       }).ToList()
                   })
                   .ToListAsync();
*/
        }
        public SkipLogicViewModel TestMethod(int id)
        {

            SkipLogicViewModel model = new SkipLogicViewModel();
            try
            {
                var questions = _dbContext.Questions
                           .Where(x => x.Form_Id == id && !x.skipChild.Any())
                           .Include(x => x.Options)
                           .Select(x => new Questions
                           {
                               Skip_Logic = x.skipParent.Any(),
                               Id = x.Id,
                               QuestionDescription = x.QuestionDescription,
                               Field_Type = x.Field_Type,
                               IsRequired = x.IsRequired,
                               QuestionText = x.QuestionText,
                               Options = x.Options.Select(y => new QuestionOptions
                               {
                                   Skip_Logic = y.ChildOption.Any(),
                                   OptionText = y.OptionText,
                                   Question_Id = x.Id,
                                   Id = y.Id
                               }).ToList()

                           })
                           .ToList();
                model.Form = _dbContext.Forms.FirstOrDefault(x => x.Id == id);
                List<Questions> numberquestion = questions.Where(x => x.Field_Type == QType.Numbers).ToList();
                if (numberquestion.Count > 0)
                {
                    var skiplogic = _dbContext.SkipLogic
                         .Where(x => numberquestion.Select(y => y.Id).Contains(x.Parent_Question_Id))
                         .Select(x => new SkipLogicLocalDTO
                         {
                             
                             parentquestion = x.Parent_Question_Id,
                             value = x.Comparable_Value,
                             Operator = Operator.getOneOperator(x.Operator).OperatorSymbol
                         })
                         .ToList();
                    var json = JsonConvert.SerializeObject(skiplogic, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                    model.json = json;

                }

                var formview = new List<FormViewModel>();
                foreach (Questions question in questions)
                {
                    var formViewModel = new FormViewModel
                    {
                        QuestionId = question.Id,
                        QuestionDescription = question.QuestionDescription,
                        QuestionText = question.QuestionText,
                        QuestionType = question.Field_Type,
                        isParent = question.Skip_Logic,
                        _Options = question.Options
                    };
                    model.Model.Add(formViewModel);
                }


            }
            catch (Exception e)
            {

            }
            return model;

        }

        public IEnumerable<Questions> GetQuestionsByFormId(int? obj)
        {
            return _dbContext.Questions.Where(q => q.Form_Id == obj);
        }

        public int GetQuestionsFormId(int obj)
        {
            int query = _dbContext.Questions.Where(x => x.Id == obj).Select(x => x.Form_Id).FirstOrDefault();

            return query;
        }

        public IEnumerable<Questions> GetQuestionById(int obj)
        {
            return _dbContext.Questions.Include(y => y.skipChild).Where(x => x.Id == obj);
        }

        public IEnumerable<SelectListItem> ParentQuestions(int id, int formId)
        {
            var haskip = _dbContext.Questions.Include(x => x.skipChild).Include(x => x.skipParent).FirstOrDefault(x => x.Id == id);
            var list = _dbContext.Questions.Where(x => x.Id != id&&x.QuestionOrder<haskip.QuestionOrder && x.Form_Id == formId && (x.Field_Type == QType.Select_Multiple || x.Field_Type == QType.Select_One || x.Field_Type == QType.Numbers  || x.Field_Type == QType.Decimal)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.QuestionText
            });
          
            return list;
        }

        public int GetQuestionType(int obj)
        {
            return Convert.ToInt32(_dbContext.Questions.Where(x => x.Id == obj).Select(x => x.Field_Type).FirstOrDefault());
        }

        public Questions GetOldQuestion(int id)
        {
            return _dbContext.Questions.Where(x => x.Id == id).FirstOrDefault();
        }

        public SkipLogicViewModel ShowSkippedQuestions(int id)
        {
            SkipLogicViewModel model = new SkipLogicViewModel();
            List<Questions> questions = new List<Questions>();
            


                 questions = _dbContext.Questions
                       .Where(x => x.skipChild.Any(y => y.Condition_Option == id))
                       .Include(x => x.Options)
                       .Select(x => new Questions
                       {
                           Skip_Logic = x.skipParent.Any(),
                           Id = x.Id,
                           Field_Type = x.Field_Type,
                           IsRequired = x.IsRequired,
                           Options = x.Options.Select(y => new QuestionOptions
                           {
                               Skip_Logic = y.ChildOption.Any(),
                               Question_Id = x.Id,
                               Id = y.Id
                           }).ToList()

                       })
                       .ToList();
           
            List<Questions> numberquestion = questions.Where(x => x.Field_Type == QType.Numbers).ToList();
            if (numberquestion.Count > 0)
            {
                var skiplogic = _dbContext.SkipLogic
                     .Where(x => numberquestion.Select(y => y.Id).Contains(x.Parent_Question_Id))
                     .Select(x => new SkipLogicLocalDTO
                     {
                         
                         parentquestion = x.Parent_Question_Id,
                         value = x.Comparable_Value,
                         Operator = Operator.getOneOperator(x.Operator).OperatorSymbol
                     })
                     .ToList();
                var json = JsonConvert.SerializeObject(skiplogic, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });
                model.json = json;

            }

            var formview = new List<FormViewModel>();
            foreach (Questions question in questions)
            {
                var formViewModel = new FormViewModel
                {
                    QuestionId = question.Id,
                    QuestionDescription=question.QuestionDescription,
                    QuestionText = question.QuestionText,
                    QuestionType = question.Field_Type,
                    isParent = question.Skip_Logic,
                    _Options = question.Options
                };
                model.Model.Add(formViewModel);
            }
            return model;
        }

        public SkipLogicViewModel GetSkippedQuestionNumber(int id)
        {
            SkipLogicViewModel model = new SkipLogicViewModel();
            List<Questions> questions = new List<Questions>();
            
                questions = _dbContext.Questions
               .Where(x => x.skipChild.Any(y => y.Id == id))
               
               .Include(x => x.Options)
               .Select(x => new Questions
               {
                   Skip_Logic = x.skipParent.Any(),
                   Id = x.Id,
                   Field_Type = x.Field_Type,
                   IsRequired = x.IsRequired,
                   Options = x.Options.Select(y => new QuestionOptions
                   {
                       Skip_Logic = y.ChildOption.Any(),
                       Question_Id = x.Id,
                       Id = y.Id
                   }).ToList()

               })
               .ToList();

            List<Questions> numberquestion = questions.Where(x => x.Field_Type == QType.Numbers).ToList();
            if (numberquestion.Count > 0)
            {
                var skiplogic = _dbContext.SkipLogic
                     .Where(x => numberquestion.Select(y => y.Id).Contains(x.Parent_Question_Id))
                     .Select(x => new SkipLogicLocalDTO
                     {
                       
                         parentquestion = x.Parent_Question_Id,
                         value = x.Comparable_Value,
                         Operator = Operator.getOneOperator(x.Operator).OperatorSymbol
                     })
                     .ToList();

                var json = JsonConvert.SerializeObject(skiplogic, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });
                model.json = json;

            }

            var formview = new List<FormViewModel>();
            foreach (Questions question in questions)
            {
                var formViewModel = new FormViewModel
                {
                    QuestionId = question.Id,
                    QuestionText = question.QuestionText,
                    QuestionType = question.Field_Type,
                    QuestionDescription=question.QuestionDescription,
                    isParent = question.Skip_Logic,
                    _Options = question.Options
                };
                model.Model.Add(formViewModel);
            }
            return model;

        }

        public ServiceResponse<SkipLogicViewModel> DeleteQuestion(int id)
        {
            ServiceResponse<SkipLogicViewModel> response = new();
            try
            {
                var question = _dbContext.Questions.Include(x=>x.skipParent).Include(x => x.skipChild).FirstOrDefault(x => x.Id == id);
                if (question is null)
                {
                    response.Success = false;
                    response.Message = "Question not found";
                }
                _dbContext.Remove(question);
                _dbContext.SaveChanges();
            }
            catch (Exception e) 
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public ServiceResponse<FormViewModel> getQuestion(int id)
        {
            ServiceResponse<FormViewModel> response = new();
            FormViewModel model = new FormViewModel();
            try
            {
               
                var question = _dbContext.Questions.Include(x=>x.Options).FirstOrDefault(x => x.Id == id);
                question.Options=question.Options.OrderBy(x => x.OrderNumber).ToList();
                model.wasPublished = _dbContext.Forms.FirstOrDefault(x => x.Id == question.Form_Id).WasPublished;
                if (question is null) 
                {
                    response.Success = false;
                    response.Message = "Question Not Found";
                    return response;
                }
                model.Question = question;
                response.Data = model;

            }
            catch (Exception e) 
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public ServiceResponse<bool> UpdateQuestion(Questions question,string[] newoptions)
        {
            ServiceResponse<bool> response = new();
            
            try
            {
                var oldquestion = _dbContext.Questions.Include(x => x.Options).FirstOrDefault(x => x.Id == question.Id);
                if (question is null)
                {
                    response.Success = false;
                    response.Message = "Question Not Found";
                    return response;
                }
                oldquestion.QuestionText = question.QuestionText;
                oldquestion.QuestionDescription = question.QuestionDescription;
                oldquestion.IsRequired = question.IsRequired;
                
                foreach (var option in question.Options) 
                {
                    var oldoption = oldquestion.Options.FirstOrDefault(x => x.Id == option.Id);
                    if (oldoption is null) 
                    {
                        continue;
                    }
                    oldoption.OptionText = option.OptionText;
                }
                var optionids = question.Options.Select(x => x.Id).ToList();
               oldquestion.Options.RemoveAll(x => !optionids.Contains(x.Id));

                if (oldquestion.Field_Type == QType.Select_One || oldquestion.Field_Type == QType.Select_Multiple && newoptions!=null && newoptions.Count() > 0)
                {
                    if (newoptions != null) { 
                   
                    foreach (var newop in newoptions) 
                    {
                        QuestionOptions newoption = new QuestionOptions { OptionText = newop };
                        oldquestion.Options.Add(newoption);
                    }

                    }

                }
                _dbContext.Update(oldquestion);
                _dbContext.SaveChanges();

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

       

        public async Task<TakeSurveyViewModel> StartSurvey(int id)
        {

            TakeSurveyViewModel model = new TakeSurveyViewModel();
            try {
                var questions =await _dbContext.Questions
                                 .Where(x => x.Form_Id == id)
                                 .Include(x => x.Options)
                                 .Include(x => x.skipChild)
                                 .Include(x=>x.skipParent).AsSplitQuery().AsNoTracking().ToListAsync();
                questions.ForEach(x =>x.Options= x.Options.OrderBy(x => x.OrderNumber).ToList());
                
             
                model.Form =  _dbContext.Forms.FirstOrDefault(x => x.Id == id);
                questions = questions.OrderBy(x => x.QuestionOrder).ToList();
                var formview = new List<FormViewModel>();
                questions.ForEach(x =>
                {
                    QuestionDto dto = new QuestionDto();
                    dto.MapPropertys(x);
                    model.Questions.Add(dto);
                    model.SkipLogicLocalDTOs.AddRange(dto.SkipDto);

                });
                model.JsonSkipDest();
            }
            catch (Exception e) 
            {

            }
            return model;
        }

        public async Task<int> Count(int id)
        {
            int next=0;
            try
            {
                 next = await _dbContext.Questions.Where(x => x.Form_Id == id).CountAsync();
                return next;
            }
            catch (Exception e)
            {

            }
            return next;
        }

        public async Task<ServiceResponse<bool>> UpdateOrder(List<Questions> questions,int id)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();
            try
            {
                if (questions == null || questions.Count < 0) 
                {
                    response.Success = false;
                    response.Message = "No Question Order";
                }
                var questionlist = await _dbContext.Questions.Where(x => x.Form_Id == id).ToListAsync();
                foreach (var qu in questionlist) 
                {
                    var newquestion = questions.FirstOrDefault(x => x.Id == qu.Id);
                    if (newquestion is not null) 
                    {
                        qu.QuestionOrder = newquestion.QuestionOrder;
                    }

                }
                _dbContext.UpdateRange(questionlist);
                await _dbContext.SaveChangesAsync();
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
