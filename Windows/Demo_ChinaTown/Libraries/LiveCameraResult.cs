using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_ChinaTown.Libraries
{
    class LiveCameraResult
    {
        //  Initializing Logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("LiveCameraResult.cs");

        //  Initializing Face API
        private const string subscriptionKey = "fa6122356e204afea55c8c590b3caa82";
        private const string faceEndpoint = "https://southcentralus.api.cognitive.microsoft.com";

        public Microsoft.ProjectOxford.Face.Contract.Face[] Faces { get; set; } = null;

        public IList<Microsoft.Azure.CognitiveServices.Vision.Face.Models.DetectedFace> UserFace { get; set; }

        public string groupPersonId { get; set; }

        private static IFaceClient _faceClient;
        public IFaceClient faceClient
        {
            get => _faceClient;
        }
        static LiveCameraResult()
        {
            _faceClient = new FaceClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            //  Face API Initiating endpoint
            if (Uri.IsWellFormedUriString(faceEndpoint, UriKind.Absolute))
            {
                _faceClient.Endpoint = faceEndpoint;
                log.Info("Endpont Initiated");
            }
            else
            {
                log.Error($"{faceEndpoint} Invalid URI");
                Environment.Exit(0);
            }
        }
    }
}
