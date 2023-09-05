using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ButtonSpinner.Controls;

public class ButtonSpinner : ContentView
{
    #region Properties

    private Grid ContainerGrid { get; init; }

    private Button Button { get; init; }

    private ActivityIndicator ActivityIndicator { get; init; }

    #endregion Properties

    #region Bindable Properties (Static)

    public static BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ButtonSpinner), string.Empty, BindingMode.OneWay, propertyChanged: OnValuePropertyChanged);

    public static BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ButtonSpinner), propertyChanged: OnCommandPropertyChanged);

    public static BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ButtonSpinner));

    #endregion Bindable Properties (Static)

    #region Bindable Properties (Instance)

    public string Text
    {
        get => (string)GetValue(TextProperty); 
        set => SetValue(TextProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    #endregion Bindable Properties (Instance)

    private Color originalTextColor;

    public ButtonSpinner()
    {
        ContainerGrid = new Grid();
        Button = new Button();
        ActivityIndicator = new ActivityIndicator()
        {
            IsVisible = false,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        this.originalTextColor = Button.TextColor;

        this.ContainerGrid.Children.Add(Button);
        this.ContainerGrid.Children.Add(ActivityIndicator);

        this.Content = ContainerGrid;

        var gestureRecogniser = new TapGestureRecognizer();

        gestureRecogniser.Buttons = ButtonsMask.Primary;
        gestureRecogniser.Tapped += (s, e) =>
        {
            Command?.Execute(CommandParameter);
        };

        this.GestureRecognizers.Add(gestureRecogniser);
    }

    static void OnValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        => ((ButtonSpinner)bindable).OnValuePropertyChanged();

    static void OnCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        => ((ButtonSpinner)bindable).OnCommandPropertyChanged((ICommand)newValue);

    private void OnValuePropertyChanged()
    {
        Button.BatchBegin();

        Button.Text = Text;

        Button.BatchCommit();
    }

    private void OnCommandPropertyChanged(ICommand newCommand)
    {
        newCommand.CanExecuteChanged += OnCanExecuteChanged;

        Button.Command = newCommand;
        Button.CommandParameter = CommandParameter;
    }

    private async void OnCanExecuteChanged(object sender, EventArgs e)
    {
        var showSpinner = !Command.CanExecute(CommandParameter);

        if (showSpinner)
        {
            Debug.WriteLine("Show Spinner");
            ActivityIndicator.Opacity = 0;
            ActivityIndicator.IsVisible = true;
            ActivityIndicator.IsRunning = true;
            Button.TextColor = Colors.Transparent;

            await ActivityIndicator.FadeTo(1, 250, Easing.CubicInOut);
        }
        else
        {
            Debug.WriteLine("Hide Spinner");

            Button.TextColor = originalTextColor;

            await ActivityIndicator.FadeTo(0, 250, Easing.CubicInOut);

            ActivityIndicator.IsVisible = true;
            ActivityIndicator.IsRunning = false;
        }
    }
}
