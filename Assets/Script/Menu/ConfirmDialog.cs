using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog : MonoBehaviour
{
    public static ConfirmDialog Instance { get; private set; }
    public static bool Tampil { get { return Instance != null && Instance.kotak.activeSelf; } }

    [SerializeField] GameObject kotak;
    [SerializeField] TMP_Text teksJudul;
    [SerializeField] TMP_Text teksPesan;
    [SerializeField] TMP_Text teksYa;
    [SerializeField] TMP_Text teksTidak;
    [SerializeField] Button tombolYa;
    [SerializeField] Button tombolTidak;

    Action aksiYa;
    Action aksiTidak;

    void Awake()
    {
        Instance = this;
        kotak.SetActive(false);
        tombolYa.onClick.AddListener(Ya);
        tombolTidak.onClick.AddListener(Tutup);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // labelTidak = null menyembunyikan tombol kedua (dialog info biasa)
    public void Buka(string judul, string pesan, Action ya,
                 string labelYa = "Ya", string labelTidak = "Batal", Action tidak = null)
    {
        teksJudul.text = judul;
        teksPesan.text = pesan;
        teksYa.text = labelYa;
        teksTidak.text = labelTidak ?? "";
        tombolTidak.gameObject.SetActive(labelTidak != null);

        aksiYa = ya;
        aksiTidak = tidak;
        kotak.SetActive(true);
    }

    void Ya()
    {
        var a = aksiYa;
        Sembunyikan();
        if (a != null) a();
    }

    public void Tutup()
    {
        var a = aksiTidak;
        Sembunyikan();
        if (a != null) a();
    }

    void Sembunyikan()
    {
        aksiYa = null;
        aksiTidak = null;
        kotak.SetActive(false);
    }
}