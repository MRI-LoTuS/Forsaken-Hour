using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NightVision : MonoBehaviour
{
    [Header("Referensi")]
    public Inventory inventory;
    public GameObject overlayHijau;
    public Light cahayaNV;
    public TMP_Text teksBaterai;
    public TMP_Text teksStatus;

    [Header("Pengaturan")]
    public float energiMaks = 100f;
    public float energi = 100f;
    public float pengurangPerDetik = 5f;
    public string namaItemBaterai = "Baterai";

    [SerializeField] private InputActionReference abilityAction;
    [SerializeField] private InputActionReference isiUlangAction;

    bool aktif;
    float timerStatus;

    void Start()
    {
        SetAktif(false);
        if (teksStatus != null) teksStatus.text = "";
        UpdateUI();
    }

    void OnEnable()
    {
        if (abilityAction != null && abilityAction.action != null)
        {
            abilityAction.action.Enable();
            abilityAction.action.performed += OnAbility;
        }
        if (isiUlangAction != null && isiUlangAction.action != null)
        {
            isiUlangAction.action.Enable();
            isiUlangAction.action.performed += OnIsiUlang;
        }
    }

    void OnDisable()
    {
        if (abilityAction != null && abilityAction.action != null)
            abilityAction.action.performed -= OnAbility;
        if (isiUlangAction != null && isiUlangAction.action != null)
            isiUlangAction.action.performed -= OnIsiUlang;
    }

    void Update()
    {
        if (aktif)
        {
            energi -= pengurangPerDetik * Time.deltaTime;
            if (energi <= 0f)
            {
                energi = 0f;
                SetAktif(false);
                Status("Baterai habis");
            }
        }

        if (timerStatus > 0f)
        {
            timerStatus -= Time.unscaledDeltaTime;
            if (timerStatus <= 0f && teksStatus != null) teksStatus.text = "";
        }

        UpdateUI();
    }

    void OnAbility(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused) return;

        if (aktif)
        {
            SetAktif(false);
            return;
        }

        // Validasi resource: tidak bisa aktif kalau energi habis
        if (energi <= 0f)
        {
            Status("Baterai habis, tekan R untuk isi ulang");
            return;
        }

        SetAktif(true);
    }

    void OnIsiUlang(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused) return;

        if (energi >= energiMaks)
        {
            Status("Baterai masih penuh");
            return;
        }

        if (inventory != null && inventory.Pakai(namaItemBaterai))
        {
            energi = energiMaks;
            Status("Baterai diisi ulang");
        }
        else
        {
            Status("Tidak ada baterai di inventory");
        }
    }

    void SetAktif(bool nilai)
    {
        aktif = nilai;
        if (overlayHijau != null) overlayHijau.SetActive(nilai);
        if (cahayaNV != null) cahayaNV.enabled = nilai;
    }

    void Status(string teks)
    {
        if (teksStatus == null) return;
        teksStatus.text = teks;
        timerStatus = 2f;
    }

    void UpdateUI()
    {
        if (teksBaterai == null) return;
        int persen = Mathf.CeilToInt(energi / energiMaks * 100f);
        int cadangan = inventory != null ? inventory.Jumlah(namaItemBaterai) : 0;
        teksBaterai.text = "DKV Night Vision: " + persen + "% " + (aktif ? "[ON]" : "[OFF]")
            + "\nBaterai cadangan: " + cadangan;
    }
}