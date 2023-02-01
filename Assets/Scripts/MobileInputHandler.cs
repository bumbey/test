using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("Input/On-Screen Controller")]
public class MobileInputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    public float movementRange; // Maximum distance from center that joystick can move
    public float holdTime = 0.2f; // Time until hold is registered
    public float flickTime = 0.2f; // Time after moving pointer until flick is registered
    public float minSwipeDistance = 30f; // Minimum distance to be moved until swipe is registered
    public RectTransform originTransform;
    public RectTransform handleTransform;
    public LineRenderer lineRenderer;
    public JoystickRenderer joystick;

    private bool tapping; // Check if finger is down and stationary
    private bool swiping; // Check if finger has moved since touching down
    private float t_tap; // Timer for holding
    private float t_flick; // Timer for flicking
    private float swipeMagnitude; // Cached magnitude of swipe

    private int touchCount;
    private int pointerId;

    private Vector2 pointerDownPos; // Local point of pointer on press
    private Vector2 pointerPos; // Current local point of pointer

    Camera cam;

    private Image originImage;
    private Image handleImage;
    private RectTransform rect;

    private void Awake() {
        cam = Camera.main; // Cache camera

        originImage = originTransform.GetComponent<Image>();
        handleImage = handleTransform.GetComponent<Image>();

        rect = transform.parent.GetComponentInParent<RectTransform>();
    }

    private void Start() {
        SetImageEnabled(false);
    }

    private void Update() {
        if (tapping) {
            t_tap += Time.deltaTime;
            if (t_tap > holdTime) {
                tapping = false;
                Debug.Log("Hold");
            }
        }
        if (swiping) { t_flick += Time.deltaTime; }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));
        touchCount++;
        if (touchCount <= 1) {
            pointerId = eventData.pointerId;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, cam, out pointerDownPos); // Get pointerDownPos

            SetImageEnabled(true);
            TapDown();
            JoystickDown();
            SwipeDown();
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));
        if (pointerId == eventData.pointerId) { // If first touch
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, cam, out pointerPos); // Get pointerPos

            TapDrag();
            JoystickDrag();
            SwipeDrag();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (pointerId == eventData.pointerId) { // Checks if pointerID is first touch
            SetImageEnabled(false);
            TapUp();
            JoystickUp();
            SwipeUp();
        }
        touchCount--;
    }

    private void JoystickDown() {
        originTransform.anchoredPosition = pointerDownPos;
        handleTransform.anchoredPosition = Vector2.zero;
        lineRenderer.SetPosition(0, pointerDownPos);
        SetEndpointPosition(pointerDownPos);
    }

    private void JoystickDrag() {
        var delta = Vector2.ClampMagnitude(pointerPos - originTransform.anchoredPosition, movementRange);

        SetEndpointPosition(delta);

        var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
        Debug.Log(newPos);
    }

    private void JoystickUp() {
        handleTransform.anchoredPosition = Vector2.zero;
    }

    public void SetEndpointPosition(Vector2 delta) {
        handleTransform.anchoredPosition = delta;
        lineRenderer.SetPosition(1, handleTransform.anchoredPosition);
        handleTransform.up = handleTransform.anchoredPosition - originTransform.anchoredPosition;
    }

    private void TapDown() {
        tapping = true;
        t_tap = 0;
    }

    private void TapDrag() {
        tapping = false;
    }

    private void TapUp() {
        if (t_tap <= holdTime && tapping) {
            Debug.Log("Tapped");
        }
        tapping = false;
    }

    private void SwipeDown() {
        swiping = true;
        swipeMagnitude = 0;
        t_flick = 0;
    }

    private void SwipeDrag() {
        swipeMagnitude = (pointerPos - pointerDownPos).magnitude;
    }

    private void SwipeUp() {
        if (swiping && t_flick <= flickTime && swipeMagnitude >= minSwipeDistance) {
            Debug.Log("Flick");
        }
        swiping = false;
    }

    private void SetImageEnabled(bool b) {
        originImage.enabled = b;
        handleImage.enabled = b;
    }
}