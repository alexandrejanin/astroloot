using Sirenix.OdinInspector;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField]
    private CharacterController2D target;

    [SerializeField]
    private Vector2 focusAreaSize;

    [SerializeField]
    private float yOffset;

    [SerializeField, MinValue(0)]
    private float ySmoothing, lookAheadDistance, xSmoothing;

    private float lookAhead, lookAheadTarget, lookAheadDirection, smoothVelocityX, smoothVelocityY;
    private bool lookAheadStopped;

    private FocusArea focusArea;

    private void Start() {
        focusArea = new FocusArea(target.Collider.bounds, focusAreaSize);
    }

    private void LateUpdate() {
        focusArea.Update(target.Collider.bounds);
        Vector2 focusPosition = focusArea.center + Vector2.up * yOffset;

        if (focusArea.velocity.x != 0) {
            lookAheadDirection = Mathf.Sign(focusArea.velocity.x);
            if (target.PlayerInput.x != 0 && Mathf.Sign(target.PlayerInput.x) == Mathf.Sign(focusArea.velocity.x)) {
                lookAheadTarget = lookAheadDirection * lookAheadDistance;
                lookAheadStopped = false;
            } else if (!lookAheadStopped) {
                lookAheadStopped = true;
                lookAheadTarget = lookAhead + (lookAheadDirection * lookAheadDistance - lookAhead) / 4f;
            }
        }

        lookAhead = Mathf.SmoothDamp(lookAhead, lookAheadTarget, ref smoothVelocityX, xSmoothing);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, ySmoothing);
        focusPosition += Vector2.right * lookAhead;

        transform.position = (Vector3) focusPosition + Vector3.back * 10;
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea {
        public Vector2 center;
        public float left, right, top, bottom;

        public Vector2 velocity;

        public FocusArea(Bounds targetBounds, Vector2 size) {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            center = new Vector2(left + right / 2, top + bottom / 2);

            velocity = Vector2.zero;
        }

        public void Update(Bounds targetBounds) {
            float shiftX = 0;
            if (targetBounds.min.x < left) {
                shiftX = targetBounds.min.x - left;
            } else if (targetBounds.max.x > right) {
                shiftX = targetBounds.max.x - right;
            }

            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom) {
                shiftY = targetBounds.min.y - bottom;
            } else if (targetBounds.max.y > top) {
                shiftY = targetBounds.max.y - top;
            }

            bottom += shiftY;
            top += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}