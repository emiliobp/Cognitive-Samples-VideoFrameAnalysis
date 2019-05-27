using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacialRecognition.Libraries
{
    class FaceAnalysisLibrary
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("FaceAnalysisLibrary.cs");
        private static readonly MainWindow faceAPI = new MainWindow();
        private static readonly ImageEncodingParam[] s_jpegParams = {
            new ImageEncodingParam(ImwriteFlags.JpegQuality, 60)
        };

        // Initialize Instances
        LiveCameraResult props = new LiveCameraResult();

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
            faces = await faceAPI.faceClient.Face.DetectWithStreamAsync(jpg, true, false, faceAttributes);

            foreach (var ls in faces)
            {
                log.Debug($"Face Attributes: Age:{ls.FaceAttributes.Age} Gender:{ls.FaceAttributes.Gender}");
            }
            GroupPersonLibrary identify = new GroupPersonLibrary();
            string messageIdentify = await identify.IdentifyPerson(groupPersonId, faces);

            log.Debug($"Message of user identified: {messageIdentify}");

            return new LiveCameraResult
            {
                UserFace = faces
            };

        }
    }
}
