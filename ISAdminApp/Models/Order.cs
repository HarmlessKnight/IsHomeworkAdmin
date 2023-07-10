using Newtonsoft.Json;
using System.Collections.Generic;

namespace ISAdminApp.Models
{
    public class Order
    {
            public int Id { get; set; }

            [JsonProperty("userID")]
            public string UserID { get; set; }

            public ShopUser OrderedBy { get; set; }

            public List<TicketInOrder> Tickets { get; set; }
        
    }
}
