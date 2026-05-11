namespace StokTakip.Entities;

/// <summary>
/// Urunlerin saklama sicakligi, nem ve raf omru gibi kosullarini temsil eder.
/// </summary>
public class SaklamaKosulu
{
    public int Id { get; set; }
    public string KosulAdi { get; set; } = string.Empty;
    public string KategoriAnahtarKelime { get; set; } = string.Empty;
    public string OnerilenSicaklik { get; set; } = string.Empty;
    public string NemOrani { get; set; } = string.Empty;
    public string SaklamaAciklamasi { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
}
