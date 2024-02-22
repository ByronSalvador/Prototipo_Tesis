using System.Collections;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public GameManager gameManager;

    //Pantalla de carga previa al nivel del juego
    [SerializeField] private GameObject loadInProgress;

    //Progressbar del tiempo
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI maxTime;

    private int currentScene;

    private float currentTime = -1;

    [SerializeField] public TextMeshProUGUI puntos;

    //Impresion del nombre de usuario
    [SerializeField] private TextMeshProUGUI displayNamePlayer;

    //Seleccion de personajes
    private int index;
    [SerializeField] private Image image;
    [SerializeField] private new TextMeshProUGUI name;


    //Seleccion de escenas
    private int indexScene;
    [SerializeField] private Image imageScene;
    [SerializeField] private TextMeshProUGUI nameScene;

    //Menu de reporte
    [SerializeField] private GameObject menuReport;
    [SerializeField] private TextMeshProUGUI stepCounter;
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private TextMeshProUGUI timer;

    private void Start()
    {
        Time.timeScale = 1f;
        GameManager.auxUpdatePoints = 0;
        gameManager = GameManager.Instance;
        //displayNamePlayer.text = "Bienvenido: " + PlayerPrefs.GetString("displayName");
        displayNamePlayer.text = "Bienvenido " + PlayerPrefs.GetString("displayName2");
        //displayNamePlayer.text = "Bienvenido: " + PlayFabConstants.displayName;
        Debug.Log("Bienvenido: " + PlayerPrefs.GetString("displayName"));
        Debug.Log("Bienvenido2: " + PlayerPrefs.GetString("displayName2"));

        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        
        //Seleccion de personaje
        index = PlayerPrefs.GetInt("IndexPlayer");

        if (index > gameManager.charactersList.Count - 1)
        {
            index = 0;
        }

        //Seleccion de escena
        indexScene = PlayerPrefs.GetInt("IndexScene");

        if (indexScene > gameManager.scenesList.Count - 1)
        {
            indexScene = 0;
        }

        ChangeScene();

        if (currentScene == 6)
        {
            currentTime = Options.GlobalVar.currentTime;
            maxTime.text = (Options.GlobalVar.currentTime/60).ToString();
            Debug.Log("tiempo del niveel: " + maxTime);
            progressBar.maxValue = currentTime;
        }
    }

    void Update()
    {
        puntos.text = gameManager.PuntosTotales.ToString();

        // Resta el tiempo transcurrido desde el último frame
        currentTime -= Time.deltaTime;
        //Debug.Log("tiempo del nivel: " + currentTime);
        progressBar.value = currentTime;
        //gameManager.SumarPuntos(1);
        // Si se cumple el tiempo deseado, llama al método GameOver
        if (currentTime < 0.5f && currentTime > -0.5f)
        {
            gameManager.GameOver();
            Time.timeScale = 0f;
            menuReport.SetActive(true);
            userName.text = PlayerPrefs.GetString("displayName2");
            timer.text = (Options.GlobalVar.currentTime/60).ToString();
            stepCounter.text = Reports.Reports.StepCounter.ToString();
        }

        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.P) || Input.GetKeyUp(KeyCode.Space))
        {
            Pause();
        }
    }

    public void ActualizarPuntos(int puntosTotales)
    {
        puntos.text = puntosTotales.ToString();
    }

    #region Botones

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        Time.timeScale = 1f;
    }

    #endregion

    private void ChangeScene()
    {
        //Seleccion de personajes
        PlayerPrefs.SetInt("IndexPlayer", index);
        image.sprite = gameManager.charactersList[index].image;
        name.text = gameManager.charactersList[index].name;

        //Seleccion de escenas
        PlayerPrefs.SetInt("IndexScene", indexScene);
        imageScene.sprite = gameManager.scenesList[indexScene].imageScene;
        nameScene.text = gameManager.scenesList[indexScene].nameScene;
    }

    #region ButtonCharacter

    public void NextCharacter()
    {
        if (index == gameManager.charactersList.Count - 1)
        {
            index = 0;
        }
        else
        {
            index += 1;
        }

        ChangeScene();
    }

    public void PrevCharacter()
    {
        if (index == 0)
        {
            index = gameManager.charactersList.Count - 1;
        }
        else
        {
            index -= 1;
        }

        ChangeScene();
    }

    #endregion

    #region ButtonScene

    public void NextScene()
    {
        if (indexScene == gameManager.scenesList.Count - 1)
        {
            indexScene = 0;
        }
        else
        {
            indexScene += 1;
        }

        ChangeScene();
    }

    public void PrevScene()
    {
        if (indexScene == 0)
        {
            indexScene = gameManager.scenesList.Count - 1;
        }
        else
        {
            indexScene -= 1;
        }

        ChangeScene();
    }

    #endregion

    public void NextLevel()
    {
        int indexNextLevel = indexScene + 1;

        if (indexNextLevel > gameManager.scenesList.Count - 1)
        {
            indexNextLevel = 0;
        }

        //Seleccion de escenas
        PlayerPrefs.SetInt("IndexScene", indexNextLevel);
        imageScene.sprite = gameManager.scenesList[indexNextLevel].imageScene;
        nameScene.text = gameManager.scenesList[indexNextLevel].nameScene;

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        //Time.timeScale = 1f;
        //Resume();
        //REVISAR POR QUE SE QEUDA EN PAUSE EL JUEGO
    }

    public void PlayLevel()
    {
        //AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        //Cargar escena del nivel del juego
        StartCoroutine(LoadAsync());
        loadInProgress.SetActive(true);
    }

    IEnumerator LoadAsync()
    {
        loadInProgress.SetActive(true);
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);

        while (!operation.isDone)
        {
            //float progress = Mathf.Clamp01(operation.progress / .9f);

            //Debug.Log("progeso de carga: " + progress);
            yield return null;
        }

        //yield return null;
    }
}