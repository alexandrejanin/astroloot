using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField]
    private float speed = 20;

    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private float rayLength = 0.3f;

    public Player Player { set; private get; }

    public uint BulletId { set; private get; }

    private bool active = true;

    private void Update() {
        transform.position += Time.deltaTime * speed * transform.right;

        if (active && Player.IsLocalPlayer) {
            var hit = Physics2D.Raycast(transform.position, transform.right, rayLength);
            if (hit) {
                var player = hit.transform.GetComponent<Player>();
                if (player) {
                    if (!player.IsLocalPlayer) {
                        player.Damage(damage);
                        SendDestroyRequest();
                    }
                } else {
                    SendDestroyRequest();
                }
            }
        }
    }

    private void SendDestroyRequest() {
        active = false;
        Player.DestroyBullet(BulletId);
    }

    public void Destroy() {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawRay(transform.position, transform.right * rayLength);
    }
}