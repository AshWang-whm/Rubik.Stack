﻿namespace Rubik.Identity.AuthServer.Models
{
    public class LoginInput
    {
        public string? ReturnUrl { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool RememberLogin { get; set; }

    }
}
