using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface ICases : IRepository<Cases>
    {
        void Update(Cases cases);
        public int GetDatasetId(int obj);
        IEnumerable<Cases> GetCases(int obj);
    }
}
