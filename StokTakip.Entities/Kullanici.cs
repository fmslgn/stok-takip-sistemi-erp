namespace StokTakip.Entities;

/// <summary>
/// Sisteme giris yapacak kullanici bilgilerini temsil eder.
/// </summary>
public class Kullanici
{
    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = string.Empty;
    public string Sifre { get; set; } = string.Empty;
    public string AdSoyad { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
}
