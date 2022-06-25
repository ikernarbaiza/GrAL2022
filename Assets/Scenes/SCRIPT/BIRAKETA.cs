using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script honen bitartez, Lur planeta bere Y ardatzean biratzea lortuko dut.
public class BIRAKETA : MonoBehaviour
{
    [Tooltip("Biratu: Yes or No")] //biratzeko edo ez aukeratzeko
    public bool biratu;
    public float abiadura = 10f; //hasierako abiadura

    [HideInInspector]
    public float norabidea = 1f;


    // Update is called once per frame
    void Update()
    {
        if (biratu){
            transform.Rotate(Vector3.up, (abiadura * norabidea) * Time.deltaTime);
        }
    }
} 
