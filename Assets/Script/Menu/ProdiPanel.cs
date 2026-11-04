using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProdiPanel : MenuPanel
{
    [Header("Daftar")]
    [SerializeField] Transform wadahKartu;
    [SerializeField] KartuProdi kartuPrefab;

    [Header("Detail")]
    [SerializeField] TMP_Text teksNama;
    [SerializeField] TMP_Text teksStatus;
    [SerializeField] TMP_Text teksDeskripsi;
    [SerializeField] TMP_Text teksAbility;
    [SerializeField] Image gambarIkon;

    [Header("Tombol")]
    [SerializeField] Button tombolAksi;
    [SerializeField] TMP_Text teksTombolAksi;
    [SerializeField] Button tombolTampilan;

    [Header("Tujuan")]
    [SerializeField] MenuPanel panelShop;
    [SerializeField] MenuPanel panelTampilan;

    readonly List<KartuProdi> kartu = new List<KartuProdi>();
    ProdiData dilihat;
    bool terbangun;

    void Awake()
    {
        tombolAksi.onClick.AddListener(OnAksi);
        tombolTampilan.onClick.AddListener(OnTampilan);
    }

    void OnEnable()
    {
        ProfileService.OnBerubah += Segarkan;
        Bangun();

        // Awal dibuka: lihat prodi yang sedang terpilih
        var db = GameDatabase.Instance;
        dilihat = db != null ? db.CariProdi(ProfileService.Current.prodiTerpilih) : null;
        if (dilihat == null && kartu.Count > 0) dilihat = kartu[0].Data;

        Segarkan();
    }

    void OnDisable()
    {
        ProfileService.OnBerubah -= Segarkan;
    }

    void Bangun()
    {
        if (terbangun) return;

        var db = GameDatabase.Instance;
        if (db == null) return;

        foreach (var prodi in db.semuaProdi)
        {
            if (prodi == null) continue;
            var k = Instantiate(kartuPrefab, wadahKartu);
            k.Siapkan(prodi, Lihat);
            kartu.Add(k);
        }
        terbangun = true;
    }

    void Lihat(ProdiData prodi)
    {
        dilihat = prodi;
        Segarkan();
    }

    void Segarkan()
    {
        var p = ProfileService.Current;

        foreach (var k in kartu)
        {
            k.Perbarui(ProfileService.ProdiTerbuka(k.Data.id),
                       k.Data.id == p.prodiTerpilih,
                       k.Data == dilihat);
        }

        TampilkanDetail();
    }

    void TampilkanDetail()
    {
        if (dilihat == null) return;

        var p = ProfileService.Current;
        bool terbuka = ProfileService.ProdiTerbuka(dilihat.id);
        bool terpilih = dilihat.id == p.prodiTerpilih;

        teksNama.text = dilihat.namaTampil;
        teksDeskripsi.text = dilihat.deskripsi;
        teksAbility.text = "Ability: " + dilihat.namaAbility + "\n" + dilihat.deskripsiAbility;

        if (gambarIkon != null)
        {
            gambarIkon.sprite = dilihat.ikon;
            gambarIkon.enabled = dilihat.ikon != null;
        }

        if (terpilih)
        {
            teksStatus.text = "Prodi aktif";
            teksTombolAksi.text = "Terpilih";
            tombolAksi.interactable = false;
        }
        else if (terbuka)
        {
            teksStatus.text = "Terbuka";
            teksTombolAksi.text = "Pilih Prodi";
            tombolAksi.interactable = true;
        }
        else
        {
            teksStatus.text = "Terkunci";
            teksTombolAksi.text = "Buka di Shop (" + dilihat.hargaUnlock + ")";
            tombolAksi.interactable = true;
        }

        // Ubah tampilan hanya untuk prodi yang sedang terpilih
        tombolTampilan.gameObject.SetActive(terpilih);
    }

    void OnAksi()
    {
        if (dilihat == null) return;

        if (ProfileService.ProdiTerbuka(dilihat.id))
        {
            ProfileService.PilihProdi(dilihat.id);   // Simpan memicu Segarkan lewat OnBerubah
        }
        else if (MenuNavigator.Instance != null && panelShop != null)
        {
            MenuNavigator.Instance.Buka(panelShop);
        }
    }

    void OnTampilan()
    {
        if (MenuNavigator.Instance != null && panelTampilan != null)
            MenuNavigator.Instance.Buka(panelTampilan);
    }
}