using Survey.Core.HelperClasses;
using Survey.Core.Models;
using Survey.Core.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.DTOmodels
{
    public class QuestionDto :Questions
    {
     
        public string Answer { get; set; }
        public int selected_option { get; set; }
        public List<IFormFile> File { get; set; } = new List<IFormFile>();
        public List<SkipLogicLocalDTO> SkipDto { get; set; } = new List<SkipLogicLocalDTO>();

        public void SkipToJson(List<SkipLogic> logic) 
        {
            var data = logic.Select(x => new SkipLogicLocalDTO {Optionid=x.Condition_Option, childquestion=x.Child_Question_Id,parentquestion = x.Parent_Question_Id, value = x.Comparable_Value, Operator = Operator.getOneOperator(x.Operator).OperatorSymbol }).ToList();
           /* json = JsonConvert.SerializeObject(data, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });*/
           SkipDto= data;
            
        }
        public void MapPropertys(Questions question)
        {
            Id = question.Id;
            skipParent = question.skipParent;
            QuestionOrder = question.QuestionOrder;
            skipChild = question.skipChild;
            Field_Type = question.Field_Type;
            Form_Id = question.Form_Id;
            IsRequired = question.IsRequired;
            QuestionDescription = question.QuestionDescription;
            QuestionText = question.QuestionText;
            Skip_Logic = question.skipChild.Any();
            Options = question.Options;
            Answers = question.Answers;
            SkipLogicType = question.SkipLogicType;
            if (Skip_Logic) { SkipToJson(question.skipChild); }
    }
    }
}
