﻿namespace TieBetting.Services.Navigation.NavigationParameters;

public class TeamMatchesViewNavigationParameter : NavigationParameterBase
{
    public TeamMatchesViewNavigationParameter(TeamViewModel team)
    {
        Team = team;
    }

    public TeamViewModel Team { get; }
}