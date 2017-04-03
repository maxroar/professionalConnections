using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace csbb0328.Models
{
    
    public class User : BaseEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username {get; set;}
        public string email { get; set; }
        public string password { get; set; }
        public string description { get; set; }
    
        
    }
}