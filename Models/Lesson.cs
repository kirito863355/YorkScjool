using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YorkScjool.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath {  get; set; }
        public DateTime DateAdded { get; set; }

        // Foreign Key for Student
        public string Student_Id { get; set; }  // Ensure this exists
        public virtual ApplicationUser Student { get; set; } // Navigation property
    }

}
