using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PemainSembunyi : MonoBehaviour
{
    public static bool Bersembunyi { get; private set; }

    [SerializeField] Camera kamera;
    [SerializeField] float jarak = 3f;
    [SerializeField] TMP_Text teksPrompt;
    [SerializeField] InputActionReference interactAction;     // Player/Take (F)
    [SerializeField] CharacterController controller;
    [SerializeField] MonoBehaviour[] skripGerakDimatikan;     // playerMovement saja

    TempatSembunyi dilihat;
    TempatSembunyi aktif;
    float waktuToggle;

    void Awake()
    {
        Bersembunyi = false;
    }

    void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteract;
            interactAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.performed -= OnInteract;
    }

    static bool Terblokir()
    {
        return PauseMenu.isPaused || Inventory.terbuka
            || ForsakenClock.selesai || TangkapManager.selesai;
    }

    void Update()
    {
        dilihat = null;
        if (Bersembunyi || Terblokir()) return;

        Ray ray = new Ray(kamera.transform.position, kamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, jarak, ~0, QueryTriggerInteraction.Ignore))
            dilihat = hit.collider.GetComponentInParent<TempatSembunyi>();
    }

    // LateUpdate supaya tulisan ini tampil setelah Interactor mengatur prompt-nya
    void LateUpdate()
    {
        if (teksPrompt == null) return;

        string tombol = interactAction != null
            ? interactAction.action.GetBindingDisplayString() : "F";

        if (Bersembunyi)
            teksPrompt.text = "[" + tombol + "] Keluar";
        else if (dilihat != null && !dilihat.Terisi)
            teksPrompt.text = "[" + tombol + "] Sembunyi di " + dilihat.namaTempat;
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (Time.unscaledTime - waktuToggle < 0.4f) return;
        if (Terblokir()) return;

        if (Bersembunyi)
        {
            Keluar();
            waktuToggle = Time.unscaledTime;
        }
        else if (dilihat != null && !dilihat.Terisi)
        {
            Masuk(dilihat);
            waktuToggle = Time.unscaledTime;
        }
    }

    void Masuk(TempatSembunyi tempat)
    {
        aktif = tempat;
        Bersembunyi = true;

        foreach (var s in skripGerakDimatikan)
            if (s != null) s.enabled = false;

        controller.enabled = false;
        transform.SetPositionAndRotation(tempat.TitikSembunyi.position, tempat.TitikSembunyi.rotation);
        tempat.SaatMasuk();
    }

    void Keluar()
    {
        var tempat = aktif;
        aktif = null;

        transform.SetPositionAndRotation(tempat.TitikKeluar.position, tempat.TitikKeluar.rotation);
        controller.enabled = true;

        foreach (var s in skripGerakDimatikan)
            if (s != null) s.enabled = true;

        Bersembunyi = false;
        tempat.SaatKeluar();
    }
}