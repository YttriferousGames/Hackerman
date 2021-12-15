using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class Cutscene : MonoBehaviour {
    private VideoPlayer vp;

    private void ChangeScene(VideoPlayer v) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Start is called before the first frame update
    private void Start() {
        vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += ChangeScene;
    }
}
