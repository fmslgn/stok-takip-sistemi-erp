using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Saklama kosullari ve kural tabanli akilli oneri ekraninin is kurallarini yonetir.
/// </summary>
public class SaklamaKosuluManager
{
    private readonly SaklamaKosuluDal _saklamaKosuluDal = new();
    private readonly WebUrunBilgiService _webUrunBilgiService = new();

    /// <summary>
    /// Tum saklama kosullarini listeler.
    /// </summary>
    public List<SaklamaKosulu> GetAll()
    {
        return _saklamaKosuluDal.GetAll();
    }

    /// <summary>
    /// Id bilgisine gore saklama kosulu getirir.
    /// </summary>
    public SaklamaKosulu? GetById(int id)
    {
        IdKontrol(id);
        return _saklamaKosuluDal.GetById(id);
    }

    /// <summary>
    /// Saklama kosulu bilgilerini kontrol edip yeni kayit ekler.
    /// </summary>
    public int Add(SaklamaKosulu saklamaKosulu)
    {
        SaklamaKosuluDogrula(saklamaKosulu);
        return _saklamaKosuluDal.Add(saklamaKosulu);
    }

    /// <summary>
    /// Saklama kosulu bilgilerini kontrol edip mevcut kaydi gunceller.
    /// </summary>
    public void Update(SaklamaKosulu saklamaKosulu)
    {
        IdKontrol(saklamaKosulu.Id);
        SaklamaKosuluDogrula(saklamaKosulu);
        _saklamaKosuluDal.Update(saklamaKosulu);
    }

    /// <summary>
    /// Id kontrolu yapildiktan sonra saklama kosulu kaydini siler.
    /// </summary>
    public void Delete(int id)
    {
        IdKontrol(id);
        _saklamaKosuluDal.Delete(id);
    }

    /// <summary>
    /// Urun adi ve kategoriye gore kural tabanli saklama onerisi uretir.
    /// </summary>
    public string SaklamaKosuluOner(string urunAdi, string kategoriAdi)
    {
        var urun = NormalizeText(urunAdi);
        var kategori = NormalizeText(kategoriAdi);
        var metin = $"{urun} {kategori}";

        // Sut ve benzeri hassas gidalarda kategori ne olursa olsun soguk zincir onceliklidir.
        if (Icerir(metin, "sut", "yogurt", "peynir"))
        {
            return "Bu ürün soğuk zincirde saklanmalıdır. 0-4°C aralığında, güneş ışığından uzak şekilde muhafaza edilmelidir.";
        }

        if (Icerir(metin, "et", "tavuk", "balik"))
        {
            return "Bu ürün soğuk zincirde veya dondurucuda saklanmalıdır. Çözündürüldükten sonra tekrar dondurulmamalıdır.";
        }

        if (Icerir(metin, "kuru gida", "gida", "makarna", "pirinc", "biskuvi"))
        {
            return "Ürün kuru, temiz, havalandırılmış ve doğrudan güneş almayan bir ortamda saklanmalıdır.";
        }

        if (Icerir(metin, "temizlik", "camasir suyu", "deterjan"))
        {
            return "Çocuklardan uzak, serin ve kuru ortamda saklanmalıdır. Gıda ürünleriyle aynı alanda saklanmamalıdır.";
        }

        if (Icerir(metin, "elektronik", "sarj", "telefon", "kablo"))
        {
            return "Nemden, sıvı temasından ve yüksek sıcaklıktan uzak bir ortamda saklanmalıdır.";
        }

        if (Icerir(metin, "kirtasiye", "defter", "kalem", "kagit"))
        {
            return "Kuru ve temiz ortamda saklanmalıdır. Ürünler düzenli raf sisteminde muhafaza edilmelidir.";
        }

        return "Genel saklama önerisi: Ürün kuru, temiz, havalandırılmış ve doğrudan güneş almayan bir ortamda saklanmalıdır.";
    }

    /// <summary>
    /// Once web verisiyle urun turunu zenginlestirir; veri alinamazsa sessizce kural tabanli yedek oneriyi kullanir.
    /// </summary>
    public async Task<string> SaklamaKosuluOnerWebDestekliAsync(string urunAdi, string kategoriAdi, string barkod)
    {
        try
        {
            string? webKategoriBilgisi = await _webUrunBilgiService.UrunKategoriBilgisiGetirAsync(urunAdi, barkod);
            if (!string.IsNullOrWhiteSpace(webKategoriBilgisi))
            {
                return SaklamaKosuluOner($"{urunAdi} {webKategoriBilgisi}", $"{kategoriAdi} {webKategoriBilgisi}");
            }
        }
        catch
        {
            // Teknik web hatalari kullaniciya yansitilmez; sistem kural tabanli oneriyi uretmeye devam eder.
        }

        return SaklamaKosuluOner(urunAdi, kategoriAdi);
    }

    /// <summary>
    /// Kullanici revize notuna gore mevcut saklama onerisine kural tabanli ekleme veya kisaltma uygular.
    /// </summary>
    public string SaklamaOnerisiniRevizeEt(string mevcutOneri, string revizeNotu)
    {
        string oneri = (mevcutOneri ?? string.Empty).Trim();
        string not = NormalizeText(revizeNotu);

        if (string.IsNullOrWhiteSpace(oneri))
        {
            throw new ArgumentException("Revize edilecek saklama onerisi bulunamadi.");
        }

        if (string.IsNullOrWhiteSpace(not))
        {
            throw new ArgumentException("Revize istegi bos birakilamaz.");
        }

        if (Icerir(not, "kisa", "ozet"))
        {
            return OneriyiKisalt(oneri);
        }

        if (Icerir(not, "detayli"))
        {
            return CumleEkle(oneri, "Ürün, sıcaklık değişimlerinden korunmalı ve mümkün olduğunca kapalı ambalajında saklanmalıdır.");
        }

        if (Icerir(not, "guvenli", "uyari"))
        {
            return CumleEkle(oneri, "Güvenlik için ürün çocukların erişemeyeceği, temiz ve düzenli bir alanda kontrol altında tutulmalıdır.");
        }

        if (Icerir(not, "sicaklik"))
        {
            return SicaklikBilgisiyleRevizeEt(oneri);
        }

        return CumleEkle(oneri, "Revize notu dikkate alınarak ürün düzenli aralıklarla kontrol edilmelidir.");
    }

    /// <summary>
    /// Saklama Asistani chat ekraninda kullanicinin sorusuna kural tabanli, anlasilir cevap uretir.
    /// </summary>
    public string SaklamaChatbotYanitiUret(
        string urunAdi,
        string kategoriAdi,
        string barkod,
        string mevcutOneri,
        string kullaniciMesaji)
    {
        string mesaj = NormalizeText(kullaniciMesaji);
        string mesajdanUrunIpucu = MesajdanUrunIpucuAl(mesaj);
        string etkiliUrunAdi = string.IsNullOrWhiteSpace(urunAdi) ? mesajdanUrunIpucu : urunAdi;
        string temelOneri = string.IsNullOrWhiteSpace(mevcutOneri)
            ? SaklamaKosuluOner(etkiliUrunAdi, kategoriAdi)
            : mevcutOneri.Trim();

        if (string.IsNullOrWhiteSpace(mesaj))
        {
            throw new ArgumentException("Mesaj bos birakilamaz.");
        }

        if (Icerir(mesaj, "kisa", "ozet", "daha kisa"))
        {
            return $"Kısa özet: {OneriyiKisalt(temelOneri)}";
        }

        if (Icerir(mesaj, "detayli", "ayrintili"))
        {
            return CumleEkle(temelOneri, "Daha güvenli kullanım için ürünün ambalajı kapalı tutulmalı, saklama alanı düzenli kontrol edilmeli ve ürün benzer türlerle aynı raf düzeninde muhafaza edilmelidir.");
        }

        if (Icerir(mesaj, "neden", "niye", "sebep"))
        {
            return $"Bu önerinin nedeni: {SaklamaGerekcesiUret(urunAdi, kategoriAdi, temelOneri)}";
        }

        if (Icerir(mesaj, "sicaklik", "derece", "kac derece"))
        {
            return SicaklikCevabiUret(temelOneri);
        }

        if (Icerir(mesaj, "guvenli mi", "guvenli", "uyari", "risk", "tehlike"))
        {
            return "Güvenlik uyarısı: Ürün uygun saklama alanında, çocukların erişemeyeceği ve farklı ürün türleriyle karışmayacağı şekilde tutulmalıdır. Ambalaj hasarlıysa ürün ayrıca kontrol edilmelidir.";
        }

        if (Icerir(mesaj, "raf", "depo", "ambalaj", "yerlestir"))
        {
            return "Depolama önerisi: Ürün raf üzerinde düzenli şekilde tutulmalı, ambalajı kapalı olmalı ve nem, güneş ışığı, yoğun sıcaklık değişimi gibi etkenlerden korunmalıdır.";
        }

        if (Icerir(mesaj, "barkod") && !string.IsNullOrWhiteSpace(barkod))
        {
            return $"Barkod bilgisi ({barkod}) web destekli ürün tanıma için kullanılabilir. Veri alınamazsa sistem kural tabanlı öneriyle devam eder: {temelOneri}";
        }

        if (!string.IsNullOrWhiteSpace(mesajdanUrunIpucu))
        {
            return $"Bu ürün için önerim: {SaklamaKosuluOner(mesajdanUrunIpucu, kategoriAdi)}";
        }

        if (UrunleIlgiliSoruMu(urunAdi, kategoriAdi, mesaj))
        {
            return $"Bu ürün için önerim: {SaklamaKosuluOner(urunAdi, kategoriAdi)}";
        }

        return "Bu ürün için genel saklama önerisini tekrar kontrol edebilirim. Daha kısa, daha detaylı, sıcaklık bilgisi veya güvenlik uyarısı isteyebilirsiniz.";
    }

    /// <summary>
    /// Eski UI iskeleti ile uyumluluk icin tutulur.
    /// </summary>
    public void Hazirla(SaklamaKosulu saklamaKosulu)
    {
        _saklamaKosuluDal.Hazirla(saklamaKosulu);
    }

    private static void SaklamaKosuluDogrula(SaklamaKosulu saklamaKosulu)
    {
        if (string.IsNullOrWhiteSpace(saklamaKosulu.KosulAdi))
        {
            throw new ArgumentException("Saklama kosulu adi bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(saklamaKosulu.KategoriAnahtarKelime))
        {
            throw new ArgumentException("Kategori anahtar kelimesi bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(saklamaKosulu.OnerilenSicaklik))
        {
            throw new ArgumentException("Onerilen sicaklik bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(saklamaKosulu.NemOrani))
        {
            throw new ArgumentException("Nem orani bos birakilamaz.");
        }
    }

    private static bool Icerir(string metin, params string[] anahtarKelimeler)
    {
        return anahtarKelimeler.Any(metin.Contains);
    }

    private static string OneriyiKisalt(string oneri)
    {
        string normalOneri = NormalizeText(oneri);

        if (normalOneri.Contains("soguk zincir"))
        {
            return "Ürün soğuk zincirde, 0-4°C aralığında saklanmalıdır.";
        }

        if (normalOneri.Contains("dondurucu"))
        {
            return "Ürün soğuk zincirde veya dondurucuda saklanmalıdır.";
        }

        if (normalOneri.Contains("cocuk"))
        {
            return "Ürün çocuklardan uzak, serin ve kuru ortamda saklanmalıdır.";
        }

        int ilkCumleSonu = oneri.IndexOf('.');
        return ilkCumleSonu > 0 ? oneri[..(ilkCumleSonu + 1)] : oneri;
    }

    private static string SicaklikBilgisiyleRevizeEt(string oneri)
    {
        string normalOneri = NormalizeText(oneri);

        if (oneri.Contains('°') || normalOneri.Contains("sicaklik") || normalOneri.Contains("0-4"))
        {
            return CumleEkle(oneri, "Belirtilen sıcaklık aralığının korunmasına dikkat edilmelidir.");
        }

        return CumleEkle(oneri, "Ürün ani sıcaklık değişimlerinden uzak tutulmalı ve ortam sıcaklığı düzenli kontrol edilmelidir.");
    }

    private static string SicaklikCevabiUret(string oneri)
    {
        string normalOneri = NormalizeText(oneri);

        if (normalOneri.Contains("0-4") || normalOneri.Contains("soguk zincir"))
        {
            return "Sıcaklık bilgisi: Bu ürün için 0-4°C aralığı korunmalıdır. Soğuk zincir bozulursa ürün kalitesi ve güvenliği risk altına girebilir.";
        }

        if (normalOneri.Contains("dondurucu"))
        {
            return "Sıcaklık bilgisi: Ürün dondurucuda saklanacaksa çözündürüldükten sonra tekrar dondurulmamalıdır.";
        }

        return "Sıcaklık bilgisi: Ürün ani sıcaklık değişiminden uzak tutulmalı, serin ve havalandırılmış bir ortamda saklanmalıdır.";
    }

    private static string SaklamaGerekcesiUret(string urunAdi, string kategoriAdi, string oneri)
    {
        string metin = NormalizeText($"{urunAdi} {kategoriAdi} {oneri}");

        if (Icerir(metin, "sut", "yogurt", "peynir", "soguk zincir"))
        {
            return "Süt ve süt ürünü gibi hassas gıdalarda sıcaklık değişimi bozulma riskini artırır, bu nedenle soğuk zincir önerilir.";
        }

        if (Icerir(metin, "et", "tavuk", "balik", "dondurucu"))
        {
            return "Et, tavuk ve balık ürünleri mikroorganizma riskine karşı soğuk ortamda veya dondurucuda saklanmalıdır.";
        }

        if (Icerir(metin, "temizlik", "deterjan", "camasir suyu"))
        {
            return "Temizlik ürünleri gıda ürünleriyle temas etmemeli ve çocuklardan uzak tutulmalıdır.";
        }

        if (Icerir(metin, "elektronik", "telefon", "kablo", "sarj"))
        {
            return "Elektronik ürünlerde nem ve sıvı teması arıza riskini artırdığı için kuru ortam önerilir.";
        }

        return "Ürünün kuru, temiz ve güneş almayan ortamda saklanması kaliteyi korumaya ve raf düzenini sağlamaya yardımcı olur.";
    }

    private static bool UrunleIlgiliSoruMu(string urunAdi, string kategoriAdi, string mesaj)
    {
        string urun = NormalizeText(urunAdi);
        string kategori = NormalizeText(kategoriAdi);

        return (!string.IsNullOrWhiteSpace(urun) && mesaj.Contains(urun)) ||
               (!string.IsNullOrWhiteSpace(kategori) && mesaj.Contains(kategori)) ||
               Icerir(mesaj, "urun", "saklama", "kosul", "muhafaza");
    }

    private static string MesajdanUrunIpucuAl(string mesaj)
    {
        string[] anahtarlar =
        [
            "sut",
            "yogurt",
            "peynir",
            "et",
            "tavuk",
            "balik",
            "makarna",
            "pirinc",
            "biskuvi",
            "deterjan",
            "camasir suyu",
            "telefon",
            "kablo",
            "sarj",
            "defter",
            "kalem",
            "kagit"
        ];

        return anahtarlar.FirstOrDefault(mesaj.Contains) ?? string.Empty;
    }

    private static string CumleEkle(string mevcutOneri, string eklenecekCumle)
    {
        string temizOneri = mevcutOneri.Trim();
        return temizOneri.EndsWith('.') ? $"{temizOneri} {eklenecekCumle}" : $"{temizOneri}. {eklenecekCumle}";
    }

    /// <summary>
    /// Karsilastirma oncesi Turkce karakterleri sadelestirir ve metni kucuk harfe cevirir.
    /// </summary>
    private static string NormalizeText(string text)
    {
        return (text ?? string.Empty)
            .Trim()
            .Replace('İ', 'i')
            .Replace('I', 'i')
            .Replace('Ğ', 'g')
            .Replace('Ü', 'u')
            .Replace('Ş', 's')
            .Replace('Ö', 'o')
            .Replace('Ç', 'c')
            .ToLowerInvariant()
            .Replace('ğ', 'g')
            .Replace('ü', 'u')
            .Replace('ş', 's')
            .Replace('ı', 'i')
            .Replace('ö', 'o')
            .Replace('ç', 'c');
    }

    private static void IdKontrol(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Gecerli bir saklama kosulu secilmelidir.");
        }
    }
}
