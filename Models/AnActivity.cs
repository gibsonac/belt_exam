using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace belt_exam.Models
{
    public class AnActivity
    {
        [Key]
        [Column("id")]
        public int Activityid { get; set; }
        [Required]
        [Column("title", TypeName = "VARCHAR(255)")]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Required]
        [Column("description")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [FutureDate]
        [Column("date")]
        public DateTime date {get;set;}
        [Required]
        [Column("duration")]
        public int Duration {get;set;}
        [Required]
        [Column("time")]
        public DateTime Time {get;set;}
        [Required]
        [Column("hour_min_day")]
        public string Hour_Min_Day {get;set;}
        [Column("created_at")]
        public DateTime Created_At { get; set; } = DateTime.Now;
        [Column("updated_at")]
        public DateTime Updated_At { get; set; } = DateTime.Now;
        public int Userid {get;set;}
        public User Creator {get;set;}
        public List<AttendeeList> RSVPList {get;set;}
    }
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime CurrentTime = DateTime.Now;
            if (((DateTime)value) < CurrentTime)
            {
                return new ValidationResult("You must pick a future date!");
            }
            return ValidationResult.Success;
        }
    }
}
