#!/usr/bin/env dotnet
#:property PublishAot=false
#:property OutputType=WinExe

#:package Microsoft.Extensions.Hosting@10.0.0
#:package Avalonia@11.3.8
#:package Avalonia.Desktop@11.3.8
#:package Avalonia.Themes.Simple@11.3.8
#:package Avalonia.Fonts.Inter@11.3.8
#:package CommunityToolkit.Mvvm@8.4.0

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Themes.Simple;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

if (OperatingSystem.IsWindows())
{
    Thread.CurrentThread?.SetApartmentState(ApartmentState.Unknown);
    Thread.CurrentThread?.SetApartmentState(ApartmentState.STA);
}

var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings() { Args = args, });
builder.Services.AddSingleton<App>();
builder.Services.AddSingleton<MainWindow>();
builder.Services.AddSingleton<MainViewModel>();
builder.Services.AddSingleton<CalculatorService>();

using var host = builder.Build();
host.Start();

AppBuilder
    .Configure(host.Services.GetRequiredService<App>)
    .UsePlatformDetect()
    .WithInterFont()
    .LogToTrace()
    .StartWithClassicDesktopLifetime(args, _ => host.StopAsync().GetAwaiter().GetResult());

public partial class MainViewModel(CalculatorService service) : ObservableObject
{
    [ObservableProperty]
    private string _display = "0";

    [ObservableProperty]
    private string _firstNumber = string.Empty;

    [ObservableProperty]
    private string _operation = string.Empty;

    [ObservableProperty]
    private bool _isNewNumber = true;

    [RelayCommand]
    private void InputNumber(string number)
    {
        if (IsNewNumber)
        {
            Display = number;
            IsNewNumber = false;
        }
        else
        {
            Display = Display == "0" ? number : Display + number;
        }
    }

    [RelayCommand]
    private void InputOperation(string op)
    {
        if (!string.IsNullOrEmpty(Operation))
        {
            Calculate();
        }
        
        FirstNumber = Display;
        Operation = op;
        IsNewNumber = true;
    }

    [RelayCommand]
    private void Calculate()
    {
        if (string.IsNullOrEmpty(Operation) || string.IsNullOrEmpty(FirstNumber))
            return;

        var result = service.Calculate(FirstNumber, Display, Operation);
        Display = result;
        Operation = string.Empty;
        FirstNumber = string.Empty;
        IsNewNumber = true;
    }

    [RelayCommand]
    private void Clear()
    {
        Display = "0";
        FirstNumber = string.Empty;
        Operation = string.Empty;
        IsNewNumber = true;
    }
}

public class CalculatorService
{
    public string Calculate(string firstNumber, string secondNumber, string operation)
    {
        if (!double.TryParse(firstNumber, out var num1) || !double.TryParse(secondNumber, out var num2))
            return "오류";

        double result = operation switch
        {
            "+" => num1 + num2,
            "-" => num1 - num2,
            "×" => num1 * num2,
            "÷" => num2 != 0 ? num1 / num2 : double.NaN,
            _ => 0
        };

        return double.IsNaN(result) ? "오류" : result.ToString();
    }
}

public class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        _services = services;
        Styles.Add(new SimpleTheme());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = (MainWindow)_services.GetService(typeof(MainWindow))!;
 
        base.OnFrameworkInitializationCompleted();
    }
}

public class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        DataContext = vm;
        Title = "계산기";
        Width = 320;
        Height = 420;
        MinWidth = 280;
        MinHeight = 380;

        var root = new Grid
        {
            Margin = new Thickness(10),
            RowDefinitions = new RowDefinitions("Auto,*,*,*,*"),
            ColumnDefinitions = new ColumnDefinitions("*,*,*,*"),
        };

        // 디스플레이
        var display = new TextBlock
        {
            FontSize = 32,
            TextAlignment = Avalonia.Media.TextAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5, 5, 5, 15),
            [!TextBlock.TextProperty] = new Binding(nameof(vm.Display)),
        };
        Grid.SetRow(display, 0);
        Grid.SetColumnSpan(display, 4);
        root.Children.Add(display);

        // 숫자 버튼 7-9, ÷
        AddButtonToGrid(root, CreateNumberButton("7", vm), 1, 0);
        AddButtonToGrid(root, CreateNumberButton("8", vm), 1, 1);
        AddButtonToGrid(root, CreateNumberButton("9", vm), 1, 2);
        AddButtonToGrid(root, CreateOperationButton("÷", vm), 1, 3);

        // 숫자 버튼 4-6, ×
        AddButtonToGrid(root, CreateNumberButton("4", vm), 2, 0);
        AddButtonToGrid(root, CreateNumberButton("5", vm), 2, 1);
        AddButtonToGrid(root, CreateNumberButton("6", vm), 2, 2);
        AddButtonToGrid(root, CreateOperationButton("×", vm), 2, 3);

        // 숫자 버튼 1-3, -
        AddButtonToGrid(root, CreateNumberButton("1", vm), 3, 0);
        AddButtonToGrid(root, CreateNumberButton("2", vm), 3, 1);
        AddButtonToGrid(root, CreateNumberButton("3", vm), 3, 2);
        AddButtonToGrid(root, CreateOperationButton("-", vm), 3, 3);

        // 숫자 버튼 0, C, =, +
        AddButtonToGrid(root, CreateNumberButton("0", vm), 4, 0);
        AddButtonToGrid(root, new Button
        {
            Content = "C",
            FontSize = 20,
            Margin = new Thickness(2),
            [!Button.CommandProperty] = new Binding(nameof(vm.ClearCommand)),
        }, 4, 1);
        AddButtonToGrid(root, new Button
        {
            Content = "=",
            FontSize = 20,
            Margin = new Thickness(2),
            [!Button.CommandProperty] = new Binding(nameof(vm.CalculateCommand)),
        }, 4, 2);
        AddButtonToGrid(root, CreateOperationButton("+", vm), 4, 3);

        Content = root;
    }

    private static void AddButtonToGrid(Grid grid, Control button, int row, int column)
    {
        Grid.SetRow(button, row);
        Grid.SetColumn(button, column);
        grid.Children.Add(button);
    }

    private static Button CreateNumberButton(string number, MainViewModel vm)
    {
        return new Button
        {
            Content = number,
            FontSize = 20,
            Margin = new Thickness(2),
            Command = vm.InputNumberCommand,
            CommandParameter = number,
        };
    }

    private static Button CreateOperationButton(string operation, MainViewModel vm)
    {
        return new Button
        {
            Content = operation,
            FontSize = 20,
            Margin = new Thickness(2),
            Command = vm.InputOperationCommand,
            CommandParameter = operation,
        };
    }
}
