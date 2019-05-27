using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacialRecognition.Libraries
{
    class LiveCameraResult
    {
        public Microsoft.ProjectOxford.Face.Contract.Face[] Faces { get; set; } = null;

        public IList<Microsoft.Azure.CognitiveServices.Vision.Face.Models.DetectedFace> UserFace { get; set; }

        public string groupPersonId { get; set; }
    }
}
