using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using UATaxBot.Entities;
using UATaxBot.Enums;
using UATaxBot.Services;

namespace UATaxBot.Actions
{
    class RouteAction
    {
        public static TelegramBotClient Bot => Program.Bot;
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;
        public static void Go(Customer customer)
        {
            ActiveCustomersCollection.TryGetValue(customer.ChatId, out customer);
            if (customer == null)
            {
                return;
            }
            switch (customer.ActionType)
            {
                case ActionType.TaxCalculation:
                    TaxCalculation.TaxCalculationProcess(Bot, customer);
                    break;
                default:
                    break;
            }
        }
    }
}
