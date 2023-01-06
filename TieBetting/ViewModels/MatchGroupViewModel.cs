namespace TieBetting.ViewModels;

public class MatchGroupViewModel : List<MatchViewModel>
{
    public MatchGroupViewModel(string name, List<MatchViewModel> matches)
        : base(matches)
    {
        Name = name;
    }

    public string Name { get; }
}