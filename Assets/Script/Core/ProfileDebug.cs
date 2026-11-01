using UnityEngine;

public class ProfileDebug : MonoBehaviour
{
    void Start()
    {
        var p = ProfileService.Current;
        Debug.Log("Folder profil: " + Application.persistentDataPath);
        Debug.Log("Currency: " + p.currency
            + " | Prodi terbuka: " + string.Join(", ", p.prodiTerbuka)
            + " | Terpilih: " + p.prodiTerpilih);

        if (ProfileService.DataDiubahTerdeteksi)
            Debug.LogWarning("Terdeteksi file profil diubah manual, dipulihkan dari cadangan");
    }

    [ContextMenu("Tambah 100 currency")]
    void Tambah()
    {
        ProfileService.TambahCurrency(100);
        Debug.Log("Currency sekarang: " + ProfileService.Current.currency);
    }

    [ContextMenu("Reset profil")]
    void Reset()
    {
        ProfileService.ResetProfil();
        Debug.Log("Profil direset");
    }
}