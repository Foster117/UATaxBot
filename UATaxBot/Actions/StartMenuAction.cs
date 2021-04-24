using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using UATaxBot.Entities;
using UATaxBot.Services;

namespace UATaxBot.Actions
{
    static class StartMenuAction
    {
        public static TelegramBotClient Bot => Program.Bot;
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;
        public static async void Go(Customer customer)
        {
            ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup(new[]{
                        new[] { new KeyboardButton("Рассчитать стоимость растаможки") },
                        new[] { new KeyboardButton("Рассчитать стоимость растаможки евробляхи") },
                        new[] { new KeyboardButton("Контакты"), new KeyboardButton("Информация") },
                        new[] { new KeyboardButton("Показать курсы НБУ") }
                        });

            await Bot.SendTextMessageAsync(customer.ChatId, Messages.StartText, replyMarkup: replyKeyboard);
            LogService.PrintLogText($"{customer.FirstName} {customer.LastName}", "started the bot");
            ActiveCustomersCollection.Remove(customer.ChatId);
        }
    }
}
