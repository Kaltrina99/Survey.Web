﻿using Survey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface IProjectCategory : IRepository<ProjectCategory>
    {
        void Update(ProjectCategory objProjectCategory);
        List<ProjectCategory> GetClients(List<int> clientId, List<int> projectId);
    }
}