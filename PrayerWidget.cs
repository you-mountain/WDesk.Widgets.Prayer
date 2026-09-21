using System.Collections.Generic;
using WDesk.Core;
using WDesk.Widgets.Prayer.Style;

namespace WDesk.Widgets.Prayer;

public class PrayerWidget : WidgetBase
{
    public PrayerWidget()
    {
        PrayerService.Initialize();
    }

    public override WidgetMetadata Metadata { get; } = new()
    {
        Id = "prayer",
        NameKey = "widget.prayer.name",
        DescriptionKey = "widget.prayer.desc",
        Category = WidgetCategory.Islamic,
        Icon = "\uE8C0",
        Author = "WDesk Team",
        Version = "1.0.0",
        DefaultWidth = 300,
        DefaultHeight = 260,
        HasSettings = false
    };

    public override IEnumerable<WStyle> GetStyles() => new List<WStyle>
    {
        new()
        {
            Id = "style1",
            Name = "Card",
            Icon = "\uE8F1",
            PreviewEmoji = "🕌"
        },
        new()
        {
            Id = "style2",
            Name = "Digital Minimal",
            Icon = "\uE7C4",     // آیکون تایم‌لاین
            PreviewEmoji = "✨"
        }
    };

    public override IStyleBuilder? GetStyleBuilder(string styleId) => styleId switch
    {
        "style2" => new Style2(),
        _ => new Style1()   // پیش‌فرض
    };
}