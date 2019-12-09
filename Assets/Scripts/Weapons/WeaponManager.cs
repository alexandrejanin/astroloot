using UnityEngine;

public class WeaponManager : MonoBehaviour {
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Gadget[] gadgets;

    public Weapon GetWeapon(int index) {
        return index < 0
            ? null
            : weapons[index];
    }

    public int GetRandomWeaponIndex() {
        return Random.Range(0, weapons.Length);
    }

    public Gadget GetGadget(int index) {
        return index < 0
            ? null
            : gadgets[index];
    }

    public int GetRandomGadgetIndex() {
        return Random.Range(0, gadgets.Length);
    }
}