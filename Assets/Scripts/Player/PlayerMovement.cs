using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Player movement and interaction</summary>
[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour {
    private CharacterController controller;
    private Vector3 velocity;
    public bool noclip = false;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float noclipSpeed = 8.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    private float gravityValue;
    [SerializeField]
    private float sensitivity = 10f;
    [SerializeField]
    private float friction = 6;
    [SerializeField]
    private float acceleration = 10;
    [SerializeField]
    private float walkSpeed = 5;
    [SerializeField]
    private float airSpeed = 1;
    private PlayerInput inp;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private bool takingInput = false;

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

    // https://adrianb.io/2015/02/14/bunnyhop.html
    private void Accelerate(Vector3 accelDir, float wishspeed, float accel) {
        Vector3 prevVelocity = velocity;
        prevVelocity.y = 0;
        float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir.
        float addspeed = wishspeed - projVel;
        if (addspeed <= 0) return;
        float accelVel = accel * wishspeed * Time.fixedDeltaTime; // Accelerated velocity in direction of movment
        if (accel > addspeed) {
            accelVel = addspeed;
        }

        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        //UnityEngine.Debug.DrawRay(transform.position + Vector3.down, prevVelocity, Color.yellow, 0, false);

        velocity += accelDir * accelVel;
    }

    private void MouseLook(float mX, float mY) {
        float angle = transform.eulerAngles.y;
        float camAngle = cam.transform.localEulerAngles.x;
        angle += mX * sensitivity;
        camAngle -= mY * sensitivity;
        camAngle = NormAngle(180 - camAngle);
        camAngle = -Mathf.Clamp(camAngle, 90, 270) + 180;
        transform.eulerAngles = new Vector3(0f, angle, 0f);
        cam.transform.localEulerAngles = new Vector3(camAngle, 0, 0f);
    }

    private bool InteractLook() {
        RaycastHit h = new RaycastHit();
        bool handleInput = true;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out h, 1f, 1 << 6,
                            QueryTriggerInteraction.Collide)) {
            GameObject go = h.collider.gameObject;
            go.GetComponent<Interactable>().hovering = true;
            SysInterface s = go.GetComponent<SysInterface>();
            handleInput = s == null;
            if (!handleInput) {
                s.HandleInput(true);
            }
        }

        return handleInput;
    }

    private static bool InFrame(Vector2 c) {
#if UNITY_EDITOR
        Vector2 s = Handles.GetMainGameViewSize();
#else
        Vector2 s = new Vector2(Screen.width, Screen.height);
#endif
        return !(c.x <= 0 || c.y <= 0 || c.x >= s.x - 1 || c.y >= s.y - 1);
    }

    // TODO split into functions
    private void Update() {
        if (Keyboard.current.escapeKey.isPressed) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            takingInput = false;
        }
        Vector2 pos = Mouse.current.position.ReadValue();
        if (InFrame(pos)) {
            if (takingInput && Cursor.lockState == CursorLockMode.Locked && !Cursor.visible) {
                float mX, mY;
                Vector2 m = lookAction.ReadValue<Vector2>();
                mX = m.x;
                mY = m.y;
                MouseLook(mX, mY);
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (!takingInput && Mouse.current.leftButton.isPressed) {
                    takingInput = true;
                }
            }
        }
        bool handleInput = InteractLook();
        Vector2 moveInp = moveAction.ReadValue<Vector2>();

        if (noclip) {
            if (handleInput) {
                transform.position += cam.transform.rotation *
                                      (Vector3.right * moveInp.x + Vector3.forward * moveInp.y) *
                                      noclipSpeed * Time.deltaTime;
            }
        } else {
            Vector3 accelDir;
            if (!handleInput || Mathf.Abs(moveInp.x) + Mathf.Abs(moveInp.y) <= 0) {
                accelDir = Vector3.zero;
            } else {
                accelDir = (transform.right * moveInp.x + transform.forward * moveInp.y).normalized;
            }
            if (controller.isGrounded) {
                if (velocity.y < 0) velocity.y = 0;
                float speed = velocity.magnitude;
                if (speed != 0) {
                    float drop = speed * friction * Time.fixedDeltaTime;
                    velocity *= Mathf.Max(1 - drop / speed, 0);
                }
                Accelerate(accelDir, walkSpeed, acceleration);
                if (handleInput && jumpAction.IsPressed()) {
                    velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                }
            } else {
                Accelerate(accelDir, airSpeed, acceleration);
            }
            velocity += Vector3.up * gravityValue * Time.fixedDeltaTime;
            controller.Move(velocity * Time.fixedDeltaTime);
        }
    }
}
