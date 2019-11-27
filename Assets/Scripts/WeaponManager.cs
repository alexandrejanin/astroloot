using UnityEngine;

public class WeaponManager : MonoBehaviour {
    [SerializeField]
    private Weapon[] weapons;

    public Weapon GetWeapon(int index) {
        return index < 0
            ? null
            : weapons[index];
    }

    public int GetRandomWeaponIndex() {
        return Random.Range(0, weapons.Length);
    }
}