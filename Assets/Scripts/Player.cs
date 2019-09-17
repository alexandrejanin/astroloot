using System;
using BeardedManStudios.Forge.Networking.Generated;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class Player : PlayerBehavior {
    [SerializeField, MinValue(0)]
    private float runSpeed = 5f, wallSlideSpeed = 2f;

    [SerializeField, MinValue(0)]
    private float accelerationTimeGrounded = .1f, accelerationTimeAirborne = .2f;

    [SerializeField, MinValue(0)]
    private float minJumpHeight = 1f, maxJumpHeight = 5f, timeToApex = .4f;

    [SerializeField]
    private Vector2 wallJumpTowards, wallJumpNeutral, wallJumpAway;

    [SerializeField, MinValue(0)]
    private float wallStickTime = .25f;

    private float timeToWallUnstick;

    private Vector2 directionalInput;

    private Vector3 velocity;
    public Vector3 Velocity => velocity;

    private float minJumpVelocity, maxJumpVelocity;
    private float gravity;

    private float speedSmoothing;

    private bool wallSliding;
    private float wallDirX;

    private CharacterController2D controller;
    public CharacterController2D Controller => controller;

    private SpriteRenderer graphics;

    private void Awake() {
        controller = GetComponent<CharacterController2D>();
        graphics = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update() {
        UpdateMovement();
//        if (networkObject == null) return;
//        if (networkObject.IsOwner) {
//            transform.position = networkObject.position;
//            transform.rotation = networkObject.rotation;
//            return;
//        }
//
//        UpdateMovement();
//        networkObject.position = transform.position;
//        networkObject.rotation = transform.rotation;
    }

    private void UpdateMovement() {
        UpdateGravity();
        UpdateJumpForces();

        UpdateVelocity();
        UpdateWallSliding();

        UpdateGraphics();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.Collisions.above || controller.Collisions.below) {
            if (controller.Collisions.slidingDownSlope) {
                velocity.y += controller.Collisions.slopeNormal.y * -gravity * Time.deltaTime;
            } else {
                velocity.y = 0;
            }
        }
    }

    private void UpdateGravity() {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
    }

    private void UpdateJumpForces() {
        maxJumpVelocity = Mathf.Abs(gravity) * timeToApex;
        minJumpVelocity = Mathf.Sqrt(2 * Math.Abs(gravity) * minJumpHeight);
    }

    private void UpdateVelocity() {
        velocity.y += gravity * Time.deltaTime;
        velocity.x = Mathf.SmoothDamp(
            velocity.x,
            directionalInput.x * runSpeed,
            ref speedSmoothing,
            controller.Collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne
        );
    }

    private void UpdateWallSliding() {
        wallDirX = controller.Collisions.left ? -1 : 1;
        wallSliding = false;

        if ((controller.Collisions.left || controller.Collisions.right) && !controller.Collisions.below && velocity.y < 0) {
            wallSliding = true;

            if (directionalInput.x == wallDirX || timeToWallUnstick > 0) {
                if (velocity.y < -wallSlideSpeed)
                    velocity.y = -wallSlideSpeed;

                velocity.x = 0;
                speedSmoothing = 0;
                if (directionalInput.x == wallDirX) {
                    timeToWallUnstick = wallStickTime;
                } else {
                    timeToWallUnstick -= Time.deltaTime;
                }
            }
        } else {
            timeToWallUnstick = wallStickTime;
        }
    }

    private void UpdateGraphics() {
        if (directionalInput.x == 0)
            return;

        var rotation = graphics.transform.localRotation;
        rotation.y = directionalInput.x < 0 ? 180 : 0;
        graphics.transform.localRotation = rotation;
    }

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }

    public void OnJumpInputDown() {
        if (wallSliding) {
            if (wallDirX == directionalInput.x) {
                velocity.x = -wallDirX * wallJumpTowards.x;
                velocity.y = wallJumpTowards.y;
            } else if (directionalInput.x == 0) {
                velocity.x = -wallDirX * wallJumpNeutral.x;
                velocity.y = wallJumpNeutral.y;
            } else {
                velocity.x = -wallDirX * wallJumpAway.x;
                velocity.y = wallJumpAway.y;
            }
        }

        if (controller.Collisions.below)
            velocity.y = maxJumpVelocity;
    }

    public void OnJumpInputUp() {
        if (velocity.y > minJumpVelocity)
            velocity.y = minJumpVelocity;
    }
}