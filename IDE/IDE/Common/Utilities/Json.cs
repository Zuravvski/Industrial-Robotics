using Newtonsoft.Json;
using System.IO;

namespace IDE.Common.Utilities
{
    /// <summary>
    /// Contains Json serialize and deserialize methods.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Object serializer.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="fileName">File name to save object as.</param>
        /// <param name="fileExtension">File extension to save object with.</param>
        public static void SerializeObject(object obj, string fileName, string fileExtension = "txt", Formatting formatingStyle = Formatting.None)
        {
            File.WriteAllText($@"{fileName}.{fileExtension}", "aaaa");    //to clear desired file before writing

            FileStream fs = File.Open($@"{fileName}.{fileExtension}", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = formatingStyle;
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, obj);
            }
        }

        /// <summary>
        /// Object deserializer.
        /// </summary>
        /// <param name="fileName">Name of a file containing json.</param>
        /// <param name="fileExtension">Extension of a file containing json.</param>
        public static T DeserializeObject<T>(string fileName, string fileExtension = "txt")
        {
            FileStream fs = File.Open($@"{fileName}.{fileExtension}", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string json = sr.ReadToEnd();
            T myObject =  JsonConvert.DeserializeObject<T>(json);

            return myObject;
        }
    }
}
