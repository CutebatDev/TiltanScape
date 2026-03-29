using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Save_System
{
    public static class SaveSystem
    {
        public static void SavePlayer(PlayerStats playerStats)
        {
            string path = Application.persistentDataPath + "/playerData.bin";
            BinaryFormatter formatter = new BinaryFormatter();
            
            FileStream stream = new FileStream(path, FileMode.Create);

            PlayerData playerData = new PlayerData(playerStats);
            
            formatter.Serialize(stream, playerData);
            stream.Close();
        }

        public static PlayerData LoadPlayer()
        {
            string path = Application.persistentDataPath + "/playerData.bin";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                
                PlayerData playerData = formatter.Deserialize(stream) as PlayerData;
                stream.Close();
                
                return playerData;
            }
            else
            {
                Debug.Log("No save file found in" + path);
                return null;
            }
        }
    }
}
