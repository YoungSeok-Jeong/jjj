using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour {
    [SerializeField]
    private List<string> m_has = new List<string>();

    public void Add(string name) {
        this.m_has.Add(name);
    }

    public string SelectCard(int idx) {
        if (idx < this.m_has.Count) {
            return this.m_has[idx];
        } else {
            return null;
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
