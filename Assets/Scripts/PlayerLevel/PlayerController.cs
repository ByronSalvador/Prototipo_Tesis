using UnityEngine;
using System;
using OpenCvSharp;
using System.Threading.Tasks;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody))]
public class NewBehaviourScript : MonoBehaviour
{
    Rigidbody rb;

    //[SerializeField] public RawImage imageDisplay;
    //public float Velocidad = 8f;

    //Variables Globales

    // Variable para almacenar el umbral de diferencia para objetos de color rojo
    private double speedThreshold = Options.GlobalVar.speedThreshold;

    // Variables para almacenar la velocidad positiva y negativa
    private float velocidadPositiva;
    private float velocidadNegativa;


    // Variable para almacenar el video
    private WebCamTexture webcamTexture;

    // Variable para almacenar el fondo
    private BackgroundSubtractorMOG2 backgroundSubtractor;

    // Variable para almacenar el kernel morfológico
    private Mat kernel;

    // Variable para almacenar el estado del movimiento
    //private bool hayMovimiento = false;
    // Variable para almacenar el estado del movimiento de color rojo
    //public bool hayMovimientoRojo = false;
    // Variable para almacenar el estado del movimiento de color verde
    //public bool hayMovimientoVerde = false;
    // Variable para almacenar el fotograma anterior
    //private Mat prevFrame = null;
    // Variable para almacenar el umbral de diferencia
    //private double threshold = 1;
    // Variable para almacenar el umbral de diferencia para objetos de color rojo
    //private double threshold_red = 2;
    //public double speed_threshold = 60;
    // Variable para almacenar el umbral de diferencia para objetos de color verde
    //private double threshold_green = 52; 

    // Variable para almacenar el número de objetos de color rojo
    //private int num_objects_red = 0;
    // Variable para almacenar el número de objetos de color verde
    //private int _numObjectsGreen = 0;

    //Variable para almacenar el centro del rectángulo en el fotograma anterior
    Point prevPrevCenterRed;

    //Variable para almacenar el estado del movimiento rápido
    private bool hayMovimientoRapidoRojo;


    //Variable para almacenar el centro del rectángulo en el fotograma anterior
    Point prevCenterRed;

    //Definicion maquina de estado finito
    public enum EstadoMovimiento
    {
        Reposo,
        MovimientoDetectado
    }

    EstadoMovimiento estadoActual = EstadoMovimiento.Reposo;
    //int framesMovimientoDetectado = 0;

    //Buffer historico de frames
    //private const int BufferSize = 12;    //Funciona bien con 12
    private const int BufferSize = 20;
    bool[] moveBuffer = new bool[BufferSize];
    int bufferIndex;

    //Contador de Pasos
    public Reports.Reports reports;


    // Start is called before the first frame update
    void Start()
    {
        velocidadPositiva = PlayerPrefs.GetFloat("velocidadPositiva", 6f);
        velocidadNegativa = PlayerPrefs.GetFloat("velocidadNegativa", 0f);

        reports = new Reports.Reports();

        rb = GetComponent<Rigidbody>();

        // Inicializar el video
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
        // Inicializar el fondo
        backgroundSubtractor = BackgroundSubtractorMOG2.Create();
        // Inicializar el kernel morfológico
        kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));

        //Debug.Log("Dificultad seteada con velpos" + velocidadPositiva + " con velneg " + velocidadNegativa);
    }


    // Update is called once per frame
    async void Update()
    {
        // Obtener el fotograma actual de forma asíncrona
        Mat frame = await GetFrameAsync();
        // Mostrar el resultado solo si hay movimiento y hay movimiento de color rojo o verde
        //Debug.Log("la dificultad esta con velpos " + velocidadPositiva + " con velneg " + velocidadNegativa);
        Cv2.ImShow("Color Detection", frame);
        rb.velocity = Vector3.zero;

        //Detectar movimiento instantaneo
        bool instantMove = hayMovimientoRapidoRojo;

        //Almacenar en buffer
        moveBuffer[bufferIndex] = instantMove;
        bufferIndex = (bufferIndex + 1) % BufferSize;

        //Analizar buffer
        bool trueMovement = BufferAnalyze(moveBuffer);

        /*
        switch (hayMovimientoRapidoRojo, hayMovimientoVerde)
        {
            case (true, false):
                //Time.timeScale = 1f;
                //velocidad = velocidadPositiva;
                transform.position += new Vector3(0, 0, velocidadPositiva * Time.deltaTime);
                //gameManager.SumarPuntos(1);
                Debug.Log("Se ha detectado un objeto de color rojo que se mueve: " + hayMovimientoRapidoRojo +
                          " con velocidad de: " + velocidadPositiva);
                //StartCoroutine(Esperar(2));

                break;
            case (false, true):
                //Time.timeScale = 0f;
                //velocidad = 0;
                transform.position += new Vector3(0, 0, velocidadNegativa * Time.deltaTime);
                //rb.velocity = Velocidad * (Vector3.left);
                Debug.Log("Se ha detectado un objeto de color verde que se mueve: " + hayMovimientoVerde +
                          " con velocidad de: " + velocidadNegativa);
                break;

            case (false, false):
                //Time.timeScale = 0f; // Pausar el juego
                //velocidad = 0;
                transform.position += new Vector3(0, 0, velocidadNegativa * Time.deltaTime);
                Debug.Log("No se ha detectado un objeto de color rojo y verde que se mueve" + " con velocidad de: " +
                          velocidadNegativa);
                break;
        }
        */
        if (Time.timeScale != 0)
        {
            switch (estadoActual)
            {
                case EstadoMovimiento.Reposo:
                    transform.position += new Vector3(0, 0, velocidadNegativa * Time.deltaTime);
                    Debug.Log("Estado actual: " + estadoActual);
                    if (trueMovement)
                    {
                        estadoActual = EstadoMovimiento.MovimientoDetectado;
                        reports.AddStep();
                        //int stepAux = reports.StepCounter;
                        //Debug.Log("REPORTERIA: " + reports.StepCounter);
                        // Empezar a contar frames
                        //framesMovimientoDetectado = 0;
                    }

                    break;

                case EstadoMovimiento.MovimientoDetectado:
                    //framesMovimientoDetectado++;
                    transform.position += new Vector3(0, 0, velocidadPositiva * Time.deltaTime);
                    //transform.position += new Vector3(0, 0, 2 * Time.deltaTime);
                    Debug.Log("Estado actual: " + estadoActual);
                    //Debug.Log("valor de hayMovimientoRapidoRojo: " + hayMovimientoRapidoRojo);
                    Debug.Log("valor de trueMovement: " + trueMovement);
                    if (!trueMovement)
                    {
                        estadoActual = EstadoMovimiento.Reposo;
                    }

                    break;
            }
        }

        // Mantener true mientras esté en estado de movimiento detectado
        //hayMovimientoRapidoRojo = (estadoActual == EstadoMovimiento.MovimientoDetectado);

        //rb.velocity = Velocidad * (Input.GetAxis("Horizontal") * Vector3.right + Input.GetAxis("Vertical") * Vector3.forward);
    }

    #region Deteccion de Movimiento

    async Task<Mat> GetFrameAsync()
    {
        // Obtener el fotograma actual
        Mat frame = OpenCvSharp.Unity.TextureToMat(webcamTexture);

        // Convertir el fotograma a HSV
        Mat hsv = new Mat();
        Cv2.CvtColor(frame, hsv, ColorConversionCodes.BGR2HSV);

        // Definir el rango de color rojo en HSV
        Scalar lowerRed = new Scalar(0, 120, 70); //Valor del tono para un rojo amplio
        Scalar upperRed = new Scalar(10, 255, 255);

        // Crear una máscara binaria para el color rojo
        Mat redMask = new Mat();
        Cv2.InRange(hsv, lowerRed, upperRed, redMask);

        // Aplicar la operación morfológica de cierre a cada máscara
        Mat closedRed = new Mat();
        Cv2.MorphologyEx(redMask, closedRed, MorphTypes.Close, kernel);

        // Encontrar los contornos de los objetos rojos usando la máscara
        Point[][] contoursRed;
        HierarchyIndex[] hierarchyRed;
        Cv2.FindContours(closedRed, out contoursRed, out hierarchyRed, RetrievalModes.External,
            ContourApproximationModes.ApproxSimple);

        //Analizar ROI (Region de Interes) de objetos de color rojo
        foreach (var contour in contoursRed)
        {
            double area = Cv2.ContourArea(contour);
            if (area > 8000)
            {
                //Obtener una ROI para cada contorno de cada objeto rojo
                OpenCvSharp.Rect roiRed = Cv2.BoundingRect(contour);

/*
                //*******************************Impresion del texto en el frame, no se necesita para la mecánica*******************************
                // Definir el texto del mensaje
                string message = $"Se han detectado objetos de color ROJO";
                // Definir la posición del mensaje
                Point position = new Point(10, 30);
                // Definir el color del mensaje
                Scalar color = Scalar.White;
                // Definir la fuente del mensaje
                HersheyFonts font = HersheyFonts.HersheySimplex;
                // Definir el tamaño del mensaje
                double size = 1.5;
                // Escribir el mensaje en el fotograma original
                Cv2.PutText(frame, message, position, font, size, color);
                //******************************************************************************************************************************


                //*******************************Dibujo del contorno en los objetos rojos, no se necesita para la mecánica*******************************
                // Definir el color de los contornos rojos
                Scalar contourColorRed = Scalar.Red;
                // Definir el grosor de los contornos
                int contourThickness = 3;
                // Dibujar los contornos en el fotograma original
                Cv2.DrawContours(frame, contoursRed, -1, contourColorRed, contourThickness);
                //***************************************************************************************************************************************

*/
                // Calcular el centro del rectángulo actual
                Point centerRed = new Point(roiRed.X + roiRed.Width / 2, roiRed.Y + roiRed.Height / 2);
                //Debug.Log("el centro rojo (center_red): " + centerRed);

                // Si existe un centro previo, calcular la distancia entre los dos centros
                if (prevCenterRed != null)
                {
                    double distance = Math.Sqrt(Math.Pow(centerRed.X - prevCenterRed.X, 2) +
                                                Math.Pow(centerRed.Y - prevCenterRed.Y, 2));

                    // Si la distancia supera el umbral de velocidad, asignar true a la variable booleana y cambiar el color del mensaje a rojo
                    if (distance > speedThreshold)
                    {
                        hayMovimientoRapidoRojo = true;
                        //Debug.Log("La distancia recorrida por el objeto es: " + distance);
                        //color = Scalar.Red;
                        //Cv2.PutText(frame, message, position, font, size, color);
                    }
                    else
                    {
                        //StartCoroutine(Esperar(2));
                        hayMovimientoRapidoRojo = false;
                    }
                }

                // Actualizar el centro previo con el actual
                prevCenterRed = centerRed;
            }
        }
        //Texture2D texture = OpenCvSharp.Unity.MatToTexture(frame);
        //imageDisplay.texture = texture;
        return redMask;
        //return frame;
    }

    #endregion

    //Funcion para analizar el buffer
    bool BufferAnalyze(bool[] buffer)
    {
        int trueCount = 0;
        foreach (bool b in buffer)
        {
            if (b)
            {
                trueCount++;
            }
        }

        return trueCount > ((BufferSize / 2) - 1);
    }


    void OnDestroy()
    {
        // Liberar los recursos
        webcamTexture.Stop();
        backgroundSubtractor.Dispose();
        kernel.Dispose();
        //prevFrame.Dispose();
        Cv2.DestroyAllWindows();
    }
}