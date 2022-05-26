using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Librerías que necesitamos
using System.Data;
using System.IO;
using Mono.Data.Sqlite;

public class RankingManager : MonoBehaviour
{
    //Variable para controlar la ruta de la base de datos, constructor de la ruta, y el nombre de la base de datos
    string rutaDB;
    string strConexion;
    string DBFileName = "Ranking.db";

    //Variable para trabajar con las conexiones
    IDbConnection dbConnection;
    //Para poder ejecutar comandos
    IDbCommand dbCommand;
    //Variable para leer
    IDataReader reader;

    //Lista para el Ranking
    private List<Ranking> rankings = new List<Ranking>();

    //Variables para almacenar el prefab y la posición del padre
    public GameObject puntosPrefab;
    public Transform puntosPadre;

    //Límite de datos que aparecerá en la UI
    public int topRank;
    //Limitamos el ranking que podemos tener
    public int limiteRanking;

    public bool isScene;


    // Start is called before the first frame update
    void Start()
    {
        //si estamos en la escena del juego no hacemos esto porque da error
        if(isScene)
            return;        
        BorrarPuntosExtra();
        MostrarRanking();
    }

    //Método para abrir la DB
    void AbrirDB()
    {
        // Crear y abrir la conexión
        // Comprobar en que plataforma estamos
        // Si es el Editor de Unity mantenemos la ruta
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
        }
        //Si estamos en PC
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
        }
        //Si es Android
        else if (Application.platform == RuntimePlatform.Android)
        {
            rutaDB = Application.persistentDataPath + "/" + DBFileName;
            // Comprobar si el archivo se encuentra almecenado en persistant data
            if (!File.Exists(rutaDB))
            {
                // Almaceno el archivo en load db
                WWW loadDB = new WWW("jar;file://" + Application.dataPath + "!/assets/" + DBFileName);
                while (!loadDB.isDone)
                {

                }
                // Copio el archivo a persistant data
                File.WriteAllBytes(rutaDB, loadDB.bytes);
            }
        }

        strConexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(strConexion);
        dbConnection.Open();
    }

    //Método para obtener el Ranking de la DB
    void ObtenerRanking()
    {
        //Primero dejamos la lista de Rankings limpia
        rankings.Clear();
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT * FROM ranking";
        dbCommand.CommandText = sqlQuery;

        // Leer la base de datos
        reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            rankings.Add(new Ranking(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
        }
        reader.Close();
        reader = null;
        //Cerramos la DB
        CerrarDB();
        //Ordenamos la lista
        rankings.Sort();
    }

    //Método para insertar puntos en la DB
    public void InsertarPuntos(string n, int s)
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("INSERT INTO ranking(Name, Score) values(\"{0}\",\"{1}\")", n, s);
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteScalar();
        //Cerramos la DB
        CerrarDB();
    }

    //Método para borrar puntos de la DB
    void BorrarPuntos(int id)
    {
        //Abrimos la DB
        AbrirDB();
        // Crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "DELETE FROM Ranking WHERE PlayerId = \"" + id + "\"";
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteScalar();
        //Cerramos la DB
        CerrarDB();
    }

    //Método para mostrar el ranking en la UI
    void MostrarRanking()
    {
        //Obtener el ranking de la DB
        ObtenerRanking();
        //Hacemos una pasada por la lista para ir posicionando los puntos en la UI
        for (int i = 0; i < topRank; i++)
        {
            //Si siguen habiendo elementos en la lista
            if (i < rankings.Count)
            {
                //Instanciamos el objeto Puntaje
                GameObject tempPrefab = Instantiate(puntosPrefab);
                //Hacemos este objeto hijo de Puntos
                tempPrefab.transform.SetParent(puntosPadre);
                //Le ponemos la escala para que se adapte a la resolución actual de la UI
                tempPrefab.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                //Posición en la lista
                Ranking rankTemp = rankings[i];
                //Llamamos al método que pone los puntos
                tempPrefab.GetComponent<RankingScript>().PonerPuntos("#" + (i + 1).ToString(),
                                                        rankTemp.Name, rankTemp.Score.ToString());
            }
        }
    }

    //Método para borrar los puntos extra
    void BorrarPuntosExtra()
    {
        //Obtener el Ranking
        ObtenerRanking();
        //Comprobar que el ranking sea mas grande que el limite
        if (limiteRanking <= rankings.Count)
        {
            //Le damos la vuelta a la lista para borrar las menores puntuaciones
            rankings.Reverse();
            //obtenemos la diferencia entre el ranking y el limite, para ver cuantos registros nos sobran
            int diferencia = rankings.Count - limiteRanking;
            //Abrimos DB
            AbrirDB();
            //Creo Comando
            dbCommand = dbConnection.CreateCommand();
            //Bucle con la diferencia
            for (int i = 0; i < diferencia; i++)
            {
                //Borrar por ID en la posicion del ranking
                string sqlQuery = "DELETE FROM Ranking WHERE PlayerId = \"" + rankings[i].Id + "\"";
                dbCommand.CommandText = sqlQuery;
                dbCommand.ExecuteScalar();
            }
            //Cerrar DB
            CerrarDB();

        }
    }

    //Método para cerrar la DB
    void CerrarDB()
    {
        // Cerrar las conexiones
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }
}
