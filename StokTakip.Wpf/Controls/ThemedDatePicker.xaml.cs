using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StokTakip.Wpf.Controls;

/// <summary>
/// Tema uyumlu tarih secici: gun, ay ve yil secim modlari; WPF Calendar kullanilmaz.
/// </summary>
public partial class ThemedDatePicker : UserControl
{
    private const int TakvimSatirSayisi = 6;
    private const int TakvimSutunSayisi = 7;
    private const int AyGridSutunSayisi = 4;
    private const int AyGridSatirSayisi = 3;
    private const int YilGridSutunSayisi = 4;
    private const int YilGridSatirSayisi = 3;
    private const int YilAralikBoyutu = 12;

    private static readonly CultureInfo TurkceKultur = new("tr-TR");

    private DateTime _goruntulenenAy;
    private CalendarViewMode _gorunumModu = CalendarViewMode.Days;
    private int _yilAraligiBaslangic;
    private bool _gunAdlariHazir;

    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register(
            nameof(SelectedDate),
            typeof(DateTime?),
            typeof(ThemedDatePicker),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public ThemedDatePicker()
    {
        InitializeComponent();
        _goruntulenenAy = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        _yilAraligiBaslangic = HesaplaYilAraligiBaslangic(_goruntulenenAy.Year);
        Loaded += (_, _) => GuncelleGorunum();
        IsEnabledChanged += (_, _) => GuncelleGorunum();
    }

    private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ThemedDatePicker picker)
        {
            picker.GuncelleTarihMetni();
            if (picker.TakvimPopup.IsOpen)
            {
                picker.RenderCalendar();
            }
        }
    }

    private void BtnTakvim_Click(object sender, RoutedEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        var referans = SelectedDate ?? DateTime.Today;
        _goruntulenenAy = new DateTime(referans.Year, referans.Month, 1);
        _yilAraligiBaslangic = HesaplaYilAraligiBaslangic(_goruntulenenAy.Year);
        _gorunumModu = CalendarViewMode.Days;
        RenderCalendar();
        TakvimPopup.IsOpen = true;
        InputChrome.BorderBrush = (Brush)FindResource("PrimaryBrush");
    }

    private void BtnBaslik_Click(object sender, RoutedEventArgs e)
    {
        switch (_gorunumModu)
        {
            case CalendarViewMode.Days:
                _gorunumModu = CalendarViewMode.Months;
                break;
            case CalendarViewMode.Months:
                _yilAraligiBaslangic = HesaplaYilAraligiBaslangic(_goruntulenenAy.Year);
                _gorunumModu = CalendarViewMode.Years;
                break;
        }

        RenderCalendar();
    }

    private void BtnOncekiAy_Click(object sender, RoutedEventArgs e)
    {
        switch (_gorunumModu)
        {
            case CalendarViewMode.Days:
                ChangeMonth(-1);
                break;
            case CalendarViewMode.Months:
                ChangeYear(-1);
                break;
            case CalendarViewMode.Years:
                ChangeYearRange(-1);
                break;
        }
    }

    private void BtnSonrakiAy_Click(object sender, RoutedEventArgs e)
    {
        switch (_gorunumModu)
        {
            case CalendarViewMode.Days:
                ChangeMonth(1);
                break;
            case CalendarViewMode.Months:
                ChangeYear(1);
                break;
            case CalendarViewMode.Years:
                ChangeYearRange(1);
                break;
        }
    }

    private void TakvimPopup_Closed(object? sender, EventArgs e)
    {
        _gorunumModu = CalendarViewMode.Days;
        KenarlikRenginiGuncelle();
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        if (IsEnabled && !TakvimPopup.IsOpen)
        {
            InputChrome.BorderBrush = (Brush)FindResource("InputHoverBorderBrush");
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if (!TakvimPopup.IsOpen)
        {
            KenarlikRenginiGuncelle();
        }
    }

    /// <summary>Secili tarihi dd.MM.yyyy olarak gosterir.</summary>
    private void GuncelleTarihMetni()
    {
        TxtTarihGosterim.Text = SelectedDate?.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private void GuncelleGorunum()
    {
        InputChrome.Opacity = IsEnabled ? 1 : 0.55;
        GuncelleTarihMetni();
        KenarlikRenginiGuncelle();
    }

    private void KenarlikRenginiGuncelle()
    {
        InputChrome.BorderBrush = (Brush)FindResource("InputBorderBrush");
    }

    /// <summary>Aktif moda gore basligi ve icerik alanini yeniden cizer.</summary>
    private void RenderCalendar()
    {
        GuncelleBaslikMetni();
        GunAdlariPanel.Visibility = _gorunumModu == CalendarViewMode.Days
            ? Visibility.Visible
            : Visibility.Collapsed;

        switch (_gorunumModu)
        {
            case CalendarViewMode.Days:
                RenderDaysView();
                break;
            case CalendarViewMode.Months:
                RenderMonthsView();
                break;
            case CalendarViewMode.Years:
                RenderYearsView();
                break;
        }
    }

    /// <summary>Gun secimi modu: 6x7 gun butonlari.</summary>
    private void RenderDaysView()
    {
        GunAdlariniHazirla();
        IcerikGrid.Children.Clear();
        IcerikGrid.RowDefinitions.Clear();
        IcerikGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < TakvimSatirSayisi; i++)
        {
            IcerikGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        for (int i = 0; i < TakvimSutunSayisi; i++)
        {
            IcerikGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        var ayinIlkGunu = new DateTime(_goruntulenenAy.Year, _goruntulenenAy.Month, 1);
        int ayGunSayisi = DateTime.DaysInMonth(ayinIlkGunu.Year, ayinIlkGunu.Month);
        int baslangicOfseti = HaftaninBaslangicOfseti(ayinIlkGunu);

        for (int hucre = 0; hucre < TakvimSatirSayisi * TakvimSutunSayisi; hucre++)
        {
            int gunNumarasi = hucre - baslangicOfseti + 1;
            DateTime hucreTarihi = ayinIlkGunu.AddDays(gunNumarasi - 1);
            bool buAy = gunNumarasi >= 1 && gunNumarasi <= ayGunSayisi;

            var gunButonu = OlusturGunButonu(hucreTarihi, buAy);
            Grid.SetRow(gunButonu, hucre / TakvimSutunSayisi);
            Grid.SetColumn(gunButonu, hucre % TakvimSutunSayisi);
            IcerikGrid.Children.Add(gunButonu);
        }
    }

    /// <summary>Ay secimi modu: 12 ay, 4x3 grid.</summary>
    private void RenderMonthsView()
    {
        IcerikGrid.Children.Clear();
        IcerikGrid.RowDefinitions.Clear();
        IcerikGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < AyGridSatirSayisi; i++)
        {
            IcerikGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        for (int i = 0; i < AyGridSutunSayisi; i++)
        {
            IcerikGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        for (int ay = 1; ay <= 12; ay++)
        {
            int indeks = ay - 1;
            bool secili = _goruntulenenAy.Month == ay;
            string ayAdi = TurkceKultur.DateTimeFormat.GetMonthName(ay);
            var buton = OlusturSecimButonu(ayAdi, secili, () => SelectMonth(ay));
            Grid.SetRow(buton, indeks / AyGridSutunSayisi);
            Grid.SetColumn(buton, indeks % AyGridSutunSayisi);
            IcerikGrid.Children.Add(buton);
        }
    }

    /// <summary>Yil secimi modu: 12 yillik aralik, 4x3 grid.</summary>
    private void RenderYearsView()
    {
        IcerikGrid.Children.Clear();
        IcerikGrid.RowDefinitions.Clear();
        IcerikGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < YilGridSatirSayisi; i++)
        {
            IcerikGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        for (int i = 0; i < YilGridSutunSayisi; i++)
        {
            IcerikGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        for (int i = 0; i < YilAralikBoyutu; i++)
        {
            int yil = _yilAraligiBaslangic + i;
            bool secili = _goruntulenenAy.Year == yil;
            var buton = OlusturSecimButonu(yil.ToString(TurkceKultur), secili, () => SelectYear(yil));
            Grid.SetRow(buton, i / YilGridSutunSayisi);
            Grid.SetColumn(buton, i % YilGridSutunSayisi);
            IcerikGrid.Children.Add(buton);
        }
    }

    /// <summary>Onceki veya sonraki aya gecer.</summary>
    private void ChangeMonth(int monthOffset)
    {
        _goruntulenenAy = _goruntulenenAy.AddMonths(monthOffset);
        RenderCalendar();
    }

    /// <summary>Ay secimi modunda onceki veya sonraki yila gecer.</summary>
    private void ChangeYear(int yearOffset)
    {
        _goruntulenenAy = _goruntulenenAy.AddYears(yearOffset);
        RenderCalendar();
    }

    /// <summary>Yil secimi modunda 12 yillik araligi kaydirir.</summary>
    private void ChangeYearRange(int rangeOffset)
    {
        _yilAraligiBaslangic += rangeOffset * YilAralikBoyutu;
        RenderCalendar();
    }

    /// <summary>Secilen aya gecip gun moduna doner.</summary>
    private void SelectMonth(int month)
    {
        _goruntulenenAy = new DateTime(_goruntulenenAy.Year, month, 1);
        _gorunumModu = CalendarViewMode.Days;
        RenderCalendar();
    }

    /// <summary>Secilen yila gecip ay moduna doner.</summary>
    private void SelectYear(int year)
    {
        _goruntulenenAy = new DateTime(year, _goruntulenenAy.Month, 1);
        _gorunumModu = CalendarViewMode.Months;
        RenderCalendar();
    }

    /// <summary>Kullanicinin sectigi gunu kaydeder ve popup'i kapatir.</summary>
    private void SelectDate(DateTime date)
    {
        SelectedDate = date.Date;
        TakvimPopup.IsOpen = false;
        GuncelleTarihMetni();
    }

    private void GuncelleBaslikMetni()
    {
        TxtAyYil.Text = _gorunumModu switch
        {
            CalendarViewMode.Days => _goruntulenenAy.ToString("MMMM yyyy", TurkceKultur),
            CalendarViewMode.Months => _goruntulenenAy.Year.ToString(TurkceKultur),
            CalendarViewMode.Years => $"{_yilAraligiBaslangic} - {_yilAraligiBaslangic + YilAralikBoyutu - 1}",
            _ => string.Empty,
        };

        BtnBaslik.IsEnabled = _gorunumModu != CalendarViewMode.Years;
    }

    private static int HesaplaYilAraligiBaslangic(int yil)
    {
        return yil - 6;
    }

    private void GunAdlariniHazirla()
    {
        if (_gunAdlariHazir)
        {
            return;
        }

        GunAdlariPanel.Children.Clear();
        var gunAdlari = TurkceKultur.DateTimeFormat.AbbreviatedDayNames;
        int ilkGun = (int)TurkceKultur.DateTimeFormat.FirstDayOfWeek;

        for (int i = 0; i < TakvimSutunSayisi; i++)
        {
            var etiket = new TextBlock
            {
                Text = gunAdlari[(i + ilkGun) % 7].ToUpper(TurkceKultur),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
            };
            etiket.SetResourceReference(ForegroundProperty, "CalendarMutedTextBrush");
            GunAdlariPanel.Children.Add(etiket);
        }

        _gunAdlariHazir = true;
    }

    private static int HaftaninBaslangicOfseti(DateTime ayinIlkGunu)
    {
        int ilkGunIndeksi = (int)TurkceKultur.DateTimeFormat.FirstDayOfWeek;
        int ayinIlkGunIndeksi = (int)ayinIlkGunu.DayOfWeek;
        return (ayinIlkGunIndeksi - ilkGunIndeksi + 7) % 7;
    }

    private Button OlusturGunButonu(DateTime tarih, bool buAy)
    {
        bool secili = SelectedDate.HasValue && SelectedDate.Value.Date == tarih.Date;
        bool bugun = tarih.Date == DateTime.Today;

        var buton = new Button
        {
            Content = tarih.Day.ToString(TurkceKultur),
            Tag = tarih,
            Style = (Style)FindResource(
                secili ? "CalendarPickerButtonSelectedStyle"
                : buAy ? "CalendarPickerButtonStyle" : "CalendarPickerButtonMutedStyle"),
            MinWidth = 34,
            MinHeight = 32,
            Margin = new Thickness(1),
            Padding = new Thickness(0),
            BorderThickness = bugun && !secili ? new Thickness(1) : new Thickness(0),
        };

        if (bugun && !secili)
        {
            buton.SetResourceReference(BorderBrushProperty, "CalendarTodayBorderBrush");
        }

        buton.Click += (_, _) => SelectDate(tarih);
        return buton;
    }

    /// <summary>Ay ve yil secim hucreleri icin ortak buton.</summary>
    private Button OlusturSecimButonu(string metin, bool secili, Action tiklama)
    {
        var buton = new Button
        {
            Content = metin,
            Style = (Style)FindResource(
                secili ? "CalendarPickerButtonSelectedStyle" : "CalendarPickerButtonStyle"),
            MinWidth = 58,
            MinHeight = 36,
            Margin = new Thickness(2),
            Padding = new Thickness(4, 6, 4, 6),
        };

        buton.Click += (_, _) => tiklama();
        return buton;
    }
}
