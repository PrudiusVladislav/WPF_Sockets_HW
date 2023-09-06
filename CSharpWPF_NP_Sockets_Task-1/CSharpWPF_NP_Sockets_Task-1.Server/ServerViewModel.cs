using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSharpWPF_NP_Sockets_Task_1.Server;

public class ServerViewModel
{
    public ObservableCollection<string> ReceivedMessages { get; set; }
    public ObservableCollection<string> SentMessages { get; set; }
    private Server server;
    public ServerViewModel() 
    {
        ReceivedMessages = new ObservableCollection<string>();
        SentMessages = new ObservableCollection<string>();

        server = new Server();
        server.MessageReceived += HandleMessageReceived;
        server.MessageSent += HandleMessageSent;
        server.ErrorOccurred += HandleErrorOccurred;
        server.Start();
    }

    private void HandleMessageReceived(string message)
    {
        ReceivedMessages.Add(message);
    }

    private void HandleMessageSent(string message)
    {
        SentMessages.Add(message);
    }
    private void HandleErrorOccurred(string exceptionMessage)
    {
        MessageBox.Show(exceptionMessage);
    }

}
