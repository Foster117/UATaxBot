using System;
using UATaxBot.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using UATaxBot.Entities;
using UATaxBot.Services;

// bot.imex@ukr.net
// qweasdzxc1234

// http://www.freenom.com/
// bot.imex@ukr.net
// qweasdzxc1234

//"1509689010:AAE7TSS0mDGnLS1x-SFsXR2k-L_IM1X-cq0"    TRUE bot
//"1560358205:AAG4thqkHip7fBv2XabKntdZeErGFHM_290"    Test bot

namespace UATaxBot
{
    class Program
    {
        public static readonly TelegramBotClient Bot = new TelegramBotClient("1560358205:AAG4thqkHip7fBv2XabKntdZeErGFHM_290");
        public static Dictionary<string, Customer> ActiveCustomersCollection => new Dictionary<string, Customer>();
        public static ActionManager ActionManager => new ActionManager();
        public static CustomerMessage UserMessage { get; private set; }
        public static CustomerService CustomerService => new CustomerService();
        public static MessageService MessageService => new MessageService();

        static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnReceiveError += BotOnReceiveError;
            Bot.StartReceiving(Array.Empty<UpdateType>());

            Visualizer.DrawStartText(Bot.GetMeAsync().Result);

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            Visualizer.DrawErrorMessage(e.ApiRequestException.ErrorCode, e.ApiRequestException.Message);
        }

        private static void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            MessageHandler(null, e);
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            MessageHandler(e, null);
        }


        ///////////////////////
        ///////////////////////
        private static void MessageHandler(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            Customer customer;
            CustomerMessage message = MessageService.GetCustomerMessage(messageArgs, callbackArgs);

            if (CustomerService.CheckForExistantCustomer(message.ChatId))
            {
                 ActiveCustomersCollection.TryGetValue(message.ChatId, out customer);
            }
            else
            {
                 customer = CustomerService.GetCustomer(messageArgs, callbackArgs);
                 ActiveCustomersCollection.Add(customer.ChatId, customer);
            }
            ActionManager.SelectAction(customer);
        }
    }
}