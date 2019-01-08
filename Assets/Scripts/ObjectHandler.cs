using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class ObjectHandler : MonoBehaviour {
    public MyFloatEvent rotateFunc = new MyFloatEvent();

    public void onRotateX(float rotX) {
        rotateFunc.AddListener((float arg0) => { RotateX(rotX); });
    }

    public void onRotateY(float rotY) {
        rotateFunc.AddListener((float arg0) => { RotateY(rotY); });
    }

    public void onRotateZ(float rotZ) {
        rotateFunc.AddListener((float arg0) => { RotateZ(rotZ); });
    }

    private void RotateX(float rotX) {
        this.transform.Rotate(rotX * Time.deltaTime, 0.0f, 0.0f);
    }

    private void RotateY(float rotY) {
        this.transform.Rotate(0.0f, rotY * Time.deltaTime, 0.0f);
    }

    private void RotateZ(float rotZ) {
        this.transform.Rotate(0.0f, 0.0f, rotZ * Time.deltaTime);
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (this.rotateFunc != null) {
            rotateFunc.Invoke(0.0f);
        }
    }
}

[System.Serializable]
public class MyFloatEvent : UnityEvent<float> {
}
