﻿namespace TieBetting.Models;

[FirestoreData]
public class Team
{
    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public string Image { get; set; }

    [FirestoreProperty]
    public int CurrentBetSession { get; set; }

    [FirestoreProperty]
    public int TotalBet { get; set; }

    [FirestoreProperty]
    public double TotalWin { get; set; }

    [FirestoreProperty] 
    public List<bool> Statuses { get; set; } = new();

    public int Profit => (int)TotalWin - TotalBet - CurrentBetSession;
}