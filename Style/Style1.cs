using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WDesk.Core;

namespace WDesk.Widgets.Prayer.Style;

public class Style1 : IStyleBuilder
{
    public string StyleId => "style1";

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
            Margin = new Thickness(16, 14, 16, 14)
        };
        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });                       // Header
        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });                       // Hero
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // List
        root.Children.Add(content);

        // ═══════════════════════════════════════════
        //  HEADER — Icon + Title + City
        // ═══════════════════════════════════════════
        var header = new Grid();
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // آیکون
        var iconBox = new Border
        {
            Width = 32,
            Height = 32,
            CornerRadius = new CornerRadius(9)
        };
        iconBox.SetResourceReference(Border.BackgroundProperty, "WidgetAccentFaded");

        var iconText = new TextBlock
        {
            Text = "\uE8C0",  // مسجد
            FontFamily = new FontFamily("Segoe Fluent Icons, Segoe MDL2 Assets"),
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        iconText.SetResourceReference(TextBlock.ForegroundProperty, "WidgetAccent");
        iconBox.Child = iconText;
        Grid.SetColumn(iconBox, 0);
        header.Children.Add(iconBox);

        // عنوان + شهر
        var titleStack = new StackPanel
        {
            Margin = new Thickness(10, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        var titleText = new TextBlock
        {
            Text = "PRAYER TIMES",
            FontSize = 11,
            FontWeight = FontWeights.SemiBold
        };
        titleText.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextPrimary");
        titleStack.Children.Add(titleText);

        var cityText = new TextBlock
        {
            Text = "Tehran, Iran",
            FontSize = 10,
            Margin = new Thickness(0, 1, 0, 0)
        };
        cityText.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextMuted");
        titleStack.Children.Add(cityText);

        Grid.SetColumn(titleStack, 1);
        header.Children.Add(titleStack);

        // تاریخ شمسی (راست)
        var hijriText = new TextBlock
        {
            Text = "",
            FontSize = 9,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Right
        };
        hijriText.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextMuted");
        Grid.SetColumn(hijriText, 2);
        header.Children.Add(hijriText);

        Grid.SetRow(header, 0);
        content.Children.Add(header);

        // ═══════════════════════════════════════════
        //  HERO — Next Prayer
        // ═══════════════════════════════════════════
        var hero = new Border
        {
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(14, 10, 14, 10),
            Margin = new Thickness(0, 12, 0, 12)
        };
        hero.SetResourceReference(Border.BackgroundProperty, "WidgetAccentFaded");

        var heroGrid = new Grid();
        heroGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        heroGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // ستون چپ: عنوان + نام نماز
        var heroLeft = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center
        };

        var nextLabel = new TextBlock
        {
            Text = "NEXT PRAYER",
            FontSize = 8,
            FontWeight = FontWeights.SemiBold,
            Opacity = 0.7
        };
        nextLabel.SetResourceReference(TextBlock.ForegroundProperty, "WidgetAccent");
        heroLeft.Children.Add(nextLabel);

        var nextName = new TextBlock
        {
            Text = "—",
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 2, 0, 0)
        };
        nextName.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextPrimary");
        heroLeft.Children.Add(nextName);

        Grid.SetColumn(heroLeft, 0);
        heroGrid.Children.Add(heroLeft);

        // ستون راست: ساعت + countdown
        var heroRight = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        var nextTime = new TextBlock
        {
            Text = "—",
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        nextTime.SetResourceReference(TextBlock.ForegroundProperty, "WidgetAccent");
        heroRight.Children.Add(nextTime);

        var countdown = new TextBlock
        {
            Text = "",
            FontSize = 10,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 2, 0, 0)
        };
        countdown.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextMuted");
        heroRight.Children.Add(countdown);

        Grid.SetColumn(heroRight, 1);
        heroGrid.Children.Add(heroRight);

        hero.Child = heroGrid;
        Grid.SetRow(hero, 1);
        content.Children.Add(hero);

        // ═══════════════════════════════════════════
        //  LIST — All Prayers
        // ═══════════════════════════════════════════
        var listStack = new StackPanel();

        // ★ نگه‌داری reference به TextBlockها برای آپدیت
        var rowTimes = new System.Collections.Generic.Dictionary<string, TextBlock>();
        var rowBorders = new System.Collections.Generic.Dictionary<string, Border>();
        var rowDots = new System.Collections.Generic.Dictionary<string, System.Windows.Shapes.Ellipse>();

        var prayers = PrayerService.GetCurrent().Prayers;
        if (prayers == null || prayers.Count == 0)
        {
            // Placeholder اگه دیتا هنوز لود نشده
            prayers = new System.Collections.Generic.List<PrayerTime>
            {
                new() { Key = "fajr",    Name = "Fajr",    NameFa = "فجر",  Time = "—" },
                new() { Key = "sunrise", Name = "Sunrise", NameFa = "طلوع", Time = "—" },
                new() { Key = "dhuhr",   Name = "Dhuhr",   NameFa = "ظهر",  Time = "—" },
                new() { Key = "asr",     Name = "Asr",     NameFa = "عصر",  Time = "—" },
                new() { Key = "maghrib", Name = "Maghrib", NameFa = "مغرب", Time = "—" },
                new() { Key = "isha",    Name = "Isha",    NameFa = "عشاء", Time = "—" },
            };
        }

        foreach (var prayer in prayers)
        {
            var row = BuildPrayerRow(prayer, out var timeBlock, out var border, out var dot);
            listStack.Children.Add(row);

            rowTimes[prayer.Key] = timeBlock;
            rowBorders[prayer.Key] = border;
            rowDots[prayer.Key] = dot;
        }

        Grid.SetRow(listStack, 2);
        content.Children.Add(listStack);

        // ═══════════════════════════════════════════
        //  UPDATE FUNCTION
        // ═══════════════════════════════════════════
        Action updateUI = () =>
        {
            var data = PrayerService.GetCurrent();
            if (data == null) return;

            // Header
            cityText.Text = $"{data.City}, {data.Country}";

            if (!string.IsNullOrEmpty(data.HijriDate))
                hijriText.Text = data.HijriDate;

            // Hero
            nextName.Text = !string.IsNullOrEmpty(data.NextPrayerName)
                ? data.NextPrayerName.ToUpperInvariant()
                : "—";
            nextTime.Text = data.NextPrayerTime ?? "—";
            countdown.Text = !string.IsNullOrEmpty(data.DisplayCountdown)
                ? $"in {data.DisplayCountdown}"
                : "";

            // List
            foreach (var prayer in data.Prayers)
            {
                if (!rowTimes.TryGetValue(prayer.Key, out var tb)) continue;
                if (!rowBorders.TryGetValue(prayer.Key, out var bd)) continue;
                if (!rowDots.TryGetValue(prayer.Key, out var dt)) continue;

                tb.Text = prayer.Time;

                if (prayer.IsNext)
                {
                    bd.SetResourceReference(Border.BackgroundProperty, "WidgetAccentFaded");
                    tb.SetResourceReference(TextBlock.ForegroundProperty, "WidgetAccent");
                    tb.FontWeight = FontWeights.Bold;
                    dt.Visibility = Visibility.Visible;
                }
                else
                {
                    bd.Background = Brushes.Transparent;
                    tb.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextPrimary");
                    tb.FontWeight = FontWeights.Normal;
                    dt.Visibility = Visibility.Collapsed;
                }
            }
        };

        // ═══════════════════════════════════════════
        //  SUBSCRIBE TO SERVICE
        // ═══════════════════════════════════════════
        Action onDataChanged = () =>
        {
            root.Dispatcher.BeginInvoke(new Action(updateUI));
        };

        PrayerService.DataChanged += onDataChanged;

        // ═══════════════════════════════════════════
        //  TIMER — هر ۱ ثانیه countdown
        // ═══════════════════════════════════════════
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) =>
        {
            PrayerService.CalculateNext();
            var data = PrayerService.GetCurrent();
            if (data != null)
            {
                countdown.Text = !string.IsNullOrEmpty(data.DisplayCountdown)
                    ? $"in {data.DisplayCountdown}"
                    : "";
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
    //  Build Prayer Row
    // ═══════════════════════════════════════════
    private Border BuildPrayerRow(
        PrayerTime prayer,
        out TextBlock timeBlock,
        out Border rowBorder,
        out System.Windows.Shapes.Ellipse dot)
    {
        var border = new Border
        {
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(10, 5, 10, 5),
            Margin = new Thickness(0, 0, 0, 2),
            Background = Brushes.Transparent
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });                       // Dot
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Name
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });                       // Time

        // ★ دات نماز بعدی
        var dotEl = new System.Windows.Shapes.Ellipse
        {
            Width = 6,
            Height = 6,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 8, 0),
            Visibility = Visibility.Collapsed
        };
        dotEl.SetResourceReference(System.Windows.Shapes.Shape.FillProperty, "WidgetAccent");
        Grid.SetColumn(dotEl, 0);
        grid.Children.Add(dotEl);

        // نام
        var nameBlock = new TextBlock
        {
            Text = prayer.Name,
            FontSize = 11,
            VerticalAlignment = VerticalAlignment.Center
        };
        nameBlock.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextSecondary");
        Grid.SetColumn(nameBlock, 1);
        grid.Children.Add(nameBlock);

        // زمان
        var time = new TextBlock
        {
            Text = prayer.Time,
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center
        };
        time.SetResourceReference(TextBlock.ForegroundProperty, "WidgetTextPrimary");
        Grid.SetColumn(time, 2);
        grid.Children.Add(time);

        border.Child = grid;

        timeBlock = time;
        rowBorder = border;
        dot = dotEl;

        return border;
    }
}