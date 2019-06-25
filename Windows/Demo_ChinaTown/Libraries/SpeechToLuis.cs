using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;

namespace Demo_ChinaTown.Libraries
{
    class SpeechToLuis
    {
        //  Initializing Logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SpeechToLuis.cs");

        //  LUIS Subscription
        string subscriptionKey = "b0c7e22eb55d4a5c8e36542cd4708668";
        string serviceRegion = "southcentralus";
        string appId = "5021ac1b-7bad-4bfe-a04f-a90222f30b11";

        // Intent recognition using microphone.
        public async Task<String> RecognitionWithMicrophoneAsync()
        {
            // <intentRecognitionWithMicrophone>
            // Creates an instance of a speech config with specified subscription key
            // and service region. Note that in contrast to other services supported by
            // the Cognitive Services Speech SDK, the Language Understanding service
            // requires a specific subscription key from https://www.luis.ai/.
            // The Language Understanding service calls the required key 'endpoint key'.
            // Once you've obtained it, replace with below with your own Language Understanding subscription key
            // and service region (e.g., "westus").
            // The default language is "en-us".
            var config = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);

            // Creates an intent recognizer using microphone as audio input.
            using (var recognizer = new IntentRecognizer(config))
            {
                // Creates a Language Understanding model using the app id, and adds specific intents from your model
                var model = LanguageUnderstandingModel.FromAppId("YourLanguageUnderstandingAppId");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName1", "id1");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName2", "id2");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName3", "any-IntentId-here");

                // Starts recognizing.
                log.Debug("Say something...");

                // Starts intent recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query. 
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                if (result.Reason == ResultReason.RecognizedIntent)
                {
                    log.Debug($"RECOGNIZED: Text={result.Text}");
                    log.Debug($"    Intent Id: {result.IntentId}.");
                    log.Debug($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");
                    return result.IntentId;
                }
                else if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    log.Debug($"RECOGNIZED: Text={result.Text}");
                    log.Debug($"    Intent not recognized.");
                    return null;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    log.Debug($"NOMATCH: Speech could not be recognized.");
                    return null;
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    log.Debug($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        log.Debug($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        log.Debug($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        log.Debug($"CANCELED: Did you update the subscription info?");
                    }
                    return null;
                }
                return null;
            }
            // </intentRecognitionWithMicrophone>
        }
              

        // Intent recognition in the specified language, using microphone.
        public async Task<String> RecognitionWithMicrophoneUsingLanguageAsync()
        {
            log.Info("ENTER - RecognitionWithMicrophoneUsingLanguageAsync");
            // Creates an instance of a speech config with specified subscription key
            // and service region. Note that in contrast to other services supported by
            // the Cognitive Services Speech SDK, the Language Understanding service
            // requires a specific subscription key from https://www.luis.ai/.
            // The Language Understanding service calls the required key 'endpoint key'.
            // Once you've obtained it, replace with below with your own Language Understanding subscription key
            // and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);
            var language = "es-mx";
            config.SpeechRecognitionLanguage = language;

            // Creates an intent recognizer in the specified language using microphone as audio input.
            using (var recognizer = new IntentRecognizer(config))
            {
                // Creates a Language Understanding model using the app id, and adds specific intents from your model
                var model = LanguageUnderstandingModel.FromAppId(appId);
                recognizer.AddIntent(model, "Greetings", "Greetings");
                recognizer.AddIntent(model, "None", "None");
                recognizer.AddIntent(model, "Ubicacion", "Ubicacion");
                recognizer.AddIntent(model, "Despedida", "Despedida");
                recognizer.AddIntent(model, "Compras", "Compras");
                recognizer.AddIntent(model, "Descuento", "Descuento");



                // Starts recognizing.
                log.Debug("Say something in " + language + "...");

                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query. 
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                if (result.Reason == ResultReason.RecognizedIntent)
                {
                    log.Debug($"RECOGNIZED: Text={result.Text}");
                    log.Debug($"    Intent Id: {result.IntentId}.");
                    log.Debug($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");

                    //  Updating MessageArea
                    Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Dispatcher.Invoke((Action)delegate
                    {
                        Demo_ChinaTown.MainWindow.AppWindow.MessageArea.Text += $"\nUsuario: {result.Text}";
                    });

                    return result.IntentId;
                }
                else if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    log.Debug($"RECOGNIZED: Text={result.Text}");
                    log.Debug($"    Intent not recognized.");
                    return "Intent not recognized.";
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    log.Debug($"NOMATCH: Speech could not be recognized.");
                    return "Speech could not be recognized.";
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    log.Debug($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        log.Debug($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        log.Debug($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        log.Debug($"CANCELED: Did you update the subscription info?");
                    }
                }
                return null;
            }
        }
    }
}
