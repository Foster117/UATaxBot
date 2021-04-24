using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using UATaxBot.Entities;
using UATaxBot.Services;

namespace UATaxBot.Actions
{
    class ContactAction
    {
        public static TelegramBotClient Bot => Program.Bot;
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;
        public static async void Go(Customer customer)
        {
            await Bot.SendTextMessageAsync(customer.ChatId, Messages.ContactsText);
            LogService.PrintLogText($"{customer.FirstName} {customer.LastName}", "checked contacts");
            ActiveCustomersCollection.Remove(customer.ChatId);
        }
    }
}
