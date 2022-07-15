using Survey.Core.HelperClasses;
using Survey.Core.Models;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Survey.Infrastructure.Utility
{
    public class ExcelToForm
    {
        private Dictionary<(string name,string category),QuestionOptions> Formoptions = new();
        private Regex LangRegex { get; set; } = new Regex(@"(?<=\:\()(.+)(?=\))",RegexOptions.Compiled|RegexOptions.Singleline);
        private XSSFWorkbook excelfile;
        private  IFormFile _file;
        public Forms MainForm { get; set; } = new();
        public Dictionary<string, Tuple<Questions,string>> Formquestions { get; set; } = new();
        public ExcelToForm()
        {
            MainForm.Questions = new();
        }

        public void MapObject(IFormFile file) 
        {
            _file = file;

            using (var fileStream = _file.OpenReadStream())
            {
                excelfile = new XSSFWorkbook(fileStream);
            }
            var settings= excelfile.GetSheet("settings");
            if (settings == null) 
            {
                throw new ExcelToFormException("Settings Sheet Not Found");
            }
               GetSettings(settings);
            var choices = excelfile.GetSheet("options");
            if (choices == null)
            {
                throw new ExcelToFormException("Options Sheet Not Found");
            }
            GetChoices(choices);
            var questions = excelfile.GetSheet("questions");
            if (questions == null)
            {
                throw new ExcelToFormException("Questions Sheet Not Found");
            }
            GetQuestions(questions);

        }
        private  void GetQuestions(ISheet sheet)
        {
            int rowindex = 1;
            IRow header = sheet.GetRow(0);
            CheckHeader(header, "type","options", "name", "required", "sk_match","skip", "label","Questions");
            int questionorder = 0;
            while (sheet.GetRow(rowindex) != null)
            {
                Questions question = new();
                question.QuestionOrder = questionorder;
                questionorder++;
                question.Options=new();
                string optioncategory="";
                string name = "";
                var row = sheet.GetRow(rowindex);
                int rowcol = row.LastCellNum;
                for (int i = 0; i < rowcol; i++)
                {
                    var cell = row.GetCell(i);
                    if (cell == null && i == 0) { 
                        throw new ExcelToFormException("Each question must have a type"); }
                    else if (cell == null && i == 2) { throw new ExcelToFormException("Each question must have a uniqe name"); }
                
                    else if (cell == null) { continue; }
                    if (i == 3)
                    {
                        try
                        {
                            question.IsRequired = cell.BooleanCellValue;
                        }
                        catch (Exception e) 
                        {
                            throw new ExcelToFormException("Can't convert required value. Please check your required column in 'Questions' sheet.");
                        }
                        continue;
                    }
                    string colvalue = cell.StringCellValue;
                    colvalue = colvalue.Trim();
                    if (string.IsNullOrWhiteSpace(colvalue)) 
                    {
                        continue;
                    }
                    if (i == 0)
                    {
                        GetQuestionType(question, colvalue);
                    }
                    else if (i == 1)
                    {
                        if (colvalue.Trim() != null)
                        {
                            if (question.Field_Type != QType.Select_Multiple && question.Field_Type != QType.Select_One)
                            {
                                throw new ExcelToFormException("You can't add options to questions of type " + question.Field_Type.ToString());
                            }
                            var optionslist = Formoptions.Where(x => x.Key.category == colvalue).Select(x => x.Value).ToList();
                            if (optionslist.Count==0) { throw new ExcelToFormException($"Could not find option with category '{colvalue}'"); }
                            question.Options = CreateNewOptions(optionslist);
                            optioncategory = colvalue;
                        }
                    }
                    else if (i == 2)
                    {
                        name = colvalue;
                    }
                    else if (i == 4) { continue; }
                    else if (i == 5)
                    {
                        string typevalue=null;
                        var skiptype = row.GetCell(4);
                        if (skiptype != null) 
                        {
                            typevalue = skiptype.StringCellValue;
                        }
                        GetSkipLogic(question, typevalue, colvalue);
                    }
                    else if (i == 6)
                    {
                        question.QuestionText = colvalue;
                    }
                    else if (i == 7)
                    {
                        question.QuestionDescription = colvalue;
                    }
                    else
                    {
                        var label = header.GetCell(i);
                    }
                    
                }
                MainForm.Questions.Add(question);
                Formquestions.Add(name, Tuple.Create(question,optioncategory));
                rowindex++;
            }
          
        }
        private List<QuestionOptions> CreateNewOptions(List<QuestionOptions> options) 
        {
            List<QuestionOptions> response = new();
            foreach (QuestionOptions option in options) 
            {
                QuestionOptions opt = new QuestionOptions();
                opt.OptionText= option.OptionText;
                response.Add(opt);
            }
            return response;
        }
        private void GetSkipLogic(Questions question,string skiptype,string colvalue) 
        {
           colvalue= colvalue.Trim();
            Regex operatorRegex = new Regex(@"(?<=\()(.+)(?=\))|(==|<=|>=|>|<)|(?<=\')(.+)(?=\')");
            question.skipChild = new List<SkipLogic>();
            if (skiptype != null&& skiptype.Contains("(all)",StringComparison.OrdinalIgnoreCase)) 
            {
                question.SkipLogicType = SkipLogicType.MatchAllConditions;
            }
            string[] skipStringList = colvalue.Split(";",StringSplitOptions.RemoveEmptyEntries);
            foreach (string skipstring in skipStringList) 
            {
               
                var matches = operatorRegex.Matches(skipstring);
                if (matches.Count != 3) 
                {
                    throw new ExcelToFormException($"Skip Logic Invalid. Please Check You'r Format '{colvalue}'");
                }
                var tuple =Formquestions.GetValueOrDefault(matches[0].Value);
                var parentQuestion = tuple.Item1;
                if (parentQuestion == null) 
                {
                    throw new ExcelToFormException($"Parent question for skip logic ${colvalue} with name {matches[0].Value} could not be found.");
                }
                if (parentQuestion.Field_Type != QType.Select_Multiple && parentQuestion.Field_Type != QType.Select_One && parentQuestion.Field_Type != QType.Numbers && parentQuestion.Field_Type != QType.Decimal )
                {
                    throw new ExcelToFormException($"You can't add skip logic to a parent question of type {parentQuestion.Field_Type.ToString()}. Please check your skip logic '{colvalue}'");
                }
                Operators op = Operator.getOneOperator(matches[1].Value);
                if (op == null) 
                {
                    throw new ExcelToFormException($"Invalid operator. Please check your skip logic '{colvalue}'");
                }
                if ((parentQuestion.Field_Type == QType.Select_Multiple && parentQuestion.Field_Type == QType.Select_One) && op.Id != 3)
                {
                    throw new ExcelToFormException($"You can not use operator {op.OperatorSymbol} with parent questions of type {parentQuestion.Field_Type.ToString()}.Please check your skip logic {colvalue}");
                }
                if (parentQuestion.Field_Type == QType.Select_Multiple || parentQuestion.Field_Type == QType.Select_One)
                {
                    
                    var option =Formoptions.Where(x=>x.Key.name==matches[2].Value && x.Key.category==tuple.Item2).Select(x=>x.Value).FirstOrDefault();
                    var opt = parentQuestion.Options.FirstOrDefault(x => x.OptionText.Equals(option.OptionText));
                    if (option == null) 
                    {
                        throw new ExcelToFormException($"Option with name '{matches[2].Value}' in category '{tuple.Item2}' could not be found. Please check your skip logic {colvalue}");

                    }
                    question.skipChild.Add(new SkipLogic { ParentQuestion = parentQuestion, Operator = op.Id, Option = opt });
                    continue;
                }
                if (!double.TryParse(matches[2].Value, out double compvalue))
                {
                    throw new ExcelToFormException($"Could not read comperable value {matches[2].Value} as a number");
                }
                question.skipChild.Add(new SkipLogic { ParentQuestion = parentQuestion, Operator = op.Id, Comparable_Value = compvalue });
            }
        }
        private  void GetChoices(ISheet sheet) 
        {
            int rowindex = 1;
            IRow header = sheet.GetRow(0);
            CheckHeader(header, "category", "name", "label","Options");
            while (sheet.GetRow(rowindex) != null)
            {
                QuestionOptions option = new QuestionOptions();
                string name="", category="";
                var row = sheet.GetRow(rowindex);
                int rowcol = row.LastCellNum;
                for (int i = 0; i < rowcol; i++)
                {
                    var cell = row.GetCell(i);
                    if (cell == null && i == 0) { throw new ExcelToFormException("Each option must have a category"); }
                    else if (cell == null && i == 1) { throw new ExcelToFormException("Each option must have a uniqe name"); }
                    else if (cell == null && i == 2) { throw new ExcelToFormException("Each option must have at least one translation"); }
                    else if (cell == null) { continue; }

                    string colvalue = cell.StringCellValue;
                    if (i == 0)
                    {
                        category = colvalue;
                    }
                   else if (i == 1)
                    {
                        name = colvalue;
                    }
                   else if (i == 2)
                    {
                        option.OptionText = colvalue;
                    }
                    else
                    {
                        string label = header.GetCell(i).StringCellValue;
                      
                    }
                    
                }
                if (Formoptions.Any(x => x.Key.name == name && x.Key.category==category)) 
                {
                    throw new ExcelToFormException($"An option with the name '{name}' and category '{category}' already exists");
                }
                Formoptions.Add((name,category),option);
                rowindex++;
            }
          
        }
        private void GetSettings(ISheet sheet) 
        {
            int rowindex = 1;
            IRow header = sheet.GetRow(0);
            CheckHeader(header, "form-name", "form-desc", "languages","Settings");
            while (sheet.GetRow(rowindex) != null)
            {
                var row = sheet.GetRow(rowindex);
                int rowcol = row.LastCellNum;
                for (int i = 0; i < rowcol; i++)
                {
                    var cell = row.GetCell(i);
                    if (cell == null && i == 0 && MainForm.FormTitle == null) { throw new ExcelToFormException("Form must have a title"); }
                    else if (cell == null && i == 2) { throw new ExcelToFormException("Form must have at least one language"); }
                    else if (cell == null) { continue; }
                    string colvalue = cell.StringCellValue; if (i == 0&& !string.IsNullOrWhiteSpace(colvalue))
                    {
                        MainForm.FormTitle = colvalue;
                    }
                   else if (i == 1&&!string.IsNullOrWhiteSpace(colvalue))
                    {
                        MainForm.Description = colvalue;
                        
                    }
                    else if (i == 2&&!string.IsNullOrWhiteSpace(colvalue))
                    {
                        string[] langcomp = colvalue.Split(":");
                        if (langcomp.Length != 2) 
                        {
                            throw new ExcelToFormException("Language is not in the right format. Please check your settings.");
                        }
                    }

                }
                rowindex++;

            }
        }
       
        private void GetQuestionType(Questions question, string colvalue) 
        {
           colvalue= colvalue.Trim();
           
            try
            {
                var converted = Enum.TryParse(colvalue,true,out QType type);
                if (converted == false) 
                {
                    throw new ExcelToFormException($"Could not find question type {colvalue}");
                }
                question.Field_Type = type;

            }
            catch (ArgumentException invalidtype)
            {
                throw new ExcelToFormException("You have entered an invalid question type");
            }
        }
        
        private void CheckHeader(IRow row,string first,string second,string third,string sheetname) 
        {
            if (!row.GetCell(0).StringCellValue.Equals(first))
            {
                throw new ExcelToFormException($"Your header does not contain {first} in your {sheetname} sheet");
            }
            else if (!row.GetCell(1).StringCellValue.Equals(second))
            {
                throw new ExcelToFormException($"Your header does not contain {second} in your {sheetname} sheet");
            }
            else if (!row.GetCell(2).StringCellValue.Contains(third))
            {
                throw new ExcelToFormException($"Your header does not contain {third} in your {sheetname} sheet");
            }

        }
        private void CheckHeader(IRow row, string first, string second, string third,string fourth,string fifth,string sixth,string seventh, string sheetname)
        {
            
            if (!row.GetCell(0).StringCellValue.Equals(first))
            {
                throw new ExcelToFormException($"Your header does not contain {first} in your {sheetname} sheet");
            }
           else if (!row.GetCell(1).StringCellValue.Equals(second))
            {
                throw new ExcelToFormException($"Your header does not contain {second} in your {sheetname} sheet");
            }
            else if (!row.GetCell(2).StringCellValue.Contains(third))
            {
                throw new ExcelToFormException($"Your header does not contain {third} in your {sheetname} sheet");
            }
            else if (!row.GetCell(3).StringCellValue.Contains(fourth))
            {
                throw new ExcelToFormException($"Your header does not contain {fourth} in your {sheetname} sheet");
            }
            else if (!row.GetCell(4).StringCellValue.Contains(fifth))
            {
                throw new ExcelToFormException($"Your header does not contain {fifth} in your {sheetname} sheet");
            }
            else if (!row.GetCell(5).StringCellValue.Contains(sixth))
            {
                throw new ExcelToFormException($"Your header does not contain {sixth} in your {sheetname} sheet");
            }
            else if (!row.GetCell(6).StringCellValue.Contains(seventh))
            {
                throw new ExcelToFormException($"Your header does not contain {seventh} in your {sheetname} sheet");
            }
        }

    }
}
