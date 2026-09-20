using System.Collections.Generic;
using WDesk.Core;
using WDesk.Widgets;
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
        DefaultWidth = 280,
        DefaultHeight = 220,
        HasSettings = false
    };

    public override IEnumerable<WStyle> GetStyles() => new List<WStyle>
    {
        new() { Id = "style1", Name = "Card", Icon = "\uE8F1", PreviewEmoji = "🕌" }
    };

    public override IStyleBuilder? GetStyleBuilder(string styleId) => new Style1();
}