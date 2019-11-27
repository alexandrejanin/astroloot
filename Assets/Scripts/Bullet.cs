using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private float speed = 20;

    public int Damage { set; private get; }

    public Player Player { set; private get; }

    public uint BulletId { set; private get; }

    private bool active = true;

    private void Update() {
        transform.position += Time.deltaTime * speed * transform.right;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!active || !Player.IsLocalPlayer)
            return;

        var player = other.gameObject.GetComponent<Player>();

        if (!player) {
            SendDestroyRequest();
            return;
        }

        if (player.IsLocalPlayer)
            return;

        player.Damage(Damage);
        SendDestroyRequest();
    }

    private void SendDestroyRequest() {
        active = false;
        Player.DestroyBullet(BulletId);
    }

    public void Destroy() {
        Destroy(gameObject);
    }
}