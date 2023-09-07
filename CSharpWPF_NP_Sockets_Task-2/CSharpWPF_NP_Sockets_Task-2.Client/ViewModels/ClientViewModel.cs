using CSharpWPF_NP_Sockets_Task_2.Client.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSharpWPF_NP_Sockets_Task_2.Client.ViewModels;

public class ClientViewModel: INotifyPropertyChanged
{
    public string? SelectedRequestOption { get; set; }
    public ObservableCollection<string> RequestOptions { get; set; }
    private string? serverResponse;
    public string? ServerResponse 
    {
        get => serverResponse;
        set
        {
            serverResponse = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private Core.Client client;
    public ClientViewModel()
    {
        RequestOptions = new ObservableCollection<string>() 
        {
            "Date", 
            "Time"
        };

        client = new();
        client.MessageReceived += HandleMessageReceived;
        client.Connect();
    }

    private void HandleMessageReceived(string message)
    {
        ServerResponse = message;
        client.Disconnect();
    }

    private bool CanExecuteMakeRequest(object? param)
    {
        return string.IsNullOrEmpty(ServerResponse);
    }

    private ICommand? _requestCommand;

    public ICommand RequestCommand
    {
        get
        {
            return _requestCommand ?? (_requestCommand = new RelayCommand((action) =>
            {
                client.SendAndReceive(SelectedRequestOption ?? " " );
            }, CanExecuteMakeRequest));
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
