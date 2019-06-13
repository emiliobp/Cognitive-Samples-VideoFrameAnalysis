using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                }
                else
                {
                    log.Debug("Intent not Ubicacion");
                    mensajeTemp = $"No entendi";

                    log.Debug("Initialize Text to Speech services");
                    await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                    log.Debug($"Finished Text to Speech");
                }
            } while (luisResult != "Ubicacion");
        }
    }
}
