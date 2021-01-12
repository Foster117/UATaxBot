using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;

namespace UATaxBot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("1509689010:AAE7TSS0mDGnLS1x-SFsXR2k-L_IM1X-cq0");
        static Dictionary<string, TaxForm> calcTaxData = new Dictionary<string, TaxForm>();
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
            Console.WriteLine("Error!");
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            MessageHandler(null, e);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            MessageHandler(e, null);
        }

        private static async void MessageHandler(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            Message message;
            string messageText;
            string chatId;
            TaxForm userForm;

            if (messageArgs != null)
            {
                message = messageArgs.Message;
                messageText = messageArgs.Message.Text;
                chatId = messageArgs.Message.From.Id.ToString();
            }
            else
            {
                message = callbackArgs.CallbackQuery.Message;
                messageText = callbackArgs.CallbackQuery.Data;
                chatId = callbackArgs.CallbackQuery.From.Id.ToString();
            }

            switch (messageText)
            {
                case "/start":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]{
                        new[] { new KeyboardButton("Расчитать стоимость растаможки") },
                        new[] { new KeyboardButton("Контакты"), new KeyboardButton("Перезвоните мне") }
                    });
                    await Bot.SendTextMessageAsync(chatId, "Привет, я Taxbot!", replyMarkup: replyKeyboard);
                    return;
                /////--------------------------////
                case "Расчитать стоимость растаможки":
                    TaxForm form = new TaxForm(message.From.Id.ToString(), message.Chat.Id.ToString(), $"{message.From.FirstName} {message.From.LastName}", ActionType.TaxCalculation);
                    calcTaxData.Remove(message.From.Id.ToString());
                    calcTaxData.Add(message.From.Id.ToString(), form);
                    (string, int) firstStageText = form.GetCalcTaxStageText();
                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("USD", "USD"),
                                   InlineKeyboardButton.WithCallbackData("EUR", "EUR")}
                            });
                    await Bot.SendTextMessageAsync(message.Chat.Id, firstStageText.Item1, replyMarkup: inlineKeyboard);
                    return;
                /////--------------------------////
                case "Контакты":
                    calcTaxData.Remove(message.From.Id.ToString());
                    string contacts = "Васямба Андреевич\nрастаможит любое ваше корыто\nтел: +380635205050";
                    await Bot.SendTextMessageAsync(message.Chat.Id, contacts);
                    return;
                /////--------------------------////
                case "Перезвоните мне":
                    calcTaxData.Remove(message.From.Id.ToString());
                    //string contacts = "Васямба Андреевич\nрастаможит любое ваше корыто\nтел: +380635205050";
                    //await Bot.SendTextMessageAsync(message.Chat.Id, contacts);
                    return;
                /////--------------------------////
                default:
                    calcTaxData.TryGetValue(chatId, out userForm);
                    if (userForm == null)
                        return;

                    switch (userForm.ActionType)
                    {
                        case ActionType.TaxCalculation:
                            TaxCalculationProcess(userForm, messageText, message);
                            break;
                        case ActionType.SendContact:
                            SendContactProcess(userForm, messageText, message);
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }

        private static async void TaxCalculationProcess(TaxForm findedForm, string messageText, Message message)
        {
            bool validation = findedForm.SetCalcTaxParam(messageText);
            if (!validation)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Введены некорректные данные. Попробуйте еще раз.");
            }
            var stageText = findedForm.GetCalcTaxStageText();
            switch (stageText.Item2)
            {
                case 1:
                    var inlineKeyboard1 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("USD", "USD"),
                                   InlineKeyboardButton.WithCallbackData("EUR", "EUR")}
                            });
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1, replyMarkup: inlineKeyboard1);
                    return;

                case 4:
                    var inlineKeyboard4 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("Бензин и/или газ", "petrol"),
                                   InlineKeyboardButton.WithCallbackData("Дизель", "diesel")},
                            new[]{ InlineKeyboardButton.WithCallbackData("Гибрид", "gybrid"),
                                   InlineKeyboardButton.WithCallbackData("Электро", "electro")}
                            });
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1, replyMarkup: inlineKeyboard4);
                    return;

                case 6:
                    var inlineKeyboard6 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("USD", "USD"),
                                   InlineKeyboardButton.WithCallbackData("EUR", "EUR")}
                            });
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1, replyMarkup: inlineKeyboard6);
                    return;

                case -1:
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1);
                    calcTaxData.Remove(message.From.Id.ToString());
                    return;

                default:
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1);
                    return;
            }
        }

        private static async void SendContactProcess(TaxForm findedForm, string messageText, Message message)
        {
        }

    }
}