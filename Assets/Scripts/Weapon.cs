using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject {
    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int damage;

    [SerializeField]
    private float rateOfFire;

    [SerializeField]
    private bool fullAuto;

    [SerializeField]
    private Bullet bullet;

    public Sprite Sprite => sprite;
    public int Damage => damage;
    public float RateOfFire => rateOfFire;
    public bool FullAuto => fullAuto;
    public Bullet Bullet => bullet;
}