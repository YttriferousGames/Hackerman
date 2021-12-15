// Controls buttons on Main Menu and loading to next scene

using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
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

    private void Start() {
        cam = GetComponent<Camera>();
        play_bt = play.GetComponent<BitText>();
        quit_bt = quit.GetComponent<BitText>();
    }

    private void LoadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // TODO loading screen by loading scene async
    private void Update() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        cursor.col = new Color(0f, 1f, 0f, Mathf.Sin(2f * Time.time) / 2f + 0.5f);
        play_bt.col = Color.green;
        quit_bt.col = Color.green;
        if (Physics.Raycast(ray, out hitInfo, 10f, 1 << 5, QueryTriggerInteraction.Collide)) {
            if (hitInfo.collider.gameObject == play) {
                play_bt.col = Color.white;
                if (Input.GetMouseButton(0))
                    LoadScene();
            } else if (hitInfo.collider.gameObject == quit) {
                BitText bt = quit.GetComponent<BitText>();
                quit_bt.col = Color.white;
                if (Input.GetMouseButton(0))
                    Application.Quit();
            }
        }
        play_bt.UpdateMesh();
        quit_bt.UpdateMesh();
        cursor.UpdateMesh();
    }
}
