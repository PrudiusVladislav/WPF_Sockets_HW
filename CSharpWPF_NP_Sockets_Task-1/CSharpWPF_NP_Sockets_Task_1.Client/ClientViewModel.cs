using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CSharpWPF_NP_Sockets_Task_1.Client;

public class ClientViewModel
{
    public ObservableCollection<string> ReceivedMessages { get; set; }
    public ObservableCollection<string> SentMessages { get; set; }
    private Client client;
    public ClientViewModel()
    {
        ReceivedMessages = new ObservableCollection<string>();
        SentMessages = new ObservableCollection<string>();

        client = new Client();
        client.MessageReceived += HandleMessageReceived;
        client.MessageSent += HandleMessageSent;
        client.Connect();     
    }

    private void HandleMessageReceived(string message)
    {
        ReceivedMessages.Add(message);
    }

    private void HandleMessageSent(string message)
    {
        SentMessages.Add(message);
    }

    private bool CanExecuteSendGreetings(object? param)
    {
        return SentMessages.Count == 0;
    }

    private ICommand? _sendGreetingsCommand;
    public ICommand SendGreetingsCommand
    {
        get
        {
            return _sendGreetingsCommand ?? (_sendGreetingsCommand = new RelayCommand((action) =>
            {
                client.SendAndReceive("Hello, Server!");
            }, CanExecuteSendGreetings));
        }
    }
}
