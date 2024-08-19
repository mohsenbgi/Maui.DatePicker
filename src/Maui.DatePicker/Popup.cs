using Maui.DatePicker.Animations;
using Maui.DatePicker.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.ComponentModel;

namespace Maui.DatePicker;
[ContentProperty(nameof(Content))]
public partial class Popup : ContentView
{
    public static new readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(Popup),
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnContentChanged((View)oldValue, (View)newValue));

    public static new readonly BindableProperty PaddingProperty = BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(Popup),
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnPaddingChanged((Thickness)oldValue, (Thickness)newValue));

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(View), typeof(Popup),
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnHeaderChanged((View)oldValue, (View)newValue));

    public static readonly BindableProperty FooterProperty = BindableProperty.Create(nameof(Footer), typeof(View), typeof(Popup),
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnFooterChanged((View)oldValue, (View)newValue));

    public static readonly BindableProperty BackdropOpacityProperty = BindableProperty.Create(nameof(BackdropOpacity), typeof(float), typeof(Popup), .5f,
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnBackdropOpacityChanged((float)oldValue, (float)newValue));

    public static readonly BindableProperty BackdropColorProperty = BindableProperty.Create(nameof(BackdropColor), typeof(Color), typeof(Popup), Color.FromArgb("#000"),
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnBackdropColorChanged((Color)oldValue, (Color)newValue));

    public static readonly BindableProperty CloseByTappingOutsideProperty = BindableProperty.Create(nameof(CloseByTappingOutside), typeof(bool), typeof(Popup), false);

    public static readonly BindableProperty HeaderHasDividerProperty = BindableProperty.Create(nameof(HeaderHasDivider), typeof(bool), typeof(Popup), false,
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnHeaderHasDividerChanged((bool)newValue));

    public static readonly BindableProperty FooterHasDividerProperty = BindableProperty.Create(nameof(FooterHasDivider), typeof(bool), typeof(Popup), false,
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnFooterHasDividerChanged((bool)newValue));

    public bool HeaderHasDivider
    {
        get => (bool)GetValue(HeaderHasDividerProperty);
        set => SetValue(HeaderHasDividerProperty, value);
    }

    public bool FooterHasDivider 
    {
        get => (bool)GetValue(FooterHasDividerProperty);
        set => SetValue(FooterHasDividerProperty, value);
    }

    public View Header
    {
        get => (View)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public View Footer
    {
        get => (View)GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public new View Content
    {
        get => (View)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public new Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public float BackdropOpacity
    {
        get => (float)GetValue(BackdropOpacityProperty);
        set => SetValue(BackdropOpacityProperty, value);
    }

    public Color BackdropColor
    {
        get => (Color)GetValue(BackdropColorProperty);
        set => SetValue(BackdropColorProperty, value);
    }

    public bool CloseByTappingOutside
    {
        get => (bool)GetValue(CloseByTappingOutsideProperty);
        set => SetValue(CloseByTappingOutsideProperty, value);
    }

    public View CurrentContent => _contentViews.Peek();

    readonly PopupContentPresenter _contentPresenter;
    bool _backdropColorIsChanging;
    bool _backgroundColorIsChanging;
    double _minimumTranslationY = 0;
    double _appliedTotalTranslationYDiff;
    bool _isHorizontalPan;
    bool _isVerticalPan;
    Stack<View> _contentViews = new Stack<View>();
    public Popup()
    {
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;
        Padding = new Thickness(5, 5);
        Opacity = 0;
        ZIndex = 2;
        InputTransparent = true;
        IsClippedToBounds = true;
        BackgroundColor = BackdropColor.WithAlpha(BackdropOpacity);
        AbsoluteLayout.SetLayoutBounds(this, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.SizeProportional);

        _contentPresenter = new PopupContentPresenter();
        base.Content = _contentPresenter;

        PropertyChanged += OnPropertyChanged;

        InitGestures();
    }

    private void InitGestures()
    {
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnTapped;
        GestureRecognizers.Add(tapGesture);
    }

    private void OnTapped(object? sender, TappedEventArgs eventArgs)
    {
        if (CloseByTappingOutside)
        {
            var position = eventArgs.GetPosition(_contentPresenter);
            if (position?.Y < 0 || position?.Y > _contentPresenter.DesiredSize.Height) Close();
            else if (position?.X < 0 || position?.X > _contentPresenter.DesiredSize.Width) Close();
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == BackgroundColorProperty.PropertyName)
            OnBackgroundColorChanged();
    }

    private void OnPaddingChanged(Thickness oldValue, Thickness newValue)
    {
        if (_contentPresenter is null) return;

        double left = newValue.Left - oldValue.Left;
        double top = newValue.Top - oldValue.Top;
        double right = oldValue.Right - newValue.Right;
        double bottom = oldValue.Bottom - newValue.Bottom;

        if (_contentPresenter.Content is not null)
        {
            _contentPresenter.Content.Margin = new Thickness(
                                                            _contentPresenter.Content.Margin.Left + left,
                                                            _contentPresenter.Content.Margin.Top,
                                                            _contentPresenter.Content.Margin.Right + right,
                                                            _contentPresenter.Content.Margin.Bottom);
        }

        if (_contentPresenter.Header is not null)
        {
            _contentPresenter.Header.Margin = new Thickness(
                                                            _contentPresenter.Header.Margin.Left + left,
                                                            _contentPresenter.Header.Margin.Top + top,
                                                            _contentPresenter.Header.Margin.Right + right,
                                                            _contentPresenter.Header.Margin.Bottom);
        }

        if (_contentPresenter.Footer is not null)
        {
            _contentPresenter.Footer.Margin = new Thickness(
                                                            _contentPresenter.Footer.Margin.Left + left,
                                                            _contentPresenter.Footer.Margin.Top,
                                                            _contentPresenter.Footer.Margin.Right + right,
                                                            _contentPresenter.Footer.Margin.Bottom + bottom);
        }
    }

    private void OnHeaderHasDividerChanged(bool hasDivider)
    {
        if (hasDivider) _contentPresenter.ShowHeaderDivider();
        else _contentPresenter.HideHeaderDivider();
    }

    private void OnFooterHasDividerChanged(bool hasDivider)
    {
        if (hasDivider) _contentPresenter.ShowFooterDivider();
        else _contentPresenter.HideFooterDivider();
    }

    private void OnContentChanged(View oldView, View newView)
    {
        if (newView is null) return;

        newView.Margin = new Thickness(newView.Margin.Left + Padding.Left, newView.Margin.Top, newView.Margin.Right + Padding.Right, newView.Margin.Bottom);

        _contentPresenter.SetContent(newView);

        _contentViews.Clear();
        _contentViews.Push(newView);
    }

    private void OnHeaderChanged(View oldView, View newView)
    {
        if (newView is null) return;

        newView.Margin = new Thickness(newView.Margin.Left + Padding.Left, newView.Margin.Top + Padding.Top, newView.Margin.Right + Padding.Right, newView.Margin.Bottom);

        _contentPresenter.SetHeader(newView);
    }

    private void OnFooterChanged(View oldView, View newView)
    {
        if (newView is null) return;

        newView.Margin = new Thickness(newView.Margin.Left + Padding.Left, newView.Margin.Top, newView.Margin.Right + Padding.Right, newView.Margin.Bottom + Padding.Bottom);

        _contentPresenter.SetFooter(newView);
    }

    public async Task NavigateTo(View view)
    {
        var currentView = _contentViews.Peek();

        var firstView = _contentViews.First();
        view.MaximumHeightRequest = firstView.Height;
        view.MaximumWidthRequest = firstView.Width;

        _contentViews.Push(view);

        view.Opacity = 0;
        view.Scale = 1.5;

        _contentPresenter.AddContent(view);

        currentView.Opacity = 0;
        await Task.WhenAll(
                view.OpacityTo(0, 1),
                view.ScaleTo(1));

        _contentPresenter.Remove(currentView);
    }

    public async Task NavigateBack()
    {
        var poppedView = _contentViews.Pop();
        var toShowView = _contentViews.Peek();

        _contentPresenter.AddContent(toShowView);

        poppedView.Opacity = 0;
        toShowView.Scale = 0.5;
        await Task.WhenAll(
                toShowView.OpacityTo(0, 1),
                toShowView.ScaleTo(1));

        _contentPresenter.Remove(poppedView);
    }

    private void OnBackdropColorChanged(Color oldColor, Color newColor)
    {
        _backdropColorIsChanging = true;
        BackgroundColor = newColor.WithAlpha(BackdropOpacity);
    }

    private void OnBackdropOpacityChanged(float oldAmount, float newAmount)
    {
        _backdropColorIsChanging = true;
        BackgroundColor = BackgroundColor.WithAlpha(newAmount);
    }

    private void OnBackgroundColorChanged()
    {
        if (_backdropColorIsChanging)
        {
            _backdropColorIsChanging = false;
            return;
        }

        if (_backgroundColorIsChanging)
        {
            _backgroundColorIsChanging = false;
            return;
        }

        _backgroundColorIsChanging = true;
        _contentPresenter.BackgroundColor = BackgroundColor;
        BackgroundColor = BackdropColor.WithAlpha(BackdropOpacity);
    }

    public void Open()
    {
        IsVisible = true;
        InputTransparent = false;
        this.OpacityTo(0, 1, 250, Easing.SinIn);
    }

    public async void Close()
    {
        await this.OpacityTo(1, 0, 250, Easing.SinInOut);
        IsVisible = false;
        InputTransparent = true;
    }

}
