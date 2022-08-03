using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Constants
{
    public class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"{module}",
            };
        }
        //
        public static class PremissionList
        {

            #region Survey Permissions
            public const string Survey_View = "Permissions.Survey.View";
            public const string Survey_Create = "Permissions.Survey.Create";
            public const string Survey_Update = "Permissions.Survey.Update";
            public const string Survey_Delete = "Permissions.Survey.Delete";
            public const string Survey_Collect = "Permissions.Survey.Collect";
            public const string Survey_SeeResults = "Permissions.Survey.SeeSurveyResults";
            #endregion

            #region Project Permissions
            public const string Project_Add = "Permissions.Project.Add";
            public const string Project_Delete = "Permissions.Project.Delete";
            public const string Project_Update = "Permissions.Project.Update";
            public const string Project_View = "Permissions.Project.View";
            public const string Project_AssignUser = "Permissions.Project.AssignUsers";
            #endregion

            #region User Permissions
            public const string User_AddUser = "Permissions.User.AddUser";
            public const string User_DeleteUser = "Permissions.User.DeleteUser";
            public const string User_UpdateUser = "Permissions.User.UpdateUser";
            public const string User_ViewUsers = "Permissions.User.ViewUsers";
            #endregion

            #region Cases Permissions
            public const string DataSet_AssignCases = "Permissions.DataSet.AssignCases";
            public const string DataSet_AddCases = "Permissions.DataSet.AddCases";
            public const string DataSet_DeleteCases = "Permissions.DataSet.DeleteCases";
            public const string DataSet_Update = "Permissions.DataSet.UpdateCases";
            public const string DataSet_View = "Permissions.DataSet.ViewCases";
            #endregion
           
            public static List<string> GetAllUserPermissionList()
            {
                List<string> list = new List<string>()
                {
                   
                    Survey_View,
                    Survey_Create,
                    Survey_Update,
                    Survey_Delete,
                    Survey_Collect,
                    Survey_SeeResults,
                    User_ViewUsers,
                    User_UpdateUser,
                    User_DeleteUser,
                    User_AddUser,
                    Project_AssignUser,
                    Project_Add,
                    Project_Delete,
                    Project_Update,
                    Project_View,
                    DataSet_AddCases,
                    DataSet_AssignCases,
                    DataSet_DeleteCases,
                    DataSet_View,
                    DataSet_Update

                };

                return list;
            }
        }

        public static class SuperPremissionList
        {
            public static List<string> GetAllSuperPermissionList()
            {
                List<string> list = new List<string>()
                {
                   
                };

                return list;
            }
        }
        public static List<string> GetAllPermissionsList()
        {
            List<string> permissons = PremissionList.GetAllUserPermissionList();
            permissons.AddRange(SuperPremissionList.GetAllSuperPermissionList());
            return permissons;
        }
    }
}
