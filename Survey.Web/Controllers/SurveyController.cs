using Survey.Core.Constants;
using Survey.Core.HelperClasses;
using Survey.Core.Interfaces;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Survey.Infrastructure.Data;
using Survey.Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Survey.Core.Filter;

namespace Survey.Web.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveySubmission _submition;
        private readonly IForms _form;
        private readonly IProjects _projects;
        private readonly IProjectCategory _projectCategory;
        private readonly IQuestions _question;
        private readonly IQuestionOptions _option;
        private readonly IAnswers _answer;
        private readonly ISkipLogic _skipLogic;
        //private readonly ICases _cases;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public SurveyController(ISurveySubmission submition, IForms form, IProjects projects, IProjectCategory projectCategory, IQuestions question, IQuestionOptions option,
          IAnswers answer, ISkipLogic skipLogic, /*ICases cases,*/ IWebHostEnvironment webHostEnvironment,
          UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _submition = submition;
            _form = form;
            _question = question;
            _option = option;
            _answer = answer;
            _skipLogic = skipLogic;
            //_cases = cases;
            _projects = projects;
            _projectCategory = projectCategory;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult EndPointCheck()
        {
            return Ok();
        }


        #region Create Form
        public IActionResult AddForm()
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            var p=_dbContext.Projects.ToList();
            FormViewModel addViewModel = new FormViewModel()
            {
                Form = new Forms(),
                ProjectList=p
                
            };
            return View(addViewModel);
        }
        [HttpGet]
        public IActionResult UpdateForm(int id)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            var p = _dbContext.Projects.ToList();
            FormViewModel addViewModel = new FormViewModel()
            {
                Form = new Forms(),
                ProjectList = p

            };
            return View(addViewModel);
        }
       
        public IActionResult UpdateForm(FormViewModel formViewModel)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {
                _form.UpdateForm(formViewModel.Form);
                return RedirectToAction("ManageForm");
            }
            return View(formViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddForm(FormViewModel formViewModel)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {
                formViewModel.Form.Form_Status = FormStatus.Created;

                _form.Add(formViewModel.Form);
                _form.Save();

                return RedirectToAction("ManageForm");
            }
            return View(formViewModel);
        }
        [Authorize]
        [HttpGet]

        public IActionResult ImportSurvey()
        {
            var model = new FormViewModel();
            var u = _userManager.GetUserAsync(HttpContext.User);


            model.ProjectList = _projects.GetProjects( null, "");
            return View(model);
        }
        [Authorize]
        [HttpGet]

        public async Task<IActionResult> CreateExcelFromSurvey(int id)
        {
            var response = await _form.CreateExcelFromForm(id);
            if (response.Success)
            {
                using (var exportData = new MemoryStream())
                {
                    response.Data.Write(exportData);
                    string saveAsFileName = string.Format("FormExport-{0:d}.xlsx", DateTime.Now).Replace("/", "-");

                    byte[] bytes = exportData.ToArray();
                    return File(bytes, "application/vnd.ms-excel", saveAsFileName);
                }
            }
            return Ok(response); ;
        }
        [Authorize]
        [HttpPost]

        public async Task<IActionResult> CreateSurveyExcel(FormViewModel file)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);

            if (file.Excel == null)
            {
                return BadRequest("Please provide a file");
            }
            if (file.ProjectId == 0)
            {
                return BadRequest("Please select a project");
            }

            var response = await _form.CreateFormFromExcel(file.Excel, file.ProjectId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok();
        }
        #endregion

        #region Delete Form
        public IActionResult DeleteForm(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Forms form = _form.FirstOrDefault(f => f.Id == id);
            if (form == null)
            {
                return NotFound();
            }
            return View(form);
        }

        [HttpPost, ActionName("DeleteForm")]
        [ValidateAntiForgeryToken]

        public IActionResult DeleteExistingForm(int? id)
        {
            /*            var table_checkboxes = _checkBoxAnswers.GetSelectedOptionsByFormId(id);
            */
            var table_answers = _answer.GetAnswersByFormId(id);
            var table_questions = _question.GetQuestionsByFormId(id);

            /* foreach (var option in table_checkboxes)
             {
                 _checkBoxAnswers.Remove(option);
             }
             _checkBoxAnswers.Save();*/

            foreach (var answer in table_answers)
            {
                _answer.Remove(answer);
            }
            _answer.Save();

            foreach (var question in table_questions)
            {
                var table_options = _option.GetOptionsByQuestionId(question.Id);
                var table_skipLogic = _skipLogic.GetSkipLogicByQuestionId(question.Id);

                foreach (var option in table_options)
                {
                    _option.Remove(option);
                }
                foreach (var skiplogic in table_skipLogic)
                {
                    _skipLogic.Remove(skiplogic);
                }

                _question.Remove(question);
            }
            _option.Save();
            _skipLogic.Save();
            _question.Save();

            var objForm = _form.Find(id.GetValueOrDefault());

            if (objForm == null)
            {
                return NotFound();
            }
            _form.Remove(objForm);
            _form.Save();
            TempData[WebConstants.Success] = "Action Completed Successfully !";

            return RedirectToAction("ManageForm");
        }

        #endregion

        #region Manage Form

        public IActionResult ManageForm(FormViewModel model, int pageSize = 10, int pageNumber = 1)
        {
            if (pageNumber < 1) { pageNumber = 1; }
            if (pageSize < 10 || pageSize > 50) { pageSize = 10; }
            var u = _userManager.GetUserAsync(HttpContext.User);


            FormViewModel formViewModel = new FormViewModel()
            {
                //Forms = _form.GetAll().Where(x => x.Tenant_Id == ten.Id)
                Forms = _form.GetForms( model.Filter, pageNumber, pageSize),
                GetProjects = _projects.GetProjects( null, ""),
                GetClients = _projectCategory.GetClients()
            };

            return View(formViewModel);
        }

        #endregion

        #region Deactivate Form

        public IActionResult DeactivateForm(int id)
        {
            var table_form = _form.GetAll();
            foreach (var form in table_form)
            {
                if (form.Id == id)
                {
                    form.Form_Status = FormStatus.Deactive;
                    _form.Update(form);
                }
            }
            _form.Save();
            return RedirectToAction("ManageForm");
        }

        #endregion

        #region Publish Form

        public IActionResult PublishForm(int id)
        {
            var table_form = _form.FirstOrDefault(x => x.Id == id);
            if (table_form is null)
            {
                return NotFound();
            }
            table_form.Form_Status = FormStatus.Active;
            table_form.WasPublished = true;
            _form.Update(table_form);
            _form.Save();
            return RedirectToAction("ManageForm");
        }

        #endregion

        #region Designer
        [HttpGet]

        public IActionResult DeleteQuestion(int id)
        {
            var response = _question.DeleteQuestion(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok();
        }


        public IActionResult EditQuestion(int id)
        {
            FormViewModel model = new();
            var response = _question.getQuestion(id);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            model = response.Data;
            return View(model);
        }

        public IActionResult updateQuestion(FormViewModel model)
        {
            var response = _question.UpdateQuestion(model.Question, model.otherOptions);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return RedirectToAction("Designer", new { id = model.Question.Form_Id });
        }
        [HttpGet]

        public IActionResult Designer(int id)
        {
            FormViewModel formViewModel = new FormViewModel()
            {
                Form = new Forms(),
                //FieldTypesList = _fieldTypes.GetAllFieldTypes(),
                Question = new Questions(),
                formid = id,
                OriginalQuestion = _question.GetQuestions(id)


            };
            formViewModel.OriginalQuestion.ForEach(x => x.Options = x.Options.OrderBy(x => x.OrderNumber).ToList());
            return View(formViewModel);
        }

        [HttpPost]

        public async Task<IActionResult> OrderQuestions(FormViewModel model)
        {
            var response = await _question.UpdateOrder(model.OriginalQuestion, model.formid);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Designer(FormViewModel formViewModel, QType FieldType, string _isRequired)
        {
            bool isRequired = false;
            //if (ModelState.IsValid)
            //{
            if (!string.IsNullOrEmpty(_isRequired) && _isRequired.Equals("on"))
            {
                isRequired = true;
            }
            if (formViewModel.Question.Field_Type == QType.Select_Multiple || formViewModel.Question.Field_Type == QType.Select_One)
            {
                if (formViewModel.otherOptions is not null)
                {
                    foreach (var op in formViewModel.otherOptions)
                    {
                        if (string.IsNullOrWhiteSpace(op))
                        {
                            return BadRequest("You can't have an empty option");
                        }
                    }
                    if (string.IsNullOrWhiteSpace(formViewModel.option.OptionText))
                    {
                        return BadRequest("You can't have an empty option");
                    }
                }
            }

            try
            {
                formViewModel.Question.Id = 0;
                formViewModel.Question.Form_Id = formViewModel.formid;
                formViewModel.Question.Field_Type = FieldType;
                formViewModel.Question.IsRequired = isRequired;
                formViewModel.Question.Skip_Logic = false;
                formViewModel.Question.QuestionOrder = await _question.Count(formViewModel.formid);

                if (string.IsNullOrWhiteSpace(formViewModel.Question.QuestionText))
                {
                    return BadRequest("Please insert the question label");
                }
                _question.Add(formViewModel.Question);
                _question.Save();

                if (FieldType == QType.Select_Multiple || FieldType == QType.Select_One)
                {
                    formViewModel.Option.Id = 0;
                    formViewModel.Option.Question_Id = formViewModel.Question.Id;

                    _option.Add(formViewModel.Option);
                    _option.Save();
                    if (formViewModel.otherOptions != null)
                    {
                        for (int position = 0; position < formViewModel.otherOptions.Length; position++)
                        {
                            formViewModel.Option.Id = 0;
                            formViewModel.Option.Question_Id = formViewModel.Question.Id;
                            formViewModel.Option.OptionText = formViewModel.otherOptions[position];
                            _option.Add(formViewModel.Option);
                            _option.Save();
                        }
                    }
                }
                //    return RedirectToAction("ManageForm");
                //}
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        #endregion


        #region Skip Logic

        public IActionResult SkipLogic(int id)
        {
            int question_formId = _question.GetQuestionsFormId(id);

            var ParentQuestionsList = _question.ParentQuestions(id, question_formId);
            FormViewModel formViewModel = new();
            formViewModel.SkipLogicList = _skipLogic.GetSkipLogicByQuestionwithparentId(id).ToList();
            formViewModel.Question = _question.GetOldQuestion(id);
            foreach (var skip in formViewModel.SkipLogicList)
            {
                var op = Operator.getOneOperator(skip.Operator);
                skip.operator_string = $"{op.OperatorText} ({op.OperatorSymbol}) ";
            }
            formViewModel.ParentQuestionsList = ParentQuestionsList;
            formViewModel.formid = question_formId;
            return View(formViewModel);
        }

        public IActionResult RefreshOptions(int id)
        {
            FormViewModel formViewModel = _option.GetConditionOptions(id);
            // Operator = SkipLogicOperator.

            return PartialView(formViewModel);
        }

        [HttpPost]
        public IActionResult SkipLogic(FormViewModel formViewModel, int id, int selectedOperator, int? selectedCondition)
        {
            FormViewModel skipLogicViewModel = new FormViewModel()
            {
                SkipLogic = new SkipLogic(),
            };
            if (formViewModel.Question.Id == 0)
            {
                return BadRequest("Please Select A Parent Question");
            }
            if (selectedOperator == 0)
            {
                return BadRequest("Please Select An Operator For The Skip Logic");
            }


            try
            {

                int questionType = _question.GetQuestionType(formViewModel.Question.Id);
                if ((questionType == Convert.ToInt32(QType.Select_One) || questionType == Convert.ToInt32(QType.Select_Multiple)) && selectedCondition == null)
                {
                    return BadRequest("Please Select An Option For The Skip Logic");
                }
                else if (formViewModel.SkipLogic.Comparable_Value == null)
                {
                    return BadRequest("Please Select A Number To Compare For The Skip Logic");

                }
                if (questionType != 0)
                {
                    skipLogicViewModel.SkipLogic.Parent_Question_Id = formViewModel.Question.Id;
                    skipLogicViewModel.SkipLogic.Child_Question_Id = id;
                    skipLogicViewModel.SkipLogic.Operator = selectedOperator;

                    if (questionType == Convert.ToInt32(QType.Select_One) || questionType == Convert.ToInt32(QType.Select_Multiple))
                    {
                        skipLogicViewModel.SkipLogic.Condition_Option = selectedCondition;
                    }
                    else
                    {
                        skipLogicViewModel.SkipLogic.Comparable_Value = formViewModel.SkipLogic.Comparable_Value;
                    }
                    _skipLogic.Add(skipLogicViewModel.SkipLogic);
                    _skipLogic.Save();
                }

                var question = _dbContext.Questions.FirstOrDefault(x => x.Id == id);
                question.SkipLogicType = formViewModel.Question.SkipLogicType;

                _dbContext.Update(question);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        #endregion

        #region Delete Skip Logic

        public IActionResult DeleteSkipLogic(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            SkipLogic skipLogic = _skipLogic.FirstOrDefault(s => s.Id == id);
            if (skipLogic == null)
            {
                return NotFound();
            }

            return View(skipLogic);
        }

        [ValidateAntiForgeryToken]
        public IActionResult DeleteExistingSkipLogic(SkipLogic skip)
        {
            var skipLogic = _skipLogic.FirstOrDefault(x => x.Id == skip.Id);
            if (skipLogic == null)
            {
                return NotFound();
            }
            _skipLogic.Remove(skipLogic);
            _skipLogic.Save();
            TempData[WebConstants.Success] = "Action Completed Successfully !";

            return RedirectToAction("SkipLogic", new { id = skipLogic.Child_Question_Id });
        }

        #endregion

        #region Test / Submit Survey
        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> Survey(int id, [FromQuery] string SAgTRid, [FromQuery] int? caseId)
        {
            if (SAgTRid == null || !_dbContext.IdentityUsers.Any(x => x.Id == SAgTRid))
            {
                return NotFound();
            }
            var testModel = await _question.StartSurvey(id);
            testModel.formid = id;
            if (SAgTRid is null)
            {
                testModel.isTest = true;
                return View(testModel);
            }
            var u = _dbContext.Users.FirstOrDefault(x => x.Id == SAgTRid);
            ViewData["Agent"] = SAgTRid;
            ViewData["Case"] = caseId;

            return View(testModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> Survey(TakeSurveyViewModel skipLogicModel, int id, string SAgTRid, int? caseId)
        {
            var model = skipLogicModel.Questions;
            //if (ModelState.IsValid)
            //{
            var ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
            var submition = new SurveySubmission();
            submition.AgentId = skipLogicModel.SAgTRid;
            submition.StartTime = skipLogicModel.StartDate;
            submition.EndTime = skipLogicModel.EndTime;
            submition.FormId = skipLogicModel.formid;
            if (skipLogicModel.SAgTRid == null || !_dbContext.IdentityUsers.Any(x => x.Id == skipLogicModel.SAgTRid))
            {
                return NotFound();
            }
            for (int i = 0; i < model.Count; i++)
            {
                if (model[i].Field_Type == QType.Text || model[i].Field_Type == QType.Numbers || model[i].Field_Type == QType.Date_Time
                    || model[i].Field_Type == QType.Decimal)
                {
                    if (model[i].Answer != null)
                    {
                        var userAnswer = new Answers
                        {
                            Form_Id = id,
                            Question_Id = model[i].Id,
                            Answer = model[i].Answer
                        };
                        submition.Answers.Add(userAnswer);
                    }
                }

                else if (model[i].Field_Type == QType.Select_One)
                {
                    if (model[i].selected_option != 0)
                    {
                        var userAnswer = new Answers
                        {
                            Form_Id = id,
                            Question_Id = model[i].Id,
                            OptionId = model[i].selected_option
                        };
                        submition.Answers.Add(userAnswer);
                    }
                }

                else if (model[i].Field_Type == QType.Select_Multiple)
                {
                    for (int j = 0; j < model[i].Options.Count; j++)
                    {
                        if (model[i].Options[j].isSelected)
                        {
                            var userAnswer = new Answers
                            {
                                Form_Id = id,
                                Question_Id = model[i].Id,
                                OptionId = model[i].Options[j].Id
                            };
                            submition.Answers.Add(userAnswer);
                        }
                    }
                }

            }

            //if (caseId != null)
            //{
            //    var _case = _dbContext.Cases.Where(x => x.Id == Convert.ToInt32(caseId)).FirstOrDefault();

            //    _case.Status = CaseStatus.Completed;
            //    _cases.Update(_case);
            //    _cases.Save();
            //    submition.CaseId = caseId;
            //}

            var response = await _submition.saveSubmission(submition);
            if (!response.Success) { return BadRequest(response.Message); }
            return Ok("Data Saved");
        }
        #endregion

        #region Submitted Notification
        [AllowAnonymous]

        public IActionResult SuccessfullySubmitted()
        {
            return View();
        }

        #endregion

        #region Collect

        public IActionResult Collect(int id)
        {
            var uId = _dbContext.Users.FirstOrDefault(x => x.UserName == User.Identity.Name).Id;
            //var u = DateTime.Now.Ticks;
            FormViewModel viewModel = new FormViewModel()
            {
                SurveyLink = Request.Scheme + "://" + HttpContext.Request.Host + "/Survey/Survey/" + id +"?SAgTRid=" + uId
           };
            return View(viewModel);
        }

        #endregion

      
    }

}