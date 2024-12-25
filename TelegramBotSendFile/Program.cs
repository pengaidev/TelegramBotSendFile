using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotSendFile;
public class Program
{
    private static readonly string BotToken = "";
    // Thread-safe collection to store chatId and file path
    private static ConcurrentDictionary<long, string> Chats = new ConcurrentDictionary<long, string>();

    private static TelegramBotClient BotClient = null!;

    static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        BotClient = new TelegramBotClient(BotToken, cancellationToken: cts.Token);
        var me = await BotClient.GetMe();

        BotClient.OnError += OnError;
        BotClient.OnMessage += OnMessage;
        BotClient.OnUpdate += OnUpdate;

        Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

        Console.WriteLine("Bot is listening for incoming messages...");


        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. List Available Chats");
            Console.WriteLine("2. Send a File");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option (1-3): ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ListChats();
                    break;
                case "2":
                    await SendFileAsync();
                    break;
                case "3":
                    Console.WriteLine("Exiting...");
                    cts.Cancel();
                    return;
                default:
                    Console.WriteLine("Invalid option. Please select 1, 2, or 3.");
                    break;
            }
        }
    }

    private static async Task SendFileAsync()
    {
        //if (Chats.IsEmpty)
        //{
        //    Console.WriteLine("No chats available to send the file. Ensure the bot is added to at least one chat.");
        //    return;
        //}

        var filePath = @"";
        try
        {
            var message = "សូមជំរាបសួរ," +
            "ខ្ញុំសូមជូនដំណឹងថាវិក្កយបត្រ លេខ TIN005-20241127160322 របស់លោកអ្នក ត្រូវបានភ្ជាប់ជាមួយសារនេះ។" +
            "សូមពិនិត្យមើល និងទូទាត់ប្រាក់ក្នុងរយៈពេល [ថ្ងៃផុតកំណត់]។ប្រសិនបើមានសំណួរឬត្រូវការព័ត៌មានបន្ថែម សូមកុំរាក់ទាក់ក្នុងការទំនាក់ទំនងមកខ្ញុំ។" +
            "\nសូមអរគុណច្រើនសម្រាប់ការគាំទ្ររបស់លោកអ្នក។" +
            "\nគោរពយ៉ាងខ្ពង់ខ្ពស់,";
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await BotClient.SendDocument(
                    chatId: "",
                    document: stream,
                    caption: message
                );
            }

            Console.WriteLine("File sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending file: {ex.Message}");
        }
    }

    private static void ListChats()
    {
        if (Chats.IsEmpty)
        {
            Console.WriteLine("No chats available. The bot hasn't interacted with any chats yet.");
            return;
        }

        Console.WriteLine("\nAvailable Chats:");
        foreach (var chat in Chats)
        {
            Console.WriteLine($"- ID: {chat.Key}, Title: {chat.Value}");
        }
    }

    private static async Task OnUpdate(Update update)
    {
    }

    private static async Task OnMessage(Message message, UpdateType type)
    {
        if (message.Type == MessageType.Text)
        {
            var chatId = message.Chat.Id;
            var chatTitle = message.Chat.Title ?? message.Chat.Username ?? "Private Chat";

            // Add or update the chat in the collection
            Chats.AddOrUpdate(chatId, chatTitle, (key, oldValue) => chatTitle);

            if (message.Text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
            {
                await BotClient.SendMessage(
                    chatId,
                    "Hello! This is FaceAzure Desktop Bot by CAS-BIZ Technology.\n\n" +
                    "Below is your Telegram Group ID." +
                    "Please tab to copy it and switch bcak to FaceAzure Application to complete the setup process!" +
                    $"\n\nYour Telegram Group ID: {chatId}"
                );
            }
            Console.WriteLine($"\nReceived a message from chat '{chatTitle}' (ID: {chatId}).");
        }
    }

    private static async Task OnError(Exception exception, HandleErrorSource source)
    {
    }
}
