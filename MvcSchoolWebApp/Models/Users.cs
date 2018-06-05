using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Models
{
    public class Users
    {
        [Required]
        public string userName { get; set; }
        [Required]
        public string userPassword { get; set; }

        public string user_id { get; set; }
        public string user_earea { get; set; }
        public string user_fullname { get; set; }
        public string user_image { get; set; }
        public string user_role { get; set; }
        
    }
}