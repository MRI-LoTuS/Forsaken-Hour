using UnityEngine;

public struct HasilRun
{
    public bool menang;
    public int kunci;
    public int dasar;
    public int bonusKunci;
    public int bonusMenang;
    public int total;
    public int saldo;

    public string Ringkasan()
    {
        return "Currency didapat: +" + total + "\n"
             + "Dasar: " + dasar + "\n"
             + "Kunci (" + kunci + "): " + bonusKunci + "\n"
             + (menang ? "Bonus menang: " + bonusMenang + "\n" : "")
             + "Saldo: " + saldo;
    }
}

// Satu-satunya tempat yang menghitung dan memberi hadiah akhir run.
public static class RunSelesai
{
    public const int Dasar = 50;
    public const int PerKunci = 25;
    public const int BonusMenang = 150;
    public const int GedungSaatIni = 1;   // naik saat gedung lain dibuat

    static bool sudah;
    static HasilRun hasil;

    public static void Reset()
    {
        sudah = false;
        hasil = default(HasilRun);
    }

    // Aman dipanggil berkali-kali: hadiah hanya diberikan sekali per run
    public static HasilRun Akhiri(bool menang)
    {
        if (sudah) return hasil;
        sudah = true;

        int kunci = KeyManager.Instance != null ? KeyManager.Instance.Terkumpul : 0;

        hasil = new HasilRun
        {
            menang = menang,
            kunci = kunci,
            dasar = Dasar,
            bonusKunci = kunci * PerKunci,
            bonusMenang = menang ? BonusMenang : 0
        };
        hasil.total = hasil.dasar + hasil.bonusKunci + hasil.bonusMenang;

        var profil = ProfileService.Current;
        profil.gedungTerjauh = Mathf.Max(profil.gedungTerjauh, GedungSaatIni);

        ProfileService.TambahCurrency(hasil.total);   // menyimpan otomatis ke JSON
        hasil.saldo = ProfileService.Current.currency;

        return hasil;
    }
}