namespace TieBetting.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        MyCommand = new AsyncRelayCommand(ExecuteMyCommand);
    }

    private Task ExecuteMyCommand()
    {
        var dbg = "";

        return Task.CompletedTask;
    }

    public AsyncRelayCommand MyCommand { get; set; }
}