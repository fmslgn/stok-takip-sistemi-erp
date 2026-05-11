namespace StokTakip.Entities;

/// <summary>
/// Urunlerin gruplandirildigi kategori kaydini temsil eder.
/// </summary>
public class Kategori
{
    public int Id { get; set; }
    public string KategoriAdi { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
}
