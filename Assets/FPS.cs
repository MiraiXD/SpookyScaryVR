using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    UnityEngine.UI.Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<UnityEngine.UI.Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "FPS: " + ((int)(1f / Time.deltaTime)).ToString();
    }
}
