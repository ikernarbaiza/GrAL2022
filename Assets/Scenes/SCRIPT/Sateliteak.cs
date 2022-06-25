using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sateliteak : MonoBehaviour
{

    private TextMeshProUGUI kontTestua;

    public GameObject satelite;
    private int sateliteKont = 0;
    private int sateliteKont2 = 0;

    //TRUE BIHURTU INFINITU SATELITE GEHITZEKO//
    private bool infinitu = false;

    private Vector3 q = new Vector3(0f, 37947.73745727695f, 0f);
    private Vector3 v = new Vector3(3.297676220718193f, 0f, 0.8244190551795483f);



    void Start(){

        kontTestua = GameObject.Find("Satelite kopurua").GetComponent<TextMeshProUGUI>();


    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || OVRInput.GetDown(OVRInput.Button.Four)){

            //Lehen 4 sateliteak eszenara gehitzeko
            if (sateliteKont < 4){
                sateliteKont += 1;
                sateliteaJaurtitu(sateliteKont);

            //Satelite gehiago gehitzeko (infinitu == true izan behar du!!)
            }else if (sateliteKont >= 4 && infinitu){
                sateliteKont += 1;
                sateliteKont2 = sateliteKont % 5;
                sateliteaJaurtitu(sateliteKont2);

            }
        }
        kontTestua.text = "Satelite kopurua: " + (sateliteKont+1);
        
    }

    private void sateliteaJaurtitu(int sKont){

        //Dagokion satelitea jaurtitzeko kontrola
        switch(sKont-1){
            case 0:
                q = new Vector3(7170.822f, 0f, 0f);
                v = new Vector3(0f, -1.111575722555183f, 7.376070926571781f);
                GameObject sateliteBerria3 = Instantiate(satelite);
                break;
            case 1:
                q = new Vector3(42149.1336f, 0f, 0f);
                v = new Vector3(0f, 3.075823259987749f, 0.0010736649055318406f);
                GameObject sateliteBerria4 = Instantiate(satelite);
                break;
            case 2:
                q = new Vector3(11959.886901183693f, -16289.448826603336f, -5963.757695165331f);
                v = new Vector3(4.724300951633136f, -1.1099935305609756f, -0.3847854410416176f);
                GameObject sateliteBerria5 = Instantiate(satelite);
                break;
            case 3:
                q = new Vector3(10000f, 40000f, -5000f);
                v = new Vector3(-1.500f, 1.000f, -0.100f);
                GameObject sateliteBerria6 = Instantiate(satelite);
                break;
        }       

    }


    public Vector3 getQ(){
        return q;
    }

    public Vector3 getV(){
        return v;
    }

    public int getSateliteKont(){
        return sateliteKont;
    }
}
