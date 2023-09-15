using SharedInfrastructureLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CSharpWPF_NP_Sockets_Task_3.Client.ViewModels;

public class ChatViewModel: ObservableObject
{
    private Core.Client client;
    public ObservableCollection<string> ChatMessages { get; set; }
    public bool AutoResponsesMode { get; set; }
    private bool isDialogEnded { get; set; }
    private string? enteredMessage;
    public string? EnteredMessage 
    {  
        get => enteredMessage;
        set
        {
            enteredMessage = value;
            OnPropertyChanged();
        }
    }
    private string[] responsesArray;
    private Random random;
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
        random = new Random();

        client = new(ipAddress, port);
        client.MessageReceived += HandleMessageReceived;
        client.MessageSent += HandleMessageSent;
        client.ErrorOccurred += HandleErrorOccurred;
    }

    public async Task ConnectClientAsync()
    {
        client.Connect();
        if(AutoResponsesMode)
        {
            var randomMessage = responsesArray[random.Next(responsesArray.Length)];          
            client.Send(randomMessage);
            if (!randomMessage.Equals("Bye", StringComparison.OrdinalIgnoreCase))
                return;
        }
        await client.ReceiveAsync();
    }

    private async void HandleMessageReceived(string message)
    {
        Application.Current.Dispatcher.Invoke(() => {ChatMessages.Add($"Server: {message}"); });
        if (message.Equals("Bye", StringComparison.OrdinalIgnoreCase))
        {
            isDialogEnded = true;
            client.Disconnect();
            return;
        }
            
        if (AutoResponsesMode)
        {
            await Task.Delay(2000);
            client.Send(responsesArray[random.Next(responsesArray.Length)]);
        }
        await client.ReceiveAsync();
    }
    private async void HandleMessageSent(string message)
    {
        Application.Current.Dispatcher.Invoke(() => { ChatMessages.Add($"Me: {message}"); });
        EnteredMessage = string.Empty;
        if (message.Equals("Bye", StringComparison.OrdinalIgnoreCase))
        {
            isDialogEnded = true;
            client.Disconnect();
            return;
        }
        await client.ReceiveAsync();
    }

    private void HandleErrorOccurred(string exceptionMessage)
    {
        MessageBox.Show(exceptionMessage, "Error (Client)", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private bool CanExecuteSendMessageCommand(object param)
    {
        return !(AutoResponsesMode || string.IsNullOrWhiteSpace(EnteredMessage) || isDialogEnded);
    }
    private ICommand? sendMessageCommand;
    public ICommand SendMessageCommand
    {
        get
        {
            return sendMessageCommand ?? (sendMessageCommand = new RelayCommand((action) =>
            {
                if (EnteredMessage.Equals("Bye", StringComparison.OrdinalIgnoreCase))
                    client.CancelAsyncReceiving();
                client.Send(EnteredMessage!);
            }, CanExecuteSendMessageCommand));
        }
    }
}
