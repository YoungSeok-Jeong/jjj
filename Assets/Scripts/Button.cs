using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour {
    public float nextSceneDelay = 0.0f;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void NextScene(string sceneName) {
        Debug.Log("next scene!! " + sceneName);
        Initiate.Fade(sceneName, Color.black, 2.0f, nextSceneDelay);
    }
}
