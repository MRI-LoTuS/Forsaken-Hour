using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PemancarSuara : MonoBehaviour
{
    [Header("Ambang kecepatan (satuan dunia per detik)")]
    [SerializeField] float ambangJalan = 1f;
    [SerializeField] float ambangLari = 5f;

    [Header("Radius suara (0 = tidak berbunyi)")]
    [SerializeField] float radiusJalan = 0f;
    [SerializeField] float radiusLari = 20f;
    [SerializeField] float interval = 0.5f;

    [Header("Bantuan menyetel ambang")]
    [SerializeField] bool tampilkanKecepatan;

    CharacterController cc;
    float timer;
    float kecepatan;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (PemainSembunyi.Bersembunyi || TangkapManager.selesai) { kecepatan = 0f; return; }

        Vector3 v = cc.velocity;
        v.y = 0f;
        kecepatan = v.magnitude;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        if (kecepatan >= ambangLari && radiusLari > 0f)
        {
            Kebisingan.Buat(transform.position, radiusLari);
            timer = interval;
        }
        else if (kecepatan >= ambangJalan && radiusJalan > 0f)
        {
            Kebisingan.Buat(transform.position, radiusJalan);
            timer = interval;
        }
    }

    void OnGUI()
    {
        if (tampilkanKecepatan)
            GUI.Label(new Rect(10, 10, 300, 30), "Kecepatan: " + kecepatan.ToString("0.0"));
    }
}