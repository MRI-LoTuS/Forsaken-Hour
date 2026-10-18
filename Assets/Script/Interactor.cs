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

    void Update()
    {
        target = null;
        Ray ray = new Ray(kamera.transform.position, kamera.transform.forward);

        if (!PauseMenu.isPaused && !Inventory.terbuka && Physics.Raycast(ray, out RaycastHit hit, jarak))
        {       
                target = hit.collider.GetComponentInParent<Pickup>();
        }

        if (teksPrompt != null)
        {
            if (target != null)
            {
                // Mengambil nama tombol secara otomatis dari New Input System (contoh: "E", "F", atau "Gamepad West")
                string tombolInteract = GetBindingDisplayName();
                teksPrompt.text = $"[{tombolInteract}] Ambil {target.namaItem}";
            }
            else
            {
                teksPrompt.text = "";
            }
        }
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused || Inventory.terbuka || target == null) return;

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