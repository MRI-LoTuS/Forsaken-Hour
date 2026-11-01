using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentWarningPanel : MenuPanel
{
    [SerializeField] Button tombolLanjut;
    [SerializeField] TMP_Text teksTombol;
    [SerializeField] float jedaDetik = 3f;

    public override void SaatDibuka()
    {
        tombolLanjut.interactable = false;
        StartCoroutine(Hitung());
    }

    IEnumerator Hitung()
    {
        float sisa = jedaDetik;
        while (sisa > 0f)
        {
            teksTombol.text = "Saya Mengerti (" + Mathf.CeilToInt(sisa) + ")";
            sisa -= Time.unscaledDeltaTime;
            yield return null;
        }
        teksTombol.text = "Saya Mengerti";
        tombolLanjut.interactable = true;
    }

    // OnClick tombol Saya Mengerti
    public void Lanjut()
    {
        ProfileService.Current.sudahLihatPeringatan = true;
        ProfileService.Simpan();
        MenuNavigator.Instance.KeUtama();
    }

    // OnClick tombol Keluar
    public void Keluar()
    {
        KeluarGame.Keluar();
    }
}