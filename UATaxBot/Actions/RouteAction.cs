using System.Collections.Generic;
using Telegram.Bot;
using UATaxBot.Entities;
using UATaxBot.Enums;

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
                case ActionType.TaxAccurate:
                    TaxAccurateCalculation.TaxCalculationProcess(Bot, customer);
                    break;
                case ActionType.TaxEurobadgeAccurate:
                    TaxEuroCalculation.TaxCalculationProcess(Bot, customer);
                    break;
                default:
                    break;
            }
        }
    }
}
