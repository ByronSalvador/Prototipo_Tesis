using UnityEngine;
using System;
using OpenCvSharp;
using System.Threading.Tasks;

public class ColorTracking : MonoBehaviour
{
    // Variable para almacenar el video
    private WebCamTexture webcamTexture;
    // Variable para almacenar el fondo
    private BackgroundSubtractorMOG2 backgroundSubtractor;
    // Variable para almacenar el kernel morfol�gico
    private Mat kernel;
    // Variable para almacenar el estado del movimiento de color rojo
    private bool hayMovimientoRapidoRojo = false;

    // Variable para almacenar el fotograma anterior
    private Mat prevFrame = null;
    // Variable para almacenar el umbral de diferencia
    private double speedThreshold = 60; // Variable para almacenar el umbral de diferencia para objetos de color rojo

    //Variable para almacenar el centro del rect�ngulo en el fotograma anterior
    Point prevCenterRed;

    void Start()
    {
        // Inicializar el video
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
        // Inicializar el fondo
        backgroundSubtractor = BackgroundSubtractorMOG2.Create();
        // Inicializar el kernel morfol�gico
        kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
    }

    async void Update()
    {
        // Obtener el fotograma actual de forma as�ncrona
        Mat frame = await GetFrameAsync();
        Cv2.ImShow("Color Detection", frame);

        switch (hayMovimientoRapidoRojo)
        {
            case (true):
                Debug.Log("se movio el rojo: ");
            break;
        }
        
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Q))
        {
            
            // Liberar los recursos
            webcamTexture.Stop();
            backgroundSubtractor.Dispose();
            kernel.Dispose();
            prevFrame.Dispose();
            Cv2.DestroyAllWindows();
            //frame.Destroy();
            // Se cierra el juego
            Application.Quit();
            
        }
        
        
    }

    async Task<Mat> GetFrameAsync()
    {
        // Obtener el fotograma actual
        Mat frame = OpenCvSharp.Unity.TextureToMat(webcamTexture);

        // Convertir el fotograma a HSV
        Mat hsv = new Mat();
        Cv2.CvtColor(frame, hsv, ColorConversionCodes.BGR2HSV);

        // Definir el rango de color rojo en HSV
        Scalar lowerRed = new Scalar(0, 120, 70);    //Valor del tono para un rojo amplio
        Scalar upperRed = new Scalar(10, 255, 255);

        // Crear una m�scara binaria para el color rojo
        Mat redMask = new Mat();
        Cv2.InRange(hsv, lowerRed, upperRed, redMask);

        // Aplicar la operaci�n morfol�gica de cierre a cada m�scara
        Mat closedRed = new Mat();
        Cv2.MorphologyEx(redMask, closedRed, MorphTypes.Close, kernel);

        // Encontrar los contornos de los objetos rojos usando la m�scara
        Point[][] contoursRed;
        HierarchyIndex[] hierarchyRed;
        Cv2.FindContours(closedRed, out contoursRed, out hierarchyRed, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        //Analizar ROI (Region de Interes) de objetos de color rojo
        foreach (var contour in contoursRed)
        {
            double area = Cv2.ContourArea(contour);
            if (area > 5000)
            {
                //Obtener una ROI para cada contorno de cada objeto rojo
                OpenCvSharp.Rect roiRed = Cv2.BoundingRect(contour);

                //*******************************Impresion del texto en el frame, no se necesita para la mec�nica*******************************
                // Definir el texto del mensaje
                string message = $"Se han detectado objetos de color ROJO que se mueven mucho";       
                // Definir la posici�n del mensaje
                Point position = new Point(10, 30);
                // Definir el color del mensaje
                Scalar color = Scalar.White;
                // Definir la fuente del mensaje
                HersheyFonts font = HersheyFonts.HersheySimplex;
                // Definir el tama�o del mensaje
                double size = 1.0;
                // Escribir el mensaje en el fotograma original
                Cv2.PutText(frame, message, position, font, size, color);
                //******************************************************************************************************************************

                //*******************************Dibujo del contorno en los objetos rojos, no se necesita para la mec�nica*******************************
                // Definir el color de los contornos rojos
                Scalar contourColorRed = Scalar.Red;
                // Definir el grosor de los contornos
                int contourThickness = 2;
                // Dibujar los contornos en el fotograma original
                Cv2.DrawContours(frame, contoursRed, -1, contourColorRed, contourThickness);
                //***************************************************************************************************************************************

                // Calcular el centro del rect�ngulo actual
                Point centerRed = new Point(roiRed.X + roiRed.Width / 2, roiRed.Y + roiRed.Height / 2);
                Debug.Log("el centro rojo (center_red): " + centerRed);

                // Si existe un centro previo, calcular la distancia entre los dos centros
                if (prevCenterRed != null)
                {
                    double distance = Math.Sqrt(Math.Pow(centerRed.X - prevCenterRed.X, 2) + Math.Pow(centerRed.Y - prevCenterRed.Y, 2));
                    
                    // Si la distancia supera el umbral de velocidad, asignar true a la variable booleana y cambiar el color del mensaje a rojo
                    if (distance > speedThreshold)
                    {
                        hayMovimientoRapidoRojo = true;
                        Debug.Log("La distancia recorrida por el objeto es: " + distance);
                        color = Scalar.Red;
                        Cv2.PutText(frame, message, position, font, size, color);
                    }
                    else
                    {
                        hayMovimientoRapidoRojo = false;
                    }
                }
                // Actualizar el centro previo con el actual
                prevCenterRed = centerRed;
            }
        }
        return redMask;
    }

    void OnDestroy()
    {
        // Liberar los recursos
        webcamTexture.Stop();
        backgroundSubtractor.Dispose();
        kernel.Dispose();
        prevFrame.Dispose();
        Cv2.DestroyAllWindows();
    }
}
