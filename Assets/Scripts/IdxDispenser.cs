using UnityEngine;
using System.Collections;

public class IdxDispenser : Singleton<IdxDispenser> {
    private static int m_idx;

    public void Awake() {
        m_idx = 0;
    }

    public static int GetIdx() {
        return ++m_idx;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
