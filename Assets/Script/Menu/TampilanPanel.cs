using TMPro;
using UnityEngine;

public class TampilanPanel : MenuPanel
{
    [SerializeField] TMP_Text teksProdi;
    [SerializeField] SlotKosmetik[] slot;

    void OnEnable()
    {
        var db = GameDatabase.Instance;
        var prodi = db != null ? db.CariProdi(ProfileService.Current.prodiTerpilih) : null;

        teksProdi.text = prodi != null
            ? "Tampilan untuk: " + prodi.namaTampil
            : "Pilih prodi dulu";

        foreach (var s in slot)
        {
            s.gameObject.SetActive(prodi != null);
            if (prodi != null) s.Muat(prodi.id);
        }
    }
}