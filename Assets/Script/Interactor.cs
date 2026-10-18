using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Camera kamera;
    public float jarak = 3f;
    public Inventory inventory;
    public TMP_Text teksPrompt;
    [SerializeField] private InputActionReference interactAction;

    Pickup target;
    ForsakenClock jam;

    void OnEnable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
    }

    void OnDisable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.performed -= OnInteract;
        }
    }

    bool Terblokir()
    {
        return PauseMenu.isPaused || Inventory.terbuka || ForsakenClock.selesai;
    }

    void Update()
    {
        target = null;
        jam = null;

        if (!Terblokir())
        {
            Ray ray = new Ray(kamera.transform.position, kamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, jarak))
            {
                target = hit.collider.GetComponentInParent<Pickup>();
                jam = hit.collider.GetComponentInParent<ForsakenClock>();
            }
        }

        if (teksPrompt != null)
        {
            if (jam != null) teksPrompt.text = jam.TeksPrompt();
            else if (target != null) teksPrompt.text = "[F] Ambil " + target.namaItem;
            else teksPrompt.text = "";
        }
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (Terblokir()) return;

        if (jam != null)
        {
            jam.Interaksi();
            return;
        }

        if (target == null) return;

        if (target.adalahKunci)
        {
            if (KeyManager.Instance != null) KeyManager.Instance.AmbilKunci();
            Destroy(target.gameObject);
            return;
        }

        if (inventory.Tambah(target.namaItem))
            Destroy(target.gameObject);
    }

    // Fungsi pembantu untuk membaca nama tombol dari Input Action secara dinamis
    private string GetBindingDisplayName()
    {
        if (interactAction == null || interactAction.action == null) return "Interact";

        // Mencari binding pertama yang aktif untuk action ini
        for (int i = 0; i < interactAction.action.bindings.Count; i++)
        {
            // Menghindari binding komposit (seperti WASD) agar mengambil tombol tunggal yang bersih
            if (!interactAction.action.bindings[i].isComposite)
            {
                return interactAction.action.GetBindingDisplayString(i);
            }
        }
        return "E"; // Nilai cadangan jika tidak ditemukan
    }
}