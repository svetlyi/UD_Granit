﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UD_Granit.Models
{
    public class Speciality
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "Номер специальности")]
        public string Number { set; get; }

        [Required]
        [Display(Name = "Отрасль науки")]
        public string ScienceBranch { set; get; }

        [Required]
        [Display(Name = "Название специальности")]
        public string Name { set; get; }

        public virtual ICollection<Member> Members { set; get; }
    }
}