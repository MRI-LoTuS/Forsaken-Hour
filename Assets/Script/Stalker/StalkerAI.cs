using UnityEngine;
using UnityEngine.AI;

// Sprint 3: state machine Patrol, Investigate, Hunt, Lost Track, Catch.
// Parameter bawaan mengikuti Level 1 (The Searcher): lambat dan radius kecil.
[RequireComponent(typeof(NavMeshAgent))]
public class StalkerAI : MonoBehaviour
{
    public enum State { Patrol, Investigate, Hunt, LostTrack, Catch }

    [Header("Patrol")]
    [SerializeField] Transform[] titikPatroli;
    [SerializeField] float kecepatanPatroli = 3f;
    [SerializeField] float jedaDiTitik = 2f;
    [SerializeField] float jarakSampai = 2f;

    [Header("Penglihatan")]
    [SerializeField] Transform pemain;
    [SerializeField] float tinggiMata = 2f;
    [SerializeField] float jarakLihat = 20f;
    [SerializeField] float sudutLihat = 110f;
    [SerializeField] LayerMask penghalang = ~0;

    [Header("Pendengaran")]
    [SerializeField] float pengaliPendengaran = 1f;

    [Header("Investigate")]
    [SerializeField] float kecepatanSelidik = 4f;
    [SerializeField] float waktuSelidik = 4f;
    [SerializeField] float kecepatanPutar = 60f;
    [SerializeField] float waktuMaksJalan = 12f;

    [Header("Hunt")]
    [SerializeField] float kecepatanKejar = 5.5f;
    [SerializeField] float waktuLupa = 2f;
    [SerializeField] float jarakTangkap = 4.5f;
    [SerializeField] float toleransiTinggi = 6f;

    [Header("Lost Track")]
    [SerializeField] float kecepatanLostTrack = 2.5f;
    [SerializeField] float waktuLostTrack = 6f;

    [Header("Debug")]
    [SerializeField] Renderer visual;
    [SerializeField] bool catatState = true;

    NavMeshAgent agent;
    Collider kolPemain;

    State state;
    int indeks;
    float jedaSisa;
    float mulaiState;
    float timerSelidik;
    bool sudahSampai;
    float terakhirLihat;
    Vector3 titikTujuan;
    Vector3 posisiTerakhir;

    public State StateSekarang { get { return state; } }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (pemain != null) kolPemain = pemain.GetComponent<Collider>();
    }

    void OnEnable()
    {
        Kebisingan.OnSuara += DengarSuara;
    }

    void OnDisable()
    {
        Kebisingan.OnSuara -= DengarSuara;
    }

    void Start()
    {
        Ganti(State.Patrol);
    }

    // ---------- Loop utama ----------

    void Update()
    {
        if (TangkapManager.selesai || ForsakenClock.selesai)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            return;
        }
        if (state == State.Catch) return;

        bool lihat = BisaMelihatPemain();
        if (lihat) posisiTerakhir = pemain.position;

        switch (state)
        {
            case State.Patrol:
                if (lihat) { Ganti(State.Hunt); break; }
                UpdatePatrol();
                break;

            case State.Investigate:
                if (lihat) { Ganti(State.Hunt); break; }
                UpdateSelidik(waktuSelidik);
                break;

            case State.Hunt:
                UpdateHunt(lihat);
                break;

            case State.LostTrack:
                if (lihat) { Ganti(State.Hunt); break; }
                UpdateSelidik(waktuLostTrack);
                break;
        }
    }

    // ---------- Perpindahan state ----------

    void Ganti(State baru)
    {
        if (catatState && baru != state)
            Debug.Log("Stalker: " + state + " -> " + baru);

        state = baru;
        mulaiState = Time.time;
        sudahSampai = false;
        timerSelidik = 0f;
        agent.isStopped = false;

        switch (baru)
        {
            case State.Patrol:
                agent.speed = kecepatanPatroli;
                jedaSisa = jedaDiTitik;
                indeks = IndeksTerdekat();
                if (titikPatroli != null && titikPatroli.Length > 0)
                    agent.SetDestination(titikPatroli[indeks].position);
                break;

            case State.Investigate:
                agent.speed = kecepatanSelidik;
                agent.SetDestination(titikTujuan);
                break;

            case State.Hunt:
                agent.speed = kecepatanKejar;
                terakhirLihat = Time.time;
                break;

            case State.LostTrack:
                agent.speed = kecepatanLostTrack;
                agent.SetDestination(posisiTerakhir);
                break;

            case State.Catch:
                agent.isStopped = true;
                if (TangkapManager.Instance != null)
                    TangkapManager.Instance.Tangkap();
                else
                    Debug.LogWarning("TangkapManager tidak ada di scene (cek GameManager)");
                break;
        }

        WarnaiVisual(baru);
    }

    // ---------- Patrol ----------

    void UpdatePatrol()
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

    int IndeksTerdekat()
    {
        if (titikPatroli == null || titikPatroli.Length == 0) return 0;

        int terdekat = 0;
        float jarakMin = float.MaxValue;

        for (int i = 0; i < titikPatroli.Length; i++)
        {
            if (titikPatroli[i] == null) continue;
            float j = (titikPatroli[i].position - transform.position).sqrMagnitude;
            if (j < jarakMin) { jarakMin = j; terdekat = i; }
        }
        return terdekat;
    }

    // ---------- Investigate dan Lost Track ----------

    void UpdateSelidik(float waktuTunggu)
    {
        if (!sudahSampai)
        {
            if (Time.time - mulaiState > waktuMaksJalan) { Ganti(State.Patrol); return; }
            if (agent.pathPending) return;

            if (agent.remainingDistance <= jarakSampai)
            {
                sudahSampai = true;
                timerSelidik = 0f;
                agent.isStopped = true;
            }
            return;
        }

        // Sampai di titik: berdiri sambil menoleh ke sekitar
        timerSelidik += Time.deltaTime;
        transform.Rotate(0f, kecepatanPutar * Time.deltaTime, 0f);

        if (timerSelidik >= waktuTunggu) Ganti(State.Patrol);
    }

    // ---------- Hunt ----------

    void UpdateHunt(bool lihat)
    {
        if (lihat) terakhirLihat = Time.time;
        else if (Time.time - terakhirLihat > waktuLupa)
        {
            Ganti(State.LostTrack);
            return;
        }

        agent.SetDestination(lihat ? pemain.position : posisiTerakhir);

        if (SudahDekat()) Ganti(State.Catch);
    }

    bool SudahDekat()
    {
        if (PemainSembunyi.Bersembunyi) return false;

        float selisihY = Mathf.Abs(pemain.position.y - transform.position.y);
        return selisihY <= toleransiTinggi && JarakDatar(pemain.position) <= jarakTangkap;
    }

    // ---------- Pendengaran ----------

    void DengarSuara(Vector3 posisi, float radius)
    {
        if (state == State.Hunt || state == State.Catch) return;
        if (TangkapManager.selesai) return;

        if (Mathf.Abs(posisi.y - transform.position.y) > toleransiTinggi) return;
        if (Vector3.Distance(transform.position, posisi) > radius * pengaliPendengaran) return;

        if (!NavMesh.SamplePosition(posisi, out NavMeshHit hit, 5f, NavMesh.AllAreas)) return;

        titikTujuan = hit.position;
        Ganti(State.Investigate);
    }

    // ---------- Penglihatan ----------

    bool BisaMelihatPemain()
    {
        if (pemain == null || PemainSembunyi.Bersembunyi) return false;

        Vector3 mata = transform.position + Vector3.up * tinggiMata;
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

    // ---------- Visual dan gizmo ----------

    void WarnaiVisual(State s)
    {
        if (visual == null) return;

        Color c;
        switch (s)
        {
            case State.Investigate: c = new Color(1f, 0.9f, 0.2f); break;   // kuning
            case State.Hunt: c = new Color(0.9f, 0.1f, 0.1f); break; // merah
            case State.LostTrack: c = new Color(1f, 0.5f, 0.1f); break;   // oranye
            case State.Catch: c = new Color(0.4f, 0f, 0f); break;     // merah tua
            default: c = new Color(0.7f, 0.7f, 0.75f); break; // abu
        }
        visual.material.color = c;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * tinggiMata, jarakLihat);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jarakTangkap);
    }
}