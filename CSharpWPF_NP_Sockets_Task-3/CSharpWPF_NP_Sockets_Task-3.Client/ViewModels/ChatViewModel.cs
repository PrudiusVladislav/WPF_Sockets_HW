using SharedInfrastructureLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSharpWPF_NP_Sockets_Task_3.Client.ViewModels;

public class ChatViewModel: ObservableObject
{
    private Core.Client client;
    public ObservableCollection<string> ChatMessages { get; set; }
    public bool AutoResponsesMode { get; set; }
    public string? EnteredMessage { get; set; }
    private string[] responsesArray;
    public ChatViewModel(bool autoInputType, string ipAddress, int port) 
    {
        AutoResponsesMode = autoInputType;
        ChatMessages = new ObservableCollection<string>();
        responsesArray = new string[]
        {
            "Hello, how can I assist you?",
            "Thank you for your message.",
            "I'm here to help.",
            "Please wait a moment.",
            "Message received and noted.",
            "Got it, processing your request.",
            "Is there anything else you need?",
            "Request successfully completed.",
            "Your request has been forwarded.",
            "Have a great day!",
            "Bye"
        };

        client = new(ipAddress, port);
        client.MessageReceived += HandleMessageReceived;
        client.MessageSent += HandleMessageSent;
        client.Connect();
    }

    private void HandleMessageReceived(string message)
    {
        ChatMessages.Add($"Server: {message}");
        if(message.Equals("Bye", StringComparison.OrdinalIgnoreCase))
            client.Disconnect();
        if (AutoResponsesMode)
        {
            client.Send(responsesArray[new Random().Next(responsesArray.Length)]);
        }
    }
    private void HandleMessageSent(string message)
    {
        ChatMessages.Add($"Me: {message}");
    }

    private bool CanExecuteSendMessageCommand(object param)
    {
        return !AutoResponsesMode && !string.IsNullOrWhiteSpace(EnteredMessage);
    }
    private ICommand? sendMessageCommand;
    public ICommand SendMessageCommand
    {
        get
        {
            return sendMessageCommand ?? (sendMessageCommand = new RelayCommand((action) =>
            {
                client.Send(EnteredMessage);
                EnteredMessage = string.Empty;
                if (EnteredMessage.Equals("Bye", StringComparison.OrdinalIgnoreCase))
                {
                    AutoResponsesMode = true;
                    client.Disconnect();
                    return;
                }
                client.Receive();           
            }, CanExecuteSendMessageCommand));
        }
    }
}
