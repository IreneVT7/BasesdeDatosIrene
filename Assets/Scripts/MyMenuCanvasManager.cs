using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMenuCanvasManager : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject rankingsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        mainCanvas.SetActive(true);
        rankingsCanvas.SetActive(false);
    }


    public void MainEnable()
    {
        //activa el canvas del menu principal
        mainCanvas.SetActive(true);
        rankingsCanvas.SetActive(false);
    }

    public void RankingEnable()
    {
        //activa el canvas del ranking
        mainCanvas.SetActive(false);
        rankingsCanvas.SetActive(true);
    }


}
