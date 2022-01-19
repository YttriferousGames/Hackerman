// Transitions to next scene after video is complete

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class Cutscene : MonoBehaviour {
    private VideoPlayer vp;

    private void ChangeScene(VideoPlayer v) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Awake() {
        vp = GetComponent<VideoPlayer>();
    }

    private void Start() {
        vp.loopPointReached += ChangeScene;
    }
}
