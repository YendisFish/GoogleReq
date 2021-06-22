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
        public static String stats(bool isRunning)
        {
            if (isRunning == true)
            {
                String toReturn = File.ReadAllText("Stats.txt");
                return $"STATS \n{toReturn}";

            } else
            {
                return "There is nothing in the stats file!";
            }
        }

        public static void writeStats(int numReqs, int numCycles)
        {
            File.WriteAllText($@"Stats.txt", $"REQUESTS = {numReqs} | CYCLES = {numCycles}");
            File.WriteAllText($@"Stats.Init.txt", "True");
        }

        public static bool checkStats()
        {
            if (File.Exists("Stats.Init.txt"))
            {
                String checkFile = File.ReadAllText("Stats.Init.txt");

                if (checkFile.Contains("True"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } else
            {
                return false;
            }
        }

        public static void searchFile(String fileName)
        {
            JObject reader = JObject.Parse(File.ReadAllText($@"config.json"));
            String saveFile = (string)reader["filesettings"]["save-dir-name"];
            
            if (Directory.Exists($@"{saveFile}"))
            {
                String[] saveFiles = Directory.GetFiles($@"{saveFile}");
                String saveFilesString = saveFiles.ToString();

                if (saveFilesString.Contains(fileName))
                {
                    Console.WriteLine("File Found");
                    Console.Write("Would you like to read the file? [y/n] > ");
                    String yorn = Console.ReadLine();

                    if (yorn == "y" || yorn == "n" || yorn == "Y" || yorn == "N")
                    {
                        if (yorn == "y" || yorn == "Y")
                        {
                            String fileText = File.ReadAllText(fileName);
                            Console.WriteLine(fileName);
                        }
                        if (yorn == "n" || yorn == "N")
                        {
                            Console.WriteLine("Aborting!");
                        }
                    }
                }
            } else
            {
                Console.WriteLine("There are no files to search for!");
            }
        }
        
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
            int numReqs = 0;
            int numCycles = 0;

            Console.WriteLine("GoogleReq!");

            File.Create($@"Stat.Init.txt");

            while (true)
            {
                bool checkStat = checkStats();
                
                Console.Write("Enter your options [-google (-g) | -searchfile (-sf) | -stats | exit] > ");

                String mainLine = Console.ReadLine();

                if (mainLine == "-google" || mainLine == "-g" || mainLine == "-searchFile" || mainLine == "-sf" || mainLine == "exit" || mainLine == "-stats")
                {
                    if (mainLine == "-google" || mainLine == "-g")
                    {
                        pythonScriptHandler();

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
                        ++numReqs;

                        File.Delete(@"Python\Scripts\Query.txt");
                        File.Delete(@"Python\Scripts\langsettings.txt");

                        while (true)
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
                    }
                    if (mainLine == "-searchfile" || mainLine == "-sf")
                    {
                        Console.Write("Please enter the name of the file you are searching for (include the directory) > ");
                        String userIn = Console.ReadLine();

                        searchFile(userIn);
                    }
                    if (mainLine == "-stats")
                    {
                        String statStr = stats(checkStat);
                        Console.WriteLine(statStr);
                    }
                    if (mainLine == "exit")
                    {
                        File.Delete("Stats.Init.txt");
                        System.Environment.Exit(0);
                    }
                } else
                {
                    Console.WriteLine("This argument does not exist! Please try again!");
                }

                ++numCycles;
                writeStats(numReqs, numCycles);
            }
            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
        }
    }
}