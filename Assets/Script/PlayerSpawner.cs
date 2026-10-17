using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform spawnPoint;
    void Start()
    {
        CharacterController controller = GetComponent<CharacterController>();

        if (controller != null)
        {
            transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }

        if (controller != null)
        {
            controller.enabled = true;
        }
    }
}
