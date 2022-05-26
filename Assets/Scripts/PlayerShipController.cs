using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class PlayerShipController : MonoBehaviour
{
    public static PlayerShipController instance;

    [Header("Movement")]
    [Tooltip("Rigidbody del padre")]
    public Rigidbody2D rb;
    [Tooltip("Velocidad a la que se mueve Reimu")]
    public float moveSpeed;
    [HideInInspector]
    public Vector2 moveDirection;
    [Header("Barrier Limit")]
    [Tooltip("collider que indica por donde se puede mover el jugador")]
    public Transform[] limits;

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
        this.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Movimiento
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 playerPos = this.gameObject.transform.position;

        moveDirection = new Vector2(x, y);
        moveDirection = moveDirection * moveSpeed;

        //area de juego
        //si se pasa de ciertas posiciones indicadas por dos transforms en esquinas contrarias:
        //frena, coge la posición y lo mueve un poco para que no se quede atascado
        if (this.gameObject.transform.position.x <= limits[0].position.x) //x-
        {
            rb.velocity = new Vector2(0, moveDirection.y);
            playerPos = new Vector2(this.gameObject.transform.position.x + 0.1f, this.gameObject.transform.position.y);
            transform.position = new Vector3(playerPos.x, playerPos.y, 0);
        }
        else if (this.gameObject.transform.position.x >= limits[1].position.x) //x+
        {
            rb.velocity = new Vector2(0, moveDirection.y);
            playerPos = new Vector2(this.gameObject.transform.position.x - 0.1f, this.gameObject.transform.position.y);
            transform.position = new Vector3(playerPos.x, playerPos.y, 0);
        }
        else if (this.gameObject.transform.position.y <= limits[0].position.y) //y-
        {
            rb.velocity = new Vector2(moveDirection.x, 0);
            playerPos = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 0.1f);
            transform.position = new Vector3(playerPos.x, playerPos.y, 0);
        }
        else if (this.gameObject.transform.position.y >= limits[1].position.y) //y+
        {
            rb.velocity = new Vector2(moveDirection.x, 0);
            playerPos = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 0.1f);
            transform.position = new Vector3(playerPos.x, playerPos.y, 0);
        }
        else
        {
            rb.velocity = moveDirection;
        }


        if (GameManager.instance.isDead == true)
        {
            //si esta muerto, deja de moverse
            rb.velocity = Vector3.zero;
            this.enabled = false;
        }        

    }    

    public JObject Serialize()
    {
        //generamos un jobj que guarde las variables importantes para poder cargar partida
        JObject jobj = new JObject();
        jobj.Add("PlayerShipPos",  JsonUtility.ToJson(transform.position));
        return jobj;
    }

    public void Deserialize(JObject jobj)
    {
        //para cargar la partida, deserializamos y asignamos el valor a cada variable de nuevo
        transform.position = JsonUtility.FromJson<Vector3>(jobj["PlayerShipPos"].ToString());
    }

}



