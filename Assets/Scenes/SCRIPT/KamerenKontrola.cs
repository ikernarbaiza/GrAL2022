using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamerenKontrola : MonoBehaviour
{

    public GameObject kamera1;

    private Sateliteak satelitea;

    private int kameraKont;
    private int kont;
    private int kamera0ID = 0;
    private int kamera1ID = 1;
    private int lag;
    private bool kokatuta = true;


    private Vector3 kamera0Pos = new Vector3(10, 0, 0);
    private Vector3 kamera0For = new Vector3(-1, 0, 0);

    private Vector3 kamera1Pos = new Vector3(0, 7, 0);

    // Start is called before the first frame update
    void Start () {
        satelitea = GameObject.FindObjectOfType<Sateliteak> ();
    }

    // Update is called once per frame
    void Update () {
        //1. eta 2. kamera moten arteko aldaketak kudeatzeko
        if (Input.GetKeyDown(KeyCode.K) || OVRInput.GetDown(OVRInput.Button.Three)){

            kameraKont = satelitea.getSateliteKont()+3;
            kont = kont + 1;
            lag = kont % kameraKont;

            if (kamera0ID == lag && !kokatuta){
                kamera1.transform.position = kamera0Pos;
                kamera1.transform.forward = kamera0For;
                kokatuta = true;
            }
            
            if (kamera1ID == lag){
                kamera0Pos = kamera1.transform.position;
                kamera0For = kamera1.transform.forward;
                kokatuta = false;
                kamera1.transform.position = kamera1Pos;
                kamera1.transform.LookAt(Vector3.zero);
            }
            if (kont == kameraKont){
                kont = 0;
            }
        }
    }

    public int getKont(){
        return kont;
    }

    public int getLag(){
        return lag;
    }

}

    

