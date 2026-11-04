using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KartuProdi : MonoBehaviour
{
    [SerializeField] Button tombol;
    [SerializeField] Image latar;
    [SerializeField] Image ikon;
    [SerializeField] TMP_Text teksNama;
    [SerializeField] TMP_Text teksStatus;

    [Header("Warna")]
    [SerializeField] Color warnaNormal = new Color32(0x2A, 0x2A, 0x30, 255);
    [SerializeField] Color warnaDilihat = new Color32(0x8B, 0x1E, 0x1E, 255);
    [SerializeField] Color warnaTerkunci = new Color32(0x1A, 0x1A, 0x1E, 255);

    ProdiData data;
    Action<ProdiData> saatKlik;

    public ProdiData Data { get { return data; } }

    public void Siapkan(ProdiData prodi, Action<ProdiData> aksiKlik)
    {
        data = prodi;
        saatKlik = aksiKlik;

        teksNama.text = prodi.namaTampil;

        if (ikon != null)
        {
            ikon.sprite = prodi.ikon;
            ikon.enabled = prodi.ikon != null;
        }

        tombol.onClick.RemoveAllListeners();
        tombol.onClick.AddListener(() => { if (saatKlik != null) saatKlik(data); });
    }

    public void Perbarui(bool terbuka, bool terpilih, bool dilihat)
    {
        teksStatus.text = terpilih ? "Terpilih" : (terbuka ? "Terbuka" : "Terkunci");
        latar.color = dilihat ? warnaDilihat : (terbuka ? warnaNormal : warnaTerkunci);
    }
}