using System;
using System.Collections.Generic;

namespace WDesk.Widgets.Prayer;

public class PrayerData
{
    // ═══ Location ═══
    public string City { get; set; } = "Tehran";
    public string Country { get; set; } = "Iran";

    // ═══ Times (raw) ═══
    public string Fajr { get; set; } = "—";
    public string Sunrise { get; set; } = "—";
    public string Dhuhr { get; set; } = "—";
    public string Asr { get; set; } = "—";
    public string Maghrib { get; set; } = "—";
    public string Isha { get; set; } = "—";

    // ═══ Next Prayer ═══
    public string NextPrayerName { get; set; } = "";
    public string NextPrayerNameFa { get; set; } = "";
    public string NextPrayerTime { get; set; } = "";
    public TimeSpan TimeUntilNext { get; set; }

    // ═══ Date ═══
    public string HijriDate { get; set; } = "";
    public string HijriMonth { get; set; } = "";

    // ═══ List ═══
    public List<PrayerTime> Prayers { get; set; } = new();

    // ═══ Display ═══
    public string DisplayCountdown
    {
        get
        {
            if (TimeUntilNext <= TimeSpan.Zero) return "—";

            var h = (int)TimeUntilNext.TotalHours;
            var m = TimeUntilNext.Minutes;

            if (h > 0) return $"{h}h {m}m";
            return $"{m}m";
        }
    }
}

public class PrayerTime
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string NameFa { get; set; } = "";
    public string Time { get; set; } = "";
    public bool IsNext { get; set; } = false;
}