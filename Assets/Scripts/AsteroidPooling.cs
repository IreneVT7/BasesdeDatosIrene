using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AsteroidPooling : MonoBehaviour
{
    private List<AsteroidBehaviour> disabledAsteroids;
    public GameObject prefabAsteroid;
    public static AsteroidPooling instance;
    public List<AsteroidBehaviour> allAsteroids;
    [HideInInspector]
    public int warmup=50;
    
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        //inicializa las listas
        disabledAsteroids = new List<AsteroidBehaviour>();
        allAsteroids = new List<AsteroidBehaviour>();

        //carga varios asteroides al prncipio en la lista pero los desactiva
        for (int i = 0; i < warmup; i++)
        {
            GameObject asteroid = Instantiate(prefabAsteroid, Vector3.zero, Quaternion.identity, transform);
            asteroid.SetActive(false);
            allAsteroids.Add(asteroid.GetComponent<AsteroidBehaviour>());
        }
        
    }


    public GameObject InstantiateFromList(Vector2 position)
    {
        if (disabledAsteroids.Count > 0)
        {
            //Si hay asteroids en la lista de asteroids: coge el primero, lo activa, le cambia la posición y lo quita de la lista
            GameObject asteroid = disabledAsteroids[0].gameObject;
            asteroid.SetActive(true);
            asteroid.transform.position = position;
            disabledAsteroids.RemoveAt(0);            

            return asteroid;
        }
        else
        {
            //Si no hay asteroids en la lista de asteroids: instancia uno nuevo
            GameObject asteroid = Instantiate(prefabAsteroid, position, Quaternion.identity, transform);
            allAsteroids.Add(asteroid.GetComponent<AsteroidBehaviour>());
            return asteroid;
        }
    }

    public void MeterEnLista(AsteroidBehaviour asteroid)
    {
        //mete una asteroid desactivada en la lista (el resto esta en el bulletbehaviour)
        disabledAsteroids.Add(asteroid);
    }

    public JObject Serialize()
    {
        //generamos un jobj que guarde las variables importantes para poder cargar partida
        JObject jObject = new JObject();
        jObject.Add("AsteroidCount", allAsteroids.Count);
        for (var index = 0; index < allAsteroids.Count; index++)
        {
            var item = allAsteroids[index];
            jObject.Add("asteroide" +index,  item.Serialize());
           
        }

        return jObject;
    }
    public void Deserialize(JObject jObject)
    {
        //borramos todos los asteroides y vaciamos la lista para luego generar nuevos asteroides en las posiciones que tenían antes
        foreach (var item in allAsteroids)
        {
            Destroy(item.gameObject);
        }

        allAsteroids.Clear();
        disabledAsteroids.Clear();
        
        //para cargar la partida, deserializamos y asignamos el valor a cada variable de nuevo
        int count = JsonConvert.DeserializeObject<int>(jObject["AsteroidCount"].ToString());
        for (int i = 0; i < count; i++)
        {
            GameObject asteroid = Instantiate(prefabAsteroid, Vector3.zero, Quaternion.identity, transform);
            AsteroidBehaviour script= asteroid.GetComponent<AsteroidBehaviour>();
            allAsteroids.Add(script);
            script.Deserialize(jObject["asteroide" + i].ToObject<JObject>());
        }
        
    }
}