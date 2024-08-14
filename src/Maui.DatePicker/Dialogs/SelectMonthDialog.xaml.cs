using Maui.DatePicker.Behaviors;
using Maui.DatePicker.Constants;
using Microsoft.Maui.Controls.Shapes;

namespace Maui.DatePicker.Dialogs;

public partial class SelectMonthDialog : Grid
{
    public EventHandler<int> MonthSelected;
    
    Dictionary<string,int> monthNumbers = new Dictionary<string,int>();
    public SelectMonthDialog(List<string> months)
    {
        VerticalOptions = LayoutOptions.Fill;
        RowDefinitions = new RowDefinitionCollection
        (
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star)
        );

        ColumnDefinitions = new ColumnDefinitionCollection
        (
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star)
        );
        int monthNumber = 1;
        int row = 0;
        int column = 0;
        foreach (var month in months)
        {
            var monthView = new Border
            {
                StrokeThickness = 0,
                BackgroundColor = Colors.White,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 100
                },
                HeightRequest = 55,
                WidthRequest = 55,
                Content = new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 13,
                    Text = month,
                }
            };
            monthView.Behaviors.Add(new HoverBehavior());
            var tap = new TapGestureRecognizer();
            tap.Tapped += OnMonthTapped;
            monthView.GestureRecognizers.Add(tap);

            monthNumbers.Add(month, monthNumber);

            if (column == 4)
            {
                row++;
                column = 0;
            }

            Grid.SetRow(monthView, row);
            Grid.SetColumn(monthView, column);

            Add(monthView);
            monthNumber++;
            column++;
        }
    }

    public void OnMonthTapped(object sender, TappedEventArgs eventArgs)
    {
        var selectedMonth = ((Label)((Border)sender).Content).Text;
        MonthSelected?.Invoke(sender, monthNumbers[selectedMonth]);
    }
}