using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class SpawnController : MonoBehaviour {
    [SerializeField]
    private GameObject playerPrefab;

    public static GameObject LocalPlayerObject { get; private set; }

    public delegate void OnPlayerObjectSpawned(GameObject localPlayerObject);

    public static OnPlayerObjectSpawned onPlayerObjectSpawned;

    [SerializeField]
    private Transform spawnPointsParent;

    private void Start() {
        var spawnPoint = GetRandomSpawnPoint();

        LocalPlayerObject = NetworkManager.Instance
            ? NetworkManager.Instance.InstantiatePlayer(position: spawnPoint.position).gameObject
            : Instantiate(playerPrefab, spawnPoint.position, playerPrefab.transform.rotation);

        onPlayerObjectSpawned?.Invoke(LocalPlayerObject);
    }

    private Transform GetRandomSpawnPoint() => spawnPointsParent.GetChild(Random.Range(0, spawnPointsParent.childCount));
}