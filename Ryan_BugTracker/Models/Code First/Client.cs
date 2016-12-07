using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ryan_BugTracker.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}