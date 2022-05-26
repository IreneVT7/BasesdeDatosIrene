using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public static AsteroidSpawner instance;
    public BoxCollider2D bc;
    Vector2 cubeSize;
    Vector2 cubeCenter;

    [HideInInspector]
    public float lowlimit = 0.1f;
    [HideInInspector]
    public float currentlimit = 1f;


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

    private Vector2 RandomPos()
    {
        //generamos una posicion random cualquiera dentro del collider para que spawnee el asteroide
        Vector2 randomPosition = new Vector2(Random.Range(-cubeSize.x / 2, cubeSize.x / 2), Random.Range(-cubeSize.y / 2, cubeSize.y / 2));
        return cubeCenter + randomPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        //al empezar, obtenemos el area para el spawner, que sera el tamano del collider
        Transform cubeTrans = bc.GetComponent<Transform>();
        cubeCenter = cubeTrans.position;

        //se multiplica por la escala porque afecta al tamaño del collider
        cubeSize.x = cubeTrans.localScale.x * bc.size.x;
        cubeSize.y = cubeTrans.localScale.y * bc.size.y;

        StartCoroutine(Spawn());
    }

    private void Update()
    {
        if (GameManager.instance.isDead == true)
        {
            //si el jugador muere, desactiva el script para que no spawneen más
            this.enabled = false;
        }
    }

    IEnumerator Spawn()
    {
        //necesitabamos una condición cualquiera para que entrase en bucle
        //como el tiempo siempre va a escala normal pues usamos esa
        while (Time.timeScale == 1)
        {
            yield return new WaitForSeconds(currentlimit);
            AsteroidPooling.instance.InstantiateFromList(RandomPos());
        }
    }


}
