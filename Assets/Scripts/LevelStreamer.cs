// Controls streaming of level data in background (no loading screens!)

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class LevelStreamer : MonoBehaviour {
    [SerializeField]
    private PlayerMovement player;
    private CharacterController cc;
    // First buildId of a level scene
    private int firstLevelId = 3;

    private IEnumerable<Scene> scenesIter {
        get {
            for (int n = 0; n < SceneManager.sceneCount; n++) {
                Scene s = SceneManager.GetSceneAt(n);
                if (s.buildIndex >= firstLevelId) {
                    yield return SceneManager.GetSceneAt(n);
                }
            }
        }
    }

    private static Bounds GetSceneBounds(Scene s) {
        GameObject[] root = s.GetRootGameObjects();
        GameObject room = root.Single(o => o.tag == "Level");
        BoxCollider col = room.GetComponent<BoxCollider>(); return col.bounds;
    }

    private void Awake() {
        cc = player.GetComponent<CharacterController>();
        if (SceneManager.sceneCount == 1) {
            SceneManager.LoadScene("1", LoadSceneMode.Additive);
        }
    }

    // Update is called once per frame
    private void UpdateStreaming() {
        Scene[] scenes = scenesIter.OrderBy(s => s.buildIndex).ToArray();
        Scene? minLevelLoaded = null; Scene? minLevelInside = null; Scene? maxLevelLoaded = null;
            Scene? maxLevelInside = null; foreach (Scene s in scenes) {
                if (minLevelLoaded is Scene minVal) {
                    if (s.buildIndex < minVal.buildIndex) {
                        minLevelLoaded = s;
                    }
                } else {
                    minLevelLoaded = s;
                }

                if (maxLevelLoaded is Scene maxVal) {
                    if (s.buildIndex > maxVal.buildIndex) {
                        maxLevelLoaded = s;
                    }
                } else {
                    maxLevelLoaded = s;
                }

                if (s.isLoaded && cc.bounds.Intersects(GetSceneBounds(s))) {
                    if (minLevelInside is Scene minIVal) {
                        if (s.buildIndex < minIVal.buildIndex) {
                            minLevelInside = s;
                        }
                    } else {
                        minLevelInside = s;
                    }

                    if (maxLevelInside is Scene maxIVal) {
                        if (s.buildIndex > maxIVal.buildIndex) {
                            maxLevelInside = s;
                        }
                    } else {
                        maxLevelInside = s;
                    }
                }
            }
        // UnityEngine.Debug.Log(minLevelLoaded.Value.name + "," + minLevelInside.Value.name + "," +
        // maxLevelLoaded.Value.name + "," + maxLevelInside.Value.name);
        if (maxLevelInside is Scene maxLI && maxLI.buildIndex == maxLevelLoaded.Value.buildIndex &&
            maxLI.buildIndex + 1 < SceneManager.sceneCountInBuildSettings) {
            int s = maxLI.buildIndex + 1;
            UnityEngine.Debug.Log("Loading scene ID " + s.ToString());
            SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive);
        } if (minLevelInside is Scene minLI &&
              minLI.buildIndex == minLevelLoaded.Value.buildIndex &&
              minLI.buildIndex > firstLevelId) {
            int s = minLI.buildIndex - 1;
            UnityEngine.Debug.Log("Loading scene ID " + s.ToString());
            SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive);
        } foreach (Scene s in scenes) {
            if (s.isLoaded && (s.buildIndex < minLevelInside.Value.buildIndex - 1 ||
                               s.buildIndex > maxLevelInside.Value.buildIndex + 1)) {
                UnityEngine.Debug.Log("Unloading scene ID " + s.buildIndex.ToString());
                SceneManager.UnloadSceneAsync(s);
            }
        }
    }

    private void Update() {
        if (Time.frameCount % 30 == 0) {
            UpdateStreaming();
        }
    }
}
