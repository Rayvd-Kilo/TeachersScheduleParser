using TeachersScheduleParser.MVVM.Core;

namespace TeachersScheduleParser.MVVM.ViewModel;

public class MainViewModel : ObservableObject
{
    private ClientsListViewModel ClientsListViewModel { get; set; }
    
    public object CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    private object _currentView;
    
    public MainViewModel()
    {
        ClientsListViewModel = new ClientsListViewModel();
        _currentView = ClientsListViewModel;
        CurrentView = ClientsListViewModel;
    }
}