using Survey.Core.Constants;
using Survey.Core.DTOmodels;
using Survey.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.HelperClasses
{
    public class DownloadResults
    {
        #region VerifyPath
        public static string VerifyPath(string wwwpath,string filename) 
        {
            if (!Directory.Exists(wwwpath))
            {
                Directory.CreateDirectory(wwwpath);
            }

            string pathToCheck = wwwpath + filename;
            if (File.Exists(pathToCheck))
            {
                string tempfileName = "";
                int counter = 2;
                while (File.Exists(pathToCheck))
                {
                    tempfileName = counter.ToString() + filename;
                    pathToCheck = wwwpath + tempfileName;
                    counter++;
                }
                filename = tempfileName;
               
            }
            string fullpath = Path.Combine(wwwpath, filename);
            return fullpath;
        }
        #endregion

        #region DownloadType

        #region DownloadJson
        public static string CreateDownloadJson(Forms form,string path)
        {
            string filename = $@"{Guid.NewGuid()}.json";
            string wwwpath = path+ "\\DownloadHistory\\Json\\";
            var data = new DownloadJsonDto(form).FormDto;
            string fullpath = VerifyPath(wwwpath,filename);

            using (StreamWriter fs =new  StreamWriter(fullpath)) 
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                }); ;
              
                fs.Write(json);
            }
                return filename;
        }
       
        #endregion

        

        #endregion

        #region GetFilePath
        public static string GetFilePath(SurveyDownload record,string path) 
        {
            string fullpath=path+ @"\DownloadHistory\";
           if (record.DownloadType == DownloadTypes.Excel)
            {
                 fullpath = fullpath + $@"Excel\{record.FileName}";
            }
            else if (record.DownloadType == DownloadTypes.JSON)
            {
                fullpath = fullpath + $@"Json\{record.FileName}";
            }
            else
            {
                throw new Exception("File Not Found");
            }
            if (!File.Exists(fullpath))
            {
                throw new Exception("File Not Found");
            }
            return fullpath;
        }
        #endregion
    }

}
