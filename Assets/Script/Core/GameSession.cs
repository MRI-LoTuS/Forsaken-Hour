public static class GameSession
{
    public static ProdiData ProdiAktif { get; private set; }

    // Dipanggil saat tombol Mulai ditekan
    public static void MulaiRun()
    {
        var db = GameDatabase.Instance;
        var profil = ProfileService.Current;

        ProdiAktif = db.CariProdi(profil.prodiTerpilih);

        // cadangan: kalau pilihan tidak valid, pakai prodi terbuka pertama
        if (ProdiAktif == null || !ProfileService.ProdiTerbuka(ProdiAktif.id))
        {
            string idPertama = profil.prodiTerbuka.Count > 0 ? profil.prodiTerbuka[0] : "";
            ProdiAktif = db.CariProdi(idPertama);
        }
    }

    public static CosmeticData Kosmetik(TipeKosmetik tipe)
    {
        if (ProdiAktif == null) return null;
        return ProfileService.KosmetikTerpasangUntuk(ProdiAktif.id, tipe);
    }
}