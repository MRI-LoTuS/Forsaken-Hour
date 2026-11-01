using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPanel : MenuPanel
{
    [SerializeField] string namaSceneGame = "gedung";
    [SerializeField] TMP_Text teksCurrency;
    [SerializeField] TMP_Text teksProdi;

    void OnEnable()
    {
        ProfileService.OnBerubah += Segarkan;
        Segarkan();
    }

    void OnDisable()
    {
        ProfileService.OnBerubah -= Segarkan;
    }

    void Segarkan()
    {
        var p = ProfileService.Current;
        var db = GameDatabase.Instance;

        if (teksCurrency != null) teksCurrency.text = "Currency: " + p.currency;

        if (teksProdi != null)
        {
            var prodi = db != null ? db.CariProdi(p.prodiTerpilih) : null;
            teksProdi.text = "Prodi: " + (prodi != null ? prodi.namaTampil : "-");
        }
    }

    // OnClick tombol Start Game
    public void Mulai()
    {
        GameSession.MulaiRun();

        if (GameSession.ProdiAktif == null)
        {
            ConfirmDialog.Instance.Buka("Belum ada prodi",
                "Pilih prodi dulu di menu Study Program.", null, "OK", null);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(namaSceneGame);
    }

    // OnClick tombol Keluar
    public void Keluar()
    {
        ConfirmDialog.Instance.Buka("Keluar dari game?",
            "Progres sudah tersimpan otomatis.",
            KeluarGame.Keluar, "Keluar", "Batal");
    }
}