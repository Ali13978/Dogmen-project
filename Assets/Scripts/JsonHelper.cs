using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }
    public static string[] GetJsonArray(string json)
    {
        json = WrapArray(json);
        return json.Split('}');
    }
    private static string WrapArray(string json)
    {
        return "{\"items\":" + json + "}";
    }
    public static List<Dictionary<string, object>> FromJson(string json)
    {
        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        json = json.Trim('[', ']');
        string[] pairs = json.Split('}');
        foreach (string pair in pairs)
        {
            if (pair.Trim() == "")
                continue;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            string[] values = pair.Trim('{').Split(',');
            foreach (string value in values)
            {
                string[] keyValue = value.Split(':');
                if (keyValue.Length != 2) continue; // Skip malformed key-value pairs
                string key = keyValue[0].Trim().Trim('"');
                string stringValue = keyValue[1].Trim().Trim('"');
                object parsedValue;
                if (stringValue.ToLower() == "true" || stringValue.ToLower() == "false")
                    parsedValue = bool.Parse(stringValue);
                else if (float.TryParse(stringValue, out float floatValue))
                    parsedValue = floatValue;
                else
                    parsedValue = int.Parse(stringValue);
                dict.Add(key, parsedValue);
            }
            list.Add(dict);
        }
        return list;
    }


    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
