namespace TieBetting.ViewModels;

public class MatchBettingGroupViewModel : List<MatchBettingViewModel>
{
    public MatchBettingGroupViewModel(string name, IReadOnlyCollection<MatchBettingViewModel> matches)
        : base(matches)
    {
        Name = name;
    }

    public string Name { get; }
}