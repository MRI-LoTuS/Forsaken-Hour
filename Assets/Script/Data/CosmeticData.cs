using UnityEngine;

public enum TipeKosmetik { Aksesori, SkinTangan, TampilanSenter }

[CreateAssetMenu(fileName = "Kosmetik_", menuName = "Forsaken Hour/Kosmetik")]
public class CosmeticData : ScriptableObject
{
    public string id;
    public string namaTampil;
    public TipeKosmetik tipe;
    public int harga = 100;
    public bool dimilikiAwal;
    public ProdiData prodiKhusus;      // kosong = bisa dipakai semua prodi
    public Sprite ikon;
    public GameObject prefabTampilan;  // diisi nanti
}