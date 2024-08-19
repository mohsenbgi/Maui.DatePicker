using Maui.DatePicker.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace Maui.DatePicker;

public class PopupContentPresenter : Border
{
    public static new readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(PopupContentPresenter),
        propertyChanged: (bindable, oldValue, newValue) => ((PopupContentPresenter)bindable).OnContentChanged((View)oldValue, (View)newValue));

    public View? Header => _header;
    public View? Footer => _footer;
    public new View Content
    {
        get => (View)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    SizeRequest? _size;
    Grid _grid;
    Divider _headerDivider, _footerDivider;
    View? _content;
    View? _header;
    View? _footer;
    public PopupContentPresenter()
    {
        StrokeThickness = 0;
        BackgroundColor = Colors.White;
        VerticalOptions = LayoutOptions.Center;
        HorizontalOptions = LayoutOptions.Center;
        StrokeShape = new RoundRectangle
        {
            CornerRadius = new CornerRadius(8)
        };
        _headerDivider = new Divider() { IsVisible = false, ZIndex = 1, BackgroundColor = Colors.Gray };
        _footerDivider = new Divider() { IsVisible = false, ZIndex = 1, BackgroundColor = Colors.Gray };
        Grid.SetRow(_headerDivider, 1);
        Grid.SetRow(_footerDivider, 3);

        base.Content = _grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection(
                                    new RowDefinition(GridLength.Auto),
                                    new RowDefinition(GridLength.Auto),
                                    new RowDefinition(GridLength.Star),
                                    new RowDefinition(GridLength.Auto),
                                    new RowDefinition(GridLength.Auto)),
        };
    }

    void OnContentChanged(View oldValue, View newValue)
    {
        if (newValue is null) return;

        if (_content is not null) _grid.Remove(_content);

        _content = newValue;

        Grid.SetRow(_content, 2);
        _grid.Add(_content);
    }

    public void ShowHeaderDivider()
    {
        _headerDivider.IsVisible = true;
    }

    public void HideHeaderDivider()
    {
        _headerDivider.IsVisible = false;
    }

    public void ShowFooterDivider()
    {
        _footerDivider.IsVisible = true;
    }

    public void HideFooterDivider()
    {
        _footerDivider.IsVisible = false;
    }

    public void SetHeader(View header)
    {
        if (header is null) return;

        if (_header is not null) _grid.Remove(_header);

        _header = header;
        _header.ZIndex = 1;

        Grid.SetRow(_header, 0);
        _grid.Add(_header);
    }

    public void SetFooter(View footer)
    {
        if (footer is null) return;

        if (_footer is not null) _grid.Remove(_footer);

        _footer = footer;
        _footer.ZIndex = 1;

        Grid.SetRow(_footer, 4);
        _grid.Add(_footer);
    }

    public void SetContent(View content)
    {
        if (content is null) return;

        if (_content is not null) _grid.Remove(_content);

        _content = content;

        Grid.SetRow(_content, 2);
        _grid.Add(_content);
    }

    public void AddContent(View content)
    {
        if (content is null) return;

        _content = content;

        Grid.SetRow(_content, 2);
        _grid.Add(_content);
    }

    public void Remove(View view)
    {
        _grid.Remove(view);
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var sizeRequest = base.MeasureOverride(widthConstraint, heightConstraint);

        if (_size is not null) return _size.Value;

        _size = sizeRequest;

        // add after measuring to avoid affecting the size
        _grid.Children.Add(_headerDivider);
        _grid.Children.Add(_footerDivider);


        return sizeRequest;
    }
}
