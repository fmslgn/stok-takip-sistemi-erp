namespace StokTakip.Wpf;

/// <summary>
/// Ana menüdeki hızlı erişim kartlarının açacağı hedef modül (MainWindow ile gevşek bağlantı için).
/// </summary>
public enum DashboardHizliErisimHedef
{
    UrunYonetimi,
    KategoriYonetimi,
    StokGiris,
    StokCikis,
    KritikStok,
    Raporlama
}

/// <summary>
/// DashboardView tarafından üretilen hızlı erişim talebini taşır.
/// </summary>
public sealed class DashboardHizliErisimEventArgs : EventArgs
{
    public DashboardHizliErisimHedef Hedef { get; }

    public DashboardHizliErisimEventArgs(DashboardHizliErisimHedef hedef) => Hedef = hedef;
}
