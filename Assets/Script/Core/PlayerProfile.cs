using System;
using System.Collections.Generic;

[Serializable]
public class KosmetikTerpasang
{
    public string prodiId;
    public string kosmetikId;
}

[Serializable]
public class PlayerProfile
{
    public int versi = 1;

    // progres
    public int currency;
    public int gedungTerjauh;
    public List<string> prodiTerbuka = new List<string>();
    public List<string> kosmetikDimiliki = new List<string>();
    public string prodiTerpilih = "";
    public List<KosmetikTerpasang> terpasang = new List<KosmetikTerpasang>();

    // pengaturan
    public float volumeMaster = 1f;
    public float volumeMusik = 0.7f;
    public float volumeSfx = 1f;
    public float sensitivitas = 1f;
    public int resolusiIndex = -1;
    public bool layarPenuh = true;
    public bool sudahLihatPeringatan = false;

    public string tandaTangan = "";
}