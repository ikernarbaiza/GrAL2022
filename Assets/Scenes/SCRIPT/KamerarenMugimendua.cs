using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamerarenMugimendua : MonoBehaviour
{
    
    public float abiadura = 0.005f; 
    public float sens = 0.5f;

    private bool aTekla = false;
    private bool sTekla = false;
    private bool dTekla = false;
    private bool wTekla = false;


    private KamerenKontrola kamerenKontrola;

    // Start is called before the first frame update
    void Start () {
        kamerenKontrola = GameObject.FindObjectOfType<KamerenKontrola> ();
    }


    // Update is called once per frame
    void Update()
    {
        // KAMERA HAU ERABILTZEN ARI GARELA JAKITEKO
        if (kamerenKontrola.getLag() == 0){
            // MUGIMENDUA TEKLATU BIDEZ HASI
            if(Input.GetKeyDown(KeyCode.A)){

                aTekla = true;

            }
            if(Input.GetKeyUp(KeyCode.A)){

                aTekla = false;

            }
            if(Input.GetKeyDown(KeyCode.S)){

                sTekla = true;

            }
            if(Input.GetKeyUp(KeyCode.S)){

                sTekla = false;

            }
            if(Input.GetKeyDown(KeyCode.D)){

                dTekla = true;

            }
            if(Input.GetKeyUp(KeyCode.D)){

                dTekla = false;

            }
            if(Input.GetKeyDown(KeyCode.W)){

                wTekla = true;

            }
            if(Input.GetKeyUp(KeyCode.W)){

                wTekla = false;

            }
            if(aTekla){

                transform.position = transform.position - transform.right*abiadura;

            }
            if(dTekla){

                transform.position = transform.position + transform.right*abiadura;

            }
            if(sTekla){

                transform.position = transform.position - transform.forward*abiadura;

            }  
            if(wTekla){

                transform.position = transform.position + transform.forward*abiadura;

            }
            if(Input.GetKeyDown(KeyCode.KeypadPeriod) || OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick)){

                transform.LookAt(Vector3.zero);

            }
            //MUGIMENDUA TEKLATU BIDEZ BUKATU

            // KAMERAREN BIRAKETA SAGUAREN BITARTEZ HASI
            transform.Rotate(Vector3.right *  Input.GetAxis("Mouse Y") * sens);
            transform.Rotate(Vector3.up *  Input.GetAxis("Mouse X") * sens);
            //KAMERAREN BIRAKETA SAGUAREN BITARTEZ BUKATU

            //KAMERAREN MUGIMENDUA OCULUS CONTROLLERAREN BITARTEZ HASI
            if(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) != Vector2.zero){
                Vector2 sarrera = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
                transform.position = transform.position + (transform.forward*(float)(sarrera[1]*0.05) + transform.right*(float)(sarrera[0]*0.05));
            }
            //KAMERAREN MUGIMENDUA OCULUS CONTROLLERAREN BITARTEZ BUKATU
        }
    } 


}
