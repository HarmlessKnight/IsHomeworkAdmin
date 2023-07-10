using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace ISAdminApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string MovieTitle { get; set; }

        public string MovieImage { get; set; }

        public double Price { get; set; }

        public string Genre { get; set; }

        public DateTime DateTime { get; set; }
    }
}
