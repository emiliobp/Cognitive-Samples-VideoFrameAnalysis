using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_ChinaTown.Libraries
{
    class TextToSpeech
    {
        //  Initializing Logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("TextToSpeech.cs");

        //  Speech Services Subscription
        string subscriptionKey = "7cbb492241b0479d9320b0b20355c8ca";
        string serviceRegion = "southcentralus";
        string SynthesisLanguage = "es-MX";
        string SynthesisVoice = "es-MX-Raul-Apollo";

        public async Task SynthesisToSpeakerAsync(String messageToSpeak)
        {
            log.Info("ENTER - SynthesisToSpeakerAsync");
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            // The default language is "en-us".
            var config = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);
            config.SpeechSynthesisLanguage = SynthesisLanguage;
            config.SpeechSynthesisVoiceName = SynthesisVoice;

            // Creates a speech synthesizer using the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                // Receive a text from console input and synthesize it to speaker.
                string text = messageToSpeak;
                log.Debug($"Texto a decir: {text}");

                using (var result = await synthesizer.SpeakTextAsync(text))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        log.Debug($"Speech synthesized to speaker for text [{text}]");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        log.Debug($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            log.Debug($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            log.Debug($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            log.Debug($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }
    }
}
