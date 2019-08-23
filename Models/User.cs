using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace belt_exam.Models
{

    public class User
    {
        [Key]
        [Column("id")]
        public int Userid { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Your name must be more than 2 characters!")]
        [Column("name", TypeName = "VARCHAR(255)")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Column("email", TypeName = "VARCHAR(255)")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [SafePassword]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        [Column("password", TypeName = "VARCHAR(255)")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Column("created_at")]
        public DateTime Created_At { get; set; } = DateTime.Now;
        [Column("updated_at")]
        public DateTime Updated_At { get; set; } = DateTime.Now;
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string password_confirm { get; set; }
        public List<AnActivity> madeActivites { get; set; }
        public List<AttendeeList> attendingActivites { get; set; }
    }
    public class SafePasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = (string)value;
            ErrorMessage = string.Empty;

            var hasNumber = new Regex(@"[0-9]+");
            var hasChar = new Regex(@"[a-zA-Z]+");


            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasChar.IsMatch(input))
            {
                return new ValidationResult("Password should atleast one letter, number, and character");
            }
            if (!hasNumber.IsMatch(input))
            {
                return new ValidationResult("Password should atleast one letter, number, and character");
            }

            if (!hasSymbols.IsMatch(input))
            {
                return new ValidationResult("Password should atleast one letter, number, and character");
            }
            else
            {
                return ValidationResult.Success;

            }
        }
        // bool hasLetter = false;
        // bool hasCharacter = false;
        // bool hasNum = false;
        // foreach(char c in (string)value){
        //     if( char.IsUpper(c) ) hasCharacter = true ;
        //      else if ( char.IsLower(c) ) hasCharacter = true ;
        //      else if ( char.IsDigit(c) ) hasNum = true ;
        //      else if ( char.IsChar(c) ) hasNum = true ;

        // }
    }
}
