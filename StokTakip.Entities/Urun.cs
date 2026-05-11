namespace StokTakip.Entities;

/// <summary>
/// Depoda takip edilen urun bilgilerini temsil eder.
/// </summary>
public class Urun
{
    public int Id { get; set; }
    public int KategoriId { get; set; }
    public int? SaklamaKosuluId { get; set; }
    public string UrunAdi { get; set; } = string.Empty;
    public string Barkod { get; set; } = string.Empty;
    public string Birim { get; set; } = string.Empty;
    public decimal BirimFiyat { get; set; }
    public int MevcutStok { get; set; }
    public int KritikStokSeviyesi { get; set; }
    public bool AktifMi { get; set; } = true;
}
