using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Saklama kosullari ve kural tabanli akilli oneri ekraninin is kurallarini yonetir.
/// </summary>
public class SaklamaKosuluManager
{
    private readonly SaklamaKosuluDal _saklamaKosuluDal = new();

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
            return "Bu ürün dondurucu veya soğuk zincir koşullarında saklanmalıdır. Çözündürülmeden ve hijyenik şekilde muhafaza edilmelidir.";
        }

        if (Icerir(metin, "soguk"))
        {
            return "Bu ürün soğuk zincirde saklanmalıdır. 0-4°C aralığında, güneş ışığından uzak şekilde muhafaza edilmelidir.";
        }

        if (Icerir(metin, "temizlik", "deterjan", "camasir", "kimyasal"))
        {
            return "Temizlik ürünleri çocuklardan uzak, serin ve kuru bir ortamda saklanmalıdır. Gıda ürünleriyle aynı alanda tutulmamalıdır.";
        }

        if (Icerir(metin, "elektronik", "sarj", "telefon", "kablo"))
        {
            return "Elektronik ürünler nemden, sudan ve yüksek sıcaklıktan uzak bir ortamda saklanmalıdır.";
        }

        if (Icerir(metin, "kirtasiye", "kalem", "kagit", "defter"))
        {
            return "Kırtasiye ürünleri kuru, temiz ve düzenli raf sisteminde saklanmalıdır.";
        }

        if (Icerir(metin, "kuru gida", "gida", "biskuvi", "un", "makarna", "pirinc"))
        {
            return "Kuru gıda ürünleri serin, kuru ve güneş almayan bir ortamda saklanmalıdır. Ambalaj kapalı tutulmalıdır.";
        }

        return "Genel saklama önerisi: Ürün kuru, temiz, havalandırılmış ve doğrudan güneş almayan bir ortamda saklanmalıdır.";
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
