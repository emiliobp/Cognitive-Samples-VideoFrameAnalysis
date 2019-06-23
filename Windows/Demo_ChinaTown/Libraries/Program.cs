using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Demo_ChinaTown.Libraries
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("MainProgram.cs");

        // Initialize Instances
        SpeechToLuis speechToLuis = new SpeechToLuis();
        TextToSpeech textToSpeech = new TextToSpeech();

        public async Task UserFound()
        {
            String luisResult = "";
            do
            {
                String mensajeTemp = $"Dime en que te puedo ayudar ";

                log.Debug("Initialize Text to Speech services");
                await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                log.Debug($"Finished Text to Speech");

                log.Debug(" Initialize Speech to LUIS services");
                luisResult = await speechToLuis.RecognitionWithMicrophoneUsingLanguageAsync();
                log.Error(luisResult);
                


                //  Verifying Ubicacion as Intent
                if (luisResult.Equals("Ubicacion"))
                {
                    log.Debug("Intent identified as  'Ubicacion'");
                    mensajeTemp = $"Starbucks se encuentra en el segundo piso, a lado de Zara ";

                    log.Debug("Initialize Text to Speech services");
                    await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                    log.Debug($"Finished Text to Speech");

                    //  Sets RightImage to visible from another thread by invoking the dispatcher
                    try
                    {
                        Demo_ChinaTown.MainWindow.AppWindow.RightImage.Dispatcher.Invoke((Action)delegate
                        {
                            Demo_ChinaTown.MainWindow.AppWindow.RightImage.Visibility = Visibility.Visible;
                        });
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Exception for show map: {ex}");
                        throw;
                    }
                } else if (luisResult.Equals("Descuento"))
                {
                    log.Debug("Intent identified as  'Descuento'");
                    mensajeTemp = $"Hoy contamos con descuentos en Zara, toda la ropa de verano a 50%. H&M 30% y Massimo Duti a 15% en toda la tienda";

                    log.Debug("Initialize Text to Speech services");
                    await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                    log.Debug($"Finished Text to Speech");

                } else if (luisResult.Equals("Compras"))
                {
                    log.Debug("Intent identified as  'Compras'");
                    mensajeTemp = $"Contamos con tres zapaterias, Aldo, Taf y Loly in the sky. Ubicadas en el segundo piso";

                    log.Debug("Initialize Text to Speech services");
                    await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                    log.Debug($"Finished Text to Speech");

                    Demo_ChinaTown.MainWindow.AppWindow.RightImage.Dispatcher.Invoke((Action)delegate
                    {
                        Demo_ChinaTown.MainWindow.AppWindow.RightImage.Visibility = Visibility.Visible;
                    });
                } else if (luisResult.Equals("Despedida"))
                {
                    log.Debug("Intent identified as  'Despedida'");
                    mensajeTemp = $"Hasta pronto";

                    log.Debug("Initialize Text to Speech services");
                    await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                    log.Debug($"Finished Text to Speech");
                }
                else
                {
                    log.Debug("Intent not recognized");
                    mensajeTemp = $"No entendi";

                    log.Debug("Initialize Text to Speech services");
                    await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                    log.Debug($"Finished Text to Speech");
                }
            } while (luisResult != "Despedida");
            //  Set image to hidden
            Demo_ChinaTown.MainWindow.AppWindow.RightImage.Dispatcher.Invoke((Action)delegate
            {
                Demo_ChinaTown.MainWindow.AppWindow.RightImage.Visibility = Visibility.Hidden;
            });
        }
    }
}
