using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "Forsaken Hour/Game Database")]
public class GameDatabase : ScriptableObject
{
    public List<ProdiData> semuaProdi = new List<ProdiData>();
    public List<CosmeticData> semuaKosmetik = new List<CosmeticData>();

    static GameDatabase _instance;

    public static GameDatabase Instance
    {
        get
        {
            if (_instance == null) _instance = Resources.Load<GameDatabase>("GameDatabase");
            if (_instance == null)
                Debug.LogError("GameDatabase tidak ditemukan di Assets/Resources/GameDatabase");
            return _instance;
        }
    }

    public ProdiData CariProdi(string id)
    {
        return semuaProdi.Find(p => p != null && p.id == id);
    }

    public CosmeticData CariKosmetik(string id)
    {
        return semuaKosmetik.Find(k => k != null && k.id == id);
    }
}