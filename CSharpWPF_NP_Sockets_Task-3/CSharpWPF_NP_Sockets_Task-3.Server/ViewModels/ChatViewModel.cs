﻿using SharedInfrastructureLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CSharpWPF_NP_Sockets_Task_3.Server.ViewModels;

public class ChatViewModel: ObservableObject
{
    private Core.Server server;
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
    private string? chatPartnerName;
    public string? ChatPartnerName 
    {
        get => chatPartnerName;
        set
        {
            if(chatPartnerName != value)
            {
                chatPartnerName = value;
                OnPropertyChanged();
            }     
        }
    }
    private string[] responsesArray;
    private Random random;
    public ChatViewModel(bool autoInputType)
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
        random = new();

        server = new();
        server.MessageReceived += HandleMessageReceived;
        server.MessageSent += HandleMessageSent;
        server.ErrorOccurred += HandleErrorOccurred;
    }

    public async Task StartServerAsync()
    {
        server.Start();

        await server.ReceiveAsync();
        ChatPartnerName = $"Chat partner name: Client ({server.ChatPartner})";
    }


    private async void HandleMessageReceived(string message)
    {
        Application.Current.Dispatcher.Invoke(() => { ChatMessages.Add($"Client: {message}"); });
        if (message.Equals("Bye", StringComparison.OrdinalIgnoreCase))
        {
            isDialogEnded = true;
            server.Dispose();
            return;
        }
        if (AutoResponsesMode)
        {
            await Task.Delay(2000);
            server.Send(responsesArray[random.Next(responsesArray.Length)]);
        }
        await server.ReceiveAsync();
    }
    private async void HandleMessageSent(string message)
    {
        Application.Current.Dispatcher.Invoke(() => { ChatMessages.Add($"Me: {message}"); });
        EnteredMessage = string.Empty;
        if (message.Equals("Bye", StringComparison.OrdinalIgnoreCase))
        {      
            isDialogEnded = true;
            server.Dispose();
            return;
        } 
        await server.ReceiveAsync();
    }

    private void HandleErrorOccurred(string exceptionMessage)
    {
        MessageBox.Show(exceptionMessage, "Error (Server)", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    server.CancelAsyncReceiving();
                server.Send(EnteredMessage!);
            }, CanExecuteSendMessageCommand));
        }
    }
}
