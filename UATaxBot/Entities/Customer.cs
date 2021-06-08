using System;
using System.Collections.Generic;
using System.Text;
using UATaxBot.Enums;

namespace UATaxBot.Entities
{
    class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ChatId { get; set; }
        public string TargetId { get; set; }
        public string MessageText { get; set; }
        public ActionType ActionType { get; set; }
        public TaxForm TaxForm { get; set; } = null;
        public TaxEuroForm TaxEuroForm { get; set; } = null;
    }
}
