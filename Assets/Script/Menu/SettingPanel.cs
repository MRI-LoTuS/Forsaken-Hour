using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MenuPanel
{
    [Header("Audio")]
    [SerializeField] Slider sliderMaster;
    [SerializeField] Slider sliderMusik;
    [SerializeField] Slider sliderSfx;
    [SerializeField] TMP_Text nilaiMaster;
    [SerializeField] TMP_Text nilaiMusik;
    [SerializeField] TMP_Text nilaiSfx;

    [Header("Kontrol")]
    [SerializeField] Slider sliderSensitivitas;
    [SerializeField] TMP_Text nilaiSensitivitas;

    [Header("Layar")]
    [SerializeField] TMP_Dropdown dropdownResolusi;
    [SerializeField] Toggle toggleLayarPenuh;

    List<Resolution> daftar = new List<Resolution>();
    bool kotor;

    void Awake()
    {
        sliderMaster.minValue = 0f; sliderMaster.maxValue = 1f;
        sliderMusik.minValue = 0f; sliderMusik.maxValue = 1f;
        sliderSfx.minValue = 0f; sliderSfx.maxValue = 1f;
        sliderSensitivitas.minValue = 0.2f;
        sliderSensitivitas.maxValue = 3f;

        sliderMaster.onValueChanged.AddListener(_ => OnUbahAudio());
        sliderMusik.onValueChanged.AddListener(_ => OnUbahAudio());
        sliderSfx.onValueChanged.AddListener(_ => OnUbahAudio());
        sliderSensitivitas.onValueChanged.AddListener(_ => OnUbahSensitivitas());
        dropdownResolusi.onValueChanged.AddListener(_ => OnUbahLayar());
        toggleLayarPenuh.onValueChanged.AddListener(_ => OnUbahLayar());
    }

    void OnEnable()
    {
        Muat();
    }

    void OnDisable()
    {
        SimpanKalauBerubah();
    }

    // ---------- Muat nilai dari profil ke UI (tanpa memicu event) ----------

    void Muat()
    {
        var p = ProfileService.Current;

        sliderMaster.SetValueWithoutNotify(p.volumeMaster);
        sliderMusik.SetValueWithoutNotify(p.volumeMusik);
        sliderSfx.SetValueWithoutNotify(p.volumeSfx);
        sliderSensitivitas.SetValueWithoutNotify(p.sensitivitas);
        toggleLayarPenuh.SetIsOnWithoutNotify(p.layarPenuh);

        daftar = SettingsService.DaftarResolusi();
        dropdownResolusi.ClearOptions();

        var opsi = new List<string>();
        foreach (var r in daftar) opsi.Add(r.width + " x " + r.height);
        dropdownResolusi.AddOptions(opsi);
        dropdownResolusi.SetValueWithoutNotify(IndexAktif(p));
        dropdownResolusi.RefreshShownValue();

        SegarkanLabel();
    }

    int IndexAktif(PlayerProfile p)
    {
        if (daftar.Count == 0) return 0;
        if (p.resolusiIndex >= 0 && p.resolusiIndex < daftar.Count) return p.resolusiIndex;

        for (int i = 0; i < daftar.Count; i++)
            if (daftar[i].width == Screen.width && daftar[i].height == Screen.height)
                return i;

        return daftar.Count - 1;
    }

    void SegarkanLabel()
    {
        nilaiMaster.text = Mathf.RoundToInt(sliderMaster.value * 100f) + "%";
        nilaiMusik.text = Mathf.RoundToInt(sliderMusik.value * 100f) + "%";
        nilaiSfx.text = Mathf.RoundToInt(sliderSfx.value * 100f) + "%";
        nilaiSensitivitas.text = sliderSensitivitas.value.ToString("0.0") + "x";
    }

    // ---------- Perubahan dari pemain ----------

    void OnUbahAudio()
    {
        var p = ProfileService.Current;
        p.volumeMaster = sliderMaster.value;
        p.volumeMusik = sliderMusik.value;
        p.volumeSfx = sliderSfx.value;

        SettingsService.TerapkanAudio(p);
        SegarkanLabel();
        kotor = true;
    }

    void OnUbahSensitivitas()
    {
        ProfileService.Current.sensitivitas = sliderSensitivitas.value;
        SegarkanLabel();
        kotor = true;
    }

    void OnUbahLayar()
    {
        var p = ProfileService.Current;
        p.resolusiIndex = dropdownResolusi.value;
        p.layarPenuh = toggleLayarPenuh.isOn;

        SettingsService.TerapkanLayar(p);
        kotor = true;
    }

    void SimpanKalauBerubah()
    {
        if (!kotor) return;
        kotor = false;
        ProfileService.Simpan();
    }

    // ---------- Tombol reset ----------

    // OnClick tombol Reset Pengaturan
    public void ResetPengaturan()
    {
        if (ConfirmDialog.Instance == null) return;

        ConfirmDialog.Instance.Buka("Reset pengaturan?",
            "Volume, sensitivitas, resolusi, dan mode layar kembali ke bawaan.",
            () =>
            {
                SettingsService.KembalikanBawaan(ProfileService.Current);
                kotor = false;
                ProfileService.Simpan();
                SettingsService.TerapkanSemua();
                Muat();
            }, "Reset", "Batal");
    }

    // OnClick tombol Reset Profil
    public void ResetProfil()
    {
        if (ConfirmDialog.Instance == null) return;

        ConfirmDialog.Instance.Buka("Reset seluruh profil?",
            "Currency, prodi, dan kosmetik akan hilang. Tindakan ini tidak bisa dibatalkan.",
            () =>
            {
                kotor = false;
                ProfileService.ResetProfil();
                SettingsService.TerapkanSemua();
                Muat();

                if (MenuNavigator.Instance != null) MenuNavigator.Instance.KeUtama();
            }, "Reset", "Batal");
    }
}