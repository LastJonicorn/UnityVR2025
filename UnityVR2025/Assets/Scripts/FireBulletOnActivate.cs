using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireBulletOnActivate : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float fireSpeed = 20f;
    public GameObject muzzleFlashPrefab; // Reference to the muzzle flash particle system prefab

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabbable;

    private void Start()
    {
        grabbable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabbable != null)
            grabbable.activated.AddListener(FireBullet);
        else
            Debug.LogError("No XRGrabInteractable component found on " + gameObject.name);
    }

    private void OnDestroy()
    {
        if (grabbable != null)
            grabbable.activated.RemoveListener(FireBullet);
    }

    private void FireBullet(ActivateEventArgs arg)
    {
        if (bulletPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Bullet prefab or spawn point not set.");
            return;
        }

        GameObject spawnedBullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody bulletRigidbody = spawnedBullet.GetComponent<Rigidbody>();
        if (bulletRigidbody != null)
            bulletRigidbody.linearVelocity = spawnPoint.forward * fireSpeed;
        else
        {
            Debug.LogError("Rigidbody component not found on bullet prefab.");
            Destroy(spawnedBullet);
            return;
        }

        // Instantiate muzzle flash if available
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, spawnPoint.position, spawnPoint.rotation);
            Destroy(muzzleFlash, 0.1f); // Adjust the duration as needed
        }

        Destroy(spawnedBullet, 5f);
    }
}
