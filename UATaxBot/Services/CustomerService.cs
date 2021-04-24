using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;
using UATaxBot.Entities;

namespace UATaxBot.Services
{
    class CustomerService
    {
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;
        public Customer GetCustomer(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            Customer customer = new Customer();
            if (messageArgs != null)
            {
                customer.MessageText = messageArgs.Message.Text;
                customer.ChatId = messageArgs.Message.From.Id.ToString();
                customer.FirstName = messageArgs.Message.From.FirstName;
                customer.LastName = messageArgs.Message.From.LastName;
            }
            else
            {
                customer.MessageText = callbackArgs.CallbackQuery.Data;
                customer.ChatId = callbackArgs.CallbackQuery.From.Id.ToString();
                customer.FirstName = callbackArgs.CallbackQuery.From.FirstName;
                customer.LastName = callbackArgs.CallbackQuery.From.LastName;
                //message = callbackArgs.CallbackQuery.Message;
            }
            return customer;
        }

        public bool CheckForExistantCustomer(string chatId)
        {
            return ActiveCustomersCollection.ContainsKey(chatId);
        }
    }
}
