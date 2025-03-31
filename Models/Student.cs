using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YorkScjool.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Mobile { get; set; }

        [Required]
        public string Country { get; set; }

        public string ProfilePicture { get; set; }

        // New Social Media Fields
        public string Telegram { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public string Snapchat { get; set; }

        // Link Student to an ApplicationUser
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Homework> Homeworks { get; set; }

        public Student()
        {
            Lessons = new List<Lesson>();
            Homeworks = new List<Homework>();
        }
    }
}
