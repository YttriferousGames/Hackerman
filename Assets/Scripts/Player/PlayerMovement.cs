// Player movement and interaction

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    private CharacterController controller;
    private Vector3 playerVelocity;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float playerSpeed = 8.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    private float gravityValue;
    [SerializeField]
    private float sensitivity = 10f;

    private void Awake() {
        controller = GetComponent<CharacterController>();
        gravityValue = Physics.gravity.y;
        if (cam == null) {
            cam = Camera.main;
        }
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private static float NormAngle(float angle) {
        return angle - 360 * Mathf.Floor(angle / 360);
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        float mX = 0f;
        float mY = 0f;
        if (Cursor.lockState != CursorLockMode.Locked && Cursor.visible) {
            Cursor.lockState = CursorLockMode.Locked;
        } else if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible) {
            Cursor.visible = false;
        } else {
            mX = Input.GetAxis("Mouse X");
            mY = Input.GetAxis("Mouse Y");
        }
        float angle = transform.eulerAngles.y;
        float camAngle = cam.transform.localEulerAngles.x;
        angle += mX * sensitivity;
        camAngle -= mY * sensitivity;
        camAngle = NormAngle(180 - camAngle);
        camAngle = -Mathf.Clamp(camAngle, 90, 270) + 180;
        transform.eulerAngles = new Vector3(0f, angle, 0f);
        cam.transform.localEulerAngles = new Vector3(camAngle, 0, 0f);

        bool handleInput = true;
        RaycastHit h = new RaycastHit();
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out h, 1f, 1 << 6,
                            QueryTriggerInteraction.Collide)) {
            SysInterface s = h.collider.gameObject.GetComponent<SysInterface>();
            if (s != null) {
                s.HandleInput();
                handleInput = false;
            }
        }

        bool groundedPlayer = controller.isGrounded;
        if (controller.isGrounded) {
            if (playerVelocity.y < 0) {
                playerVelocity.y = 0f;
            }
            if (handleInput && Input.GetButtonDown("Jump")) {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        Vector3 move = (transform.right * Input.GetAxis("Horizontal") +
                        transform.forward * Input.GetAxis("Vertical")) *
                       playerSpeed;
        if (!handleInput)
            move = Vector3.zero;
        controller.Move((move + playerVelocity) * Time.deltaTime);
    }
}
