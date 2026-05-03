using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace C971MobileAppDev.Resources.Views.Controls
{
    public partial class DateRangeProgressBar : ContentView
    {
        public static readonly BindableProperty StartDateProperty =
            BindableProperty.Create(
                nameof(StartDate),
                typeof(DateTime),
                typeof(DateRangeProgressBar),
                DateTime.Today,
                propertyChanged: OnDateChanged);

        public static readonly BindableProperty EndDateProperty =
            BindableProperty.Create(
                nameof(EndDate),
                typeof(DateTime),
                typeof(DateRangeProgressBar),
                DateTime.Today.AddDays(1),
                propertyChanged: OnDateChanged);

        public static readonly BindableProperty CurrentDateProperty =
            BindableProperty.Create(
                nameof(CurrentDate),
                typeof(DateTime),
                typeof(DateRangeProgressBar),
                DateTime.Today,
                propertyChanged: OnDateChanged);

        public DateTime StartDate
        {
            get => (DateTime)GetValue(StartDateProperty);
            set => SetValue(StartDateProperty, value);
        }

        public DateTime EndDate
        {
            get => (DateTime)GetValue(EndDateProperty);
            set => SetValue(EndDateProperty, value);
        }

        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        public DateRangeProgressBar()
        {
            InitializeComponent();
            UpdateProgress();
        }

        static void OnDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((DateRangeProgressBar)bindable).UpdateProgress();
        }

        void UpdateProgress()
        {
            if (EndDate <= StartDate)
            {
                progressBar.Progress = 1.0;
                progressBar.ProgressColor = Colors.Red;
                statusLabel.Text = "Invalid date range";
                percentLabel.Text = "100%";
                return;
            }

            var now = CurrentDate.Date;
            var total = (EndDate.Date - StartDate.Date).TotalDays;
            var elapsed = (now - StartDate.Date).TotalDays;
            double progress;

            if (elapsed <= 0) progress = 0.0;
            else if (elapsed >= total) progress = 1.0;
            else progress = Math.Clamp(elapsed / total, 0.0, 1.0);

            progressBar.Progress = progress;

            var daysRemaining = (EndDate.Date - now).TotalDays;

            if (now > EndDate.Date || daysRemaining <= 3)
            {
                progressBar.ProgressColor = Colors.Red;
            }
            else if (daysRemaining <= 14)
            {
                progressBar.ProgressColor = Colors.Goldenrod;
            }
            else
            {
                progressBar.ProgressColor = Colors.Green;
            }

            if (now < StartDate.Date)
            {
                var daysUntilStart = (StartDate.Date - now).TotalDays;
                statusLabel.Text = $"Starts in {Math.Ceiling(daysUntilStart)} day(s)";
            }
            else if (now > EndDate.Date)
            {
                statusLabel.Text = $"Ended {Math.Ceiling(Math.Abs(daysRemaining))} day(s) ago";
            }
            else
            {
                statusLabel.Text = $"{Math.Ceiling(daysRemaining)} day(s) left";
            }

            percentLabel.Text = $"{Math.Round(progress * 100)}%";
        }
    }
}