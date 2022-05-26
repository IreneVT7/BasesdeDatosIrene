using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D coll;
    public Animator anim;
    [HideInInspector]
    public int speed = 100;
    public SpriteRenderer sr;


    private void OnBecameInvisible()
    {
        //si desaparece de la pantalla, que se desactive
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.instance.isDead == true)
        {
            //si el jugador ha muerto, para no tener que parar el tiempo que puede complicar las cosas:
            //paramos el asteroide, le quitamos el collider para que no siga quitando vidas al jugador, paramos la animación de rotación.
            rb.velocity = Vector3.zero;
            coll.enabled = false;
            anim.enabled = false;
        }
        else
        {
            //mueve el asteroide hacia abajo con una velocidad determinada
            Vector2 direction = Vector2.down;
            rb.velocity = direction * speed * Time.deltaTime;
        }


        if (LevelChanger.instance.currentColor != LevelChanger.instance.lastColor)
        {
            //si se ha cambiado el color del nivel, que cambie el color del asteroide
            sr.color = LevelChanger.instance.currentColor;
        }
    }

    private void OnDisable()
    {
        //si se desactiva: mete bala en la lista del script Asteroid pooling
        AsteroidPooling.instance.MeterEnLista(this);
    }

    private void OnEnable()
    {
        //por si acaso, que cuando se active la rotación se quede en identity
        transform.rotation = Quaternion.identity;
    }

    public JObject Serialize()
    {
        //generamos un jobj que guarde las variables importantes para poder cargar partida
        JObject jObject = new JObject();
        jObject.Add("position",JsonUtility.ToJson(transform.position));
        jObject.Add("active", JsonConvert.SerializeObject(gameObject.activeInHierarchy));
        return jObject;
    }

    public void Deserialize(JObject jObject)
    {
        //para cargar la partida, deserializamos y asignamos el valor a cada variable de nuevo
        transform.position = JsonUtility.FromJson<Vector3>(jObject["position"].ToString());
        gameObject.SetActive(JsonConvert.DeserializeObject<bool>(jObject["active"].ToString()));

    }
}
