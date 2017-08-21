using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace App2
{
    public class JsonClass
    {
        public static string JSONSerialize<T>(T obj) // serialize json for sending to php
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        //      public static T JSONDeserialise<T>(string json) // deserialize json for getting data
        //      {
        //          T obj = Activator.CreateInstance<T>();
        //          using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
        //          {
        //              DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        //              obj = (T)serializer.ReadObject(ms);
        //              return obj;
        //          }
        //      }
    }
}