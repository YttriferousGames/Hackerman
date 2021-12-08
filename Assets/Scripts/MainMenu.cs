using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class MainMenu : MonoBehaviour
{
    private Camera cam;
    public BitText cursor;
    public GameObject play;
    private BitText play_bt;
    public GameObject quit;
    private BitText quit_bt;

    void Start() {
        cam = GetComponent<Camera>();
        play_bt = play.GetComponent<BitText>();
        quit_bt = quit.GetComponent<BitText>();
    }

    // TODO loading screen by loading scene async
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        cursor.col = new Color(0f, 1f, 0f, Mathf.Sin(2f * Time.time) / 2f + 0.5f);
        play_bt.col = Color.green;
        quit_bt.col = Color.green;
        if (Physics.Raycast(ray, out hitInfo, 10f, 1 << 5, QueryTriggerInteraction.Ignore)) {
            if (hitInfo.collider.gameObject == play) {
                play_bt.col = Color.white;
                if (Input.GetMouseButton(0)) SceneManager.LoadScene("SampleScene");
            } else if (hitInfo.collider.gameObject == quit) {
                BitText bt = quit.GetComponent<BitText>();
                quit_bt.col = Color.white;
                if (Input.GetMouseButton(0)) Application.Quit();
            }
        }
        play_bt.UpdateMesh();
        quit_bt.UpdateMesh();
        cursor.UpdateMesh();
    }
}
