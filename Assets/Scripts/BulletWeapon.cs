using UnityEngine;

public class BulletWeapon : Weapon {
    [SerializeField]
    private bool fullAuto;

    [SerializeField]
    private Bullet bulletPrefab;

    [SerializeField]
    private Transform bulletOriginPosition;

    public override Transform OriginPosition => bulletOriginPosition;
    public override Sprite Sprite => SpriteRenderer.sprite;

    private SpriteRenderer SpriteRenderer => spriteRenderer ? spriteRenderer : spriteRenderer = GetComponent<SpriteRenderer>();
    private SpriteRenderer spriteRenderer;

    private float timeSinceLastShot;

    private void Update() {
        timeSinceLastShot += Time.deltaTime;
    }

    public override bool CanShoot() {
        if (timeSinceLastShot > 1f / rateOfFire) {
            timeSinceLastShot = 0f;
            return true;
        }

        return false;
    }

    public override Bullet Fire(uint bulletId, Vector2 originPosition, Vector2 targetPosition, Player player) {
        var bullet = Instantiate(bulletPrefab, originPosition, Quaternion.identity);
        bullet.transform.right = targetPosition - originPosition;
        bullet.Damage = damage;
        bullet.Player = player;
        bullet.BulletId = bulletId;

        return bullet;
    }
}