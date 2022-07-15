using Survey.Core.DTOmodels;
using Survey.Core.HelperClasses;
using Survey.Core.Models;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Utility
{
    public class SurveyResultsExcel
    {
        private readonly Forms _form;
        private readonly string _path;
        

        public SurveyResultsExcel(Forms form, string path )
        {
            _form = form;
            _path = path;
        }
        public string CreateDownloadExcel()
        {
            string filename = $@"{Guid.NewGuid()}.xlsx";
            string wwwpath = _path + "\\DownloadHistory\\Excel\\";
            string fullpath = DownloadResults.VerifyPath(wwwpath, filename);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Raw Data");
            var rowIndex = 0;
            var header = sheet.CreateRow(rowIndex);
            int headerindex = 3;
            for (int i = 0; i < _form.Questions.Count; i++) 
            {
                header.CreateCell(0).SetCellValue("Start ");
                header.CreateCell(1).SetCellValue("End");
                header.CreateCell(2).SetCellValue("IdentityUser");

                if (_form.Questions[i].Field_Type == QType.Select_Multiple) 
                {
                    header.CreateCell(headerindex).SetCellValue(_form.Questions[i].QuestionText);
                    headerindex++;
                    foreach (var opt in _form.Questions[i].Options) 
                    {
                        header.CreateCell(headerindex).SetCellValue($"{_form.Questions[i].QuestionText} / {opt.OptionText}");
                        headerindex++;
                    }
                    continue;
                }
                header.CreateCell(headerindex).SetCellValue(_form.Questions[i].QuestionText);
                headerindex++;
            }
            rowIndex++;
           
            for (int i = 0;i<_form.Submissions.Count;i++) 
            {
                var columnindex = 3;
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(_form.Submissions[i].StartTime.ToString("s"));
                row.CreateCell(1).SetCellValue(((DateTime)_form.Submissions[i].EndTime).ToString("s"));
                row.CreateCell(2).SetCellValue(_form.Submissions[i].AgentUsername);

                for (int j = 0; j < _form.Questions.Count; j++)
                {
                    if (_form.Questions[j].Field_Type == QType.Select_Multiple)
                    {

                        var allanswers = _form.Submissions[i].Answers.Where(x => x.Question_Id == _form.Questions[j].Id).ToList();
                        if (allanswers.Count == 0)
                        {
                            for (int z = 0; z < _form.Questions[j].Options.Count + 1; z++)
                            {
                                row.CreateCell(columnindex).SetCellValue("");
                                columnindex++;
                            }

                        }
                        else
                        {
                            string mergedanswers = "";
                            foreach (var an in allanswers)
                            {
                                mergedanswers = mergedanswers + " " + an.Option.OptionText;
                            }
                            row.CreateCell(columnindex).SetCellValue(mergedanswers);
                            columnindex++;
                            foreach (var opt in _form.Questions[j].Options)
                            {
                                var selected = 0;
                                if (allanswers.Any(x => x.OptionId == opt.Id))
                                {
                                    selected = 1;
                                }
                                row.CreateCell(columnindex).SetCellValue(selected);
                                columnindex++;
                            }
                        }
                        continue;
                    }
                    else if (_form.Questions[j].Field_Type == QType.Select_One) 
                    {
                        var answerselect = _form.Submissions[i].Answers.FirstOrDefault(x => x.Question_Id == _form.Questions[j].Id);
                        if (answerselect is null)
                        {
                            row.CreateCell(columnindex).SetCellValue("");
                            columnindex++;
                            continue;
                        }
                        row.CreateCell(columnindex).SetCellValue(answerselect.Option.OptionText);
                        columnindex++;
                        continue;
                    }
                    var answer = _form.Submissions[i].Answers.FirstOrDefault(x => x.Question_Id== _form.Questions[j].Id);
                    if (answer is null) 
                    {
                        row.CreateCell(columnindex).SetCellValue("");
                        columnindex++;
                        continue;
                    }
                    row.CreateCell(columnindex).SetCellValue(answer.Answer);
                    columnindex++;
                }
                rowIndex++;
            }
            using (var filedata = new FileStream(fullpath, FileMode.Create)) 
            {
                workbook.Write(filedata);
            }
                return filename;
        }
    }
}
