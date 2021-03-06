﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UD_Granit.Models;

namespace UD_Granit.ViewModels.Account
{
    public class Details
    {
        public User User { set; get; }

        public bool CanEdit { set; get; }
        public bool CanRemove { set; get; }
        public bool CanShowAdditionalInfo { set; get; }
    }
}