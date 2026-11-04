using UnityEngine;

// Mengunci ability sesuai prodi yang dipilih (FR-11).
// Sementara hanya ada satu ability: Night Vision milik DKV.
public class AbilityGate : MonoBehaviour
{
    [SerializeField] NightVision nightVision;
    [SerializeField] string idProdiNightVision = "dkv";

    [Tooltip("Objek HUD yang disembunyikan bila ability tidak tersedia")]
    [SerializeField] GameObject[] hudAbility;

    void Start()
    {
        // Uji langsung dari Editor tanpa lewat menu
        if (GameSession.ProdiAktif == null) GameSession.MulaiRun();

        var prodi = GameSession.ProdiAktif;
        bool izinkan = prodi != null && prodi.id == idProdiNightVision;

        if (nightVision != null) nightVision.enabled = izinkan;

        foreach (var o in hudAbility)
            if (o != null) o.SetActive(izinkan);

        Debug.Log("Prodi aktif: " + (prodi != null ? prodi.namaTampil : "-")
                  + " | Night Vision: " + (izinkan ? "aktif" : "terkunci"));
    }
}