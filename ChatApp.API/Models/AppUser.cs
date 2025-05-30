﻿using Microsoft.AspNetCore.Identity;

namespace ChatApp.API.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
    }
}
