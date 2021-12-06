using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private CharacterController controller;
    private Vector3 playerVelocity;
    public Camera cam;
    public float playerSpeed = 8.0f;
    public float jumpHeight = 1.0f;
    private float gravityValue;

    public float sensitivity = 10f;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        gravityValue = Physics.gravity.y;
        if (cam == null) {
            cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update() {
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
        // camAngle = Mathf.Clamp(camAngle, -90, 90);
        transform.eulerAngles = new Vector3(0f, angle, 0f);
        cam.transform.localEulerAngles = new Vector3(camAngle, 0, 0f);

        bool groundedPlayer = controller.isGrounded;
        if (controller.isGrounded) {
            if (playerVelocity.y < 0) {
                playerVelocity.y = 0f;
            }
            if (Input.GetButtonDown("Jump")) {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        Vector3 move = (transform.right * Input.GetAxis("Horizontal") +
                        transform.forward * Input.GetAxis("Vertical")) *
                       playerSpeed;
        controller.Move((move + playerVelocity) * Time.deltaTime);
    }
}
