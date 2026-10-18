using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int kapasitas = 5;
    public TMP_Text teksInventory;
    public TMP_Text teksPesan;

    [Header("Panel Inventory")]
    public GameObject panelInventory;
    public Transform isiList;
    public Button tombolItemPrefab;
    public NightVision nightVision;
    public MonoBehaviour[] scriptDimatikan;
    [SerializeField] private InputActionReference inventoryAction;

    public static bool terbuka { get; private set; }

    readonly List<string> isi = new List<string>();
    float timerPesan;

    void Start()
    {
        terbuka = false;
        if (panelInventory != null) panelInventory.SetActive(false);
        Refresh();
        if (teksPesan != null) teksPesan.text = "";
    }

    void OnEnable()
    {
        if (inventoryAction != null && inventoryAction.action != null)
        {
            inventoryAction.action.Enable();
            inventoryAction.action.performed += OnInventory;
        }
    }

    void OnDisable()
    {
        if (inventoryAction != null && inventoryAction.action != null)
            inventoryAction.action.performed -= OnInventory;
    }

    void Update()
    {
        if (timerPesan > 0f)
        {
            timerPesan -= Time.unscaledDeltaTime;
            if (timerPesan <= 0f && teksPesan != null) teksPesan.text = "";
        }

        // Kalau game dijeda saat inventory terbuka, tutup panel
        // (PauseMenu yang mengatur kursor dan skrip pemain)
        if (terbuka && PauseMenu.isPaused)
        {
            terbuka = false;
            if (panelInventory != null) panelInventory.SetActive(false);
        }
    }

    void OnInventory(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused || ForsakenClock.selesai) return;
        if (terbuka) Tutup();
        else Buka();
    }

    void Buka()
    {
        terbuka = true;
        if (panelInventory != null) panelInventory.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetPlayerScript(false);
        BangunDaftar();
    }

    void Tutup()
    {
        terbuka = false;
        if (panelInventory != null) panelInventory.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetPlayerScript(true);
    }

    void SetPlayerScript(bool aktif)
    {
        foreach (var s in scriptDimatikan)
            if (s != null) s.enabled = aktif;
    }

    public bool Tambah(string nama)
    {
        if (isi.Count >= kapasitas)
        {
            Pesan("Inventory penuh");
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

    void Gunakan(string nama)
    {
        if (nama == "Baterai")
        {
            if (nightVision != null) nightVision.IsiUlangBaterai();
        }
        else
        {
            Pesan(nama + " tidak bisa dipakai");
        }
    }

    void Pesan(string teks)
    {
        if (teksPesan == null) return;
        teksPesan.text = teks;
        timerPesan = 2f;
    }

    void Refresh()
    {
        if (teksInventory != null)
        {
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

        if (terbuka) BangunDaftar();
    }

    void BangunDaftar()
    {
        if (isiList == null || tombolItemPrefab == null) return;

        for (int i = isiList.childCount - 1; i >= 0; i--)
        {
            GameObject anak = isiList.GetChild(i).gameObject;
            anak.SetActive(false);
            Destroy(anak);
        }

        var hitung = new Dictionary<string, int>();
        foreach (var s in isi)
        {
            if (hitung.TryGetValue(s, out int n)) hitung[s] = n + 1;
            else hitung[s] = 1;
        }

        foreach (var kv in hitung)
        {
            string nama = kv.Key;
            Button tombol = Instantiate(tombolItemPrefab, isiList);
            tombol.gameObject.SetActive(true);

            TMP_Text label = tombol.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = nama + " x" + kv.Value;

            tombol.onClick.AddListener(() => Gunakan(nama));
        }
    }
}