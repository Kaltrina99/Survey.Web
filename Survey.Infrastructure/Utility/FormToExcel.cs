using Survey.Core.HelperClasses;
using Survey.Core.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Utility
{
    public class FormToExcel
    {
        public Forms _form { get; set; }
        public XSSFWorkbook _excel { get; set; } = new();
        public ISheet settingsSheet  { get; set; }
        public ISheet questionsSheet { get; set; }
        public ISheet optionsSheet { get; set; }
        private Dictionary<(string name, int id) ,Questions > questionsDic = new();
        private Dictionary<(string category,string name),QuestionOptions> optionsDic = new();

        public FormToExcel(Forms form)
        {
            _form = form;
            MapObject();
        }
        private void MapObject()
        {
             settingsSheet = _excel.CreateSheet("settings");
             optionsSheet = _excel.CreateSheet("options");
             questionsSheet = _excel.CreateSheet("questions");
            MapSettings();
            MapOptionsHeader();
            MapQuestions();

        }
        private void MapSettings()
        {
            IRow headerRow = settingsSheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("form-name");
            headerRow.CreateCell(1).SetCellValue("form-desc");
            IRow data = settingsSheet.CreateRow(1);
            data.CreateCell(0).SetCellValue(_form.FormTitle);
            data.CreateCell(1).SetCellValue(_form.Description);

        }
        private void MapOptionsHeader( )
        {
            IRow header = optionsSheet.CreateRow(0);
            header.CreateCell(0).SetCellValue("category");
            header.CreateCell(1).SetCellValue("name");
            header.CreateCell(2).SetCellValue("label");
        }
        private void MapQuestionsHeader( ) 
        {
            IRow header = questionsSheet.CreateRow(0);
            List<string> headerStrings = new List<string> { "type", "options", "name", "required", "sk_match", "skip" };
            headerStrings.Add("label");
            headerStrings.Add("desc");
            for (int i = 0; i < headerStrings.Count; i++)
            {
                header.CreateCell(i).SetCellValue(headerStrings[i]);
            }
        }
        private void MapQuestions()
        {
            MapQuestionsHeader();
            int rowindex = 1;
            foreach (Questions question in _form.Questions)
            {
                string optioncategory = $"question_{ rowindex}_options";
                string questioname = $"Question_{rowindex}";
               
                IRow row = questionsSheet.CreateRow(rowindex);
                row.CreateCell(0).SetCellValue(question.Field_Type.ToString());
                if (question.Options.Count > 0)
                {
                    MapQuestionOptions(question, optioncategory);
                    row.CreateCell(1).SetCellValue(optioncategory);
                }
                row.CreateCell(2).SetCellValue(questioname);
                row.CreateCell(3).SetCellValue(question.IsRequired);
                if (question.SkipLogicType == SkipLogicType.MatchOneCondition)
                {
                    row.CreateCell(4).SetCellValue("One");
                }
                else
                {
                    row.CreateCell(4).SetCellValue("All");
                }
                string skiplogic = "";
                foreach (var skip in question.skipChild)
                {
                    var questionRecord = questionsDic.Where(x => x.Key.id == skip.Parent_Question_Id).FirstOrDefault();
                    string optname = optionsDic.Where(x => x.Value.Id == skip.Condition_Option).Select(x => x.Key.name).FirstOrDefault();
                    if (questionRecord.Value.Field_Type==QType.Select_Multiple|| questionRecord.Value.Field_Type == QType.Select_One) 
                    {
                        skiplogic = skiplogic + $@"({questionRecord.Key.name}){Operator.getOneOperator(skip.Operator).OperatorSymbol}'{optname}';";
                        continue;
                    }
                    skiplogic = skiplogic + $@"({questionRecord.Key.name}){Operator.getOneOperator(skip.Operator).OperatorSymbol}'{skip.Comparable_Value}';";
                }
                row.CreateCell(5).SetCellValue(skiplogic);
                int tranid =8 ;
              
                row.CreateCell(6).SetCellValue(question.QuestionText);
                row.CreateCell(7).SetCellValue(question.QuestionDescription);
                questionsDic.Add((questioname, question.Id), question);
                rowindex++;
            }
        }
        private void MapQuestionOptions(Questions question, string category)
        {
            int rowi = optionsSheet.LastRowNum+1;
            int optid = 1;
            foreach (var opt in question.Options) 
            {
                string name = $"Q_{rowi}_Op_{optid}";
                optionsDic.Add((category, name), opt);
                IRow row = optionsSheet.CreateRow(rowi);
                row.CreateCell(0).SetCellValue(category);
                row.CreateCell(1).SetCellValue(name);
                
                row.CreateCell(2).SetCellValue(opt.OptionText);
                rowi++;
                optid++;
            }
        }

    }
}
