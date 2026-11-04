using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotKosmetik : MonoBehaviour
{
    [SerializeField] TipeKosmetik tipe;
    [SerializeField] TMP_Text teksTipe;
    [SerializeField] TMP_Text teksNama;
    [SerializeField] Button tombolKiri;
    [SerializeField] Button tombolKanan;

    readonly List<CosmeticData> opsi = new List<CosmeticData>();
    string prodiId;
    int indeks;
    bool siap;

    public void Muat(string idProdi)
    {
        if (!siap)
        {
            tombolKiri.onClick.AddListener(() => Geser(-1));
            tombolKanan.onClick.AddListener(() => Geser(1));
            siap = true;
        }

        prodiId = idProdi;

        opsi.Clear();
        opsi.Add(null);   // pilihan "Tidak ada"

        var db = GameDatabase.Instance;
        if (db != null)
        {
            foreach (var k in db.semuaKosmetik)
            {
                if (k == null || k.tipe != tipe) continue;
                if (!ProfileService.MemilikiKosmetik(k.id)) continue;
                if (k.prodiKhusus != null && k.prodiKhusus.id != prodiId) continue;
                opsi.Add(k);
            }
        }

        var terpasang = ProfileService.KosmetikTerpasangUntuk(prodiId, tipe);
        indeks = terpasang != null ? Mathf.Max(0, opsi.IndexOf(terpasang)) : 0;

        Tampil();
    }

    void Geser(int arah)
    {
        if (opsi.Count <= 1) return;

        indeks = (indeks + arah + opsi.Count) % opsi.Count;
        var k = opsi[indeks];

        if (k == null) ProfileService.LepasKosmetik(prodiId, tipe);
        else ProfileService.PasangKosmetik(prodiId, k);

        Tampil();
    }

    void Tampil()
    {
        teksTipe.text = NamaTipe(tipe);
        teksNama.text = opsi[indeks] == null ? "Tidak ada" : opsi[indeks].namaTampil;

        bool bisaGeser = opsi.Count > 1;
        tombolKiri.interactable = bisaGeser;
        tombolKanan.interactable = bisaGeser;
    }

    static string NamaTipe(TipeKosmetik t)
    {
        switch (t)
        {
            case TipeKosmetik.Aksesori: return "Aksesori";
            case TipeKosmetik.SkinTangan: return "Skin Tangan";
            default: return "Tampilan Senter";
        }
    }
}