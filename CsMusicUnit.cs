using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using flacTag;

namespace musicRenamer
{
    public class CsMusicUnit
    {
        /***init***/
        public CsMusicUnit (string musicPath)//contructor with only path
        {
            fileInfo = new(musicPath);
            flacTag = new(fileInfo);
            musicBasicInfo = new();
            musicBasicInfo.Init(flacTag.HexFlacBaseInfo);
        }

        public CsMusicUnit (FileInfo fileInfo)//contructor with class fileInfo
        {
            this.fileInfo = fileInfo;
            flacTag = new(fileInfo);
        }
        //private string musicSource = "";
        private FileInfo fileInfo;
        public FlacTag flacTag;
        public MusicBasicInfo musicBasicInfo;
        public string directory
        {
            get
            {
                return fileInfo.DirectoryName;
            }
        }

        public string Name
        {
            get
            {
                return fileInfo.Name;
            }
        }

        public string FullName
        {
            get
            {
                return fileInfo.FullName;
            }
        }

        public string getForm
        {
            get
            {
                return RenameFormer(this);
            }
        }

        static public string RenameFormer(CsMusicUnit csMusicUnit)
        {
            string rule;
            using (StreamReader ruleHandle = File.OpenText(Program.renameRuleFile))
            {
                rule = ruleHandle.ReadLine();
            }
            rule = rule.Replace("<Artist>", csMusicUnit.flacTag.Artist, StringComparison.Ordinal).Replace("<Name>", csMusicUnit.flacTag.Title, StringComparison.OrdinalIgnoreCase).Replace("<Album>", csMusicUnit.flacTag.Album, StringComparison.OrdinalIgnoreCase).Replace("?", "？").Replace("\"", "'").Replace("*", "＊");
            return rule;
        }
    }
}
