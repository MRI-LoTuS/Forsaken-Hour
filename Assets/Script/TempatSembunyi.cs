using UnityEngine;

public class TempatSembunyi : MonoBehaviour
{
    public string namaTempat = "Lemari";
    [SerializeField] Transform titikSembunyi;   // tempat Player berada saat bersembunyi
    [SerializeField] Transform titikKeluar;     // tempat Player berdiri saat keluar

    public bool Terisi { get; private set; }

    public Transform TitikSembunyi { get { return titikSembunyi != null ? titikSembunyi : transform; } }
    public Transform TitikKeluar { get { return titikKeluar != null ? titikKeluar : transform; } }

    public void SaatMasuk() { Terisi = true; }
    public void SaatKeluar() { Terisi = false; }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(TitikSembunyi.position, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(TitikKeluar.position, 0.5f);
    }
}