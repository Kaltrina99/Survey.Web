using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface ISkipLogic : IRepository<SkipLogic>
    {
        //public SkipLogic GetSkipLogicByQuestionId(int obj, string obj2);
        public int GetSkipLogic(int id);
        public int? GetSkipLogicCondition(int id);
        IEnumerable<SkipLogic> GetSkipLogicById(int? id);
        List<SkipLogic> GetSkipLogicByQuestionId(int id);
        IEnumerable<SkipLogic> GetSkipLogicByQuestionwithparentId(int id);

    }
}
