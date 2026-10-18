using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    public int totalKunci = 4;
    public GameObject prefabKunci;
    public Transform indukTitik;     // induk dari semua titik kandidat kunci
    public TMP_Text teksKunci;

    int terkumpul;

    public int Terkumpul { get { return terkumpul; } }
    public bool Lengkap { get { return terkumpul >= totalKunci; } }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        terkumpul = 0;
        SebarKunci();
        UpdateUI();
    }

    void SebarKunci()
    {
        if (prefabKunci == null || indukTitik == null)
        {
            Debug.LogWarning("KeyManager: Prefab Kunci atau Induk Titik belum diisi");
            return;
        }

        var titik = new List<Transform>();
        foreach (Transform t in indukTitik) titik.Add(t);

        // acak urutan titik (Fisher-Yates)
        for (int i = titik.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Transform sementara = titik[i];
            titik[i] = titik[j];
            titik[j] = sementara;
        }

        int jumlah = Mathf.Min(totalKunci, titik.Count);
        if (jumlah < totalKunci)
            Debug.LogWarning("KeyManager: titik kandidat kurang dari jumlah kunci");
        totalKunci = jumlah;

        for (int i = 0; i < jumlah; i++)
            Instantiate(prefabKunci, titik[i].position, titik[i].rotation);
    }

    public void AmbilKunci()
    {
        terkumpul++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (teksKunci != null)
            teksKunci.text = "Kunci: " + terkumpul + "/" + totalKunci;
    }
}