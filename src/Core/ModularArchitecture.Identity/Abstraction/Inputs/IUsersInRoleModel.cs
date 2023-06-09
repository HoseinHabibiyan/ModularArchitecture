﻿using System.Collections.Generic;

namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface IUsersInRoleModel
    {
        public string Id { get; set; }
        public List<string> EnrolledUsers { get; set; }
        public List<string> RemovedUsers { get; set; }
    }
}
