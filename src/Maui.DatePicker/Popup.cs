using Maui.DatePicker.Animations;
using Maui.DatePicker.Extensions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.ComponentModel;

namespace Maui.DatePicker;

[ContentProperty(nameof(Content))]
public partial class Popup : ContentView
{
    public static new readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(Popup),
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnContentChanged((View)oldValue, (View)newValue));

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(View), typeof(Popup),
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnHeaderChanged((View)oldValue, (View)newValue));

    public static readonly BindableProperty FooterProperty = BindableProperty.Create(nameof(Footer), typeof(View), typeof(Popup),
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnFooterChanged((View)oldValue, (View)newValue));

    public static readonly BindableProperty BackdropOpacityProperty = BindableProperty.Create(nameof(BackdropOpacity), typeof(float), typeof(Popup), .5f,
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnBackdropOpacityChanged((float)oldValue, (float)newValue));

    public static readonly BindableProperty BackdropColorProperty = BindableProperty.Create(nameof(BackdropColor), typeof(Color), typeof(Popup), Color.FromArgb("#000"),
        propertyChanged: (bindable, oldValue, newValue) => ((Popup)bindable).OnBackdropColorChanged((Color)oldValue, (Color)newValue));

    public static readonly BindableProperty CloseByTappingOutsideProperty = BindableProperty.Create(nameof(OnTapped), typeof(bool), typeof(Popup), false);

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

    readonly Border _contentPresenter;
    View _content;
    View _header;
    View _footer;
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

    private void OnContentChanged(View oldView, View newView)
    {
        if (newView is null) return;

        var contentGrid = (Grid)_contentPresenter.Content;
        contentGrid.Remove(_content);
        
        _content = newView;
        
        Grid.SetRow(_content, 1);
        contentGrid.Add(_content);
        
        _contentViews.Clear();
        _contentViews.Push(_content);
    }

    private void OnHeaderChanged(View oldView, View newView)
    {
        if (newView is null) return;

        var contentGrid = (Grid)_contentPresenter.Content;
        contentGrid.Remove(_header);

        _header = newView;
        
        Grid.SetRow(_header, 0);
        contentGrid.Add(_header);
    }

    private void OnFooterChanged(View oldView, View newView)
    {
        if (newView is null) return;

        var contentGrid = (Grid)_contentPresenter.Content;
        contentGrid.Remove(_footer);

        _footer = newView;

        Grid.SetRow(_footer, 2);
        contentGrid.Add(_footer);
    }

    public async Task NavigateTo(View view)
    {
        var currentView = _contentViews.Peek();

        view.MaximumHeightRequest = currentView.DesiredSize.Height;
        view.MaximumWidthRequest = currentView.DesiredSize.Width;
        if(view is ScrollView scrollView)
        {
            view.MaximumHeightRequest = scrollView.Content.DesiredSize.Height;
            view.MaximumWidthRequest = scrollView.Content.DesiredSize.Width;
        }

        _contentViews.Push(view);

        var contentGrid = (Grid)_contentPresenter.Content;

        view.Opacity = 0;
        Grid.SetRow(view, 1);
        contentGrid.Add(view);

        await Task.WhenAll(
                view.OpacityTo(0, 1),
                currentView.OpacityTo(1, 0));

        contentGrid.Remove(currentView);

    }

    public async Task NavigateBack()
    {
        var poppedView = _contentViews.Pop();
        var toShowView = _contentViews.Peek();

        var contentGrid = (Grid)_contentPresenter.Content;

        contentGrid.Add(toShowView);

        await Task.WhenAll(
                toShowView.OpacityTo(0, 1),
                poppedView.OpacityTo(1, 0));

        contentGrid.Remove(poppedView);
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

public class PopupContentPresenter : Border 
{
    SizeRequest? _size;
    public PopupContentPresenter()
    {
        StrokeThickness = 0;
        BackgroundColor = Colors.White;
        Padding = new Thickness(10, 15);
        VerticalOptions = LayoutOptions.Center;
        HorizontalOptions = LayoutOptions.Center;
        StrokeShape = new RoundRectangle
        {
            CornerRadius = new CornerRadius(10, 10, 10, 10)
        };
        Content = new Grid
        {
            RowDefinitions = new RowDefinitionCollection(
                                    new RowDefinition(GridLength.Auto),
                                    new RowDefinition(GridLength.Star),
                                    new RowDefinition(GridLength.Auto)),
        };
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var sizeRequest = base.MeasureOverride(widthConstraint, heightConstraint);

        if (_size is not null) return _size.Value;

        _size = sizeRequest;
        return sizeRequest;
    }
}
