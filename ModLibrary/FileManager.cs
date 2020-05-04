using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;
using TaleWorlds.Core;

namespace ModLibrary
{
    public class FileManager
    {
        #region Singleton

        private FileManager() { }

        static FileManager()
        {
            Instance = new FileManager();
        }

        public static FileManager Instance { get; private set; }

        #endregion

        public void Save<T>(T data, string directoryPath, string fileName)
        {
            try
            {
                var securityRules = new DirectorySecurity();
                securityRules.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), FileSystemRights.FullControl, AccessControlType.Allow));

                Directory.CreateDirectory(directoryPath, securityRules);
                var filePath = Path.Combine(directoryPath, $"{fileName}.xml");

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(fileStream, data);
                }
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Could not save file '{fileName}'!", ColorManager.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorManager.Red));
            }
        }

        public T Load<T>(string directoryPath, string fileName)
        {
            try
            {
                var filePath = Path.Combine(directoryPath, $"{fileName}.xml");
                if (!File.Exists(filePath))
                {
                    this.Save<T>(default, directoryPath, fileName);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(fileStream);
                }
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Could not load file '{fileName}'! Using default model.", ColorManager.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorManager.Red));

                return (T)Activator.CreateInstance(typeof(T));
            }
        }
    }
}