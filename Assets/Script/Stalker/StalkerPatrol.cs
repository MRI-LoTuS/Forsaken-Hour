using UnityEngine;
using UnityEngine.AI;

// Sprint 2: patrol dasar + deteksi kasar.
// Sprint 3 mengganti ini dengan state machine lengkap (Patrol, Investigate, Hunt, Catch).
[RequireComponent(typeof(NavMeshAgent))]
public class StalkerPatrol : MonoBehaviour
{
    enum Mode { Patroli, Kejar }

    [Header("Patroli")]
    [SerializeField] Transform[] titikPatroli;
    [SerializeField] float kecepatanPatroli = 3.5f;
    [SerializeField] float jedaDiTitik = 2f;
    [SerializeField] float jarakSampai = 2f;

    [Header("Deteksi kasar")]
    [SerializeField] Transform pemain;
    [SerializeField] Transform titikMata;        // opsional, anak kosong di kepala Stalker
    [SerializeField] float tinggiMata = 4.5f;    // dipakai kalau titikMata kosong
    [SerializeField] float jarakLihat = 25f;
    [SerializeField] float sudutLihat = 110f;
    [SerializeField] LayerMask penghalang = ~0;

    [Header("Kejar kasar")]
    [SerializeField] float kecepatanKejar = 6f;
    [SerializeField] float waktuLupa = 3f;
    [SerializeField] float jarakTangkap = 4.5f;
    [SerializeField] float toleransiTinggi = 6f;

    NavMeshAgent agent;
    Collider kolPemain;
    Mode mode;
    int indeks;
    float jedaSisa;
    float terakhirLihat;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (pemain != null) kolPemain = pemain.GetComponent<Collider>();
    }

    void Start()
    {
        MulaiPatroli();
    }

    void Update()
    {
        if (TangkapManager.selesai)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        bool lihat = BisaMelihatPemain();

        if (mode == Mode.Patroli)
        {
            if (lihat) { MulaiKejar(); return; }
            Patroli();
        }
        else
        {
            if (lihat) terakhirLihat = Time.time;

            if (Time.time - terakhirLihat > waktuLupa)
            {
                MulaiPatroli();
                return;
            }

            agent.SetDestination(pemain.position);

            if (SudahDekat())
            {
                if (TangkapManager.Instance != null)
                    TangkapManager.Instance.Tangkap();
                else
                    Debug.LogWarning("TangkapManager tidak ada di scene (cek GameManager)");
            }
        }
    }

    // ---------- Patroli ----------

    void MulaiPatroli()
    {
        mode = Mode.Patroli;
        agent.isStopped = false;
        agent.speed = kecepatanPatroli;
        jedaSisa = jedaDiTitik;

        if (titikPatroli != null && titikPatroli.Length > 0)
            agent.SetDestination(titikPatroli[indeks].position);
    }

    void Patroli()
    {
        if (titikPatroli == null || titikPatroli.Length == 0) return;
        if (agent.pathPending) return;

        if (agent.remainingDistance <= jarakSampai)
        {
            jedaSisa -= Time.deltaTime;
            if (jedaSisa <= 0f)
            {
                indeks = (indeks + 1) % titikPatroli.Length;
                agent.SetDestination(titikPatroli[indeks].position);
                jedaSisa = jedaDiTitik;
            }
        }
    }

    // ---------- Kejar kasar ----------

    void MulaiKejar()
    {
        Debug.Log("Stalker mulai mengejar");
        mode = Mode.Kejar;
        terakhirLihat = Time.time;
        agent.isStopped = false;
        agent.speed = kecepatanKejar;
    }

    // ---------- Deteksi ----------

    Vector3 PosisiMata()
    {
        return titikMata != null ? titikMata.position : transform.position + Vector3.up * tinggiMata;
    }

    bool BisaMelihatPemain()
    {
        if (pemain == null || PemainSembunyi.Bersembunyi) return false;

        Vector3 mata = PosisiMata();
        Vector3 target = kolPemain != null ? kolPemain.bounds.center : pemain.position + Vector3.up;
        Vector3 arah = target - mata;
        float jarak = arah.magnitude;

        if (jarak > jarakLihat) return false;
        if (Vector3.Angle(transform.forward, arah) > sudutLihat * 0.5f) return false;

        var hits = Physics.RaycastAll(mata, arah / jarak, jarak, penghalang,
                              QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var h in hits)
        {
            // abaikan tubuh Stalker sendiri
            if (h.transform == transform || h.transform.IsChildOf(transform)) continue;

            return h.transform == pemain || h.transform.IsChildOf(pemain);
        }
        return true;
    }

    float JarakDatar(Vector3 titik)
    {
        Vector3 d = titik - transform.position;
        d.y = 0f;
        return d.magnitude;
    }

    bool SudahDekat()
    {
        if (PemainSembunyi.Bersembunyi) return false;

        float selisihY = Mathf.Abs(pemain.position.y - transform.position.y);
        return selisihY <= toleransiTinggi && JarakDatar(pemain.position) <= jarakTangkap;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(PosisiMata(), jarakLihat);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jarakTangkap);
    }
}