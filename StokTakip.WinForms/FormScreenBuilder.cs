using System.Drawing;

namespace StokTakip.WinForms;

/// <summary>
/// Bos modul formlarinda ortak baslik, bilgi alani ve kapat butonunu hazirlar.
/// </summary>
internal static class FormScreenBuilder
{
    /// <summary>
    /// CRUD kodlari yazilmadan once modul ekranlarinin ortak iskeletini olusturur.
    /// </summary>
    public static void BuildEmptyModule(Form form, string title)
    {
        WinFormsUiHelper.ApplyFormStyle(form, $"Stok Takip Sistemi - {title}");
        form.ClientSize = new Size(520, 320);

        var lblTitle = new Label { Text = title, Location = new Point(24, 24), AutoSize = true, Font = new Font(FontFamily.GenericSansSerif, 13, FontStyle.Bold) };
        var lblInfo = new Label { Text = "Bu ekran için CRUD kodları sonraki aşamada eklenecek.", Location = new Point(24, 70), AutoSize = true };
        var btnKapat = new Button { Text = "Kapat", Location = new Point(24, 120), Size = new Size(100, 32) };

        btnKapat.Click += (_, _) =>
        {
            // Kapat butonu ilgili modul ekranini kapatir.
            form.Close();
        };

        WinFormsUiHelper.StyleSecondaryButton(btnKapat);
        form.Controls.AddRange(new Control[] { lblTitle, lblInfo, btnKapat });
        WinFormsUiHelper.AddHeader(form, title, "Bu modül proje akışında yer alır ve sonraki aşamada detaylandırılabilir.");
    }
}
