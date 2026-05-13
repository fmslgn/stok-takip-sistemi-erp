using System.Net.Http.Json;
using System.Text.Json;

namespace StokTakip.Business;

/// <summary>
/// Open Food Facts uzerinden API key gerektirmeden urun kategori bilgisi almaya calisan servis sinifidir.
/// </summary>
public class WebUrunBilgiService
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    /// <summary>
    /// Barkod veya urun adina gore web servisinden anlamli kategori bilgisi dondurur; sonuc bulunamazsa null verir.
    /// </summary>
    public async Task<string?> UrunKategoriBilgisiGetirAsync(string urunAdi, string barkod)
    {
        try
        {
            string? barkodSonucu = await BarkodIleAraAsync(barkod);
            if (!string.IsNullOrWhiteSpace(barkodSonucu))
            {
                return barkodSonucu;
            }

            return await UrunAdiIleAraAsync(urunAdi);
        }
        catch
        {
            // Internet yoksa veya servis cevap vermezse uygulama yedek kural tabanli sisteme dusecek.
            return null;
        }
    }

    private static async Task<string?> BarkodIleAraAsync(string barkod)
    {
        if (string.IsNullOrWhiteSpace(barkod))
        {
            return null;
        }

        string temizBarkod = Uri.EscapeDataString(barkod.Trim());
        string url = $"https://world.openfoodfacts.org/api/v2/product/{temizBarkod}.json?fields=product_name,generic_name,categories,categories_tags";

        using var response = await HttpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        if (!document.RootElement.TryGetProperty("status", out JsonElement status) || status.GetInt32() != 1)
        {
            return null;
        }

        return document.RootElement.TryGetProperty("product", out JsonElement product)
            ? UrunBilgisiniMetneCevir(product)
            : null;
    }

    private static async Task<string?> UrunAdiIleAraAsync(string urunAdi)
    {
        if (string.IsNullOrWhiteSpace(urunAdi))
        {
            return null;
        }

        string temizUrunAdi = Uri.EscapeDataString(urunAdi.Trim());
        string url = $"https://world.openfoodfacts.org/cgi/search.pl?search_terms={temizUrunAdi}&search_simple=1&action=process&json=1&page_size=1&fields=product_name,generic_name,categories,categories_tags";

        using var response = await HttpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        if (!document.RootElement.TryGetProperty("products", out JsonElement products) ||
            products.ValueKind != JsonValueKind.Array ||
            products.GetArrayLength() == 0)
        {
            return null;
        }

        return UrunBilgisiniMetneCevir(products[0]);
    }

    private static string? UrunBilgisiniMetneCevir(JsonElement product)
    {
        var bilgiler = new List<string>();

        AddStringProperty(product, "product_name", bilgiler);
        AddStringProperty(product, "generic_name", bilgiler);
        AddStringProperty(product, "categories", bilgiler);
        AddCategoriesTags(product, bilgiler);

        string birlesikBilgi = string.Join(" ", bilgiler.Where(x => !string.IsNullOrWhiteSpace(x)));
        if (string.IsNullOrWhiteSpace(birlesikBilgi))
        {
            return null;
        }

        string ipucu = TurkceKategoriIpucuUret(birlesikBilgi);
        return string.IsNullOrWhiteSpace(ipucu)
            ? birlesikBilgi
            : $"{birlesikBilgi} {ipucu}";
    }

    private static void AddStringProperty(JsonElement product, string propertyName, List<string> bilgiler)
    {
        if (product.TryGetProperty(propertyName, out JsonElement value) &&
            value.ValueKind == JsonValueKind.String)
        {
            bilgiler.Add(value.GetString() ?? string.Empty);
        }
    }

    private static void AddCategoriesTags(JsonElement product, List<string> bilgiler)
    {
        if (!product.TryGetProperty("categories_tags", out JsonElement tags) ||
            tags.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        foreach (JsonElement tag in tags.EnumerateArray())
        {
            if (tag.ValueKind == JsonValueKind.String)
            {
                bilgiler.Add(tag.GetString() ?? string.Empty);
            }
        }
    }

    private static string TurkceKategoriIpucuUret(string bilgi)
    {
        string metin = bilgi.ToLowerInvariant();

        if (metin.Contains("dair") || metin.Contains("milk") || metin.Contains("yogurt") || metin.Contains("cheese"))
        {
            return "sut yogurt peynir";
        }

        if (metin.Contains("meat") || metin.Contains("chicken") || metin.Contains("fish"))
        {
            return "et tavuk balik";
        }

        if (metin.Contains("pasta") || metin.Contains("rice") || metin.Contains("biscuit") || metin.Contains("snack"))
        {
            return "kuru gida makarna pirinc biskuvi";
        }

        return string.Empty;
    }
}
