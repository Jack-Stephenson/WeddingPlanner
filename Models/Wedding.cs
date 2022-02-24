using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}
        [Required]
        [Display(Name ="Wedder One")]
        public string WedderOne {get;set;}
        [Required]
        [Display(Name ="Wedder Two")]
        public string WedderTwo {get;set;}
        [Required]
        [Display(Name ="Date of Wedding")]
        [DataType(DataType.Date)]
        public DateTime Date {get;set;}
        [Required]
        public int CreatorId {get;set;}
        [Required]
        [Display(Name ="Location")]
        public string Location {get;set;}
        public List<Attendance> Guests {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}