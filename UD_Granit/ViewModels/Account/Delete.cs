﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UD_Granit.ViewModels.Account
{
    public class Delete
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Referer { set; get; }
        public bool CanDelete { set; get; }
    }
}