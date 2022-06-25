using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSkalkulua : MonoBehaviour
{

    private TextMeshProUGUI fpsTestua;
    private TextMeshProUGUI erlojuTestua;

    private int count = 0;
    private float deltaTime = 0f;
    public float showTime = 1f;

    private float erlojua = 0f;

    // Start is called before the first frame update
    void Start()
    {
        fpsTestua = GameObject.Find("FPS").GetComponent<TextMeshProUGUI>();
        erlojuTestua = GameObject.Find("Exekutatzen").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //FPS eta ms kalkulua egin eta bistaratu
        count++;
        deltaTime += Time.deltaTime;
        if (deltaTime >= showTime) {
            float fps = count / deltaTime;
            float milliSecond = deltaTime * 1000 / count;
            string strFpsInfo = string.Format("Fotograma bakoitzaren egungo erreprodukzio-tartea: {0: 0.0} ms ({1: 0.} FPS)", milliSecond, fps);
            fpsTestua.text = strFpsInfo;
            count = 0;
            deltaTime = 0f;


        }

        //Exekutatzen daraman denbora bistaratu
        erlojua += Time.deltaTime;
        float min = Mathf.FloorToInt(erlojua / 60);  
        float seg = Mathf.FloorToInt(erlojua % 60);
        erlojuTestua.text = string.Format("{0:00}:{1:00} exekutatzen", min, seg);
    }
}
