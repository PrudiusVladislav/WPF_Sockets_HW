using SharedInfrastructureLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public string? EnteredMessage { get; set; }
    public string? ChatPartnerName { get; set; }
    public ChatViewModel(bool autoInputType)
    {
        AutoResponsesMode = autoInputType;
        ChatMessages = new ObservableCollection<string>();
        server = new();
        server.MessageReceived += HandleMessageReceived;
        server.MessageSent += HandleMessageSent;
        server.ErrorOccurred += HandleErrorOccurred;
        server.Start();
        ChatPartnerName = $"Chat partner name: Client ({server.ChatPartner})";
    }

    private void HandleMessageReceived(string message)
    {
        ChatMessages.Add($"Client: {message}");
    }
    private void HandleMessageSent(string message)
    {
        ChatMessages.Add($"Me: {message}");
    }

    private void HandleErrorOccurred(string exceptionMessage)
    {
        MessageBox.Show(exceptionMessage, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
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
                server.MessageToSend = EnteredMessage;
                EnteredMessage = string.Empty;
            }, CanExecuteSendMessageCommand));
        }
    }
}
