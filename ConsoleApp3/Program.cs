// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;

BenchmarkRunner.Run<Benchmark>();

[MemoryDiagnoser]
public class Benchmark
{
    public IEnumerable<string>? Data;

    private IEnumerable<string> GenerateData()
    {
        for (int i = 0; i < 100; i++)
        {
            yield return new string('a', 500);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        Data = GenerateData();
    }

    [Benchmark]
    public long UsingString()
    {
        var jsonString = JsonConvert.SerializeObject(Data);
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, leaveOpen: true);
        writer.Write(jsonString);
        writer.Flush();
        return ms.Length;
    }

    [Benchmark]
    public long UsingJsonWriter()
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, leaveOpen: true);
        using var jsonWriter = new JsonTextWriter(writer);
        var serializer = new JsonSerializer();
        serializer.Serialize(jsonWriter, Data);
        jsonWriter.Flush();
        return ms.Length;
    }

}