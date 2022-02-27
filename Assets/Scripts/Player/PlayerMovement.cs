using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Player movement and interaction</summary>
[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour {
    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool noclip = false;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float playerSpeed = 8.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    private float gravityValue;
    [SerializeField]
    private float sensitivity = 10f;
    private PlayerInput inp;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private static PlayerMovement _instance = null;

    public static PlayerMovement instance { get => _instance; }

    private void Awake() {
        if (_instance == null)
            _instance = this;
        controller = GetComponent<CharacterController>();
        gravityValue = Physics.gravity.y;
        if (cam == null) {
            cam = Camera.main;
        }
        inp = GetComponent<PlayerInput>();
        moveAction = inp.actions["Move"];
        lookAction = inp.actions["Look"];
        jumpAction = inp.actions["Jump"];
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private static float NormAngle(float angle) {
        return angle - 360 * Mathf.Floor(angle / 360);
    }

    private void Update() {
        if (Keyboard.current.escapeKey.isPressed) {
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
            Vector2 m = lookAction.ReadValue<Vector2>();
            mX = m.x;
            mY = m.y;
        }
        float angle = transform.eulerAngles.y;
        float camAngle = cam.transform.localEulerAngles.x;
        angle += mX * sensitivity;
        camAngle -= mY * sensitivity;
        camAngle = NormAngle(180 - camAngle);
        camAngle = -Mathf.Clamp(camAngle, 90, 270) + 180;
        transform.eulerAngles = new Vector3(0f, angle, 0f);
        cam.transform.localEulerAngles = new Vector3(camAngle, 0, 0f);

        RaycastHit h = new RaycastHit();
        bool handleInput = true;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out h, 1f, 1 << 6,
                            QueryTriggerInteraction.Collide)) {
            SysInterface s = h.collider.gameObject.GetComponent<SysInterface>();
            handleInput = s == null;
            if (!handleInput)
                s.HandleInput(true);
        }

        Vector2 moveInp = moveAction.ReadValue<Vector2>();

        Vector3 move = (transform.right * moveInp.x + transform.forward * moveInp.y) * playerSpeed;
        if (noclip) {
            if (handleInput) {
                transform.position += cam.transform.rotation *
                                      (Vector3.right * moveInp.x + Vector3.forward * moveInp.y) *
                                      playerSpeed * Time.deltaTime;
            }
        } else {
            bool groundedPlayer = controller.isGrounded;
            if (controller.isGrounded) {
                if (playerVelocity.y < 0) {
                    playerVelocity.y = 0f;
                }
                if (handleInput && jumpAction.IsPressed()) {
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                }
            }

            playerVelocity.y += gravityValue * Time.deltaTime;

            if (!handleInput)
                move = Vector3.zero;
            controller.Move((move + playerVelocity) * Time.deltaTime);
        }
    }
}
