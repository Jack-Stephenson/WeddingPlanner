using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [Display(Name = "First name")]
        public string FirstName {get;set;}
        [Required]
        [Display(Name = "Last Name")]
        public string LastName {get;set;}
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email {get;set;}
        [Required]
        [Display(Name = "Password")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password {get;set;}
        [NotMapped]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword {get;set;}
        public List<Attendance> Weddings {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}