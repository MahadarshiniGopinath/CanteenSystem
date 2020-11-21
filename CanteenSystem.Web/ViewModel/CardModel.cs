using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenSystem.Web.ViewModel
{
    public class CardModel
    {
        public int UserProfileId { get; set; }
        public int CardId { get; set; } 
        public int BankCardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryDate { get; set; }
        public decimal Amount { get; set; }
        public int CVV { get; set; }
    }
}
