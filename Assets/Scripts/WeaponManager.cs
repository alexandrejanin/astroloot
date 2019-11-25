using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponManager : MonoBehaviour {
    [SerializeField]
    private Weapon[] weapons;

    public Weapon GetWeapon(int index) {
        if (index < 0)
            return null;

        return weapons[index];
    }

    public int GetRandomWeaponIndex() {
        return Random.Range(0, weapons.Length);
    }

    public Tuple<int, Weapon> GetRandomWeapon() {
        var index = GetRandomWeaponIndex();
        return new Tuple<int, Weapon>(index, GetWeapon(index));
    }
}