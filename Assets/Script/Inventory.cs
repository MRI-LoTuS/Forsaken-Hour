using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using TMPro;
using System.Text;
using NUnit.Framework;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int kapasitas = 5;
    public TMP_Text teksInventory;
    public TMP_Text teksPesan;

    readonly List<string> isi = new List<string>();
    float timerPesan;

    void Start()
    {
        Refresh();
        if (teksPesan != null) 
        {
            teksPesan.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timerPesan > 0f) 
        {
            timerPesan -= Time.unscaledDeltaTime;
            if (timerPesan <= 0f && teksPesan != null) 
            {
                teksPesan.text = "";
            }
        }
    }
    public bool Tambah(string nama) 
    {
        if (isi.Count >= kapasitas) 
        {
            Pesan("Inventory Penuh");
            return false;
        }
        isi.Add(nama);
        Refresh();
        Pesan("Mengambil " + nama);
        return true;
    }

    public int Jumlah(string nama)    
    {
        int n = 0;
        foreach (var s in isi) if (s == nama) n++;
        return n;
    }

    public bool Pakai(string nama)   
    {
        bool berhasil = isi.Remove(nama);
        if (berhasil) Refresh();
        return berhasil;
    }

    void Pesan(string teks) 
    {
        if (teksPesan == null) return;
        teksPesan.text = teks;
        timerPesan = 2f;
    }

    void Refresh()
    {
        if (teksInventory == null) return;

        var hitung = new Dictionary<string, int>();
        foreach (var s in isi)
        {
            if (hitung.TryGetValue(s, out int n)) hitung[s] = n + 1;
            else hitung[s] = 1;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Inventory (" + isi.Count + "/" + kapasitas + ")");
        foreach (var kv in hitung)
            sb.AppendLine(kv.Key + " x" + kv.Value);

        teksInventory.text = sb.ToString();
    }

}
