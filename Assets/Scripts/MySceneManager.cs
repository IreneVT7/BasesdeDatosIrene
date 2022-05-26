using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public string Level;

    public void ChangeScene()
    {
        //cambia de escena a la que esté escrita en el string
        SceneManager.LoadScene(Level, LoadSceneMode.Single);
    }

    public void QuitApp()
    {
        //cierra el juego
        Debug.Log("Closed game");
        Application.Quit();
    }



}
