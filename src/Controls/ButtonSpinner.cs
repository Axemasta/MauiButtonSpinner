using System.Diagnostics;
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

    public static readonly new BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(ButtonSpinner), Colors.White, BindingMode.OneWay, propertyChanged: OnValuePropertyChanged);

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ButtonSpinner), propertyChanged: OnCommandPropertyChanged);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ButtonSpinner));

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(Button), defaultValue: -1);

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(ButtonSpinner), FontAttributes.None, BindingMode.OneWay, propertyChanged: OnValuePropertyChanged);

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(ButtonSpinner), 0d, propertyChanged: OnValuePropertyChanged);

    public static readonly BindableProperty IndicatorColorProperty = BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(ButtonSpinner), Colors.Black, BindingMode.OneWay, propertyChanged: OnValuePropertyChanged);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ButtonSpinner), string.Empty, BindingMode.OneWay, propertyChanged: OnValuePropertyChanged);

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ButtonSpinner), Colors.Black, BindingMode.OneWay, propertyChanged: OnValuePropertyChanged);

    #endregion Bindable Properties (Static)

    #region Bindable Properties (Instance)

    public new Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
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

    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }

    [System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontAttributesProperty, value);
    }

    public Color IndicatorColor
    {
        get => (Color)GetValue(IndicatorColorProperty);
        set => SetValue(IndicatorColorProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty); 
        set => SetValue(TextProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    #endregion Bindable Properties (Instance)

    public ButtonSpinner()
    {
        ContainerGrid = new Grid()
        {
            BackgroundColor = Colors.Transparent,
        };

        Button = new Button()
        {
            BackgroundColor = BackgroundColor,
            TextColor = TextColor,
            FontAttributes = FontAttributes,
        };

        ActivityIndicator = new ActivityIndicator()
        {
            IsVisible = false,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Color = IndicatorColor,
        };

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
        ActivityIndicator.BatchBegin();

        ActivityIndicator.Color = IndicatorColor;

        Button.Background = BackgroundColor;
        Button.FontAttributes = FontAttributes;
        Button.Text = Text;
        Button.TextColor = TextColor;

        if (FontSize >= 1)
        {
            // Maui internals were internal so this is a bit of a hacky workaround
            Button.FontSize = FontSize;
        }

        if (CornerRadius > -1)
        {
            Button.CornerRadius = CornerRadius;
        }

        Button.BatchCommit();
        ActivityIndicator.BatchCommit();
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

            Button.TextColor = TextColor;

            await ActivityIndicator.FadeTo(0, 250, Easing.CubicInOut);

            ActivityIndicator.IsVisible = true;
            ActivityIndicator.IsRunning = false;
        }
    }
}
