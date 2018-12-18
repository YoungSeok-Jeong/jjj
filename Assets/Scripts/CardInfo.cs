using System;
namespace Entity {
    [System.Serializable]
    public class CardInfo {
        public string name;
        public string type;
        public int value;
        public int aaaa;
        public TargetType target;
    }

    public enum TargetType {
        Single
        , All
        , Random
    }
}
