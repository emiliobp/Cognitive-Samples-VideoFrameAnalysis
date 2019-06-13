using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_ChinaTown.Libraries
{
    class FaceAnalysisLibrary
    {

        class Check
        {
            private bool _updating = false;
            private bool _interaction = false;
            public bool updating
            {
                get
                {
                    return _updating;
                }
            }

            public bool interaction
            {
                get
                {
                    return _interaction;
                }
            }

            public void checkit(bool _chk)
            {
                _updating = _chk;
            }

            public void FinishInteraction(bool _inter)
            {
                _interaction = _inter;
            }
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("FaceAnalysisLibrary.cs");
        private static readonly ImageEncodingParam[] s_jpegParams = {
            new ImageEncodingParam(ImwriteFlags.JpegQuality, 60)
        };

        

        // Initialize Instances
        readonly LiveCameraResult props = new LiveCameraResult();
        SpeechToLuis speechToLuis = new SpeechToLuis();
        TextToSpeech textToSpeech = new TextToSpeech();
        Program mainProgram = new Program();
        GroupPersonLibrary identify = new GroupPersonLibrary();
        private Check chk = new Check();


        /// <summary>
        /// Method to identify a person based on the livestream attributes
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public async Task<LiveCameraResult> FacesAnalysisFunction(VideoFrame frame)
        {

            log.Info("Enter - Face Analysis Function");

            

            string groupPersonId = props.groupPersonId;
            IList<DetectedFace> faces = new List<DetectedFace>();
            // Encode image. 
            var jpg = frame.Image.ToMemoryStream(".jpg", s_jpegParams);
            // Submit image to API. 

            IList<FaceAttributeType> faceAttributes =
                new FaceAttributeType[]
                {
                    FaceAttributeType.Gender, FaceAttributeType.Age,
                    FaceAttributeType.Smile, FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses, FaceAttributeType.Hair
                };
            faces = await props.faceClient.Face.DetectWithStreamAsync(jpg, true, false, faceAttributes);

            foreach (var ls in faces)
            {
                log.Debug($"Face Attributes: Age:{ls.FaceAttributes.Age} Gender:{ls.FaceAttributes.Gender}");
            }

            string messageIdentify = await identify.IdentifyPerson(groupPersonId, faces);

            log.Debug($"Message of user identified: {messageIdentify}");

            log.Debug($"Interaction Flag: {chk.interaction}");
            if (chk.interaction)
            {
                chk.checkit(false);
                chk.FinishInteraction(false);
            }


            log.Debug($"Updating check value 1: {chk.updating}");
            if (chk.updating) return new LiveCameraResult
            {
                UserFace = faces
            };

            //  else if updating is false
            UserIdentifyCheck(messageIdentify);

            log.Debug($"Updating check value 2: {chk.updating}");

            return new LiveCameraResult
            {
                UserFace = faces
            };

        }

        private async Task UserIdentifyCheck(String messageIdentify)
        {
            chk.checkit(true);

            if (messageIdentify != null)
            {
                String mensajeTemp = $"Hola {messageIdentify}";

                log.Debug("Initialize Text to Speech services");
                await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                log.Debug($"Finished Text to Speech");

                await mainProgram.UserFound();
                chk.FinishInteraction(true);
            }
            else if (messageIdentify.Equals("No one identified"))
            {
                String mensajeTemp = $"Hola no nos conocemos soy Wong assistente de Chinatown";

                log.Debug("Initialize Text to Speech services");
                await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);

                mensajeTemp = "Como te llamas tu?, dime Me llamo, y tu nombre";
                await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                log.Debug($"Finished Text to Speech");

                //  Poner regreso de nombre de LUIS crear nueva entity para nombre o usar Speech to Text y no Speech to LUIS

            }
        }
    }
}
