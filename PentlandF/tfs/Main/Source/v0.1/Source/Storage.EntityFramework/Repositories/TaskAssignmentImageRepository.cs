﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Planning;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class TaskAssignmentImageRepository : Ip1BaseRepository<TaskAssignmentImage>
    {
        public TaskAssignmentImageRepository(Ip1Context context) : base(context)
        {
        }
    }
}