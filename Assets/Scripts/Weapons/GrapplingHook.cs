using UnityEngine;

public class GrapplingHook : Gadget {
    [SerializeField] private Transform hookPrefab;
    [SerializeField] private float hookSpeed = 20;
    [SerializeField] private float maxDuration = 1f;
    [SerializeField] private float force = 1f;
    [SerializeField] private LayerMask layerMask;

    public override bool FullAuto => false;

    private Transform hook;
    private float duration;

    public override void Use() {
        if (hook)
            return;

        hook = Instantiate(hookPrefab, transform.position, Quaternion.identity);
        hook.right = player.PlayerCombat.TargetPosition - (Vector2) transform.position;
        duration = 0;
    }

    private void Update() {
        if (!hook)
            return;

        hook.Translate(Time.deltaTime * hookSpeed * Vector3.right, Space.Self);

        if (!player.IsLocalPlayer)
            return;

        var hit = Physics2D.Raycast(hook.position, hook.right, 0.1f, layerMask);
        if (hit)
            OnHookHit(hit.point);

        duration += Time.deltaTime;
        Debug.Log(duration);
        if (duration >= maxDuration) {
            Destroy(hook.gameObject);
        }
    }

    private void OnHookHit(Vector3 position) {
        Destroy(hook.gameObject);
        var direction = position - transform.position;
        player.PlayerMovement.AddForce(force * direction);
    }
}