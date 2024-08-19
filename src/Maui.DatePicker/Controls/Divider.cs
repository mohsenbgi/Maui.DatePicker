
namespace Maui.DatePicker.Controls;

public class Divider : GraphicsView, IDrawable
{
    public Divider()
    {
        Drawable = this;
        HeightRequest = 1;
        this.ParentChanged += OnParentChanged;
    }

    private void OnParentChanged(object? sender, System.EventArgs e)
    {
        SetBinding(WidthRequestProperty, new Binding(WidthRequestProperty.PropertyName, BindingMode.OneWay, source: (VisualElement)Parent));
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();

        canvas.StrokeSize = 1;
        canvas.StrokeColor = BackgroundColor;

        canvas.DrawLine(dirtyRect.X, dirtyRect.Y, dirtyRect.X + dirtyRect.Width, dirtyRect.Y);

        canvas.RestoreState();
    }
}
