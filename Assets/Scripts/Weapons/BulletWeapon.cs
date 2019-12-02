using UnityEngine;

public class BulletWeapon : Weapon {
    [SerializeField] private int damage;
    [SerializeField] protected float rateOfFire;
    [SerializeField] private bool fullAuto;

    [SerializeField] private Bullet bulletPrefab;

    [SerializeField] private Transform bulletOriginPosition;

    public override Transform OriginPosition => bulletOriginPosition;
    public override Sprite Sprite => SpriteRenderer.sprite;
    public override Vector3 ArmAngle => Vector3.zero;

    private SpriteRenderer SpriteRenderer =>
        spriteRenderer ? spriteRenderer : spriteRenderer = GetComponent<SpriteRenderer>();

    private SpriteRenderer spriteRenderer;

    private float timeSinceLastShot;

    private void Update() {
        timeSinceLastShot += Time.deltaTime;
    }

    public override bool CanShoot() {
        if (timeSinceLastShot < 1f / rateOfFire)
            return false;

        timeSinceLastShot = 0f;
        return true;
    }

    public override Bullet Fire(uint bulletId, Vector2 originPosition, Vector2 targetPosition) {
        var bullet = Instantiate(bulletPrefab, originPosition, Quaternion.identity);
        bullet.transform.right = targetPosition - originPosition;
        bullet.Damage = damage;
        bullet.Player = Player;
        bullet.BulletId = bulletId;

        return bullet;
    }
}