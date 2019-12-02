using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] private float speed = 20;

    public int Damage { set; get; }

    public Player Player { set; private get; }

    public uint BulletId { set; private get; }

    private bool active = true;

    private void Update() {
        transform.position += Time.deltaTime * speed * transform.right;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!active || !Player.IsLocalPlayer)
            return;

        if (other.gameObject.isStatic) {
            SendDestroyRequest();
            return;
        }

        var hurtBox = other.gameObject.GetComponent<HurtBox>();

        if (!hurtBox || hurtBox.Player == Player)
            return;

        hurtBox.Hit(this);

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