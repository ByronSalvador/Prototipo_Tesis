using UnityEngine;
using UnityEngine.UI;

namespace Options
{
    public class GlobalVar : MonoBehaviour
    {
        
        //Seleccion de Dificultad
        public static float velocidadPositiva;
        public static float velocidadNegativa;
        

        //public static float currentTime = 10f;
        //Umbral de velocidad de movimiento
        //public static double speedThreshold = 30;
        public static double speedThreshold = 80;

        public Slider sliderTime;
        public static float currentTime = 60f;

        public void Start()
        {
            //currentTime = sliderTime.value * 60;
            sliderTime.value = PlayerPrefs.GetFloat("sliderTime", 1);
            velocidadPositiva = PlayerPrefs.GetFloat("velocidadPositiva");
            velocidadNegativa = PlayerPrefs.GetFloat("velocidadNegativa");
        }

        public void ChangeSliderTime(float valor)
        {
            currentTime = valor * 60f;
            PlayerPrefs.SetFloat("sliderTime", currentTime);
            Debug.Log("Tiempo de juego" + currentTime);
        }

        public void SetDifficultyEasy()
        {
            PlayerPrefs.SetFloat("velocidadPositiva", 6f);
            PlayerPrefs.SetFloat("velocidadNegativa", 0f);
            Debug.Log("Dificultad seteada en facil con velpos " + PlayerPrefs.GetFloat("velocidadPositiva") +
                      " con velneg " + PlayerPrefs.GetFloat("velocidadNegativa"));
        }

        public void SetDifficultyHard()
        {
            PlayerPrefs.SetFloat("velocidadPositiva", 4f);
            PlayerPrefs.SetFloat("velocidadNegativa", -1f);
            Debug.Log("Dificultad seteada en dificil con velpos" + PlayerPrefs.GetFloat("velocidadPositiva") +
                      " con velneg " + PlayerPrefs.GetFloat("velocidadNegativa"));
        }
    }
}