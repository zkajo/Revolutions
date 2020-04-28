using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace ModLibrary.File
{
    public class FileManager
    {
        #region Singleton

        private static readonly FileManager instance;

        private FileManager() { }

        static FileManager()
        {
            FileManager.instance = new FileManager();
        }

        public static FileManager Instance => FileManager.instance;

		#endregion

		private const string SaveDirectory = "Save";
		private const string ConfigurationDirectory = "Configuration";

		public void Save<T>(T data, string fileName)
		{
			try
			{
				string filePath = Path.Combine(BasePath.Name, FileManager.ConfigurationDirectory, fileName);
				Directory.CreateDirectory(filePath);

				using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					xmlSerializer.Serialize(fileStream, data);
				}
			}
			catch (IOException)
			{
				InformationManager.DisplayMessage(new InformationMessage($"Could not create config-file '{fileName}'.", Color.Black, "Error"));
			}
		}

		public T Load<T>(string fileName)
		{
			try
			{
				string filePath = Path.Combine(BasePath.Name, FileManager.ConfigurationDirectory, fileName);
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					return (T)xmlSerializer.Deserialize(fileStream);
				}
			}
			catch (IOException)
			{
				InformationManager.DisplayMessage(new InformationMessage($"Could not load config-file '{fileName}'.", Color.Black, "Error"));
				return (T)Activator.CreateInstance(typeof(T));
			}
		}

		public void Serialize<T>(T data, string fileName)
		{
			try
			{
				string filePath = Path.Combine(BasePath.Name, FileManager.SaveDirectory, fileName);
				Directory.CreateDirectory(filePath);

				using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(fileStream, data);
				}
			}
			catch (IOException)
			{
				InformationManager.DisplayMessage(new InformationMessage($"Could not create save-file '{fileName}'.", Color.Black, "Error"));
			}
		}

		public T Deserialize<T>(string fileName)
		{
			try
			{
				string filePath = Path.Combine(BasePath.Name, FileManager.SaveDirectory, fileName);
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					return (T)binaryFormatter.Deserialize(fileStream);
				}
			}
			catch (IOException)
			{
				InformationManager.DisplayMessage(new InformationMessage($"Could not load save-file '{fileName}'.", Color.Black, "Error"));
				return (T)Activator.CreateInstance(typeof(T));
			}
		}
	}
}