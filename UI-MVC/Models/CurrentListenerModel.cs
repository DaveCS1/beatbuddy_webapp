﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BB.BL.Domain.Users;

namespace BB.UI.Web.MVC.Models
{
    public class CurrentListenerModel
    {
        public string GroupName { get; set; }
        public User User { get; set; }
    }
}