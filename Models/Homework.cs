using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YorkScjool.Models
{
    public class Homework
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
    }

}