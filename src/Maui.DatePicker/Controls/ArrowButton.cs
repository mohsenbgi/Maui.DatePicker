using Maui.DatePicker.Behaviors;
using System.Runtime.CompilerServices;

namespace Maui.DatePicker.Controls;
public class ArrowButton : GraphicsView, IDrawable
{
    public static BindableProperty TintColorProperty =
        BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(ArrowButton), Colors.Black,
            propertyChanged: TintColorChanged);

    public Color TintColor
    {
        get => (Color)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }

    public event EventHandler Clicked;

    public ArrowButton()
    {
        Drawable = this;
        BackgroundColor = Colors.Transparent;
        WidthRequest = 30;
        HeightRequest = 35;
        FlowDirection = FlowDirection.LeftToRight;
        Behaviors.Add(new HoverBehavior());

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnClick;
        GestureRecognizers.Add(tap);
    }

    private void OnClick(object? sender, System.EventArgs eventArgs)
    {
        Clicked?.Invoke(this, eventArgs);
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        DrawBackground(canvas, dirtyRect);
        DrawArrow(canvas, dirtyRect);
    }

    public void DrawBackground(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();

        canvas.FillColor = BackgroundColor;

        canvas.FillRoundedRectangle(new Rect(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height), 8d);

        canvas.RestoreState();
    }

    public void DrawArrow(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();

        var path = new PathF();

        var horizontalPadding = dirtyRect.Width * 42 / 100;
        var verticalPadding = dirtyRect.Height * 37 / 100;

        if (FlowDirection == FlowDirection.LeftToRight)
        {
            path.MoveTo(dirtyRect.X + horizontalPadding, dirtyRect.Y + verticalPadding);
            path.LineTo(dirtyRect.X + dirtyRect.Width - horizontalPadding, dirtyRect.Y + dirtyRect.Height / 2);
            path.LineTo(dirtyRect.X + horizontalPadding, dirtyRect.Y + dirtyRect.Height - verticalPadding);
        }
        else
        {
            path.MoveTo(dirtyRect.X + dirtyRect.Width - horizontalPadding, dirtyRect.Y + verticalPadding);
            path.LineTo(dirtyRect.X + horizontalPadding, dirtyRect.Y + dirtyRect.Height / 2);
            path.LineTo(dirtyRect.X + dirtyRect.Width - horizontalPadding, dirtyRect.Y + dirtyRect.Height - verticalPadding);
        }


        canvas.StrokeLineJoin = LineJoin.Round;
        canvas.StrokeColor = TintColor;
        canvas.StrokeSize = dirtyRect.Width * 7 / 100;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawPath(path);

        canvas.RestoreState();
    }

    private static void TintColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((ArrowButton)bindable).Invalidate();
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(BackgroundColor):
            case nameof(FlowDirection):
                Invalidate();
                break;
            default:
                break;
        }
    }
}