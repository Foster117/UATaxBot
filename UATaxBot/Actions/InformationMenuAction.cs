using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using UATaxBot.Entities;
using UATaxBot.Services;

namespace UATaxBot.Actions
{
    class InformationMenuAction
    {
        public static TelegramBotClient Bot => Program.Bot;
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;
        public static async void Go(Customer customer)
        {
            await Bot.SendTextMessageAsync(customer.ChatId, Messages.InformationText);
            LogService.PrintLogText($"{customer.FirstName} {customer.LastName}", "checked information");
            ActiveCustomersCollection.Remove(customer.ChatId);
        }
    }
}
