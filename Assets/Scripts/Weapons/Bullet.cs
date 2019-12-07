using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] private int damage = 10;
    [SerializeField] private float knockback = 1;
    [SerializeField] private float speed = 20;

    public int Damage => damage;
    public float Knockback => knockback;

    private Player player;
    private uint bulletId;

    private bool active = true;

    public void Init(Player player, uint bulletId) {
        this.player = player;
        this.bulletId = bulletId;
    }

    private void Update() {
        transform.position += Time.deltaTime * speed * transform.right;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!active || !player.IsLocalPlayer)
            return;

        if (other.gameObject.isStatic) {
            SendDestroyRequest();
            return;
        }

        var hurtBox = other.gameObject.GetComponent<HurtBox>();

        if (!hurtBox || hurtBox.Player == player)
            return;

        hurtBox.Hit(this);

        SendDestroyRequest();
    }

    private void SendDestroyRequest() {
        active = false;
        player.DestroyBullet(bulletId);
    }

    public void Destroy() {
        Destroy(gameObject);
    }
}