using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace csbb0328.Models
{
    
    public class RequestedUser : BaseEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public int userId1 { get; set; }
        public int userId2 { get; set; }
    
        
    }
}