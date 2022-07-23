using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace musicRenamer
{
    public enum ModeHandle : int
    {
        nonassigned = 0,
        assigned
    }

    class Program
    {
        static string defaultSourceDir = @"C:\Users\abm69\Desktop\TMP\";
        static string defaultDestinationDir = @"C:\Users\abm69\Music\";
        //static DirectoryInfo defaultSourceDirectoryInfo = new(defaultDestinationDir);
        static CsMusicUnit[] musicUnitsArr;
        public static string renameRuleFile = @"C:\works\musicRenamer\renameRule.rule";
        //DirectoryInfo directoryInfo = new(defaultSourceDir);

        static void Main(string[] args)//fileinfo array only
        {
            ModeHandle modeHandle;//if file dir was assigned or not
            /*mode sclector*/
            /*init: pls use "musicUnitsArr"in below code*/
            if (args.Length != 0)
            {
                modeHandle = ModeHandle.assigned;
                musicUnitsArr = new CsMusicUnit[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    musicUnitsArr[i] = new(args[i]);
                }
            }
            else
            {
                modeHandle = ModeHandle.nonassigned;
                DirectoryInfo tmpDirectoryInfo = new(defaultSourceDir);
                FileInfo[] tmpFileInfos = tmpDirectoryInfo.GetFiles("*.flac");//search partten
                musicUnitsArr = new CsMusicUnit[tmpFileInfos.Length];
                for (int i = 0; i < tmpFileInfos.Length; i++)
                {
                    musicUnitsArr[i] = new(tmpFileInfos[i]);
                }
            }
            /***init end.***/
            Welcome(modeHandle);
            Console.Write("creating log...");
            CreatLog();

            /***edit log by vscode***/
            Process cmd = new Process();//call outside program
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.FileName = "notepad";//via notepad
            cmd.StartInfo.Arguments = ("\""+@musicUnitsArr[0].directory + "\\musicInfoList.log\"");
            cmd.Start();
            cmd.WaitForExit();
            cmd.Close();
            /***process end.***/

            //Console.WriteLine("press any key to continue renamer...");
            //Console.ReadKey();
            Renamer();
            Console.WriteLine("press any key to continue...");
            File.Delete(musicUnitsArr[0].directory + @"\musicInfoList.log");
            Console.ReadKey();
            return;
        }

        static void Welcome(ModeHandle isassigned)
        /*page of greeting&show notification*/
        {
            FileInfo renameRule = new(renameRuleFile);
            switch (isassigned)
            {
                case ModeHandle.assigned:
                    Console.WriteLine("assigned file mode:");
                    Console.WriteLine("the log will generate at same folder.");
                    Console.WriteLine("press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case ModeHandle.nonassigned:
                    Console.WriteLine("non-assigned mode:");
                    Console.WriteLine(@"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                    Console.WriteLine("Wellcom to RenamePatch!!");
                    Console.WriteLine("ps:\n1.Source pos will be desktop\\新增資料夾\n2.In(3.),you need .log file to finish to work.\n3.output will be music file.:D");
                    Console.WriteLine(@"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                    Console.WriteLine("press any to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    break;
            }
            if (renameRule.Exists)
            {
                using (StreamReader renameRuleHandle = new(renameRule.FullName))
                {
                    Console.Write("current rename rule:");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(renameRuleHandle.ReadLine());
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("rename rule has not been established.Press 4 to build now");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                ChangeRenameRule();
            }
            Thread.Sleep(400);
            return;
        }

        static void CreatLog()
        /***generate .log file,format: 
         * orgi.FileDir
         * flac tag/newFileName.flac (form as .rule)
         * frequency/bps
         ***/
        {
            using (StreamWriter streamWriter = File.CreateText(musicUnitsArr[0].directory + @"\musicInfoList.log"))
            {
                streamWriter.WriteLine(musicUnitsArr.Length.ToString());
                foreach (CsMusicUnit musicUnit in musicUnitsArr)
                {
                    streamWriter.WriteLine(musicUnit.FullName);
                    //Console.WriteLine(musicUnitFormString);
                    streamWriter.WriteLine(musicUnit.getForm);
                    streamWriter.WriteLine(musicUnit.musicBasicInfo.HzRate + "/" + musicUnit.musicBasicInfo.BitPerSec);
                }
                streamWriter.Close();
                Console.WriteLine("done.");
            }
            return;
        }

        static void Renamer()
        {
            /***
             * use log file to find file&rename in order,
             * first line is how many set of data following.
             * each set data has [orig. posi.] and [new name] [freq./bps] three line.
             ***/
            try
            {
                int counter = 0;
                StreamReader renameHandle = new(musicUnitsArr[0].directory + @"\musicInfoList.log");
                counter = Convert.ToInt32(renameHandle.ReadLine());
                for (int i = 0; i < counter; i++)
                {
                    string[] patterns = { renameHandle.ReadLine(), renameHandle.ReadLine() };
                    renameHandle.ReadLine();//ignore [freq./bps]
                    if (File.Exists(defaultDestinationDir + patterns[1] + ".flac"))
                    {
                        File.Delete(defaultDestinationDir + patterns[1] + ".flac");
                    }
                    File.Copy(patterns[0], defaultDestinationDir + @"\" + patterns[1]+".flac");
                }
                renameHandle.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            Console.WriteLine("done");
            return;
        }

        static void ChangeRenameRule()
        /*get orig. rule->set new rule->return*/
        {
            Console.WriteLine(@"\\\\\\\\\\\\\\\\\ChangeRenameRule\\\\\\\\\\\\\\\\\");
            if (!File.Exists(renameRuleFile))
            {
                File.CreateText(renameRuleFile);//for insure
            }
            StreamReader streamReader = new(renameRuleFile);
            StreamWriter streamWriter = File.CreateText(renameRuleFile);
            /*inti*/

            Console.WriteLine("orig\t:{0}", streamReader.ReadLine());//read orig. rule
            streamReader.Close();
            Console.Write("new\t:");
            streamWriter.WriteLine(Console.ReadLine());
            streamWriter.Close();
            Console.WriteLine("done.");
            return;
        }
    }
}