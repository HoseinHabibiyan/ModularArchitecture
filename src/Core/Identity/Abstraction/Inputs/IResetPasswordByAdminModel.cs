﻿using System;

namespace ModularArchitecture.Identity.Abstraction.Inputs
{
    public interface IResetPasswordByAdminModel
    {
        [Obsolete("Use Username")]
        public string Email { get; set; }
        public string Username { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}