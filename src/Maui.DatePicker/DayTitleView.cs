﻿using Maui.DatePicker.Interfaces;

namespace Maui.DatePicker;

public class DayTitleView : Label, IDayTitleView
{
    public void Replace(IDayTitleView newView)
    {
        Text = newView.Text;
    }
}
