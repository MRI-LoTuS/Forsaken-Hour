using UnityEngine;

[CreateAssetMenu(fileName = "Prodi_", menuName ="Forsaken Hour/Prodi")]
public class ProdiData : ScriptableObject
{
    public string id;
    public string namaTampil;
    [TextArea] public string deskripsi;
    public string namaAbility;
    [TextArea] public string deskripsiAbility;
    public Sprite ikon;
    public bool terbukaAwal;
    public int hargaUnlock = 500;
    public GameObject abilityPrefab;
}