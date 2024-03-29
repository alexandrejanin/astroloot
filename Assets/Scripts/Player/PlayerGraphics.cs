﻿using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerGraphics : MonoBehaviour {
    [SerializeField] private Transform body, arm, head;

    private Player player;
    private PlayerCombat playerCombat;

    private void Awake() {
        player = GetComponent<Player>();
        playerCombat = GetComponent<PlayerCombat>();

        player.onDeath += () => body.gameObject.SetActive(false);
        player.onRespawn += () => body.gameObject.SetActive(true);
    }

    private void Update() {
        var facingLeft = playerCombat.TargetPosition.x < playerCombat.transform.position.x;

        body.localEulerAngles = new Vector3(
            body.localEulerAngles.x,
            facingLeft ? 180 : 0,
            body.localEulerAngles.z
        );

        var armAngle = Mathf.Rad2Deg * Mathf.Atan(
                           -(arm.position.y - playerCombat.TargetPosition.y) /
                           Mathf.Abs(arm.position.x - playerCombat.TargetPosition.x)
                       );

        var armEulerAngles = new Vector3(
            arm.localEulerAngles.x,
            arm.localEulerAngles.y,
            armAngle
        );

        if (playerCombat.Weapon)
            armEulerAngles += playerCombat.Weapon.ArmAngle;

        arm.localEulerAngles = armEulerAngles;

        var headAngle = armAngle / 2;
        head.localEulerAngles = new Vector3(
            head.localEulerAngles.x,
            head.localEulerAngles.y,
            headAngle
        );
    }
}