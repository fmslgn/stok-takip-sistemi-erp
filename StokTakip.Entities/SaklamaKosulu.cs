namespace StokTakip.Entities;

/// <summary>
/// Urunlerin saklama sicakligi, nem ve raf omru gibi kosullarini temsil eder.
/// </summary>
public class SaklamaKosulu
{
    public int Id { get; set; }
    public string KosulAdi { get; set; } = string.Empty;
    public string SicaklikAraligi { get; set; } = string.Empty;
    public string NemAraligi { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
}
