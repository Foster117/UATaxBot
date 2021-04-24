using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;
using UATaxBot.Entities;

namespace UATaxBot.Services
{
    class MessageService
    {
        public static CustomerMessage GetCustomerMessage(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            CustomerMessage message = new CustomerMessage();
            if (messageArgs != null)
            {
                message.Text = messageArgs.Message.Text;
                message.ChatId = messageArgs.Message.From.Id.ToString();

            }
            else
            {
                message.Text = callbackArgs.CallbackQuery.Data;
                message.ChatId = callbackArgs.CallbackQuery.From.Id.ToString();
            }
            return message;
        }
    }
}
