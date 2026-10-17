using UnityEngine;

public class AddCollidersToChildren : MonoBehaviour
{
    [ContextMenu("Add Mesh Colliders To All Children")]
    public void AddColliders()
    {
        // Mengambil semua MeshFilter di dalam objek ini dan anak-anaknya
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);

        int count = 0;
        foreach (MeshFilter mf in meshFilters)
        {
            string objName = mf.gameObject.name.ToLower();

            // Cek apakah objek belum memiliki collider
            if (mf.gameObject.GetComponent<Collider>() == null)
            {
                MeshCollider collider = mf.gameObject.AddComponent<MeshCollider>();

                // Atur Convex: false agar lantai, tangga, atau lorong bisa dilewati/dipijak dengan akurat
                collider.convex = false;

                count++;
                Debug.Log("Berhasil menambahkan MeshCollider ke: " + mf.gameObject.name);
            }
        }

        Debug.Log($"Selesai! Total {count} MeshCollider berhasil ditambahkan ke seluruh bagian gedung.");
    }
}
