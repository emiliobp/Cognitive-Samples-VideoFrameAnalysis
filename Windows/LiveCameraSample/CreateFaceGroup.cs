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
    public class CreateFaceGroup
    {
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
                IList<Person> listGroup;
                try
                {
                    writeback = "";
                    listGroup = await faceClient.PersonGroupPerson.ListAsync(personGroupId);
                    foreach (var list in listGroup)
                    {
                        writeback += $"value: {list.Name}: {list.PersonId} ";
                        // await faceClient.PersonGroupPerson.DeleteAsync(personGroupId, list.PersonId);
                    }
                    if (listGroup.Count().Equals(0))
                    {
                        writeback = listGroup.Count().ToString();
                        Person friend1 = await faceClient.PersonGroupPerson.CreateAsync(personGroupId, "Daniel");
                        //Daniel: d201468d-0635-49ea-bd93-1bf1d4ab9fbb

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
                }
                catch (Exception e)
                {
                    //await faceClient.PersonGroupPerson.CreateAsync(personGroupId, "Daniel");
                    //writeback = "New person created";
                    writeback += $"Error: {e}";
                }
            }
            return writeback;
        }
    }
}
