using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Infrastructure.Utility
{
    public class Claims
    {
        public static List<Claim> claimsList = new List<Claim>()
        {
            new Claim("CreateSurvey", "CreateSurvey"),
            new Claim("UpdateSurvey", "UpdateSurvey"),
            new Claim("DeleteSurvey", "DeleteSurvey")
        };
    }
}
