using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Converters;

namespace IOTest;

public class Person
{
    
    public int age;
    public string name;
    public string id;

    public Person(){}
    public Person(string name, int age, string id)
    {
        this.name = name;
        this.id = id;
        this.age = age;
    }

    public override string ToString()
    {
        return "姓名:" + name + "\n年龄:" + age + "\n身份证号:" + id;
    }

    public static void WriteIntoFile(Person targetPerson,FileStream fs)
    {
        BinaryWriter bw = new BinaryWriter(fs);
        
        byte[] nameBytes = Encoding.UTF8.GetBytes(targetPerson.name);
        bw.Write(nameBytes.Length);
        bw.Write(nameBytes);
        
        bw.Write(targetPerson.age);
        
        byte[] idBytes = Encoding.UTF8.GetBytes(targetPerson.id);
        bw.Write(idBytes.Length);
        bw.Write(idBytes);
    }

    public static Person ReadInFile(FileStream fs)
    {
        Person tmp = new Person();
        BinaryReader br = new BinaryReader(fs);
        int len;
        
        len = br.ReadInt32();
        tmp.name = Encoding.UTF8.GetString(br.ReadBytes(len));
        
        tmp.age = br.ReadInt32();

        len = br.ReadInt32();
        tmp.id = Encoding.UTF8.GetString(br.ReadBytes(len));
        
        return tmp;
    } 
}

class IOTest
{
    static void Main(string[] args)
    {
        string filePath = @"C:\Users\lsc\Desktop\C#学习\IO\dataFolder\data2.txt";
        Person lsc = new Person("lsc", 22, "43072320001126XXXX");
        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
        {
            Person.WriteIntoFile(lsc,fs);
            fs.Seek(0, SeekOrigin.Begin);
            Person tmp = Person.ReadInFile(fs);
            Console.WriteLine(tmp.ToString());
        }
        
        Console.WriteLine("\n************************************************");
        Console.WriteLine("************************************************");
        Console.WriteLine("==========json序列化==========\n");
        string serializeObject = JsonConvert.SerializeObject(lsc);
        Console.WriteLine("json序列化后：\n" + serializeObject + "\n");
        Person deserializedPerson = JsonConvert.DeserializeObject<Person>(serializeObject);
        Console.WriteLine("json反序列化后：\n" + deserializedPerson.ToString() + "\n");
        
        
        Console.WriteLine("\n************************************************");
        Console.WriteLine("************************************************");
        Console.WriteLine("==========json文件读写==========\n");
        
        string jsonFilePath = @"C:\Users\lsc\Desktop\C#学习\IO\dataFolder\data.json";
        File.WriteAllText(jsonFilePath,serializeObject);
        Person lsc2 = JsonConvert.DeserializeObject<Person>(File.ReadAllText(jsonFilePath));
        Console.WriteLine(lsc2.ToString());
        
        Console.WriteLine("\n************************************************");
        Console.WriteLine("************************************************");
        Console.WriteLine("==========JObject动态序列化==========\n");
        JObject jObject = JObject.Parse(serializeObject);
        jObject.Add("Mystr2","hahaha");
        Console.WriteLine(jObject.ToString(Formatting.Indented));
        Console.WriteLine(jObject["id"].Value<string>());
        Console.WriteLine(jObject["Mystr2"].Value<string>());
        
        Console.WriteLine("\n************************************************");
        Console.WriteLine("************************************************");
        Console.WriteLine("==========JArray遍历元素==========\n");

        JArray jArray = new JArray();
        jArray.Add //添加一个JObject进入jArray数组中
        (
            JObject.Parse //把整个json序列转换成一个JObject格式
            (
                JsonConvert.SerializeObject //把对象转换成json序列
                (
                    new Person("lsc1", 21, "43072320001126")
                )
            )
        );
        jArray.Add(JObject.Parse(JsonConvert.SerializeObject(new Person("lsc2", 20, "43072320001125"))));
        jArray.Add(JObject.Parse(JsonConvert.SerializeObject(new Person("lsc3", 19, "43072320001124"))));
        jArray.Add(JObject.Parse(JsonConvert.SerializeObject(new Person("lsc4", 18, "43072320001123"))));
        Console.WriteLine(jArray);
        foreach (var tmpJson in jArray)
        {
            if (tmpJson["name"].Value<string>().Equals("lsc3"))
            {
                Console.WriteLine(tmpJson);
            }
        }
    }
}