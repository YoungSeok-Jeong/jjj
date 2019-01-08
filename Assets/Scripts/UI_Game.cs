using System;
using UnityEngine;
using UnityEngine.UI;

namespace Entity {
    public class UI_Game {
        public static GameObject GetButton(string name) {
            return GameObject.Find("ui").transform.Find("Canvas").transform.Find(name).gameObject;
        }

        public static void SetButtonText(string name, string text) {
            GameObject button = GameObject.Find("ui").transform.Find("Canvas").transform.Find(name).gameObject;
            GameObject buttonChild = button.transform.Find("Text").gameObject;
            Text textComp = buttonChild.GetComponent<Text>();
            textComp.text = text;

        }
        public static void SetActive(string name, bool active) {
            GameObject.Find("ui").transform.Find("Canvas").transform.Find(name).gameObject.SetActive(active);
        }
    }
}
