using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class ProfileService
{
    // Currency awal hanya untuk demo Shop. Ubah ke 0 saat rilis.
    public const int CurrencyAwal = 500;

    // Pengaman sederhana (NFR-06), bukan enkripsi sungguhan
    const string Garam = "ForsakenHour-ITK";

    static PlayerProfile _current;

    public static event Action OnBerubah;
    public static bool DataDiubahTerdeteksi { get; private set; }

    static string PathUtama { get { return Path.Combine(Application.persistentDataPath, "profile.json"); } }
    static string PathCadangan { get { return Path.Combine(Application.persistentDataPath, "profile.bak"); } }

    public static PlayerProfile Current
    {
        get
        {
            if (_current == null) Muat();
            return _current;
        }
    }

    // ---------- Muat dan simpan ----------

    public static void Muat()
    {
        DataDiubahTerdeteksi = false;
        _current = Baca(PathUtama);

        if (_current == null)
        {
            _current = Baca(PathCadangan);
            if (_current != null) Tulis(false);   // pulihkan file utama dari cadangan
        }

        if (_current == null)
        {
            _current = Baru();
            Tulis(false);
        }

        if (OnBerubah != null) OnBerubah();
    }

    public static void Simpan()
    {
        Tulis(true);
        if (OnBerubah != null) OnBerubah();
    }

    public static void ResetProfil()
    {
        try
        {
            if (File.Exists(PathUtama)) File.Delete(PathUtama);
            if (File.Exists(PathCadangan)) File.Delete(PathCadangan);
        }
        catch (Exception e) { Debug.LogWarning("Reset profil gagal: " + e.Message); }

        _current = null;
        Muat();
    }

    static PlayerProfile Baca(string path)
    {
        if (!File.Exists(path)) return null;

        try
        {
            var p = JsonUtility.FromJson<PlayerProfile>(File.ReadAllText(path));
            if (p == null) return null;

            if (p.tandaTangan != Hitung(p))
            {
                Debug.LogWarning("Profil tidak valid (tanda tangan tidak cocok): " + path);
                DataDiubahTerdeteksi = true;
                return null;
            }
            return p;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Gagal membaca profil: " + e.Message);
            return null;
        }
    }

    static void Tulis(bool buatCadangan)
    {
        try
        {
            if (buatCadangan && File.Exists(PathUtama))
                File.Copy(PathUtama, PathCadangan, true);

            _current.tandaTangan = Hitung(_current);
            File.WriteAllText(PathUtama, JsonUtility.ToJson(_current, true));
        }
        catch (Exception e)
        {
            Debug.LogError("Gagal menyimpan profil: " + e.Message);
        }
    }

    static string Hitung(PlayerProfile p)
    {
        string simpan = p.tandaTangan;
        p.tandaTangan = "";
        string json = JsonUtility.ToJson(p);
        p.tandaTangan = simpan;

        using (var sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json + Garam));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

    static PlayerProfile Baru()
    {
        var p = new PlayerProfile();
        p.currency = CurrencyAwal;

        var db = GameDatabase.Instance;
        if (db != null)
        {
            foreach (var pr in db.semuaProdi)
                if (pr != null && pr.terbukaAwal) p.prodiTerbuka.Add(pr.id);

            foreach (var k in db.semuaKosmetik)
                if (k != null && k.dimilikiAwal) p.kosmetikDimiliki.Add(k.id);

            if (p.prodiTerbuka.Count > 0) p.prodiTerpilih = p.prodiTerbuka[0];
        }
        return p;
    }

    // ---------- Currency ----------

    public static void TambahCurrency(int jumlah)
    {
        if (jumlah <= 0) return;
        Current.currency += jumlah;
        Simpan();
    }

    public static bool KurangiCurrency(int jumlah)
    {
        if (jumlah < 0 || Current.currency < jumlah) return false;
        Current.currency -= jumlah;
        Simpan();
        return true;
    }

    // ---------- Prodi ----------

    public static bool ProdiTerbuka(string id)
    {
        return Current.prodiTerbuka.Contains(id);
    }

    public static bool BukaProdi(ProdiData prodi)
    {
        if (prodi == null || ProdiTerbuka(prodi.id)) return false;
        if (Current.currency < prodi.hargaUnlock) return false;

        Current.currency -= prodi.hargaUnlock;
        Current.prodiTerbuka.Add(prodi.id);
        Simpan();
        return true;
    }

    public static bool PilihProdi(string id)
    {
        if (!ProdiTerbuka(id)) return false;
        Current.prodiTerpilih = id;
        Simpan();
        return true;
    }

    // ---------- Kosmetik ----------

    public static bool MemilikiKosmetik(string id)
    {
        return Current.kosmetikDimiliki.Contains(id);
    }

    public static bool BeliKosmetik(CosmeticData k)
    {
        if (k == null || MemilikiKosmetik(k.id)) return false;
        if (Current.currency < k.harga) return false;

        Current.currency -= k.harga;
        Current.kosmetikDimiliki.Add(k.id);
        Simpan();
        return true;
    }

    public static bool PasangKosmetik(string prodiId, CosmeticData k)
    {
        if (k == null || !MemilikiKosmetik(k.id)) return false;
        if (k.prodiKhusus != null && k.prodiKhusus.id != prodiId) return false;

        var db = GameDatabase.Instance;
        Current.terpasang.RemoveAll(t =>
        {
            if (t.prodiId != prodiId) return false;
            var lama = db.CariKosmetik(t.kosmetikId);
            return lama != null && lama.tipe == k.tipe;
        });

        Current.terpasang.Add(new KosmetikTerpasang { prodiId = prodiId, kosmetikId = k.id });
        Simpan();
        return true;
    }

    public static void LepasKosmetik(string prodiId, TipeKosmetik tipe)
    {
        var db = GameDatabase.Instance;
        int jumlah = Current.terpasang.RemoveAll(t =>
        {
            if (t.prodiId != prodiId) return false;
            var k = db.CariKosmetik(t.kosmetikId);
            return k != null && k.tipe == tipe;
        });

        if (jumlah > 0) Simpan();
    }

    public static CosmeticData KosmetikTerpasangUntuk(string prodiId, TipeKosmetik tipe)
    {
        var db = GameDatabase.Instance;
        foreach (var t in Current.terpasang)
        {
            if (t.prodiId != prodiId) continue;
            var k = db.CariKosmetik(t.kosmetikId);
            if (k != null && k.tipe == tipe) return k;
        }
        return null;
    }
}