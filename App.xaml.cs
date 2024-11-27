﻿using System.Configuration;
using System.Data;
using System.Windows;

namespace TicketeX_;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public ViewModels.ViewModels ViewModels { get; } = new ViewModels.ViewModels();
}