using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPress : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            Debug.Log("Player view enabled");
        }
        if (Input.GetKeyDown("2"))
        {
            Debug.Log("Bot view enabled");
        }
    }
}
