using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class SettingsService
{
    const string NamaMixer = "MainMixer";
    static AudioMixer mixer;

    // Otomatis jalan sekali saat game mulai, di scene mana pun
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void SaatGameMulai()
    {
        TerapkanSemua();
    }

    public static void TerapkanSemua()
    {
        var p = ProfileService.Current;
        TerapkanAudio(p);
        TerapkanLayar(p);
    }

    static AudioMixer Mixer()
    {
        if (mixer == null)
        {
            mixer = Resources.Load<AudioMixer>(NamaMixer);
            if (mixer == null)
                Debug.LogWarning("AudioMixer 'MainMixer' tidak ditemukan di Assets/Resources");
        }
        return mixer;
    }

    // Slider 0..1 ke desibel (0 = -80 dB, 1 = 0 dB)
    static float KeDesibel(float v)
    {
        return Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
    }

    public static void TerapkanAudio(PlayerProfile p)
    {
        var m = Mixer();
        if (m == null) return;

        m.SetFloat("VolMaster", KeDesibel(p.volumeMaster));
        m.SetFloat("VolMusik", KeDesibel(p.volumeMusik));
        m.SetFloat("VolSfx", KeDesibel(p.volumeSfx));
    }

    // Satu entri per ukuran layar (refresh rate tertinggi), urut naik
    public static List<Resolution> DaftarResolusi()
    {
        var hasil = new List<Resolution>();
        foreach (var r in Screen.resolutions)
        {
            if (hasil.Count > 0 &&
                hasil[hasil.Count - 1].width == r.width &&
                hasil[hasil.Count - 1].height == r.height)
            {
                hasil[hasil.Count - 1] = r;
            }
            else
            {
                hasil.Add(r);
            }
        }
        return hasil;
    }

    public static void TerapkanLayar(PlayerProfile p)
    {
        var mode = p.layarPenuh ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        var daftar = DaftarResolusi();

        if (p.resolusiIndex >= 0 && p.resolusiIndex < daftar.Count)
        {
            var r = daftar[p.resolusiIndex];
            Screen.SetResolution(r.width, r.height, mode);
        }
        else
        {
            Screen.fullScreenMode = mode;
        }
    }

    public static void KembalikanBawaan(PlayerProfile p)
    {
        p.volumeMaster = 1f;
        p.volumeMusik = 0.7f;
        p.volumeSfx = 1f;
        p.sensitivitas = 1f;
        p.resolusiIndex = -1;
        p.layarPenuh = true;
    }
}