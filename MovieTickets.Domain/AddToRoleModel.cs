﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTickets.Domain
{
    public class AddToRoleModel
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string SelectedRole { get; set; }
    }
}
