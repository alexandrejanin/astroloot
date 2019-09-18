using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAim : MonoBehaviour {
    [SerializeField]
    private RectTransform cursor;

    [SerializeField, MaxValue(0)]
    private float zOffset = -10;

    [SerializeField, MinValue(0)]
    private float maxOffset = 5;

    private new Camera camera;

    private PlayerCombat target;

    private void Awake() {
        camera = GetComponent<Camera>();
        SpawnController.onPlayerObjectSpawned += o => target = o.GetComponent<PlayerCombat>();
    }

    private void Update() {
        if (!target)
            return;

        var aimingOffset = new Vector3(-1, -1) + 2 * camera.WorldToViewportPoint(target.AimingPosition);
        aimingOffset.z = 0;
        transform.position = target.transform.position + zOffset * Vector3.forward + aimingOffset * maxOffset;

        cursor.position = camera.WorldToScreenPoint(target.AimingPosition);
        Cursor.visible = false;
    }
}