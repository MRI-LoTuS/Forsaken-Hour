using System;
using UnityEngine;

// Papan pengumuman suara. Pemancar menyiarkan, Stalker mendengarkan.
public static class Kebisingan
{
    public static event Action<Vector3, float> OnSuara;   // posisi, radius

    public static void Buat(Vector3 posisi, float radius)
    {
        if (OnSuara != null) OnSuara(posisi, radius);
    }
}