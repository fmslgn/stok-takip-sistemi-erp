namespace StokTakip.Entities;

/// <summary>
/// Urunlerin depodan cikis hareketlerini temsil eder.
/// </summary>
public class StokCikis
{
    public int Id { get; set; }
    public int UrunId { get; set; }
    public int KullaniciId { get; set; }
    public int Miktar { get; set; }
    public string Aciklama { get; set; } = string.Empty;
    public DateTime CikisTarihi { get; set; } = DateTime.Now;
}
