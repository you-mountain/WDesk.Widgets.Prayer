using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WDesk.Core;

namespace WDesk.Widgets.Prayer.Style;

public class Style2 : IStyleBuilder
{
    public string StyleId => "style2";

    private DispatcherTimer? _timer;

    public FrameworkElement Build(PlacedWidget instance)
    {
        // ═══════════════════════════════════════════
        //  ROOT
        // ═══════════════════════════════════════════
        var root = new Grid();

        // ═══════════════════════════════════════════
        //  BACKGROUND
        // ═══════════════════════════════════════════
        var bg = new Border
        {
            CornerRadius = new CornerRadius(20)
        };
        bg.SetResourceReference(Border.BackgroundProperty, "WidgetBg");
        root.Children.Add(bg);

        // ═══════════════════════════════════════════
        //  CONTENT GRID
        // ═══════════════════════════════════════════
        var content = new Grid
        {
            Margin = new Thickness(18, 16, 18, 16)
        };
        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });                       // Top bar
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Center
        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });                       // Progress + Bottom
        root.Children.Add(content);

        // ═══════════════════════════════════════════
        //  TOP BAR — City (left) + Hijri date (right)
        // ═══════════════════════════════════════════
        var topBar = new Grid();
        topBar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        topBar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var cityLabel = new TextBlock
        {
            Text = "TEHRAN",
            FontSize = 9,
            FontWeight = FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center
        };
        cityLabel.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextMuted");
        Grid.SetColumn(cityLabel, 0);
        topBar.Children.Add(cityLabel);

        var hijriLabel = new TextBlock
        {
            Text = "",
            FontSize = 9,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        hijriLabel.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextMuted");
        Grid.SetColumn(hijriLabel, 1);
        topBar.Children.Add(hijriLabel);

        Grid.SetRow(topBar, 0);
        content.Children.Add(topBar);

        // ═══════════════════════════════════════════
        //  CENTER — Big time + Next prayer name
        // ═══════════════════════════════════════════
        var centerStack = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 4, 0, 4)
        };

        // ★ شماره‌ی بزرگ — ساعت نماز بعدی
        var bigTime = new TextBlock
        {
            Text = "--:--",
            FontSize = 44,
            FontWeight = FontWeights.Light,
            FontFamily = new FontFamily("Cascadia Mono, Consolas, Courier New"),
            HorizontalAlignment = HorizontalAlignment.Center,
            LineHeight = 48
        };
        bigTime.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextPrimary");
        centerStack.Children.Add(bigTime);

        // ★ نام نماز بعدی
        var nextNameRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 6, 0, 0)
        };

        var dot = new Ellipse
        {
            Width = 6,
            Height = 6,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 6, 0)
        };
        dot.SetResourceReference(Shape.FillProperty, "WidgetAccent");

        // ★ Pulse animation برای dot
        var pulse = new DoubleAnimation
        {
            From = 0.4,
            To = 1.0,
            Duration = TimeSpan.FromSeconds(1.2),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };
        dot.BeginAnimation(UIElement.OpacityProperty, pulse);

        nextNameRow.Children.Add(dot);

        var nextNameLabel = new TextBlock
        {
            Text = "—",
            FontSize = 11,
            FontWeight = FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center
        };
        nextNameLabel.SetResourceReference(TextBlock.ForegroundProperty, "WidgetAccent");
        nextNameRow.Children.Add(nextNameLabel);

        centerStack.Children.Add(nextNameRow);

        Grid.SetRow(centerStack, 1);
        content.Children.Add(centerStack);

        // ═══════════════════════════════════════════
        //  BOTTOM — Progress bar + Countdown + Pills
        // ═══════════════════════════════════════════
        var bottomStack = new StackPanel();

        // ─── Progress bar افقی ───
        var progressBar = new Grid
        {
            Height = 3,
            Margin = new Thickness(0, 0, 0, 10)
        };

        var progressBg = new Border
        {
            CornerRadius = new CornerRadius(2),
            Height = 3
        };
        progressBg.SetResourceReference(Border.BackgroundProperty, "WidgetAccentFaded");
        progressBar.Children.Add(progressBg);

        var progressFill = new Border
        {
            CornerRadius = new CornerRadius(2),
            Height = 3,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        progressFill.SetResourceReference(Border.BackgroundProperty, "WidgetAccent");
        progressFill.Width = 0;
        progressBar.Children.Add(progressFill);

        bottomStack.Children.Add(progressBar);

        // ─── Countdown + Label ───
        var countdownRow = new Grid
        {
            Margin = new Thickness(0, 0, 0, 10)
        };
        countdownRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        countdownRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        countdownRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var countdownLeft = new TextBlock
        {
            Text = "in —",
            FontSize = 10,
            VerticalAlignment = VerticalAlignment.Center
        };
        countdownLeft.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextSecondary");
        Grid.SetColumn(countdownLeft, 0);
        countdownRow.Children.Add(countdownLeft);

        var countdownRight = new TextBlock
        {
            Text = "",
            FontSize = 9,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        countdownRight.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextMuted");
        Grid.SetColumn(countdownRight, 2);
        countdownRow.Children.Add(countdownRight);

        bottomStack.Children.Add(countdownRow);

        // ─── Row of pills — همه‌ی نمازها ───
        var pillsGrid = new Grid();
        pillsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        pillsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        pillsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        pillsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        pillsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        pillsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        // ★ نگه‌داری referenceها
        var pillBorders = new Dictionary<string, Border>();
        var pillTexts = new Dictionary<string, TextBlock>();
        var pillLabels = new Dictionary<string, TextBlock>();

        var prayers = PrayerService.GetCurrent().Prayers;
        if (prayers == null || prayers.Count == 0)
        {
            prayers = new List<PrayerTime>
            {
                new() { Key = "fajr",    Name = "Fajr",    Time = "—" },
                new() { Key = "sunrise", Name = "Sunrise", Time = "—" },
                new() { Key = "dhuhr",   Name = "Dhuhr",   Time = "—" },
                new() { Key = "asr",     Name = "Asr",     Time = "—" },
                new() { Key = "maghrib", Name = "Maghrib", Time = "—" },
                new() { Key = "isha",    Name = "Isha",    Time = "—" },
            };
        }

        int col = 0;
        foreach (var prayer in prayers)
        {
            var (pill, timeText, labelText) = BuildPill(prayer);

            Grid.SetColumn(pill, col);
            pillsGrid.Children.Add(pill);

            pillBorders[prayer.Key] = pill;
            pillTexts[prayer.Key] = timeText;
            pillLabels[prayer.Key] = labelText;

            col++;
            if (col > 5) col = 5;
        }

        bottomStack.Children.Add(pillsGrid);

        Grid.SetRow(bottomStack, 2);
        content.Children.Add(bottomStack);

        // ═══════════════════════════════════════════
        //  UPDATE FUNCTION
        // ═══════════════════════════════════════════
        Action updateUI = () =>
        {
            var data = PrayerService.GetCurrent();
            if (data == null) return;

            // ── Top bar ──
            cityLabel.Text = data.City.ToUpperInvariant();

            if (!string.IsNullOrEmpty(data.HijriDate))
                hijriLabel.Text = data.HijriDate;

            // ── Center: Big time ──
            if (!string.IsNullOrEmpty(data.NextPrayerTime))
                bigTime.Text = data.NextPrayerTime;
            else
                bigTime.Text = "--:--";

            // ── Next name ──
            nextNameLabel.Text = !string.IsNullOrEmpty(data.NextPrayerName)
                ? data.NextPrayerName.ToUpperInvariant()
                : "—";

            // ── Countdown ──
            countdownLeft.Text = !string.IsNullOrEmpty(data.DisplayCountdown)
                ? $"in {data.DisplayCountdown}"
                : "";

            // ── Progress bar ──
            // نسبت زمان گذشته از نماز قبلی به کل بازه
            UpdateProgress(data, progressBar, progressFill);

            // ── Pills ──
            foreach (var prayer in data.Prayers)
            {
                if (!pillBorders.TryGetValue(prayer.Key, out var bd)) continue;
                if (!pillTexts.TryGetValue(prayer.Key, out var tb)) continue;
                if (!pillLabels.TryGetValue(prayer.Key, out var lb)) continue;

                tb.Text = prayer.Time;

                if (prayer.IsNext)
                {
                    // ★ هایلایت
                    bd.SetResourceReference(Border.BackgroundProperty, "WidgetAccentFaded");
                    tb.SetResourceReference(TextBlock.ForegroundProperty, "WidgetAccent");
                    tb.FontWeight = FontWeights.Bold;
                    lb.SetResourceReference(TextBlock.ForegroundProperty, "WidgetAccent");

                    // ── Countdown راست ──
                    countdownRight.Text = data.NextPrayerName.ToUpperInvariant();
                }
                else
                {
                    // ── عادی ──
                    bd.Background = Brushes.Transparent;
                    tb.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextPrimary");
                    tb.FontWeight = FontWeights.SemiBold;
                    lb.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextMuted");
                }
            }
        };

        // ═══════════════════════════════════════════
        //  SUBSCRIBE
        // ═══════════════════════════════════════════
        Action onDataChanged = () =>
        {
            root.Dispatcher.BeginInvoke(new Action(updateUI));
        };

        PrayerService.DataChanged += onDataChanged;

        // ═══════════════════════════════════════════
        //  TIMER — هر ۱ ثانیه
        // ═══════════════════════════════════════════
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) =>
        {
            PrayerService.CalculateNext();

            var data = PrayerService.GetCurrent();
            if (data != null)
            {
                countdownLeft.Text = !string.IsNullOrEmpty(data.DisplayCountdown)
                    ? $"in {data.DisplayCountdown}"
                    : "";

                UpdateProgress(data, progressBar, progressFill);
            }
        };
        _timer.Start();

        // ═══════════════════════════════════════════
        //  INITIAL UPDATE
        // ═══════════════════════════════════════════
        updateUI();

        // ═══════════════════════════════════════════
        //  CLEANUP
        // ═══════════════════════════════════════════
        root.Unloaded += (_, _) =>
        {
            try
            {
                _timer?.Stop();
                _timer = null;
                PrayerService.DataChanged -= onDataChanged;
            }
            catch { }
        };

        return root;
    }

    // ═══════════════════════════════════════════
    //  Build Pill
    // ═══════════════════════════════════════════
    private (Border pill, TextBlock timeText, TextBlock labelText) BuildPill(PrayerTime prayer)
    {
        var pill = new Border
        {
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(4, 8, 4, 8),
            Margin = new Thickness(2, 0, 2, 0),
            Background = Brushes.Transparent
        };

        var stack = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center
        };

        // ── Time ──
        var timeText = new TextBlock
        {
            Text = prayer.Time,
            FontSize = 11,
            FontWeight = FontWeights.SemiBold,
            FontFamily = new FontFamily("Cascadia Mono, Consolas, Courier New"),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        timeText.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextPrimary");
        stack.Children.Add(timeText);

        // ── Label ──
        var labelText = new TextBlock
        {
            Text = prayer.Name,
            FontSize = 8,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 3, 0, 0)
        };
        labelText.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextMuted");
        stack.Children.Add(labelText);

        pill.Child = stack;

        return (pill, timeText, labelText);
    }

    // ═══════════════════════════════════════════
    //  Update Progress Bar
    // ═══════════════════════════════════════════
    private void UpdateProgress(PrayerData data, Grid progressBar, Border progressFill)
    {
        try
        {
            if (data.Prayers == null || data.Prayers.Count == 0) return;

            var now = DateTime.Now;
            var today = DateTime.Today;

            // ── پیدا کردن نماز قبلی و بعدی ──
            PrayerTime? prev = null;
            PrayerTime? next = null;
            DateTime? prevTime = null;
            DateTime? nextTime = null;

            foreach (var p in data.Prayers)
            {
                if (!TimeSpan.TryParse(p.Time, out var ts)) continue;
                var dt = today + ts;

                if (dt <= now)
                {
                    prev = p;
                    prevTime = dt;
                }
                else if (next == null)
                {
                    next = p;
                    nextTime = dt;
                    break;
                }
            }

            // ── اگه next نبود → فجر فردا ──
            if (next == null && data.Prayers.Count > 0)
            {
                next = data.Prayers[0];
                if (TimeSpan.TryParse(next.Time, out var ts))
                    nextTime = today.AddDays(1) + ts;
            }

            // ── اگه prev نبود → عشاء دیروز ──
            if (prev == null && data.Prayers.Count > 0)
            {
                var last = data.Prayers[data.Prayers.Count - 1];
                if (TimeSpan.TryParse(last.Time, out var ts))
                {
                    prev = last;
                    prevTime = today.AddDays(-1) + ts;
                }
            }

            if (!prevTime.HasValue || !nextTime.HasValue) return;

            // ── محاسبه درصد ──
            var total = (nextTime.Value - prevTime.Value).TotalSeconds;
            var elapsed = (now - prevTime.Value).TotalSeconds;

            if (total <= 0) return;

            var pct = Math.Max(0, Math.Min(1, elapsed / total));

            // ── آپدیت width ──
            var availableWidth = progressBar.ActualWidth;
            if (availableWidth <= 0) availableWidth = 260; // fallback

            progressFill.Width = availableWidth * pct;
        }
        catch { }
    }
}