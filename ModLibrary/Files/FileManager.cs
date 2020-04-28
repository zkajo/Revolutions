using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace ModLibrary.Files
{
	public class FileManager
	{
		#region Singleton


		private FileManager() { }

		static FileManager()
		{
			FileManager.Instance = new FileManager();
		}

		public static FileManager Instance { get; private set; }

		#endregion

		private const string SaveDirectory = "Save";
		private const string ConfigurationDirectory = "Configuration";

		public void Save<T>(T data, string basePath, string fileName)
		{
			try
			{
				string filePath = Path.Combine(basePath, ConfigurationDirectory, fileName);
				Directory.CreateDirectory(filePath);

				using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					xmlSerializer.Serialize(fileStream, data);
				}
			}
			catch (IOException)
			{
				InformationManager.DisplayMessage(new InformationMessage($"Could not create configuration file '{fileName}'.", Color.FromUint(4282569842U)));
			}
		}

		public T Load<T>(string basePath, string fileName)
		{
			try
			{
				string filePath = Path.Combine(basePath, ConfigurationDirectory, fileName);
				if (!File.Exists(filePath))
				{
					return default;
				}

				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					return (T)xmlSerializer.Deserialize(fileStream);
				}
			}
			catch (IOException)
			{
				InformationManager.DisplayMessage(new InformationMessage($"Could not load configuration file '{fileName}'.", Color.FromUint(4282569842U)));
				return (T)Activator.CreateInstance(typeof(T));
			}
		}

		public void Serialize<T>(T data, string basePath, string saveId, string fileName)
		{
			try
			{
                DirectorySecurity securityRules = new DirectorySecurity();
                securityRules.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));

                string dirPath = Path.Combine(basePath, SaveDirectory, saveId);
				Directory.CreateDirectory(dirPath, securityRules);

                string filePath = Path.Combine(dirPath, fileName);
                
				using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(fileStream, data);
				}
			}
			catch (IOException) 
			{
				InformationManager.DisplayMessage(new InformationMessage($"Could not create save file '{fileName}'.", Color.FromUint(4282569842U)));
			}
		}

		public T Deserialize<T>(string basePath, string saveId, string fileName)
		{
			try
			{
				string filePath = Path.Combine(basePath, SaveDirectory, saveId, fileName);
				if(!File.Exists(filePath))
				{
					return default;
				}

				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					return (T)binaryFormatter.Deserialize(fileStream);
				}
			}
			catch (IOException)
			{
				InformationManager.DisplayMessage(new InformationMessage($"Could not load save file '{fileName}'.", Color.FromUint(4282569842U)));
				return (T)Activator.CreateInstance(typeof(T));
			}
		}
	}
}