using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YorkScjool.Models
{
	public class TrialLessonRequest
	{
        public string Name { get; set; }
        public string PhoneCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Messenger { get; set; }
        public string Recipient { get; set; }
    }
}