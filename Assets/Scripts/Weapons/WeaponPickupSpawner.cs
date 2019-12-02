using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class WeaponPickupSpawner : MonoBehaviour {
    [SerializeField] private Transform spawnPointsParent;

    private readonly HashSet<int> takenSpawnPoints = new HashSet<int>();

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
        var spawnPointIndex = Random.Range(0, spawnPointsParent.childCount);

        if (takenSpawnPoints.Contains(spawnPointIndex))
            return;

        takenSpawnPoints.Add(spawnPointIndex);

        var spawnPoint = spawnPointsParent.GetChild(spawnPointIndex);

        if (spawnPoint == null)
            return;

        var pickup = (WeaponPickup) NetworkManager.Instance.InstantiateWeaponPickup(position: spawnPoint.position);
        pickup.PickupIndex = spawnPointIndex;
    }

    public void OnPickedUp(int index) {
        takenSpawnPoints.Remove(index);
    }
}