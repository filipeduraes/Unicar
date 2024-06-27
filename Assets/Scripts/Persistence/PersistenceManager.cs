using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Unicar.Persistence
{
    public static class PersistenceManager
    {
        private static string PersistentPath => $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}Data";
        private static string ProfileDataPath => $"{PersistentPath}{Path.DirectorySeparatorChar}Profiles";

        public static bool DataIsBeingSaved { get; private set; } = false;
        
        public static async UniTask<bool> SaveProfileAsync(ProfileData profileData)
        {
            DataIsBeingSaved = true;
            string jsonProfile = JsonConvert.SerializeObject(profileData);
            await using FileStream fileStream = new(GetProfileFilePath(), FileMode.OpenOrCreate);

            byte[] buffer = Encoding.UTF8.GetBytes(jsonProfile);
            await fileStream.WriteAsync(buffer);
            fileStream.Close();
            DataIsBeingSaved = false;
            return true;
        }

        public static async UniTask<ProfileData> FindProfileWithDataAsync(ProfileCredentials credentials)
        {
            ProfileData[] profiles = await LoadAllProfilesAsync();
            return profiles.FirstOrDefault(p => p.MatchCredentials(credentials));
        }
        
        public static async UniTask<bool> CheckIfProfileIsUniqueAsync(ProfileData profileData)
        {
            ProfileData[] profilesData = await LoadAllProfilesAsync();
            return profilesData.All(data => !data.MatchEmails(profileData));
        }

        private static string GetProfileFilePath()
        {
            if (!Directory.Exists(ProfileDataPath))
                Directory.CreateDirectory(ProfileDataPath);
            
            string[] paths = Directory.GetFiles(ProfileDataPath);
            return $"{ProfileDataPath}{Path.DirectorySeparatorChar}{paths.Length}";
        }
        
        private static async UniTask<ProfileData[]> LoadAllProfilesAsync()
        {
            string[] profilePaths = Directory.GetFiles(ProfileDataPath);
            UniTask<ProfileData>[] profileLoadTasks = new UniTask<ProfileData>[profilePaths.Length];

            for (int index = 0; index < profilePaths.Length; index++)
            {
                UniTask<ProfileData> loadTask = LoadProfileAsync(profilePaths[index]);
                profileLoadTasks[index] = loadTask;
            }

            return await UniTask.WhenAll(profileLoadTasks);
        }

        private static async UniTask<ProfileData> LoadProfileAsync(string path)
        {
            await using FileStream fileStream = new(path, FileMode.Open);
            byte[] buffer = new byte[fileStream.Length];
            int length = await fileStream.ReadAsync(buffer);
            fileStream.Close();

            string json = Encoding.UTF8.GetString(buffer, 0, length);
            ProfileData profileData = JsonConvert.DeserializeObject<ProfileData>(json);
            return profileData;
        }
    }
}