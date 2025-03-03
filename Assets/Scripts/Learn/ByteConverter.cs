using UnityEngine;
using System;
using System.Text;
using Newtonsoft.Json;

public static class ByteConverter
{
    public static T ParseBase64ToJsonObject<T>(byte[] data)
    {
        // Convert byte[] to Base64 string
        string base64String = Convert.ToBase64String(data);

        // Decode Base64 string back to JSON string
        string jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

        // Parse JSON string into an object
        return JsonConvert.DeserializeObject<T>(jsonString);
    }

    public static T ParseByteArrayToJsonObject<T>(byte[] data)
    {
        if (data == null || data.Length == 0) return default;

        // Convert byte[] (Uint8Array) to JSON string
        string jsonString = Encoding.UTF8.GetString(data);

        // Parse JSON string into an object
        return JsonConvert.DeserializeObject<T>(jsonString);
    }

    public static byte[] EndcodeObjectToByteArray<T>(T obj)
    {
        string jsonString = JsonConvert.SerializeObject(obj);
        return Encoding.UTF8.GetBytes(jsonString);
    }
}