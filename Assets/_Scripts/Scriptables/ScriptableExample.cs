using UnityEngine;

namespace rene_roid { 
    [CreateAssetMenu(fileName = "ScriptableExample", menuName = "Default Project/ScriptableExample", order = 0)]
    public class ScriptableExample : ScriptableObject {
        public string exampleString;
        public int exampleInt;
        public float exampleFloat;
        public bool exampleBool;
        public Vector3 exampleVector3;
        public Color exampleColor;
        public GameObject exampleGameObject;
        public ScriptableExample exampleScriptable;
        public ScriptableExample[] exampleScriptableArray;
        public GameObject[] exampleGameObjectArray;
        public Color[] exampleColorArray;
        public Vector3[] exampleVector3Array;
        public int[] exampleIntArray;
        public float[] exampleFloatArray;
        public bool[] exampleBoolArray;
        public string[] exampleStringArray;
    }
}
