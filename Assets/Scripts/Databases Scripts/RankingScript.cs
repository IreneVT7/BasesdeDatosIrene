using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Librería UI Unity
using UnityEngine.UI;

public class RankingScript : MonoBehaviour
{
    //Variables para acceder a los objetos Text
    public Text Posicion, Nombre, Puntos;

    //Método para poner los puntos en la UI
    public void PonerPuntos(string pos, string nombre, string puntos)
    {
        Posicion.text = pos;
        Nombre.text = nombre;
        Puntos.text = puntos;
    }
}
