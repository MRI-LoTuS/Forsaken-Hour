using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TangkapManager : MonoBehaviour
{
    public static TangkapManager Instance { get; private set; }
    public static bool selesai { get; private set; }

    [SerializeField] GameObject panelTertangkap;
    [SerializeField] TMP_Text teksHadiah;
    [SerializeField] MonoBehaviour[] scriptDimatikan;   // skrip gerak dan kamera
    [SerializeField] string menuScene = "MainMenu";

    void Awake()
    {
        Instance = this;
        selesai = false;
        RunSelesai.Reset();
        if (panelTertangkap != null) panelTertangkap.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Tangkap()
    {
        if (selesai || ForsakenClock.selesai) return; 
        selesai = true;

        var hasil = RunSelesai.Akhiri(false);
        if (teksHadiah != null) teksHadiah.text = hasil.Ringkasan();

        foreach (var s in scriptDimatikan)
            if (s != null) s.enabled = false;

        if (panelTertangkap != null) panelTertangkap.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // OnClick tombol Ulangi
    public void Ulangi()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // OnClick tombol Menu Utama
    public void KeMenuUtama()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);
    }
}