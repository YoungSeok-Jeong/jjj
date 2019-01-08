using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{

    protected static T m_instance;

    public static T Instance() {
        return m_instance;
    }

    private void Awake() {
        if (m_instance == null) {
            m_instance = (T)this;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
