using Newtonsoft.Json;
using System.IO;

public class DataSerializator
{
    public void Save<T>(T data, string filePath)
    {
        var fullDirectory = GetFullDirectory<T>(filePath);

        if (!File.Exists(fullDirectory))
        {
            using var fileStream = new FileStream(fullDirectory, FileMode.Create);
        }

        using var streamWriter = new StreamWriter(fullDirectory);
        streamWriter.AutoFlush = true;
        var text = JsonConvert.SerializeObject(data);
        streamWriter.Write(text);
    }

    public T Load<T>(string filePath)
    {
        var fullDirectory = GetFullDirectory<T>(filePath);

        if (File.Exists(fullDirectory))
        {
            using var streamReader = new StreamReader(fullDirectory);
            var text = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(text);
        }
        else
        {
            return default(T);
        }
    }

    public void Reset<T>(string filePath)
    {
        var fullDirectory = GetFullDirectory<T>(filePath);

        if (File.Exists(fullDirectory))
        {
            File.Delete(fullDirectory);
        }
    }

    private string GetFullDirectory<T>(string filePath)
    {
        return $"{filePath}/{typeof(T).Name}.txt";
    }
}