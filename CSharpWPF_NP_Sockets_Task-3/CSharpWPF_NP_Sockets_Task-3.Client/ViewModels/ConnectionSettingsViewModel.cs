using CSharpWPF_NP_Sockets_Task_3.Client.Core;
using SharedInfrastructureLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSharpWPF_NP_Sockets_Task_3.Client.ViewModels;

public class ConnectionSettingsViewModel: ObservableObject
{
    private MainClientViewModel mainClientViewModel;
    private bool autoInputType;
    public string? IpAddress { get; set; }
    public string? Port { get; set; }
    public ConnectionSettingsViewModel(MainClientViewModel mainClientViewModel)
    {
        this.mainClientViewModel = mainClientViewModel;
        autoInputType = true;
    }

    private bool CanExecuteManualInputCommand(object param)
    {
        return autoInputType == true;
    }
    private ICommand? manualInputCommand;
    public ICommand ManualInputCommand
    {
        get
        {
            return manualInputCommand ?? (manualInputCommand = new RelayCommand((action) =>
            {
                autoInputType = false;
            }, CanExecuteManualInputCommand));
        }
    }

    private bool CanExecuteAutoInputCommand(object param)
    {
        return autoInputType == false;
    }
    private ICommand? autoInputCommand;
    public ICommand AutoInputCommand
    {
        get
        {
            return autoInputCommand ?? (autoInputCommand = new RelayCommand((action) =>
            {
                autoInputType = true;
            }, CanExecuteAutoInputCommand));
        }
    }

    private bool CanExecuteConnectCommand(object param)
    {
        return !(string.IsNullOrWhiteSpace(IpAddress) || string.IsNullOrWhiteSpace(Port) || !int.TryParse(Port,out _));
    }
    private ICommand? connectCommand;
    public ICommand ConnectCommand
    {
        get
        {
            return connectCommand ?? (connectCommand = new RelayCommand((action) =>
            {
                mainClientViewModel.CurrentViewModel = new ChatViewModel(autoInputType, IpAddress, int.Parse(Port));
            }, CanExecuteConnectCommand));
        }
    }
}
