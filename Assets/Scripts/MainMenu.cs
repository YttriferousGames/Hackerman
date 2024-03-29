using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>Controls buttons on Main Menu and loading to next scene</summary>
[RequireComponent(typeof(Camera), typeof(AudioSource))]
public class MainMenu : MonoBehaviour {
    private Camera cam;
    [SerializeField]
    private BitText cursor;
    [SerializeField]
    private GameObject play;
    private BitText play_bt;
    [SerializeField]
    private GameObject quit;
    private BitText quit_bt;
    private AudioSource src;

    private void Awake() {
        cam = GetComponent<Camera>();
        src = GetComponent<AudioSource>();
        play_bt = play.GetComponent<BitText>();
        quit_bt = quit.GetComponent<BitText>();
    }

    private void LoadScene() {
        // WebGL doesn't do this on scene switch
        // and it kinda ignores Stop too
        src.loop = false;
        src.mute = true;
        src.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // TODO loading screen by loading scene async
    private void Update() {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;
        cursor.col = new Color(0f, 1f, 0f, Mathf.Sin(2f * Time.time) / 2f + 0.5f);
        play_bt.col = Color.green;
        quit_bt.col = Color.green;
        if (Physics.Raycast(ray, out hitInfo, 10f, 1 << 5, QueryTriggerInteraction.Collide)) {
            if (hitInfo.collider.gameObject == play) {
                play_bt.col = Color.white;
                if (Mouse.current.leftButton.isPressed) {
                    LoadScene();
                }
            } else if (hitInfo.collider.gameObject == quit) {
#if !UNITY_WEBGL
                BitText bt = quit.GetComponent<BitText>();
                quit_bt.col = Color.white;
                if (Mouse.current.leftButton.isPressed)
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
#endif
            }
        }
        play_bt.UpdateMesh();
        quit_bt.UpdateMesh();
        cursor.UpdateMesh();
    }
}
