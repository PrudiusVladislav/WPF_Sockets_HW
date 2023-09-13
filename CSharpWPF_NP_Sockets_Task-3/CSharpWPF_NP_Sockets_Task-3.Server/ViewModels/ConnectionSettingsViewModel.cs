using SharedInfrastructureLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSharpWPF_NP_Sockets_Task_3.Server.ViewModels;

public class ConnectionSettingsViewModel: ObservableObject
{
    private MainServerViewModel mainServerViewModel;
    private bool autoInputType;
    public ConnectionSettingsViewModel(MainServerViewModel mainServerViewModel)
    {
        this.mainServerViewModel = mainServerViewModel;
        autoInputType = true;
    }

    private bool CanExecuteManualResponsesCommand(object param)
    {
        return autoInputType == true;
    }
    private ICommand? manualResponsesCommand;
    public ICommand ManualResponsesCommand
    {
        get
        {
            return manualResponsesCommand ?? (manualResponsesCommand = new RelayCommand((action) =>
            {
                autoInputType = false;
            }, CanExecuteManualResponsesCommand));
        }
    }

    private bool CanExecuteAutoResponsesCommand(object param)
    {
        return autoInputType == false;
    }
    private ICommand? autoResponsesCommand;
    public ICommand AutoResponsesCommand
    {
        get
        {
            return autoResponsesCommand ?? (autoResponsesCommand = new RelayCommand((action) =>
            {
                autoInputType = true;
            }, CanExecuteAutoResponsesCommand));
        }
    }

    private ICommand? startCommand;
    public ICommand StartCommand
    {
        get
        {
            return startCommand ?? (startCommand = new RelayCommand(async (action) =>
            {
                var chatViewModel = new ChatViewModel(autoInputType);
                mainServerViewModel.CurrentViewModel = chatViewModel;
                await chatViewModel.StartServerAsync();
            }, o => true));
        }
    }
}
