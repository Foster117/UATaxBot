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

        private static void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            MessageHandler(null, e);
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs e)
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
                        new[] { new KeyboardButton("Рассчитать стоимость растаможки") },
                        new[] { new KeyboardButton("Контакты"), new KeyboardButton("Информация") }
                    });
                    await Bot.SendTextMessageAsync(chatId, "Привет, здесь можно посчитать платежи для уплаты всех налогов на таможне.\nРасчёты верны только для б/у легковых автомобилей, предназначенных для перевозки пассажиров.\nПриступай или прочитай Информацию.", replyMarkup: replyKeyboard);
                    Visualizer.DrawLogText($"{message.From.FirstName} {message.From.LastName}", "started bot");
                    return;
                /////--------------------------////
                case "Рассчитать стоимость растаможки":
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
                    string contacts = "Растаможка авто\n\nтел: +380635205050\nTelegram: @quvag\n\n";
                    await Bot.SendTextMessageAsync(message.Chat.Id, contacts);
                    Visualizer.DrawLogText($"{message.From.FirstName} {message.From.LastName}", "checked contacts");
                    return;
                /////--------------------------////
                case "Информация":
                    calcTaxData.Remove(message.From.Id.ToString());
                    string information = "! Расчёты верны только для б/у легковых автомобилей, предназначенных для перевозки пассажиров;\n\n" +
                        "! Вычисления проводятся на текущую дату, поскольку курс доллара или евро к гривне меняется каждый банковский день. Имей это ввиду, если планируешь заранее переводить средства на расчётный счёт таможни. При условии ослабления гривны, этой суммы может быть недостаточно на день таможенной очистки (растаможки);\n\n" +
                        "- Стоимостью транспортировки авто является сумма всех понесенных тобой затрат на доставку автомобиля до таможенной границы Украины. Ты также можешь добавить сюда комиссию аукциона, и другие расходы;\n\n" +
                        "- Стоимостью автомобиля является цена авто, указанная в счёт-фактуре или договоре купли-продажи;\n\n" +
                        "- Стоимость транспортировки авто до таможенной границы Украины складывается со стоимостью самого автомобиля и является таможенной стоимостью (ТС);\n\n" +
                        "- Налоги рассчитываются исходя из таможенной стоимости автомобиля\n\n" +
                        "- Общая стоимость таможенных платежей рассчитывается по формуле: \nСУММА = АКЦИЗ + ПОШЛИНА + НДС;\n\n" +
                        "- Акцизный сбор (акциз) рассчитывается исходя из нескольких параметров автомобиля: типа и объёма двигателя, года производства авто.\n\n" +
                        "- Пошлина равна 10% от таможенной стоимости;\n\n" +
                        "- НДС (ставка 20%) рассчитывается по формуле: \nНДС = (ТС + АКЦИЗ + ПОШЛИНА) * 0.2;\n\n" +
                        "- Для точности расчёта акциза на бензиновые и дизельные автомобили рекомендую указывать точный объём двигателя в кубических сантиметрах, например 1596. Часто эта информация отображена в свидетельстве о регистрации или техническом паспорте авто.";
                    await Bot.SendTextMessageAsync(message.Chat.Id, information);
                    Visualizer.DrawLogText($"{message.From.FirstName} {message.From.LastName}", "checked information");
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

                case 3:
                    var inlineKeyboard4 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("Бензин и/или газ", "petrol"),
                                   InlineKeyboardButton.WithCallbackData("Дизель", "diesel")},
                            new[]{ InlineKeyboardButton.WithCallbackData("Гибрид", "hybrid"),
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
    }
}