using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WDesk.Widgets.Prayer;

public static class PrayerService
{
    private static PrayerData _data = new();
    private static bool _initialized = false;

    private static readonly HttpClient _http = new()
    {
        Timeout = TimeSpan.FromSeconds(15)
    };

    // ═══════════════════════════════════════════
    //  Events
    // ═══════════════════════════════════════════
    public static event Action? DataChanged;

    // ═══════════════════════════════════════════
    //  Public API
    // ═══════════════════════════════════════════
    public static PrayerData GetCurrent() => _data;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        _data = new PrayerData();
        Debug.WriteLine("[Prayer] initialized");

        _ = RefreshAsync();

        // ═══ هر ۶ ساعت refresh کن ═══
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromHours(6)
        };
        timer.Tick += (_, _) => _ = RefreshAsync();
        timer.Start();

        // ═══ هر ۳۰ ثانیه countdown آپدیت کن ═══
        var tickTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        tickTimer.Tick += (_, _) =>
        {
            CalculateNext();
            DataChanged?.Invoke();
        };
        tickTimer.Start();
    }

    // ═══════════════════════════════════════════
    //  Refresh from API
    // ═══════════════════════════════════════════
    public static async Task RefreshAsync()
    {
        try
        {
            Debug.WriteLine("[Prayer] fetching times...");

            var url = "https://api.aladhan.com/v1/timingsByCity" +
                      $"?city={Uri.EscapeDataString(_data.City)}" +
                      $"&country={Uri.EscapeDataString(_data.Country)}" +
                      "&method=7";

            var json = await _http.GetStringAsync(url);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("data", out var dataEl))
            {
                Debug.WriteLine("[Prayer] no 'data' in response");
                return;
            }

            if (dataEl.TryGetProperty("timings", out var timings))
            {
                _data.Fajr = GetTime(timings, "Fajr");
                _data.Sunrise = GetTime(timings, "Sunrise");
                _data.Dhuhr = GetTime(timings, "Dhuhr");
                _data.Asr = GetTime(timings, "Asr");
                _data.Maghrib = GetTime(timings, "Maghrib");
                _data.Isha = GetTime(timings, "Isha");
            }

            // ★ تاریخ شمسی
            if (dataEl.TryGetProperty("date", out var dateEl))
            {
                if (dateEl.TryGetProperty("hijri", out var hijri))
                {
                    if (hijri.TryGetProperty("date", out var hd))
                        _data.HijriDate = hd.GetString() ?? "";
                    if (hijri.TryGetProperty("month", out var hm) &&
                        hm.TryGetProperty("en", out var hmEn))
                        _data.HijriMonth = hmEn.GetString() ?? "";
                }
            }

            RebuildPrayers();
            CalculateNext();

            Debug.WriteLine("[Prayer] times updated");
            DataChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Prayer] fetch failed - {ex.Message}");
        }
    }

    // ═══════════════════════════════════════════
    //  Parse Helper
    // ═══════════════════════════════════════════
    private static string GetTime(JsonElement el, string key)
    {
        if (el.TryGetProperty(key, out var v))
        {
            var str = v.GetString() ?? "";
            var spaceIdx = str.IndexOf(' ');
            if (spaceIdx > 0) str = str.Substring(0, spaceIdx);
            return str;
        }
        return "—";
    }

    // ═══════════════════════════════════════════
    //  Build Prayer List
    // ═══════════════════════════════════════════
    private static void RebuildPrayers()
    {
        _data.Prayers = new List<PrayerTime>
        {
            new() { Key = "fajr",    Name = "Fajr",    NameFa = "فجر",    Time = _data.Fajr },
            new() { Key = "sunrise", Name = "Sunrise", NameFa = "طلوع",   Time = _data.Sunrise },
            new() { Key = "dhuhr",   Name = "Dhuhr",   NameFa = "ظهر",    Time = _data.Dhuhr },
            new() { Key = "asr",     Name = "Asr",     NameFa = "عصر",    Time = _data.Asr },
            new() { Key = "maghrib", Name = "Maghrib", NameFa = "مغرب",   Time = _data.Maghrib },
            new() { Key = "isha",    Name = "Isha",    NameFa = "عشاء",   Time = _data.Isha },
        };
    }

    // ═══════════════════════════════════════════
    //  Calculate Next Prayer
    // ═══════════════════════════════════════════
    public static void CalculateNext()
    {
        try
        {
            var now = DateTime.Now;
            var today = DateTime.Today;

            PrayerTime? next = null;
            DateTime? nextTime = null;

            foreach (var p in _data.Prayers)
            {
                if (!TimeSpan.TryParse(p.Time, out var ts)) continue;
                var dt = today + ts;
                if (dt > now)
                {
                    next = p;
                    nextTime = dt;
                    break;
                }
            }

            // اگه همه‌ی نمازهای امروز گذشتن → فجر فردا
            if (next == null && _data.Prayers.Count > 0)
            {
                var first = _data.Prayers[0];
                if (TimeSpan.TryParse(first.Time, out var ts))
                {
                    next = first;
                    nextTime = today.AddDays(1) + ts;
                }
            }

            foreach (var p in _data.Prayers) p.IsNext = false;

            if (next != null && nextTime.HasValue)
            {
                next.IsNext = true;
                _data.NextPrayerName = next.Name;
                _data.NextPrayerNameFa = next.NameFa;
                _data.NextPrayerTime = next.Time;
                _data.TimeUntilNext = nextTime.Value - now;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Prayer] CalculateNext failed: {ex.Message}");
        }
    }
}