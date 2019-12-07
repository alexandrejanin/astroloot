using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAim : MonoBehaviour {
    [SerializeField, MaxValue(0)] private float zOffset = -10;

    [SerializeField, MinValue(0)] private float maxOffset = 5;

    private new Camera camera;

    private PlayerCombat playerCombat;

    private void Awake() {
        camera = GetComponent<Camera>();
        Player.onLocalPlayerSpawned += player => playerCombat = player.GetComponent<PlayerCombat>();
    }

    private void Update() {
        if (!playerCombat)
            return;

        var aimingOffset = new Vector3(-1, -1) + 2 * camera.WorldToViewportPoint(playerCombat.TargetPosition);
        aimingOffset.z = 0;
        transform.position = playerCombat.transform.position + zOffset * Vector3.forward + aimingOffset * maxOffset;
    }
}