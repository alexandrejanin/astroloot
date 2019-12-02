using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class WeaponPickupSpawner : MonoBehaviour {
    [SerializeField] private Transform spawnPointsParent;

    [SerializeField] private float delay = 5f;

    private float timeSinceLastSpawn;

    private void Update() {
        if (!NetworkManager.Instance.IsServer)
            return;

        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= delay) {
            SpawnWeaponPickup();
            timeSinceLastSpawn = 0;
        }
    }

    private void SpawnWeaponPickup() {
        var spawnPoint = GetSpawnPoint();
        NetworkManager.Instance.InstantiateWeaponPickup(position: spawnPoint.position);
    }

    public Transform GetSpawnPoint() => spawnPointsParent.GetChild(Random.Range(0, spawnPointsParent.childCount));
}