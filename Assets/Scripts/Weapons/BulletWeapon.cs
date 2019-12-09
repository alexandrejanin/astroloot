using System.Collections;
using UnityEngine;

public class BulletWeapon : Weapon {
    [SerializeField] private float rateOfFire = 5;
    [SerializeField] private bool fullAuto;
    [SerializeField] private int maxBullets = 20;
    [SerializeField] private float reloadTime = 2;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletOriginPosition;

    public override Transform OriginPosition => bulletOriginPosition;
    public override Sprite Sprite => SpriteRenderer.sprite;
    public override Vector3 ArmAngle => Vector3.zero;

    public override bool FullAuto => fullAuto;

    public int MaxBullets => maxBullets;
    public int Bullets => bullets;
    public bool IsReloading => isReloading;
    public float ReloadProgress => reloadProgress / reloadTime;

    private SpriteRenderer SpriteRenderer => spriteRenderer
        ? spriteRenderer
        : spriteRenderer = GetComponent<SpriteRenderer>();

    private SpriteRenderer spriteRenderer;

    private int bullets;
    private float timeSinceLastShot;
    private bool isReloading;
    private float reloadProgress;

    protected override void Init() {
        bullets = maxBullets;
    }

    private void Update() {
        timeSinceLastShot += Time.deltaTime;

        if (!isReloading && bullets <= 0)
            Reload();
    }

    public void Reload() {
        if (bullets >= maxBullets || isReloading)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine() {
        isReloading = true;
        reloadProgress = 0f;

        while (reloadProgress < reloadTime) {
            reloadProgress += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        bullets = maxBullets;
        isReloading = false;
    }

    public override bool CanShoot() {
        if (isReloading)
            return false;

        if (timeSinceLastShot < 1f / rateOfFire)
            return false;

        bullets--;
        timeSinceLastShot = 0f;
        return true;
    }

    public override Bullet Fire(uint bulletId, Vector2 originPosition, Vector2 targetPosition) {
        var bullet = Instantiate(bulletPrefab, originPosition, Quaternion.identity);
        bullet.transform.right = targetPosition - originPosition;
        bullet.Init(player, bulletId);

        return bullet;
    }
}