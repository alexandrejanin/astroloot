using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : RaycastController {
    [SerializeField] protected LayerMask collisionMask;

    [SerializeField] private float maxSlopeAngle = 50;

    private Vector2 moveAmountOld;

    private Vector2 playerInput;

    private CollisionInfo collisions;
    public CollisionInfo Collisions => collisions;

    protected override void Awake() {
        base.Awake();
        collisions.facingDirection = 1;
    }

    // for player
    public void Move(Vector2 moveAmount, Vector2 input) => Move(moveAmount, input, false);

    // for platforms
    public void Move(Vector2 moveAmount, bool standingOnPlatform) => Move(moveAmount, Vector2.zero, standingOnPlatform);

    private void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform) {
        UpdateRaycastOrigins();
        collisions.Reset();
        moveAmountOld = moveAmount;
        playerInput = input;

        if (moveAmount.y < 0)
            DescendSlope(ref moveAmount);

        if (moveAmount.x != 0)
            collisions.facingDirection = (int) Mathf.Sign(moveAmount.x);

        HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
            VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);

        if (standingOnPlatform)
            collisions.below = true;
    }

    private void HorizontalCollisions(ref Vector2 moveAmount) {
        var xDirection = collisions.facingDirection;
        var rayLength = skinWidth + Mathf.Max(Mathf.Abs(moveAmount.x), skinWidth);

        for (var i = 0; i < horizontalRayCount; i++) {
            var rayOrigin = xDirection == -1
                ? raycastOrigins.bottomLeft
                : raycastOrigins.bottomRight;

            rayOrigin += horizontalRaySpacing * i * Vector2.up;

            var hit = Physics2D.Raycast(rayOrigin, xDirection * Vector2.right, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, rayLength * xDirection * Vector2.right, Color.red);

            if (hit) {
                if (hit.distance == 0)
                    continue;

                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxSlopeAngle) {
                    if (collisions.descendingSlope) {
                        collisions.descendingSlope = false;
                        moveAmount = moveAmountOld;
                    }

                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * xDirection;
                    }

                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * xDirection;
                }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
                    moveAmount.x = (hit.distance - skinWidth) * xDirection;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope) {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisions.right = xDirection == 1;
                    collisions.left = xDirection == -1;
                }
            }
        }
    }

    private void VerticalCollisions(ref Vector2 moveAmount) {
        var yDirection = Mathf.Sign(moveAmount.y);
        var rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (var i = 0; i < verticalRayCount; i++) {
            var rayOrigin = yDirection == -1
                ? raycastOrigins.bottomLeft
                : raycastOrigins.topLeft;

            rayOrigin += (verticalRaySpacing * i + moveAmount.x) * Vector2.right;

            var hit = Physics2D.Raycast(rayOrigin, yDirection * Vector2.up, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, rayLength * yDirection * Vector2.up, Color.red);

            if (hit) {
                if (hit.collider.CompareTag("OneWayPlatform")) {
                    if (yDirection == 1 || hit.distance == 0 || playerInput.y == -1)
                        continue;
                }

                moveAmount.y = (hit.distance - skinWidth) * yDirection;
                rayLength = hit.distance;

                if (collisions.climbingSlope) {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) *
                                   Mathf.Sign(moveAmount.x);
                }

                collisions.above = yDirection == 1;
                collisions.below = yDirection == -1;
            }
        }

        // Slope switching bug fix
        if (collisions.climbingSlope) {
            var xDirection = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            var rayOrigin = ((xDirection == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
                            Vector2.up * moveAmount.y;
            var hit = Physics2D.Raycast(rayOrigin, Vector2.right * xDirection, rayLength, collisionMask);
            if (hit) {
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle) {
                    moveAmount.x = (hit.distance = skinWidth) * xDirection;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal) {
        var moveDistance = Mathf.Abs(moveAmount.x);

        var climbY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (moveAmount.y < climbY) {
            moveAmount.y = climbY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    private void DescendSlope(ref Vector2 moveAmount) {
        var maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down,
            Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        var maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down,
            Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight) {
            SlideDownSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (collisions.slidingDownSlope)
            return;

        var xDirection = Mathf.Sign(moveAmount.x);
        var rayOrigin = xDirection == -1 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

        var hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);
        if (hit) {
            var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle) {
                if (Mathf.Sign(hit.normal.x) == xDirection) {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x)) {
                        var moveDistance = Mathf.Abs(moveAmount.x);
                        var descentY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descentY;
                        collisions.descendingSlope = true;
                        collisions.slopeAngle = slopeAngle;
                        collisions.slopeNormal = hit.normal;
                    }
                }
            }
        }
    }

    private void SlideDownSlope(RaycastHit2D hit, ref Vector2 moveAmount) {
        if (hit) {
            var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeAngle > maxSlopeAngle) {
                moveAmount.x = hit.normal.x * (Mathf.Abs(moveAmount.y) - hit.distance) /
                               Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
                collisions.slidingDownSlope = true;
                collisions.slopeAngle = slopeAngle;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    public struct CollisionInfo {
        public bool above, below, left, right;
        public bool climbingSlope, descendingSlope, slidingDownSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public int facingDirection;
        public bool fallingThroughPlatform;

        public void Reset() {
            above = below = left = right = climbingSlope = descendingSlope = slidingDownSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}