using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCameraSample
{
    public class CreateGroupPerson
    {
        //  Provisional, se debe de cambiar por el stream del detect
        //readonly IList<Guid> faceIds = { "4dac9f8c-f13e-4a2b-b03c-3b46e6303d70" };
        private IList<Guid> faceId = new List<Guid>();

        // For example, subscriptionKey = "0123456789abcdef0123456789ABCDEF"
        private const string subscriptionKey = "fa6122356e204afea55c8c590b3caa82";

        private const string faceEndpoint = "https://southcentralus.api.cognitive.microsoft.com";

        const string friend1ImageDir = @"C:\Users\daniel\Desktop\test";

        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        public async Task<string> FaceGroup(string personalGroupID)
        {
            string writeBack = "";

            if (Uri.IsWellFormedUriString(faceEndpoint, UriKind.Absolute))
            {
                faceClient.Endpoint = faceEndpoint;
            }
            else
            {
                writeBack = ($"{faceEndpoint},Invalid URI ");
                Environment.Exit(0);
            }


            writeBack = await VerifyFaceGroupExist(personalGroupID);

            return writeBack;
        }

        private async Task<string> VerifyFaceGroupExist(string personalGroupID)
        {
            bool groupExists = false;
            string writeback = "";
            string personGroupId = "myfriends";

            try
            {
                await faceClient.PersonGroup.GetAsync(personGroupId);

                groupExists = true;
                writeback = "Found";
            }
            catch (Exception e)
            {
                if (e.Message != "NotFound")
                {
                    writeback = $"Error: {e}";
                    await faceClient.PersonGroup.CreateAsync(personGroupId, "My Friends");
                }
            }

            if (groupExists)
            {
                try
                {
                    writeback = "";
                    //writeback = await GetGroupList(personGroupId);

                    //  Daniel: d201468d-0635-49ea-bd93-1bf1d4ab9fbb
                    //  Daniel2: 4dac9f8c-f13e-4a2b-b03c-3b46e6303d70

                    /************************Metodos para crear una nueva persona****************************************/
                    // Person friend1 = await faceClient.PersonGroupPerson.CreateAsync(personGroupId, "Daniel2");
                    //await AddPicsToFace(personGroupId, friend1);

                    /************************Metodos para entrenar una persona****************************************/
                    //writeback += await GetGroupList(personGroupId);
                    //writeback = await TrainGroup(personGroupId);

                    /************************Metodos para identificar una persona****************************************/
                    //writeback += await IdentifyPerson(personGroupId);
                }
                catch (Exception e)
                {
                    //await faceClient.PersonGroupPerson.CreateAsync(personGroupId, "Daniel");
                    //writeback = "New person created";
                    writeback += $" ****ERROR: {e}";
                }
            }
            return writeback;
        }

        private async Task AddPicsToFace(string personGroupId, Person friend1)
        {
            foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(
                        personGroupId, friend1.PersonId, s);
                }
            }
        }
        private async Task<string> GetGroupList(string personGroupId)
        {
            string writeBack = "";
            IList<Person> listGroup;
            listGroup = await faceClient.PersonGroupPerson.ListAsync(personGroupId);
            foreach (var list in listGroup)
            {
                writeBack += $"value: {list.Name}: {list.PersonId} ";
                // await faceClient.PersonGroupPerson.DeleteAsync(personGroupId, list.PersonId);
            }
            return writeBack;
        }

        private async Task<string> TrainGroup(string personGroupId)
        {
            string writeBack = "";
            await faceClient.PersonGroup.TrainAsync(personGroupId);

            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceClient.PersonGroup.GetTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != TrainingStatusType.Running)
                {
                    writeBack = "Training Successfull";
                    break;
                }

                await Task.Delay(1000);
            }
            return writeBack;
        }

        private async Task<string> IdentifyPerson (string personGroupId)
        {
            string writeBack = "";

            IList<IdentifyResult> result = new List<IdentifyResult>();

            LiveCameraResult lc = new LiveCameraResult();
            writeBack = $"Face ID: {lc.UserFace.ToString()}";

            //IList<Guid> faceId = lc.UserFace.Select(face => face.FaceId.GetValueOrDefault()).ToList();

            //var results = await faceClient.Face.IdentifyAsync(faceId, personGroupId);
            //foreach (var identifyResult in results)
            //{
            //    writeBack += ($"Result of face: {identifyResult.FaceId} ");
            //    if (identifyResult.Candidates.Count == 0)
            //    {
            //        writeBack += "No one identified ";
            //    }
            //    else
            //    {
            //        // Get top 1 among all candidates returned
            //        var candidateId = identifyResult.Candidates[0].PersonId;
            //        var person = await faceClient.PersonGroupPerson.GetAsync(personGroupId, candidateId);
            //        writeBack += ($"Identified as {person.Name} ");
            //    }
            //}

            return writeBack;
        }
    }
}