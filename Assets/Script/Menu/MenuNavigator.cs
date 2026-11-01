using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuNavigator : MonoBehaviour
{
    public static MenuNavigator Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] MenuPanel panelPeringatan;
    [SerializeField] MenuPanel panelUtama;
    [SerializeField] List<MenuPanel> semuaPanel = new List<MenuPanel>();

    [Header("Input (opsional, isi dengan UI/Cancel)")]
    [SerializeField] InputActionReference kembaliAction;

    readonly Stack<MenuPanel> riwayat = new Stack<MenuPanel>();
    MenuPanel aktif;

    void Awake()
    {
        Instance = this;
        foreach (var p in semuaPanel)
            if (p != null) p.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (kembaliAction != null)
        {
            kembaliAction.action.performed += OnKembali;
            kembaliAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (kembaliAction != null) kembaliAction.action.performed -= OnKembali;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!ProfileService.Current.sudahLihatPeringatan && panelPeringatan != null)
            Tampilkan(panelPeringatan);
        else
            Tampilkan(panelUtama);
    }

    void OnKembali(InputAction.CallbackContext ctx)
    {
        if (ConfirmDialog.Tampil) { ConfirmDialog.Instance.Tutup(); return; }
        Kembali();
    }

    void Tampilkan(MenuPanel p)
    {
        if (aktif != null)
        {
            aktif.SaatDitutup();
            aktif.gameObject.SetActive(false);
        }

        aktif = p;

        if (aktif != null)
        {
            aktif.gameObject.SetActive(true);
            aktif.SaatDibuka();
        }
    }

    // Dipanggil dari tombol (OnClick), seret panel tujuan ke kolom parameter
    public void Buka(MenuPanel tujuan)
    {
        if (tujuan == null || tujuan == aktif) return;
        if (aktif != null) riwayat.Push(aktif);
        Tampilkan(tujuan);
    }

    public void Kembali()
    {
        if (riwayat.Count == 0) return;
        Tampilkan(riwayat.Pop());
    }

    public void KeUtama()
    {
        riwayat.Clear();
        Tampilkan(panelUtama);
    }
}