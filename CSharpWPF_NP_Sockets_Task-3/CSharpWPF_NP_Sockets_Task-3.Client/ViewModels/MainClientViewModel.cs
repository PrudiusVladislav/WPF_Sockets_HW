using SharedInfrastructureLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSharpWPF_NP_Sockets_Task_3.Client.ViewModels;

public class MainClientViewModel: ObservableObject
{
    public ConnectionSettingsViewModel? ConnectionSettingsVM { get; set; }
    private ObservableObject? _currentViewModel;
    public ObservableObject? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public MainClientViewModel()
    {
        ConnectionSettingsVM = new ConnectionSettingsViewModel(this);
        CurrentViewModel = ConnectionSettingsVM;
    }
}
