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
        public Microsoft.ProjectOxford.Face.Contract.Face[] Faces { get; set; } = null;

        public IList<Microsoft.Azure.CognitiveServices.Vision.Face.Models.DetectedFace> UserFace { get; set; }

        public string groupPersonId { get; set; }

        public IFaceClient faceClient { get;  set; }
        public void FaceClientInit(string subscriptionKey)
        {
            faceClient = new FaceClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });
        }
    }
}
