using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Demo_ChinaTown.Libraries
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("MainProgram.cs");

        //  Global Variable
        private String uri_image = "C:\\Users\\daniel\\Desktop\\Demo_Images\\";
        private String starbucksImage = "StarbucksMap.jpg";
        private String zapateriaImage = "ZapateriaMap.jpg";
        private String descuentosImage = "Discounts.jpg";

        // Initialize Instances
        SpeechToLuis speechToLuis = new SpeechToLuis();
        TextToSpeech textToSpeech = new TextToSpeech();

        public async Task UserFound()
        {
            //  image to load into left image 
            BitmapImage logo = new BitmapImage();


            String luisResult = "";
            do
            {
                String mensajeTemp = $"Dime en que te puedo ayudar ";

                log.Debug("Initialize Text to Speech services");
                await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                log.Debug($"Finished Text to Speech");

                //  Updating MessageArea
                Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Dispatcher.Invoke((Action)delegate
                {
                    Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Text += $"\nWong: {mensajeTemp}";
                });

                log.Debug(" Initialize Speech to LUIS services");
                luisResult = await speechToLuis.RecognitionWithMicrophoneUsingLanguageAsync();
                log.Error(luisResult);

                //  Updating MessageArea
                Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Dispatcher.Invoke((Action)delegate
                {
                    Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Text += $"\nUsuario: {mensajeTemp}";
                });



                //  Verifying Ubicacion as Intent
                if (luisResult.Equals("Ubicacion"))
                {
                    log.Debug("Intent identified as  'Ubicacion'");
                    mensajeTemp = $"Starbucks se encuentra en el segundo piso, a lado de Zara ";

                    //  Sets RightImage to visible from another thread by invoking the dispatcher
                    try
                    {
                        //  setting image and making it visible
                        Demo_ChinaTown.MainWindow.AppWindow.RightImage.Dispatcher.Invoke((Action)delegate
                        {
                            Demo_ChinaTown.MainWindow.AppWindow.RightImage.Source = new BitmapImage(new Uri(uri_image + starbucksImage));
                            Demo_ChinaTown.MainWindow.AppWindow.RightImage.Visibility = Visibility.Visible;
                        });

                        //  Updating MessageArea
                        Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Dispatcher.Invoke((Action)delegate
                        {
                            Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Text += $"\nWong: {mensajeTemp}";
                        });

                        log.Debug("Initialize Text to Speech services");
                        await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                        log.Debug($"Finished Text to Speech");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Exception for show map: {ex}");
                        throw;
                    }
                } else if (luisResult.Equals("Descuento"))
                {
                    log.Debug("Intent identified as  'Descuento'");
                    mensajeTemp = $"Hoy contamos diversos descuentos en el area de comida. En Zara, toda la ropa de verano a 25%. " +
                                    $"Massimo Dutti camisas a 25% y Banana Republic a 25% en toda la tienda";

                    try
                    {
                        //  setting image and making it visible
                        Demo_ChinaTown.MainWindow.AppWindow.RightImage.Dispatcher.Invoke((Action)delegate
                        {
                            Demo_ChinaTown.MainWindow.AppWindow.RightImage.Source = new BitmapImage(new Uri(uri_image + descuentosImage));
                            Demo_ChinaTown.MainWindow.AppWindow.RightImage.Visibility = Visibility.Visible;
                        });


                        //  Updating MessageArea
                        Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Dispatcher.Invoke((Action)delegate
                        {
                            Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Text += $"\nWong: {mensajeTemp}";
                        });

                        log.Debug("Initialize Text to Speech services");
                        await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                        log.Debug($"Finished Text to Speech");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Exception for show map: {ex}");
                        throw;
                    }
                } else if (luisResult.Equals("Compras"))
                {
                    log.Debug("Intent identified as  'Compras'");
                    mensajeTemp = $"Contamos con tres zapaterias, Aldo, Steve Madden y Loly in the sky. Ubicadas en el segundo piso";

                    try
                    {
                        //  setting image and making it visible
                        Demo_ChinaTown.MainWindow.AppWindow.RightImage.Dispatcher.Invoke((Action)delegate
                        {
                            Demo_ChinaTown.MainWindow.AppWindow.RightImage.Source = new BitmapImage(new Uri(uri_image + zapateriaImage));
                            Demo_ChinaTown.MainWindow.AppWindow.RightImage.Visibility = Visibility.Visible;
                        });

                        //  Updating MessageArea
                        Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Dispatcher.Invoke((Action)delegate
                        {
                            Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Text += $"\nWong: {mensajeTemp}";
                        });

                        log.Debug("Initialize Text to Speech services");
                        await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                        log.Debug($"Finished Text to Speech");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Exception for show map: {ex}");
                        throw;
                    }
                } else if (luisResult.Equals("Despedida"))
                {
                    log.Debug("Intent identified as  'Despedida'");
                    mensajeTemp = $"Hasta pronto";

                    //  Updating MessageArea
                    Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Dispatcher.Invoke((Action)delegate
                    {
                        Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Text += $"\nWong: {mensajeTemp}";
                    });

                    log.Debug("Initialize Text to Speech services");
                    await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                    log.Debug($"Finished Text to Speech");
                }
                else
                {
                    log.Debug("Intent not recognized");
                    mensajeTemp = $"No entendi";

                    //  Updating MessageArea
                    Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Dispatcher.Invoke((Action)delegate
                    {
                        Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Text += $"\nWong: {mensajeTemp}";
                    });

                    log.Debug("Initialize Text to Speech services");
                    await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                    log.Debug($"Finished Text to Speech");

                    Demo_ChinaTown.MainWindow.AppWindow.RightImage.Dispatcher.Invoke((Action)delegate
                    {
                        Demo_ChinaTown.MainWindow.AppWindow.RightImage.Visibility = Visibility.Hidden;
                    });
                }
            } while (luisResult != "Despedida");
            //  Set image to hidden
            Demo_ChinaTown.MainWindow.AppWindow.RightImage.Dispatcher.Invoke((Action)delegate
            {
                Demo_ChinaTown.MainWindow.AppWindow.RightImage.Visibility = Visibility.Hidden;
            });

            //  Updating MessageArea
            Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Dispatcher.Invoke((Action)delegate
            {
                Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Text = "";
            });
        }
    }
}
