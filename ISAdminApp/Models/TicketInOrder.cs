using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace ISAdminApp.Models
{
    public class TicketInOrder
    {
        [ForeignKey("TicketId")]
        public int TicketId { get; set; }

        public Ticket Ticket { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int Quantity { get; set; }
    }
}
