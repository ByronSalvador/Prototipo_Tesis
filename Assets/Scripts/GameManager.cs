using System.Collections.Generic;
using Leaderboard;
using LevelCustom;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using UnityEngine;
using PlayFab;
using Scene = LevelCustom.Scene;

public class GameManager : MonoBehaviour
{
    private int puntosTotales;
    public static int auxUpdatePoints = 0;
    private const string LeaderboardName = "Leaderboard";
    private PlayFabUpdatePlayerStatistics playFabUpdatePlayerStatistics;
    private string playerId;

    //[SerializeField] private GameObject menuReport;

    //Personajes
    public List<Characters> charactersList;

    //Escenas
    public List<Scene> scenesList;

    public static GameManager Instance { get; private set; }
    public HUD hud;

    public int PuntosTotales
    {
        get { return puntosTotales; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Cuidado! Mas de un gameManager en escena.");
        }
    }

    private void Start()
    {
        CreatePlayFabServices();
    }

    private void CreatePlayFabServices()
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = playerId
        };
        PlayFabClientAPI.GetUserData(request, OnGetUserDataSuccess, OnGetUserDataFailure);
        Debug.Log("Request: " + request);

        playFabUpdatePlayerStatistics = new PlayFabUpdatePlayerStatistics();
    }

    private void OnGetUserDataSuccess(GetUserDataResult result)
    {
        Debug.Log("Id: " + playerId);
    }

    private void OnGetUserDataFailure(PlayFabError error)
    {
        Debug.Log("Error al obtener datos del jugador");
    }

    public void SumarPuntos(int puntosSumar)
    {
        puntosTotales += puntosSumar;
        //Debug.Log(puntosTotales);
        hud.ActualizarPuntos(puntosTotales);
    }

    public void PasarNivel(int nivel)
    {
        if (puntosTotales > 2000000)
        {
            Debug.Log("Pasar Nivel");
            SceneManager.LoadScene(nivel);
        }
    }

    public void SeleccionNivel(int nivel)
    {
        Debug.Log("Nivel seleccionado");
        SceneManager.LoadScene(nivel);
    }

    public void GameOver()
    {
        //SceneManager.LoadScene(4);
        //menuReport.SetActive(true);
        if (auxUpdatePoints == 0)
        {
            Debug.Log("puntos enviados: " + puntosTotales);
            playFabUpdatePlayerStatistics.UpdatePlayerStatistics(LeaderboardName, puntosTotales);
            auxUpdatePoints++;
        }
        
    }
}