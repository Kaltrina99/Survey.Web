﻿using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface IAnswers : IRepository<Answers>
    {
        void Update(Answers objAnswer);
        IEnumerable<Answers> GetAnswersByFormId(int? obj);
    }
}