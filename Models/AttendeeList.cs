using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace belt_exam.Models
{
    public class AttendeeList
    {
        [Key]
        public int id { get; set; }
        public int Userid {get;set;}
        public int Activityid {get;set;}
        public User User {get;set;}
        public AnActivity AnActivity {get;set;}
    }
}
