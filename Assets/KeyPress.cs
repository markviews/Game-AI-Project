using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This should be called Visualizer at this point but it's too late
public class KeyPress : MonoBehaviour
{
    [SerializeField]
    private GameObject playerLight;
    [SerializeField]
    private GameObject enemyLight;
    [SerializeField]
    private TagBrain playerBrain;
    [SerializeField]
    private TagBrain enemyBrain;

    [SerializeField]
    private SpriteRenderer targetDot;
    [SerializeField] 
    private SpriteRenderer predictionDot;

    //1 = player vision
    //2 = enemy vision
    //3 = both
    public int visualMode = 1;

    public bool showMarkers = true;

    void Start(){
        setVisMode1();
    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            setVisMode1();
        }

        if (Input.GetKeyDown("2"))
        {
            playerLight.SetActive(false);
            enemyLight.SetActive(true);
            visualMode = 2;
            playerBrain.vizMode = false;
            enemyBrain.vizMode = true;
            showMarkers = true;
            predictionDot.enabled = true;
            targetDot.enabled = true;
        }

        if (Input.GetKeyDown("3"))
        {
            playerLight.SetActive(false);
            enemyLight.SetActive(false);
            visualMode = 3;
            playerBrain.vizMode = true;
            enemyBrain.vizMode = true;
            showMarkers = true;
            predictionDot.enabled = true;
            targetDot.enabled = true;
        }
    }

    void setVisMode1(){
        playerLight.SetActive(true);
        enemyLight.SetActive(false);
        visualMode = 1;
        playerBrain.vizMode = true;
        enemyBrain.vizMode = false;
        showMarkers = false;
        predictionDot.enabled = false;
        targetDot.enabled = false;
    }
}
