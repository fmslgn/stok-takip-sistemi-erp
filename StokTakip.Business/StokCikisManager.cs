using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Stok cikis surecinin is kurallarini yoneten Business sinifidir.
/// </summary>
public class StokCikisManager
{
    private readonly StokCikisDal _stokCikisDal = new();

    /// <summary>
    /// Ileride cikis miktari ve stok yeterlilik kontrolu burada yapilacak.
    /// </summary>
    public void Hazirla(StokCikis stokCikis)
    {
        // Stok cikis kurallari tamamlandiktan sonra DAL katmani cagrilir.
        _stokCikisDal.Hazirla(stokCikis);
    }
}
