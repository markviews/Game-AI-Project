using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPress : MonoBehaviour
{
    [SerializeField]
    private GameObject Light1;
    [SerializeField]
    private GameObject Light2;
    
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            Light1.SetActive(true);
            Light2.SetActive(false);
            // if (Light1.activeSelf){
            //     Debug.Log("Player view disabled");
            //     Light1.SetActive(false);
            // }
            // else{
            //     Debug.Log("Player view enabled");
            //     Light1.SetActive(true);
            // }
        }

        if (Input.GetKeyDown("2"))
        {
            Light1.SetActive(false);
            Light2.SetActive(true);
            // if (Light2.activeSelf){
            //     Debug.Log("Bot view disabled");
            //     Light2.SetActive(false);
            // }
            // else{
            //     Debug.Log("Bot view enabled");
            //     Light2.SetActive(true);
            // }
        }

        if (Input.GetKeyDown("3"))
        {
            Light1.SetActive(false);
            Light2.SetActive(false);
        }
    }
}
