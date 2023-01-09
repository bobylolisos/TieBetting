﻿namespace TieBetting.ViewModels;

public class TeamViewModel
{
    private readonly Team _team;

    public TeamViewModel(Team team)
    {
        _team = team;
    }

    public string Name => _team.Name;

    public string Image => _team.Image;

    public int TotalBet => _team.TotalBet;

    public int TotalWin => (int)_team.TotalWin;

    public int Profit => _team.Profit;

    public int BetsInSession => _team.CurrentBetSession;

    public int LostMatchesInSession => _team.Statuses.CountNumberOfPreviousLostMatches();
}