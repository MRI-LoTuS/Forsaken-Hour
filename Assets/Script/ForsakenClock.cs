using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForsakenClock : MonoBehaviour
{
    public static bool selesai { get; private set; }

    public TMP_Text teksPesan;
    public GameObject panelMenang;
    public MonoBehaviour[] scriptDimatikan;
    public string menuScene = "MainMenu";
    public Renderer visual;
    public Color warnaMati = Color.gray;

    float timerPesan;

    void Start()
    {
        selesai = false;
        if (panelMenang != null) panelMenang.SetActive(false);
    }

    void Update()
    {
        if (timerPesan > 0f)
        {
            timerPesan -= Time.unscaledDeltaTime;
            if (timerPesan <= 0f && teksPesan != null) teksPesan.text = "";
        }
    }

    public string TeksPrompt()
    {
        KeyManager km = KeyManager.Instance;
        if (km == null || km.Lengkap) return "[F] Matikan Forsaken Clock";
        return "Forsaken Clock (terkunci " + km.Terkumpul + "/" + km.totalKunci + ")";
    }

    public void Interaksi()
    {
        if (selesai) return;

        KeyManager km = KeyManager.Instance;
        if (km != null && !km.Lengkap)
        {
            Pesan("Butuh " + (km.totalKunci - km.Terkumpul) + " kunci lagi");
            return;
        }

        Menang();
    }

    void Menang()
    {
        selesai = true;
        if (visual != null) visual.material.color = warnaMati;
        if (panelMenang != null) panelMenang.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        foreach (var s in scriptDimatikan)
            if (s != null) s.enabled = false;

        Time.timeScale = 0f;
    }

    public void KeMenuUtama()
    {
        Time.timeScale = 1f;
        selesai = false;
        SceneManager.LoadScene(menuScene);
    }

    void Pesan(string teks)
    {
        if (teksPesan == null) return;
        teksPesan.text = teks;
        timerPesan = 2f;
    }
}