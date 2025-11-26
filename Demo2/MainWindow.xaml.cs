using MFace.Tool.Common.Interfaces;
using MFace.Tool.MFaceProxy;
using MFace.Tool.MFaceProxy.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Demo2
{
    public partial class MainWindow : Window
    {

        private SensorWrapper sensorWrapper;
        private SimpleLogger logger;
        private byte[] referenceImageBytes;
        private string captureToken;
        private const string ENDPOINT_NAME = "MFaceSensorService";
        private const float MATCH_THRESHOLD = 0.5f; 
        public MainWindow()
        {
            InitializeComponent();
            AutomaticStartCapture();
        }

        public async void AutomaticStartCapture() 
        {
            ShowLoading(true);
            await Task.Run(() => InitializeSensor());
            //ShowLoading(false);
            //ShowLoading(true);
            await Task.Run(() => CaptureAndCompare());
            ShowLoading(false);
        }
        private void LoadReferenceImage()
        {
            try
            {
                
                var uri = new Uri("pack://application:,,,/Resources/Face1.jpg");
                var bitmap = new BitmapImage(uri);
                ReferenceImage.Source = bitmap;

                // Convertir a bytes para la comparación
                referenceImageBytes = BitmapToBytes(bitmap);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar imagen de referencia: {ex.Message}\n\n","Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void InitButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading(true);
            await Task.Run(() => InitializeSensor());
            ShowLoading(false);
        }

        private void InitializeSensor()
        {
            try
            {
                Dispatcher.Invoke(() => UpdateResult("Inicializando sensor...", Colors.LightBlue));

                // Crear el wrapper del sensor (versión 2)
                logger = new SimpleLogger();
                sensorWrapper = new SensorWrapper(logger, 2, ENDPOINT_NAME);

                // Inicializar el sensor
                bool initialized = sensorWrapper.Initialize();

                if (initialized)
                {
                    Dispatcher.Invoke(() =>
                    {
                        UpdateResult("Sensor inicializado correctamente", Colors.LightGreen);
                        CaptureButton.IsEnabled = true;
                        InitButton.IsEnabled = false;
                    });
                }
                else
                {
                    throw new Exception("No se pudo inicializar el sensor");
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateResult($"Error: {ex.Message}", Colors.LightCoral);
                    MessageBox.Show($"Error al inicializar MFace:\n{ex.Message}\n\n" ,                                 
                                   "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading(true);
            await Task.Run(() => CaptureAndCompare());
            ShowLoading(false);
        }

        private void CaptureAndCompare()
        {
            try
            {
                string ruta = string.Empty;
                Dispatcher.Invoke(() => UpdateResult("Capturando imagen...", Colors.LightBlue));

                                           //StartCapture(CaptureMode.SingleClosestFace, -1, out captureToken);
                captureToken = sensorWrapper.StartCapture(30000, MFace.Tool.MFaceProxy.Enumerations.CaptureMode.SingleClosestFace);

                Dispatcher.Invoke(() => UpdateResult("Esperando detección facial...", Colors.LightYellow));
               
                ;
                TimeSpan duration;
                var personDataList = sensorWrapper.GetCaptureResult(
                    captureToken,
                    30000,  // captureTimeout
                    true,   // autoStopCapture
                    true,  // getCroppedImage
                    true,   // getTemplate
                    true,   // getImage
                    out duration
                );

                if (personDataList != null && personDataList.Count > 0)
                {
                    var personData = personDataList[0];

                  
                    if (personData.Image.Content.Data != null && personData.Image.Content.Data.Length > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var bitmap = BytesToBitmap(personData.Image.Content.Data);
                            CapturedImage.Source = bitmap;
                        });

                        ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                               $"captura_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");

                        SaveImage(personData.Image.Content.Data, ruta);
                    }
                   

                    Dispatcher.Invoke(() => UpdateResult("Comparando imágenes...", Colors.LightBlue));
                    try
                    {
                        //Todo Alejandro stopper n1
                        //sensorWrapper.Uninitialize();

                        referenceImageBytes = File.ReadAllBytes(ruta);

                        var verifyResponse = sensorWrapper.Verify(
                        captureToken,
                        Guid.NewGuid().ToString(),
                        SourceType.Live,
                        referenceImageBytes,
                        "image/jpeg"
                    );
                        
                      
                        float matchingScore = verifyResponse.MatchingScore;
                        bool isMatch = matchingScore >= MATCH_THRESHOLD;

                      

                        Dispatcher.Invoke(() =>
                        {
                            LoadReferenceImage();
                            if (isMatch)
                            {
                                UpdateResult("COINCIDENCIA DETECTADA", Colors.LightGreen);
                                ScoreText.Text = $"Puntuación: {matchingScore:P1}";
                            }
                            else
                            {
                                UpdateResult("NO HAY COINCIDENCIA", Colors.LightCoral);
                                ScoreText.Text = $"Puntuación: {matchingScore:P1} (Umbral: {MATCH_THRESHOLD:P1})";
                            }

                        });
                    }
                    catch (Exception exx)
                    {

                       Logger.Log(exx);
                    }


                    
                }
                else
                {
                    throw new Exception("No se detectó ningún rostro");
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateResult($"Error: {ex.Message}", Colors.LightCoral);
                    MessageBox.Show($"Error durante la captura:\n{ex.Message}",
                                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });

                Logger.Log(ex);
            }
        }

        private void UpdateResult(string message, Color backgroundColor)
        {
            ResultText.Text = message;
            ResultBorder.Background = new SolidColorBrush(backgroundColor);
        }

        private void ShowLoading(bool show)
        {
            LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            InitButton.IsEnabled = !show;
            CaptureButton.IsEnabled = !show && sensorWrapper != null && sensorWrapper.IsInitialize;
        }

        private byte[] BitmapToBytes(BitmapImage bitmap)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                return ms.ToArray();
            }
        }

        private BitmapImage BytesToBitmap(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sensorWrapper != null && sensorWrapper.IsInitialize)
                {
                    sensorWrapper.Uninitialize();
                }
            }
            catch { }

            Close();
        }

        private void SaveImage(byte[] imageBytes, string filePath)
        {
            File.WriteAllBytes(filePath, imageBytes);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (sensorWrapper != null && sensorWrapper.IsInitialize)
                {
                    sensorWrapper.Uninitialize();
                }
            }
            catch { }

            base.OnClosing(e);
        }
    }

    public class SimpleLogger : ILogger
    {
        public void Log(LogLevel level, string message, string methodName = "::")
        {
            System.Diagnostics.Debug.WriteLine($"[{level}] {methodName}: {message}");
        }
    }
}
