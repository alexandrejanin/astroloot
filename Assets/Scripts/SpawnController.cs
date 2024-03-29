﻿using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnController : MonoBehaviour {
    [SerializeField] private Transform spawnPointsParent;

    private void Awake() {
        if (!NetworkManager.Instance) {
            SceneManager.LoadScene(0);
            return;
        }

        if (NetworkManager.Instance.IsServer) {
            NetworkManager.Instance.Networker.playerAccepted += (player, sender) => SpawnPlayer(player);
            SpawnPlayer();
        }
    }

    private void SpawnPlayer(NetworkingPlayer player = null) {
        MainThreadManager.Run(() => {
            var spawnPoint = GetSpawnPoint();
            var spawnedPlayer = (Player) NetworkManager.Instance.InstantiatePlayer(position: spawnPoint.position);

            // player == null => host spawned
            if (player == null) {
                Player.onLocalPlayerSpawned?.Invoke(spawnedPlayer);
            } else {
                spawnedPlayer.SetOwner(player);
            }
        });
    }

    public Transform GetSpawnPoint() => spawnPointsParent.GetChild(Random.Range(0, spawnPointsParent.childCount));
}