﻿namespace TieBetting.Services;

public interface IDialogService
{
    Task ShowMessage(string title, string message);
}