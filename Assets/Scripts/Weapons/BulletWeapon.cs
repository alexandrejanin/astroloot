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

    public int MaxBullets => maxBullets;
    public int Bullets => bullets;
    public bool IsReloading => isReloading;
    public float ReloadProgress => reloadProgress / reloadTime;

    private SpriteRenderer SpriteRenderer =>
        spriteRenderer ? spriteRenderer : spriteRenderer = GetComponent<SpriteRenderer>();

    private SpriteRenderer spriteRenderer;

    private int bullets;
    private float timeSinceLastShot;
    private bool isReloading;
    private float reloadProgress;

    protected override void Init() {
        base.Init();
        bullets = maxBullets;
    }

    private void Update() {
        timeSinceLastShot += Time.deltaTime;
    }

    private IEnumerator Reload() {
        if (isReloading)
            yield break;

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
        if (timeSinceLastShot < 1f / rateOfFire)
            return false;

        if (bullets <= 0) {
            StartCoroutine(Reload());
            return false;
        }

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