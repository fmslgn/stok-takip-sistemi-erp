namespace StokTakip.WinForms;

static class Program
{
    /// <summary>
    /// Uygulamanin baslangic noktasi. Ilk ekranda kullanici giris formu acilir.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Windows Forms uygulamasi icin standart gorsel ayarlari baslatilir.
        ApplicationConfiguration.Initialize();
        Application.Run(new FrmLogin());
    }    
}
