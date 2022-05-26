using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class LevelChanger : MonoBehaviour
{
    public static LevelChanger instance;
    public Color[] colors;
    [HideInInspector]
    public Color currentColor;
    [HideInInspector]
    public Color lastColor;
    [HideInInspector]
    public int colorNumber;
    [HideInInspector]
    public int lastColorNumber;
    public SpriteRenderer spritepantalla;


    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //se asignan valores para que siempre empiece la partida con un color determinado
        currentColor = colors[0];
        lastColor = currentColor;
        colorNumber = 0;
    }

    
    public void colorChange()
    {
        //guarda el ultimo color y el numero en el array para luego compararlo en otros scripts y reasignar colores
        lastColor = currentColor;
        lastColorNumber = colorNumber;
        //cuando el gamemanager pide cambiar de nivel, suma uno al color o vuelve al principio si se han agotado los colores en el array
        colorNumber++;
        if (colorNumber >= colors.Length)
        {
            colorNumber = 0;
        }
        //asigna el nuevo color y cambia tambien el color de la pantalla del nivel
        currentColor = colors[colorNumber];
        spritepantalla.color = currentColor;
    }

    public JObject Serialize()
    {
        //generamos un jobj que guarde las variables importantes para poder cargar partida
        JObject jobj = new JObject();
        jobj.Add("colorNumber", JsonConvert.SerializeObject(colorNumber));
        jobj.Add("lastColorNumber", JsonConvert.SerializeObject(lastColorNumber));
        return jobj;
    }

    public void Deserialize(JObject jobj)
    {
        //para cargar la partida, deserializamos y asignamos el valor a cada variable de nuevo
        colorNumber = JsonConvert.DeserializeObject<int>(jobj["colorNumber"].ToString());
        lastColorNumber = JsonConvert.DeserializeObject<int>(jobj["lastColorNumber"].ToString());
        currentColor = colors[colorNumber];
        spritepantalla.color = currentColor;
    }
}

