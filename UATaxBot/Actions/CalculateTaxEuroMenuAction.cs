using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using UATaxBot.Entities;
using UATaxBot.Enums;

namespace UATaxBot.Actions
{
    class CalculateTaxEuroMenuAction
    {
        public static TelegramBotClient Bot => Program.Bot;
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;
        public static void Go(Customer customer)
        {
            customer.ActionType = ActionType.TaxCalculation;
            customer.TaxForm = new TaxForm(customer);
            TaxCalculation.TaxCalculationProcess(Bot, customer);
        }
    }
}
