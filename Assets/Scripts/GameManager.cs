using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector]
    public int health = 3;
    [Header("Health")]
    public int maxHealth = 3;
    public Text healthText;
    [HideInInspector]
    public bool isDead;

    [HideInInspector]
    public int currentLevel;
    [Header("Level")]
    public float levelTimeMax = 5f;
    [HideInInspector]
    public float levelTime;
    public Text levelText;

    [HideInInspector]
    public int score;
    [Header("Score")]
    public Text scoreText;
    public Text nameText;
    public RankingManager RankingManager;

    [Header("Others")]
    public GameObject gameOverPanel;
    public Button saveButton;
    public Button loadButton;


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
        //al empezar la partida se cambian los valores y se muestran en pantalla
        health = maxHealth;
        healthText.text = health.ToString();
        isDead = false;

        currentLevel = 1;
        levelText.text = currentLevel.ToString();

        score = 0;
        scoreText.text = score.ToString();

        gameOverPanel.SetActive(false);
        saveButton.enabled = true;
        loadButton.enabled = true;

        //empiezan las corrutinas
        StartCoroutine(ScoreAddOverTime());
        StartCoroutine(LevelAdvance());
    }    

    public void DecreaseHealth()
    {
        //cuando recibe daño, se quita vida y se actualiza el texto en escena
        health = health - 1;
        healthText.text = health.ToString();
        if (health <= 0)
        {
            //si está muerto, que active el panel para guardar partida, cambie el bool para que el resto de scripts lo usen para parar lo que haya que parar...
            //...y active el panel para guardar el ranking
            isDead = true;
            gameOverPanel.SetActive(true);
            saveButton.enabled = false;
            loadButton.enabled = false;

            StopAllCoroutines();
        }
    }

    public void AddPoints(int points)
    {
        //añade puntos y lo muestra en escena
        score = score + points;
        scoreText.text = score.ToString();
    }

    public void ChangeLevel()
    {
        //sube de nivel, actualiza el texto en pantalla, cambia el color y aumenta el spawn rate (reduciendo tiempo de spawn)
        currentLevel++;
        levelText.text = currentLevel.ToString();
        LevelChanger.instance.colorChange();
        AsteroidSpawner.instance.currentlimit = AsteroidSpawner.instance.currentlimit - 0.05f;
        if (AsteroidSpawner.instance.currentlimit <= AsteroidSpawner.instance.lowlimit)
        {
            AsteroidSpawner.instance.currentlimit = AsteroidSpawner.instance.lowlimit;
        }
    }

    IEnumerator ScoreAddOverTime()
    {
        //necesitabamos una condición cualquiera para que entrase en bucle
        //como el tiempo siempre va a escala normal pues usamos esa
        while (Time.timeScale == 1)
        {
            //para ir subiendo la puntuación, el jugador recibe un punto cada segundo
            yield return new WaitForSeconds(1f);
            AddPoints(1);
        }

    }

    IEnumerator LevelAdvance()
    {
        //necesitabamos una condición cualquiera para que entrase en bucle
        //como el tiempo siempre va a escala normal pues usamos esa
        while (Time.timeScale == 1)
        {
            //resetea el tiempo del nivel y lo ve bajando hasta que llegue a 0 y tenga que cambiar al siguiente
            levelTime = levelTimeMax;
            while (levelTime > 0)
            {
                yield return new WaitForSeconds(1f);
                levelTime--;
            }
            ChangeLevel();
        }
    }

    public void GuardarPuntosDB()
    {
        //guarda los puntos del ranking
        RankingManager.InsertarPuntos(nameText.text, score);
    }


    public JObject Serialize()
    {
        //generamos un jobj que guarde las variables importantes para poder cargar partida
        JObject jobj = new JObject();
        jobj.Add("score", JsonConvert.SerializeObject(score));
        jobj.Add("currentLevel", JsonConvert.SerializeObject(currentLevel));
        jobj.Add("levelTime", JsonConvert.SerializeObject(levelTime));
        jobj.Add("health", JsonConvert.SerializeObject(health));

        return jobj;
    }

    public void Deserialize(JObject jobj)
    {
        //para cargar la partida, deserializamos y asignamos el valor a cada variable de nuevo
        score = JsonConvert.DeserializeObject<int>(jobj["score"].ToString());
        currentLevel = JsonConvert.DeserializeObject<int>(jobj["currentLevel"].ToString());
        levelTime = JsonConvert.DeserializeObject<float>(jobj["levelTime"].ToString());
        health = JsonConvert.DeserializeObject<int>(jobj["health"].ToString());
        healthText.text = health.ToString();
        scoreText.text = score.ToString();
        levelText.text = currentLevel.ToString();
    }
}
