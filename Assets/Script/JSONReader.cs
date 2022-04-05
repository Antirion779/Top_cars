using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        using (StreamReader r = new StreamReader("Assets/test.json"))
        {
            string json = r.ReadToEnd();
            MyStruct test = JsonConvert.DeserializeObject<MyStruct>(json);

            Debug.Log($"test = {{speed = {test.aiOutput.speed}; turn = {test.aiOutput.turn}}}");
            Debug.Log(test.feedback.xd);
        }
    }

    struct MyStruct
    {
        public AiInput aiInput;
        public AiOutput aiOutput;
        public Feedback feedback;
    }

    struct AiOutput
    {
        public float speed;
        public float turn;
    }

    struct AiInput
    {
        public List<RayCastResult> rayCastResults;
    }

    struct Feedback
    {
        public string xd;
    }

    struct RayCastResult
    {
        public string terrainName;
        public float distance;
    }
}
