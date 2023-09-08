using SharedInfrastructureLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpWPF_NP_Sockets_Task_3.Server.ViewModels;

public class MainServerViewModel: ObservableObject
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
    public MainServerViewModel() 
    {
        ConnectionSettingsVM = new ConnectionSettingsViewModel(this);
        CurrentViewModel = ConnectionSettingsVM;
    }

}
