using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_ChinaTown.Libraries
{
    class GroupPersonLibrary
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("GroupPersonLibrary.cs");
        const string friend1ImageDir = @"C:\Users\daniel\Desktop\test";

        // Initialize Instances
        LiveCameraResult props = new LiveCameraResult();

        /// <summary>
        /// Method verifies if group entered to be created exists
        /// </summary>
        /// <param name="personGroupId"></param>
        /// <returns>string depending if group already exists or if it was created</returns>
        public async Task<string> VerifyGroupExist(string personGroupId)
        {
            log.Info("Enter - VerifyGroupExist Method");
            try
            {
                log.Debug("Verifying if Person Group Id exist");
                await props.faceClient.PersonGroup.GetAsync(personGroupId);
                return "Group already exists";
            }
            catch (Exception e)
            {
                log.Debug("Person Group Id didn't exist");
                if (e.Message != "NotFound")
                {
                    log.Debug("Person Group will be created");
                    await CreateGroup(personGroupId);
                    return "New Group created";
                }
            }
            return null;
        }

        /// <summary>
        /// Method to create new group
        /// </summary>
        /// <param name="personGroupId"></param>
        /// <returns></returns>
        private async Task CreateGroup(string personGroupId)
        {
            log.Info("Enter - Creation of Group");
            try
            {
                await props.faceClient.PersonGroup.CreateAsync(personGroupId, personGroupId);
            }
            catch (Exception)
            {
                log.Error($"Error creating group");
                throw;
            }
        }

        /// <summary>
        /// Method to delete groups
        /// </summary>
        /// <param name="personGroupId"></param>
        /// <returns></returns>
        public async Task DeleteGroup(string personGroupId)
        {
            log.Info("Enter - Deletion of Group");
            try
            {
                await props.faceClient.PersonGroup.DeleteAsync(personGroupId);
            }
            catch (Exception)
            {
                log.Error($"Error deleting group");
                throw;
            }
        }

        /// <summary>
        /// Method to get all the people in a group
        /// </summary>
        /// <param name="personGroupId"></param>
        /// <returns>IList of people 'listGroup'</returns>
        private async Task<IList<Person>> GetGroupList(string personGroupId)
        {
            log.Info("Enter - Get People in Group");

            IList<Person> listGroup;

            try
            {
                listGroup = await props.faceClient.PersonGroupPerson.ListAsync(personGroupId);
            }
            catch (Exception)
            {
                log.Error($"ERROR - Getting People from group: {personGroupId}");
                throw;
            }            
            return listGroup;
        }

        /// <summary>
        /// Method to perform training of a given group
        /// </summary>
        /// <param name="personGroupId"></param>
        /// <returns>String message that indicates training was succesful or not</returns>
        public async Task<string> TrainGroup(string personGroupId)
        {
            log.Info("Enter - Training Group Method");

            string writeBack = "Training not succesful";
            await props.faceClient.PersonGroup.TrainAsync(personGroupId);

            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await props.faceClient.PersonGroup.GetTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != TrainingStatusType.Running)
                {
                    log.Debug("Training successful");
                    writeBack = "Training Successful";
                    break;
                }

                await Task.Delay(1000);
            }
            return writeBack;
        }

        /// <summary>
        /// Method to create a person, references method AddPicsToFace to associate the face_id to the pictures
        /// </summary>
        /// <param name="personGroupId"></param>
        /// <returns></returns>
        public async Task CreatePerson (string personGroupId)
        {
            log.Info($"Enter - Create Person in {personGroupId}");
            Person friend1 = await props.faceClient.PersonGroupPerson.CreateAsync(personGroupId, "Daniel2");

            //  Adding the pictures taken to the newly created person
            await AddPicsToFace(personGroupId, friend1);
        }

        /// <summary>
        /// Method to delete all people in a given group
        /// </summary>
        /// <param name="personGroupId"></param>
        /// <returns></returns>
        public async Task DeletePerson (string personGroupId)
        {
            log.Info("Enter - Delete People in Group");

            IList<Person> listGroup = await GetGroupList(personGroupId);

            foreach (var list in listGroup)
            {
                log.Debug($"People to be deleted value: {list.Name}: {list.PersonId} ");
                await props.faceClient.PersonGroupPerson.DeleteAsync(personGroupId, list.PersonId);
            }
        }

        /// <summary>
        /// Method to associate pics to face_id
        /// </summary>
        /// <param name="personGroupId"></param>
        /// <param name="friend1"></param>
        /// <returns></returns>
        private async Task AddPicsToFace(string personGroupId, Person friend1)
        {
            log.Info("Enter - Associating Pics to Person");

            foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await props.faceClient.PersonGroupPerson.AddFaceFromStreamAsync(
                        personGroupId, friend1.PersonId, s);

                    log.Info("Association completed");
                }
            }
        }

        /// <summary>
        /// Method to Identify a person from the frame
        /// </summary>
        /// <param name="personGroupId"></param>
        /// <param name="faces"></param>
        /// <returns>String indicating if null no person identified, if not person.name</returns>
        public async Task<string> IdentifyPerson(string personGroupId, IList<DetectedFace> faces)
        {
            log.Info("Enter - Identification of a person");

            string candidate = null;

            IList<Guid> faceId = faces.Select(face => face.FaceId.GetValueOrDefault()).ToList();

            var results = await props.faceClient.Face.IdentifyAsync(faceId, "myfriends");
            foreach (var identifyResult in results)
            {
                log.Debug($"Result of face: {identifyResult.FaceId} Confidence: {identifyResult.Candidates[0].Confidence}");
                if (identifyResult.Candidates.Count == 0)
                {
                    log.Debug("No one identified ");
                }
                else
                {
                    // Get top 1 among all candidates returned
                    var candidateId = identifyResult.Candidates[0].PersonId;
                    var person = await props.faceClient.PersonGroupPerson.GetAsync("myfriends", candidateId);
                    log.Debug($"Identified as {person.Name} ");

                    return person.Name;
                }
            }
            return candidate;
        }
    }
}
