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
        private readonly ICases _cases;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public SurveyController(ISurveySubmission submition, IForms form, IProjects projects, IProjectCategory projectCategory, IQuestions question, IQuestionOptions option,
          IAnswers answer, ISkipLogic skipLogic, ICases cases, IWebHostEnvironment webHostEnvironment,
          UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _submition = submition;
            _form = form;
            _question = question;
            _option = option;
            _answer = answer;
            _skipLogic = skipLogic;
            _cases = cases;
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
        [Authorize(Permissions.PremissionList.Survey_Create)]
        public IActionResult AddForm()
        {
            var u = _userManager.GetUserAsync(HttpContext.User);

            FormViewModel addViewModel = new FormViewModel()
            {
                Form = new Forms(),
                FormTypesList = _form.GetAllDropdownList(WebConstants.FormType)
            };
            return View(addViewModel);
        }
        [Authorize(Permissions.PremissionList.Survey_Update)]
        [HttpGet]
        public IActionResult UpdateForm(int id)
        {
            var u = _userManager.GetUserAsync(HttpContext.User);

            FormViewModel addViewModel = new FormViewModel()
            {
                Form = _form.GetForm(id),
                FormTypesList = _form.GetAllDropdownList(WebConstants.FormType)
            };
            return View(addViewModel);
        }
        [Authorize(Permissions.PremissionList.Survey_Update)]
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
        //[Authorize(Permissions.PremissionList.Survey_Create)]

        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public IActionResult AddForm(FormViewModel formViewModel, FormType selectedFormTypeId)
        //        {
        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            if (ModelState.IsValid)
        //            {
        //                //formViewModel.Form.Id = 0;
        //                formViewModel.Form.FormType_Id = selectedFormTypeId;
        //                formViewModel.Form.Form_Status = FormStatus.Created;

        //                _form.Add(formViewModel.Form);
        //                _form.Save();

        //                formViewModel.Language.FormId = formViewModel.Form.Id;
        //                formViewModel.Language.Tenant_Id = ten.Id;

        //                _language.Add(formViewModel.Language);
        //                _language.Save();

        //                return RedirectToAction("ManageForm");
        //            }
        //            return View(formViewModel);
        //        }
        //        [Authorize]
        //        [HttpGet]
        //        [Authorize(Permissions.PremissionList.Survey_Create)]

        //        public IActionResult ImportSurvey()
        //        {
        //            var model = new FormViewModel();
        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.Result.TenantId);
        //            var get_enrolled_clients = _dbContext.AssignUsersToClients.Where(x => x.UserId == u.Result.Id).Select(x => x.ClientId).ToList();
        //            var get_enrolled_projects_separately = _dbContext.AssignProjectsToUsers.Where(x => x.UserId == u.Result.Id).Select(x => x.ProjectId).ToList();


        //            model.ProjectList = _projects.GetProjects(get_enrolled_clients, get_enrolled_projects_separately,null,"");
        //            return View(model);
        //        }
        //        [Authorize]
        //        [HttpGet]
        //        [Authorize(Permissions.PremissionList.Survey_Create)]

        //        public async Task<IActionResult> CreateExcelFromSurvey(int id)
        //        {
        //            var response = await _form.CreateExcelFromForm(id);
        //            if (response.Success)
        //            {
        //                using (var exportData = new MemoryStream())
        //                {
        //                    response.Data.Write(exportData);
        //                    string saveAsFileName = string.Format("FormExport-{0:d}.xlsx", DateTime.Now).Replace("/", "-");

        //                    byte[] bytes = exportData.ToArray();
        //                    return File(bytes, "application/vnd.ms-excel", saveAsFileName);
        //                }
        //            }
        //            return Ok(response); ;
        //        }
        //        [Authorize]
        //        [HttpPost]
        //        [Authorize(Permissions.PremissionList.Survey_Create)]

        //        public async Task<IActionResult> CreateSurveyExcel(FormViewModel file)
        //        {
        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.Result.TenantId);

        //            if (file.Excel == null)
        //            {
        //                return BadRequest("Please provide a file");
        //            }
        //            if (file.ProjectId == 0)
        //            {
        //                return BadRequest("Please select a project");
        //            }

        //            var response = await _form.CreateFormFromExcel(file.Excel, ten.Id, file.ProjectId);
        //            if (!response.Success)
        //            {
        //                return BadRequest(response.Message);
        //            }

        //            return Ok();
        //        }
        //        #endregion

        //        #region Delete Form
        //        [Authorize(Permissions.PremissionList.Survey_Delete)]
        //        public IActionResult DeleteForm(int? id)
        //        {
        //            if (id == null || id == 0)
        //            {
        //                return NotFound();
        //            }

        //            Forms form = _form.FirstOrDefault(f => f.Id == id);
        //            if (form == null)
        //            {
        //                return NotFound();
        //            }
        //            return View(form);
        //        }

        //        [HttpPost, ActionName("DeleteForm")]
        //        [ValidateAntiForgeryToken]
        //        [Authorize(Permissions.PremissionList.Survey_Delete)]

        //        public IActionResult DeleteExistingForm(int? id)
        //        {
        //            /*            var table_checkboxes = _checkBoxAnswers.GetSelectedOptionsByFormId(id);
        //            */
        //            var table_answers = _answer.GetAnswersByFormId(id);
        //            var table_questions = _question.GetQuestionsByFormId(id);

        //            /* foreach (var option in table_checkboxes)
        //             {
        //                 _checkBoxAnswers.Remove(option);
        //             }
        //             _checkBoxAnswers.Save();*/

        //            foreach (var answer in table_answers)
        //            {
        //                _answer.Remove(answer);
        //            }
        //            _answer.Save();

        //            foreach (var question in table_questions)
        //            {
        //                var table_options = _option.GetOptionsByQuestionId(question.Id);
        //                var table_skipLogic = _skipLogic.GetSkipLogicByQuestionId(question.Id);

        //                foreach (var option in table_options)
        //                {
        //                    _option.Remove(option);
        //                }
        //                foreach (var skiplogic in table_skipLogic)
        //                {
        //                    _skipLogic.Remove(skiplogic);
        //                }

        //                _question.Remove(question);
        //            }
        //            _option.Save();
        //            _skipLogic.Save();
        //            _question.Save();

        //            var objForm = _form.Find(id.GetValueOrDefault());

        //            if (objForm == null)
        //            {
        //                return NotFound();
        //            }
        //            _form.Remove(objForm);
        //            _form.Save();
        //            TempData[WebConstants.Success] = "Action Completed Successfully !";

        //            return RedirectToAction("ManageForm");
        //        }

        //        #endregion

        //        #region Manage Form

        //        [Authorize(Permissions.PremissionList.Survey_View)]
        //        public IActionResult ManageForm(FormViewModel model, int pageSize = 10, int pageNumber = 1)
        //        {
        //            if (pageNumber < 1) { pageNumber = 1; }
        //            if (pageSize < 10 || pageSize > 50) { pageSize = 10; }
        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.Result.TenantId);
        //            var get_enrolled_clients = _dbContext.AssignUsersToClients.Where(x => x.UserId == u.Result.Id).Select(x => x.ClientId).ToList();
        //            var get_enrolled_projects_separately = _dbContext.AssignProjectsToUsers.Where(x => x.UserId == u.Result.Id).Select(x => x.ProjectId).ToList();

        //            FormViewModel formViewModel = new FormViewModel()
        //            {
        //                //Forms = _form.GetAll().Where(x => x.Tenant_Id == ten.Id)
        //                Forms = _form.GetForms(get_enrolled_clients, get_enrolled_projects_separately,model.Filter, pageNumber, pageSize),
        //                GetProjects = _projects.GetProjects(get_enrolled_clients, get_enrolled_projects_separately,null,""),
        //                GetClients = _projectCategory.GetClients(get_enrolled_clients, get_enrolled_projects_separately)
        //            };

        //            return View(formViewModel);
        //        }

        //        #endregion

        //        #region Deactivate Form
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult DeactivateForm(int id)
        //        {
        //            var table_form = _form.GetAll();
        //            foreach (var form in table_form)
        //            {
        //                if (form.Id == id)
        //                {
        //                    form.Form_Status = FormStatus.Deactive;
        //                    _form.Update(form);
        //                }
        //            }
        //            _form.Save();
        //            return RedirectToAction("ManageForm");
        //        }

        //        #endregion

        //        #region Publish Form
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult PublishForm(int id)
        //        {
        //            var table_form = _form.FirstOrDefault(x => x.Id == id);
        //            if (table_form is null)
        //            {
        //                return NotFound();
        //            }
        //            table_form.Form_Status = FormStatus.Active;
        //            table_form.WasPublished = true;
        //            _form.Update(table_form);
        //            _form.Save();
        //            return RedirectToAction("ManageForm");
        //        }

        //        #endregion

        //        #region Designer
        //        [HttpGet]
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult DeleteQuestion(int id)
        //        {
        //            var response = _question.DeleteQuestion(id);
        //            if (!response.Success)
        //            {
        //                return BadRequest(response.Message);
        //            }
        //            return Ok();
        //        }
        //        [Authorize(Permissions.PremissionList.Survey_Update)]


        //        public IActionResult EditQuestion(int id)
        //        {
        //            FormViewModel model = new();
        //            var response = _question.getQuestion(id);
        //            if (!response.Success)
        //            {
        //                return BadRequest(response.Message);
        //            }
        //            model = response.Data;
        //            return View(model);
        //        }
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult updateQuestion(FormViewModel model)
        //        {
        //            var response = _question.UpdateQuestion(model.Question, model.otherOptions);
        //            if (!response.Success)
        //            {
        //                return BadRequest(response.Message);
        //            }

        //            return RedirectToAction("Designer", new { id = model.Question.Form_Id });
        //        }
        //        [HttpGet]
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult Designer(int id)
        //        {
        //            FormViewModel formViewModel = new FormViewModel()
        //            {
        //                Form = new Forms(),
        //                //FieldTypesList = _fieldTypes.GetAllFieldTypes(),
        //                Question = new Questions(),
        //                formid = id,
        //                OriginalQuestion = _question.GetQuestions(id)


        //            };
        //            formViewModel.OriginalQuestion.ForEach(x => x.Options = x.Options.OrderBy(x => x.OrderNumber).ToList());
        //            return View(formViewModel);
        //        }

        //        [HttpPost]
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public async Task<IActionResult> OrderQuestions(FormViewModel model)
        //        {
        //            var response = await _question.UpdateOrder(model.OriginalQuestion, model.formid);
        //            if (!response.Success)
        //            {
        //                return BadRequest(response.Message);
        //            }
        //            return Ok();
        //        }

        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public async Task<IActionResult> Designer(FormViewModel formViewModel, QType FieldType, string _isRequired, int language)
        //        {
        //            bool isRequired = false;
        //            //if (ModelState.IsValid)
        //            //{
        //            if (!string.IsNullOrEmpty(_isRequired) && _isRequired.Equals("on"))
        //            {
        //                isRequired = true;
        //            }
        //            if (formViewModel.Question.Field_Type == QType.Select_Multiple || formViewModel.Question.Field_Type == QType.Select_One)
        //            {
        //                if (formViewModel.otherOptions is not null)
        //                {
        //                    foreach (var op in formViewModel.otherOptions)
        //                    {
        //                        if (string.IsNullOrWhiteSpace(op))
        //                        {
        //                            return BadRequest("You can't have an empty option");
        //                        }
        //                    }
        //                    if (string.IsNullOrWhiteSpace(formViewModel.option.OptionText))
        //                    {
        //                        return BadRequest("You can't have an empty option");
        //                    }
        //                }
        //            }

        //            try
        //            {
        //                formViewModel.Question.Id = 0;
        //                formViewModel.Question.Form_Id = formViewModel.formid;
        //                formViewModel.Question.Field_Type = FieldType;
        //                formViewModel.Question.IsRequired = isRequired;
        //                formViewModel.Question.Skip_Logic = false;
        //                formViewModel.Question.QuestionOrder = await _question.Count(formViewModel.formid);

        //                if (string.IsNullOrWhiteSpace(formViewModel.Question.QuestionText))
        //                {
        //                    return BadRequest("Please insert the question label");
        //                }
        //                _question.Add(formViewModel.Question);
        //                _question.Save();

        //                if (FieldType == QType.Select_Multiple || FieldType == QType.Select_One)
        //                {
        //                    formViewModel.Option.Id = 0;
        //                    formViewModel.Option.Question_Id = formViewModel.Question.Id;

        //                    _option.Add(formViewModel.Option);
        //                    _option.Save();
        //                    if (formViewModel.otherOptions != null)
        //                    {
        //                        for (int position = 0; position < formViewModel.otherOptions.Length; position++)
        //                        {
        //                            formViewModel.Option.Id = 0;
        //                            formViewModel.Option.Question_Id = formViewModel.Question.Id;
        //                            formViewModel.Option.OptionText = formViewModel.otherOptions[position];
        //                            _option.Add(formViewModel.Option);
        //                            _option.Save();
        //                        }
        //                    }
        //                }
        //                //    return RedirectToAction("ManageForm");
        //                //}
        //            }
        //            catch (Exception e)
        //            {
        //                return BadRequest(e.Message);
        //            }
        //            return Ok();
        //        }

        //        #endregion

        //        #region Manage Languages

        //        public IActionResult ManageLanguages(int id)
        //        {
        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.Result.TenantId);
        //            FormViewModel formViewModel = new FormViewModel()
        //            {
        //                Languages = _language.GetLanguages(id, ten.Id)
        //            };
        //            return View(formViewModel);
        //        }

        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public IActionResult ManageLanguages(FormViewModel formViewModel, int id)
        //        {
        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.Result.TenantId);
        //            if (ModelState.IsValid)
        //            {
        //                formViewModel.Language.Tenant_Id = ten.Id;
        //                formViewModel.Language.FormId = id;
        //                _language.Add(formViewModel.Language);
        //                _language.Save();
        //            }
        //            return RedirectToAction("ManageLanguages");
        //        }

        //        #endregion

        //        #region Delete Language
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult DeleteLanguage(int? id)
        //        {
        //            if (id == null || id == 0)
        //            {
        //                return NotFound();
        //            }

        //            Language language = _language.FirstOrDefault(f => f.Id == id);
        //            if (language == null)
        //            {
        //                return NotFound();
        //            }
        //            return View(language);
        //        }
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        [HttpPost, ActionName("DeleteLanguage")]
        //        [ValidateAntiForgeryToken]
        //        public IActionResult DeleteExistingLanguage(int? id)
        //        {
        //            var language = _language.Find(id.GetValueOrDefault());

        //            if (language == null)
        //            {
        //                return NotFound();
        //            }
        //            _language.Remove(language);
        //            _language.Save();
        //            TempData[WebConstants.Success] = "Action Completed Successfully !";

        //            return RedirectToAction("ManageForm");
        //        }

        //        #endregion

        //        #region Translate Questions
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult UpdateTranslation(FormViewModel model)
        //        {
        //            var response = _translateQuestion.UpdateTranslation(model.QuestionTranslation, model.optionTranslations);
        //            if (!response.Success)
        //            {
        //                return BadRequest(response.Message);
        //            }
        //            return RedirectToAction("TranslateQuestions", new { id = model.QuestionTranslation.Parent_Question_Id });
        //        }
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult DeleteTranslation(int id)
        //        {
        //            var response = _translateQuestion.DeleteTranslation(id);
        //            if (!response.Success)
        //            {
        //                return BadRequest(response.Message);
        //            }
        //            return RedirectToAction("TranslateQuestions", new { id = response.Data });
        //        }
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult TranslateQuestions(int id)
        //        {

        //            FormViewModel model = new FormViewModel();
        //            var response = _translateQuestion.GetTranslation(id);
        //            if (!response.Success)
        //            {
        //                return BadRequest(response.Message);
        //            }
        //            model = response.Data;
        //            return View(model);
        //        }
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public IActionResult TranslateQuestions(FormViewModel formViewModel, string[] otherOptions)
        //        {
        //            int formId = _question.GetQuestionsFormId(formViewModel.QuestionId);
        //            int translatedQuestionId;
        //            if (formViewModel.Language == null)
        //            {
        //                return BadRequest("Please Select A Language ");
        //            }
        //            else if (0 == formViewModel.QuestionId) { return BadRequest("Make Sure Your Editing The Right Question"); }
        //            else if (string.IsNullOrWhiteSpace(formViewModel.QuestionTranslation.Translation)) { return BadRequest("You can't add an empty transaltion"); }

        //            formViewModel.QuestionTranslation.Parent_Question_Id = formViewModel.QuestionId;
        //            formViewModel.QuestionTranslation.Form_Id = formId;
        //            formViewModel.QuestionTranslation.LanguageId = formViewModel.Language.Id;

        //            _questionTranslations.Add(formViewModel.QuestionTranslation);
        //            _questionTranslations.Save();

        //            if (formViewModel.AddedTranslations != null)
        //            {
        //                translatedQuestionId = _questionTranslations.GetTranslatedQuestionId(formViewModel.QuestionId, formViewModel.Language.Id);
        //                for (int position = 0; position < formViewModel.AddedTranslations.Count; position++)
        //                {

        //                    formViewModel.AddedTranslations[position].Translated_Question_Id = translatedQuestionId;
        //                    formViewModel.AddedTranslations[position].Form_Id = formId;
        //                    formViewModel.AddedTranslations[position].LanguageId = formViewModel.Language.Id;


        //                    _optionTranslations.Add(formViewModel.AddedTranslations[position]);

        //                }
        //                _optionTranslations.Save();
        //            }


        //            return Ok();
        //        }

        //        #endregion

        //        #region Skip Logic
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult SkipLogic(int id)
        //        {
        //            int question_formId = _question.GetQuestionsFormId(id);

        //            var ParentQuestionsList = _question.ParentQuestions(id, question_formId);
        //            FormViewModel formViewModel = new();
        //            formViewModel.SkipLogicList = _skipLogic.GetSkipLogicByQuestionwithparentId(id).ToList();
        //            formViewModel.Question = _question.GetOldQuestion(id);
        //            foreach (var skip in formViewModel.SkipLogicList)
        //            {
        //                var op = Operator.getOneOperator(skip.Operator);
        //                skip.operator_string = $"{op.OperatorText} ({op.OperatorSymbol}) ";
        //            }
        //            formViewModel.ParentQuestionsList = ParentQuestionsList;
        //            formViewModel.formid = question_formId;
        //            return View(formViewModel);
        //        }
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult RefreshOptions(int id)
        //        {
        //            FormViewModel formViewModel = _option.GetConditionOptions(id);
        //            // Operator = SkipLogicOperator.

        //            return PartialView(formViewModel);
        //        }

        //        [Authorize(Permissions.PremissionList.Survey_Update)]
        //        [HttpPost]
        //        public IActionResult SkipLogic(FormViewModel formViewModel, int id, int selectedOperator, int? selectedCondition)
        //        {
        //            FormViewModel skipLogicViewModel = new FormViewModel()
        //            {
        //                SkipLogic = new SkipLogic(),
        //            };
        //            if (formViewModel.Question.Id == 0)
        //            {
        //                return BadRequest("Please Select A Parent Question");
        //            }
        //            if (selectedOperator == 0)
        //            {
        //                return BadRequest("Please Select An Operator For The Skip Logic");
        //            }


        //            try
        //            {

        //                int questionType = _question.GetQuestionType(formViewModel.Question.Id);
        //                if ((questionType == Convert.ToInt32(QType.Select_One) || questionType == Convert.ToInt32(QType.Select_Multiple)) && selectedCondition == null)
        //                {
        //                    return BadRequest("Please Select An Option For The Skip Logic");
        //                }
        //                else if(formViewModel.SkipLogic.Comparable_Value==null) {
        //                    return BadRequest("Please Select A Number To Compare For The Skip Logic");

        //                }
        //                if (questionType != 0)
        //                {
        //                    skipLogicViewModel.SkipLogic.Parent_Question_Id = formViewModel.Question.Id;
        //                    skipLogicViewModel.SkipLogic.Child_Question_Id = id;
        //                    skipLogicViewModel.SkipLogic.Operator = selectedOperator;

        //                    if (questionType == Convert.ToInt32(QType.Select_One) || questionType == Convert.ToInt32(QType.Select_Multiple))
        //                    {
        //                        skipLogicViewModel.SkipLogic.Condition_Option = selectedCondition;
        //                    }
        //                    else
        //                    {
        //                        skipLogicViewModel.SkipLogic.Comparable_Value = formViewModel.SkipLogic.Comparable_Value;
        //                    }
        //                    _skipLogic.Add(skipLogicViewModel.SkipLogic);
        //                    _skipLogic.Save();
        //                }

        //                var question = _dbContext.Questions.FirstOrDefault(x => x.Id == id);
        //                question.SkipLogicType = formViewModel.Question.SkipLogicType;

        //                _dbContext.Update(question);
        //                _dbContext.SaveChanges();
        //            }
        //            catch (Exception e)
        //            {
        //                return BadRequest(e.Message);
        //            }
        //            return Ok();
        //        }

        //        #endregion

        //        #region Delete Skip Logic
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        public IActionResult DeleteSkipLogic(int? id)
        //        {
        //            if (id == null || id == 0)
        //            {
        //                return NotFound();
        //            }

        //            SkipLogic skipLogic = _skipLogic.FirstOrDefault(s => s.Id == id);
        //            if (skipLogic == null)
        //            {
        //                return NotFound();
        //            }

        //            return View(skipLogic);
        //        }
        //        [Authorize(Permissions.PremissionList.Survey_Update)]

        //        [ValidateAntiForgeryToken]
        //        public IActionResult DeleteExistingSkipLogic(SkipLogic skip)
        //        {
        //            var skipLogic = _skipLogic.FirstOrDefault(x => x.Id == skip.Id);
        //            if (skipLogic == null)
        //            {
        //                return NotFound();
        //            }
        //            _skipLogic.Remove(skipLogic);
        //            _skipLogic.Save();
        //            TempData[WebConstants.Success] = "Action Completed Successfully !";

        //            return RedirectToAction("SkipLogic", new { id = skipLogic.Child_Question_Id });
        //        }

        //        #endregion

        //        #region Test / Submit Survey
        //        [HttpGet]
        //        [AllowAnonymous]

        //        public async Task<IActionResult> Survey(int id, [FromQuery] string SAgTRid, [FromQuery] int? caseId)
        //        {
        //            if (SAgTRid == null || !_dbContext.ApplicationUsers.Any(x => x.Id == SAgTRid))
        //            {
        //                return NotFound();
        //            }
        //            var testModel = await _question.StartSurvey(id);
        //            testModel.formid = id;
        //            testModel.LanguagesList = _language.GetLanguagesList(id);
        //            if (SAgTRid is null)
        //            {
        //                testModel.isTest = true;
        //                return View(testModel);
        //            }
        //            var u = _dbContext.Users.FirstOrDefault(x => x.Id == SAgTRid);
        //            ViewData["Agent"] = SAgTRid;
        //            ViewData["Case"] = caseId;
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.TenantId);

        //            return View(testModel);
        //        }

        //        [HttpPost]
        //        //[ValidateAntiForgeryToken]
        //        [AllowAnonymous]

        //        public async Task<IActionResult> Survey(TakeSurveyViewModel skipLogicModel, int id, string SAgTRid, int? caseId)
        //        {
        //            var model = skipLogicModel.Questions;
        //            //if (ModelState.IsValid)
        //            //{
        //            var ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
        //            var submition = new SurveySubmission();
        //            submition.AgentId = skipLogicModel.SAgTRid;
        //            submition.StartTime = skipLogicModel.StartDate;
        //            submition.EndTime = skipLogicModel.EndTime;
        //            submition.FormId = skipLogicModel.formid;
        //            if (skipLogicModel.SAgTRid==null || !_dbContext.ApplicationUsers.Any(x=>x.Id== skipLogicModel.SAgTRid)) 
        //            {
        //                return NotFound();
        //            }
        //            for (int i = 0; i < model.Count; i++)
        //            {
        //                if (model[i].Field_Type == QType.Geo || model[i].Field_Type == QType.Text || model[i].Field_Type == QType.Numbers || model[i].Field_Type == QType.Phone_Number || model[i].Field_Type == QType.Date_Time || model[i].Field_Type == QType.Integer || model[i].Field_Type == QType.Time || model[i].Field_Type == QType.Decimal || model[i].Field_Type == QType.Date || model[i].Field_Type == QType.Note || model[i].Field_Type == QType.HTML_Code)
        //                {
        //                    if (model[i].Answer != null)
        //                    {
        //                        var userAnswer = new Answers
        //                        {
        //                            Form_Id = id,
        //                            Question_Id = model[i].Id,
        //                            Answer = model[i].Answer
        //                        };
        //                        submition.Answers.Add(userAnswer);
        //                    }
        //                }
        //                else if (model[i].Field_Type == QType.Geo)
        //                {
        //                    if (model[i].Answer != null)
        //                    {
        //                        var userGeoLocation = new Answers
        //                        {
        //                            Form_Id = id,
        //                            Question_Id = model[i].Id,
        //                            Answer = model[i].Answer
        //                        };
        //                        submition.Answers.Add(userGeoLocation);
        //                    }
        //                }
        //                else if (model[i].Field_Type == QType.Select_One)
        //                {
        //                    if (model[i].selected_option != 0)
        //                    {
        //                        var userAnswer = new Answers
        //                        {
        //                            Form_Id = id,
        //                            Question_Id = model[i].Id,
        //                            OptionId = model[i].selected_option
        //                        };
        //                        submition.Answers.Add(userAnswer);
        //                    }
        //                }

        //                else if (model[i].Field_Type == QType.Select_Multiple)
        //                {
        //                    for (int j = 0; j < model[i].Options.Count; j++)
        //                    {
        //                        if (model[i].Options[j].isSelected)
        //                        {
        //                            var userAnswer = new Answers
        //                            {
        //                                Form_Id = id,
        //                                Question_Id = model[i].Id,
        //                                OptionId = model[i].Options[j].Id
        //                            };
        //                            submition.Answers.Add(userAnswer);
        //                        }
        //                    }
        //                }
        //                else if (model[i].Field_Type == QType.Image)
        //                {
        //                    var files = model[i].File;
        //                    if (files.Count > 0)
        //                    {
        //                        if (files[0].Length / 1048576 > 10)
        //                        {
        //                            return BadRequest("Your file is larger than 10MB");
        //                        }
        //                        string webRootPath = _webHostEnvironment.WebRootPath;

        //                        string upload = webRootPath + WebConstants.SurveyImagePath;
        //                        string fileName = Guid.NewGuid().ToString();

        //                        string extension = Path.GetExtension(files[0].FileName);

        //                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
        //                        {
        //                            files[0].CopyTo(fileStream);
        //                        }
        //                        var image = new Answers
        //                        {
        //                            Form_Id = id,
        //                            Question_Id = model[i].Id,
        //                            Answer = fileName + extension
        //                        };

        //                        submition.Answers.Add(image);
        //                    }

        //                }
        //                else if (model[i].Field_Type == QType.File)
        //                {
        //                    var files = model[i].File;
        //                    if (files.Count > 0)
        //                    {
        //                        if (files[0].Length / 1048576 > 10)
        //                        {
        //                            return BadRequest("Your file is larger than 10MB");
        //                        }
        //                        string webRootPath = _webHostEnvironment.WebRootPath;

        //                        string upload = webRootPath + WebConstants.SurveyFilePath;
        //                        string fileName = Guid.NewGuid().ToString();

        //                        string extension = Path.GetExtension(files[0].FileName);

        //                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
        //                        {
        //                            files[0].CopyTo(fileStream);
        //                        }
        //                        var file = new Answers
        //                        {
        //                            Form_Id = id,
        //                            Question_Id = model[i].Id,
        //                            Answer = fileName + extension
        //                        };
        //                        submition.Answers.Add(file);
        //                    }
        //                }
        //                else if (model[i].Field_Type == QType.Video)
        //                {
        //                    var files = model[i].File;
        //                    if (files.Count > 0)
        //                    {
        //                        if (files[0].Length / 1048576 > 10)
        //                        {
        //                            return BadRequest("Your file is larger than 10MB");
        //                        }
        //                        string webRootPath = _webHostEnvironment.WebRootPath;

        //                        string upload = webRootPath + WebConstants.SurveyVideoPath;
        //                        string fileName = Guid.NewGuid().ToString();

        //                        string extension = Path.GetExtension(files[0].FileName);

        //                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
        //                        {
        //                            files[0].CopyTo(fileStream);
        //                        }
        //                        var video = new Answers
        //                        {
        //                            Form_Id = id,
        //                            Question_Id = model[i].Id,
        //                            Answer = fileName + extension
        //                        };
        //                        submition.Answers.Add(video);
        //                    }

        //                }
        //                else if (model[i].Field_Type == QType.Audio)
        //                {
        //                    var files = model[i].File;
        //                    if (files.Count > 0)
        //                    {
        //                        if (files[0].Length / 1048576 > 10)
        //                        {
        //                            return BadRequest("Your file is larger than 10MB");
        //                        }
        //                        string webRootPath = _webHostEnvironment.WebRootPath;

        //                        string upload = webRootPath + WebConstants.SurveyAudioPath;
        //                        string fileName = Guid.NewGuid().ToString();

        //                        string extension = Path.GetExtension(files[0].FileName);

        //                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
        //                        {
        //                            files[0].CopyTo(fileStream);
        //                        }
        //                        var audio = new Answers
        //                        {
        //                            Form_Id = id,
        //                            Question_Id = model[i].Id,
        //                            Answer = fileName + extension
        //                        };
        //                        submition.Answers.Add(audio);
        //                    }
        //                }
        //            }

        //            if (caseId != null)
        //            {
        //                var _case = _dbContext.Cases.Where(x => x.Id == Convert.ToInt32(caseId)).FirstOrDefault();

        //                _case.Status = CaseStatus.Completed;
        //                _cases.Update(_case);
        //                _cases.Save();
        //                submition.CaseId = caseId;
        //            }

        //            var response = await _submition.saveSubmission(submition);
        //            if (!response.Success) { return BadRequest(response.Message); }
        //            return Ok("Data Saved");
        //        }
        //        #endregion

        //        #region Submitted Notification
        //        [AllowAnonymous]

        //        public IActionResult SuccessfullySubmitted()
        //        {
        //            return View();
        //        }

        //        #endregion

        //        #region Collect

        //        public IActionResult Collect(int id)
        //        {
        //            var uId = _dbContext.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
        //            var us = _dbContext.UserSurvey.FirstOrDefault(x => x.FormId == id && x.UserId == uId.Id);
        //            ViewData["Data"] = us;
        //            FormViewModel viewModel = new FormViewModel()
        //            {
        //                SurveyLink = Request.Scheme + "://" + HttpContext.Request.Host + "/Survey/Survey/" + id + "?SAgTRid=" + uId.Id
        //            };
        //            return View(viewModel);
        //        }

        //        #endregion
        //    /*    [Authorize(Permissions.PremissionList.Survey_EnrollTeams)]*/

        //        public IActionResult Enroll(string id)
        //        {
        //            ViewBag.u = id;

        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.Result.TenantId);
        //            var t = _dbContext.Forms.FirstOrDefault(x => (x.Id).ToString() == id && x.Tenant_Id == ten.Id);
        //            if (t == null)
        //            {
        //                ViewBag.ErrorMessage = $"Team with Id = {id} cannot be found";
        //                return View("NotFound");
        //            }
        //            var model = new List<UserSuveyViewModel>();
        //            var us = _dbContext.Users.Where(x => x.Id != t.Id.ToString() && x.TenantId == ten.Id);

        //            foreach (var user in us)
        //            {
        //                var userSuveyViewModel = new UserSuveyViewModel
        //                {
        //                    UserId = user.Id,
        //                    UserName = user.UserName
        //                };
        //                var d = _dbContext.UserSurvey.FirstOrDefault(x => x.UserId == user.Id && x.FormId == t.Id);
        //                if (d != null)
        //                {
        //                    userSuveyViewModel.IsSelected = true;
        //                }
        //                else
        //                {
        //                    userSuveyViewModel.IsSelected = false;
        //                }

        //                model.Add(userSuveyViewModel);
        //            }

        //            return View(model);
        //        }
        //        [HttpPost]
        ///*        [Authorize(Permissions.PremissionList.Survey_EnrollTeams)]
        //*/
        //        public IActionResult Enroll(List<UserSuveyViewModel> model, string id)
        //        {
        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.Result.TenantId);
        //            var t = _dbContext.Forms.FirstOrDefault(x => (x.Id).ToString() == id && x.Tenant_Id == ten.Id);
        //            if (t == null)
        //            {
        //                ViewBag.ErrorMessage = $"Form with Id = {id} cannot be found";
        //                return View("NotFound");
        //            }
        //            for (int i = 0; i < model.Count; i++)
        //            {
        //                var user = _dbContext.Users.FirstOrDefault(x => x.Id == model[i].UserId && x.TenantId == ten.Id);


        //                var d = _dbContext.UserSurvey.AsNoTracking().FirstOrDefault(x => x.UserId == user.Id && x.FormId == t.Id);
        //                if (model[i].IsSelected && !(d != null))
        //                {
        //                    UserSurvey model1 = new UserSurvey()
        //                    {
        //                        UserId = user.Id,
        //                        FormId = t.Id
        //                    };
        //                    _dbContext.UserSurvey.Add(model1);
        //                    _dbContext.SaveChanges();
        //                }
        //                else if (!model[i].IsSelected && d != null)
        //                {
        //                    UserSurvey model1 = new UserSurvey()
        //                    {
        //                        UserId = user.Id,
        //                        FormId = t.Id
        //                    };
        //                    var result = _dbContext.UserSurvey.Remove(model1);
        //                    _dbContext.SaveChanges();
        //                }

        //            }

        //            return RedirectToAction("ManageForm");
        //        }
        ///*        [Authorize(Permissions.PremissionList.Survey_EnrollTeams)]
        //*/
        //        public IActionResult EnrollTeam(string id)
        //        {
        //            ViewBag.u = id;

        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.Result.TenantId);
        //            var t = _dbContext.Forms.FirstOrDefault(x => (x.Id).ToString() == id && x.Tenant_Id == ten.Id);
        //            if (t == null)
        //            {
        //                ViewBag.ErrorMessage = $"Team with Id = {id} cannot be found";
        //                return View("NotFound");
        //            }
        //            var model = new List<TeamSuveyViewModel>();
        //            var us = _dbContext.Teams.Where(x => x.Id != t.Id.ToString() && x.TenantId == ten.Id);

        //            foreach (var user in us)
        //            {
        //                var userSuveyViewModel = new TeamSuveyViewModel
        //                {
        //                    TeamId = user.Id,
        //                    Name = user.Name
        //                };
        //                var d = _dbContext.TeamSurvey.FirstOrDefault(x => x.TeamId == user.Id && x.FormId == t.Id);
        //                if (d != null)
        //                {
        //                    userSuveyViewModel.IsSelected = true;
        //                }
        //                else
        //                {
        //                    userSuveyViewModel.IsSelected = false;
        //                }

        //                model.Add(userSuveyViewModel);
        //            }

        //            return View(model);
        //        }
        ///*        [Authorize(Permissions.PremissionList.Survey_EnrollTeams)]
        //*/
        //        [HttpPost]
        //        public IActionResult EnrollTeam(List<TeamSuveyViewModel> model, string id)
        //        {
        //            var u = _userManager.GetUserAsync(HttpContext.User);
        //            var ten = _dbContext.Tenants.FirstOrDefault(x => x.Id == u.Result.TenantId);
        //            var t = _dbContext.Forms.FirstOrDefault(x => (x.Id).ToString() == id && x.Tenant_Id == ten.Id);
        //            if (t == null)
        //            {
        //                ViewBag.ErrorMessage = $"Form with Id = {id} cannot be found";
        //                return View("NotFound");
        //            }
        //            for (int i = 0; i < model.Count; i++)
        //            {
        //                var user = _dbContext.Teams.FirstOrDefault(x => x.Id == model[i].TeamId && x.TenantId == ten.Id);


        //                var d = _dbContext.TeamSurvey.FirstOrDefault(x => x.TeamId == user.Id && x.FormId == t.Id);

        //                if (model[i].IsSelected && !(d != null))
        //                {
        //                    TeamSurvey model1 = new TeamSurvey()
        //                    {
        //                        TeamId = user.Id,
        //                        FormId = t.Id
        //                    };
        //                    _dbContext.TeamSurvey.Add(model1);
        //                    _dbContext.SaveChanges();
        //                    var ut = _dbContext.UserTeams.Where(x => x.TeamId == user.Id).ToList();
        //                    foreach (var item in ut)
        //                    {
        //                        UserSurvey model2 = new UserSurvey()
        //                        {
        //                            UserId = item.UserId,
        //                            FormId = t.Id
        //                        };
        //                        _dbContext.UserSurvey.Add(model2);
        //                        _dbContext.SaveChanges();
        //                    }
        //                }
        //                else if (!model[i].IsSelected && d != null)
        //                {
        //                    var ut = _dbContext.UserTeams.Where(x => x.TeamId == user.Id).ToList();
        //                    var tt = new List<string>();
        //                    foreach (var item in ut)
        //                    {
        //                        var temp = _dbContext.UserSurvey.FirstOrDefault(x => x.UserId == item.UserId).UserId;
        //                        tt.Add(temp);
        //                    }
        //                    if (tt != null)
        //                    {
        //                        foreach (var item in tt)
        //                        {
        //                            UserSurvey model2 = new UserSurvey()
        //                            {
        //                                UserId = item,
        //                                FormId = t.Id
        //                            };
        //                            _dbContext.UserSurvey.Remove(model2);
        //                            _dbContext.SaveChanges();
        //                        }
        //                    }

        //                    TeamSurvey model1 = new TeamSurvey()
        //                    {
        //                        TeamId = user.Id,
        //                        FormId = t.Id
        //                    };

        //                    var result = _dbContext.TeamSurvey.Remove(model1);
        //                    _dbContext.SaveChanges();

        //                }

        //            }

        //            return RedirectToAction("ManageForm");

        //        }
        ///*        [Authorize(Permissions.PremissionList.Survey_AssignUsers)]
        //*/
        //        public IActionResult RemoveUser(string id, int survay)
        //        {
        //            try
        //            {
        //                var u = _dbContext.UserSurvey.FirstOrDefault(x => x.FormId == survay && x.UserId == id);
        //                _dbContext.UserSurvey.Remove(u);
        //                return RedirectToAction("ManageForm");
        //            }
        //            catch (Exception)
        //            {
        //                return RedirectToAction("ManageForm");
        //            }
        //        }
        ///*        [Authorize(Permissions.PremissionList.Survey_AssignUsers)]
        //*/
        //        public IActionResult GetUserInSurvay(string id)
        //        {

        //            var usersIds = _dbContext.UserSurvey.Where(x => x.FormId == int.Parse(id)).Select(x => x.UserId);
        //            var users = _dbContext.Users.Where(x => usersIds.Any(y => y == x.Id)).ToList();
        //            var te = _dbContext.Forms.FirstOrDefault(x => x.Id.ToString() == id);
        //            ViewBag.name = te.FormTitle;
        //            ViewBag.u = id;
        //            var model = new FormViewModel()
        //            {
        //                Users = users
        //            };
        //            return View(model);
        //        }
        #endregion
    }

}
