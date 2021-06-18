using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace GoogleReq
{
    class Program
    {
        public static void saveToFile(String query, String pyscriptOutput)
        {
            JObject reader = JObject.Parse(File.ReadAllText("config.json"));
            String saveName = (string)reader["filesettings"]["save-dir-name"];
            Console.Write("Enter 'C' to set a custom name for the save file > ");
            String ifCustomOrNot = Console.ReadLine();
            
            if (ifCustomOrNot == "C" || ifCustomOrNot == "c")
            {

                while (true)
                {
                    Console.Write("What would you like to name the file? > ");
                    String fileName = Console.ReadLine();

                    if (fileName.Contains("/") || fileName.Contains("\\"))
                    {
                        Console.WriteLine("Filename cannot contain '\\' or '/'");
                        continue;
                    } else
                    {
                        Directory.CreateDirectory(saveName);

                        var filestream = File.Create($@"{saveName}\{fileName}.txt");
                        filestream.Close();
                        File.WriteAllText($@"{saveName}\{fileName}.txt", pyscriptOutput);
                        break;
                    }
                } 
            } else
            {
                Directory.CreateDirectory(saveName);
                DateTime asdfabds = new DateTime();
                asdfabds = DateTime.UtcNow;

                String a = asdfabds.ToString("o");
                a = a.Replace("/", ".").Replace(":", ".");


                var filestream = File.Create($@"{saveName}\{query}.{a}.txt");
                filestream.Close();
                File.WriteAllText($@"{saveName}\{query}.{a}.txt", pyscriptOutput);
                Console.WriteLine("Saving File");
            }
        }
        public static void pythonScriptHandler()
        {
            JObject fileReader = JObject.Parse(File.ReadAllText(@"config.json"));
            String pyscriptCfg = (string)fileReader["langsettings"]["pyscript-config-path"];

            if (File.Exists(@"Python\Scripts\configpath.txt"))
            {
                Console.WriteLine("Python config file exists! Skipping!");
            } else
            {
                var createFileCFG = File.Create(@"Python\Scripts\configpath.txt");
                createFileCFG.Close();
                File.WriteAllText(@"Python\Scripts\configpath.txt", pyscriptCfg);
            }
        }
        
        public static String langHandler()
        {
            JObject lang = JObject.Parse(File.ReadAllText(@"config.json"));
            String langset = (string)lang["langsettings"]["lang"];
            return langset;
        }
        public static string requestHandler(int resultNumber)
        {
            JObject fileRead = JObject.Parse(File.ReadAllText(@"config.json"));
            String execPath = (string)fileRead["langsettings"]["python-executable-path"];
            String scriptPath = (string)fileRead["langsettings"]["pyscript-path"];
            
            ProcessStartInfo pyReq = new ProcessStartInfo();
            pyReq.FileName = $@"{execPath}";
            pyReq.Arguments = $@"{scriptPath} -results {resultNumber}";
            Console.WriteLine(scriptPath);
            pyReq.UseShellExecute = false;
            pyReq.RedirectStandardOutput = true;
            using (Process process = Process.Start(pyReq))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                    return result;
                }
            }
        }
        static void Main(string[] args)
        {
            pythonScriptHandler();
            
            Console.WriteLine("GoolgeReq!");
            
            String langsettings = langHandler();
            var filestream = File.Create(@"Python\Scripts\langsettings.txt");
            filestream.Close();
            File.WriteAllText(@"Python\Scripts\langsettings.txt", langsettings);

            Console.Write("Search > ");
            String toFileForSearch = Console.ReadLine();
            Console.Write("How many results would you like? > ");
            int resultNum = Convert.ToInt32(Console.ReadLine());

            var fs = File.Create(@"Python\Scripts\Query.txt");
            fs.Close();
            File.WriteAllText(@"Python\Scripts\Query.txt", toFileForSearch);
            
            String request = requestHandler(resultNum);

            File.Delete(@"Python\Scripts\Query.txt");
            File.Delete(@"Python\Scripts\langsettings.txt");
            
            while(true)
            {
                Console.Write("Would you like to write the output to a file? [y/n] > ");
                String yorn = Console.ReadLine();

                if (yorn == "y" || yorn == "n")
                {
                    if (yorn == "y")
                    {
                        Console.WriteLine("Saving to file!");
                        saveToFile(toFileForSearch, request);
                        break;
                    }
                    if (yorn == "n")
                    {
                        Console.WriteLine("Aborting save!");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid character! Please try again!");
                    continue;
                }
            }

            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
        }
    }
}