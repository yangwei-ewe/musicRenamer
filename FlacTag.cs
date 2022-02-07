using System;
using System.IO;
using System.Text;

namespace flacTag
{
    public class FlacTag
    {
        private string[,] flacTags;//[blank name,value]
        private FileInfo fileInfo;
        private byte[] hexFlacBasicInfo;

        public string Title
        {
            get
            {
                string answer = fileInfo.Name;
                int i = 0;
                while (i<flacTags.Length/2)
                {
                    if(flacTags[i, 0].ToLower() == "title")
                    {
                        answer = flacTags[i, 1];
                    }
                    i++;
                }
                return answer;
            }
        }

        public string Artist
        {
            get
            {
                string answer = "";
                int i = 0;
                while (i < flacTags.Length / 2)
                {
                    if (flacTags[i, 0].ToLower() == "artist")
                    {
                        answer = flacTags[i, 1];
                    }
                    i++;
                }
                return answer;
            }
        }

        public string Album
        {
            get
            {
                string answer = "";
                int i = 0;
                while (i < flacTags.Length / 2)
                {
                    if (flacTags[i, 0].ToLower() == "album")
                    {
                        answer = flacTags[i, 1];
                    }
                    i++;
                }
                return answer;
            }
        }

        public string Organization
        {
            get
            {
                string answer = "";
                int i = 0;
                while (i < flacTags.Length / 2)
                {
                    if (flacTags[i, 0].ToLower() == "organization")
                    {
                        answer = flacTags[i, 1];
                    }
                    i++;
                }
                return answer;
            }
        }

        public string HexFlacBaseInfo
        {
            get
            {
                return BitConverter.ToString(hexFlacBasicInfo).Replace("-", "");
            }
        }

        public FlacTag(FileInfo fileInfo)
        /*constructor*/
        {
            //04->VendorLength(->VendorString)->TagAmount->Tag[i]Length->TagContent
            long index = 4;
            long distance = 0;
            byte[] read = File.ReadAllBytes(fileInfo.FullName);
            this.fileInfo = fileInfo;

            /*find flac tag 
            **(MATADATA_BLOCK_HEADER04->BLOCK_TYPE
            */
            while ((read[index] != 4) && (read[index] != 132))
            {
                distance = BigDecToHex(index, read);
                index += 4+distance;
            }

            /*VendorLength(->VendorString)*/
            index += 3;
            distance = SmallDecToHex(index, read);
            index = index + 4 + distance;

            /*get tag amount*/
            long TagAmount = SmallDecToHex(index, read);
            string[] allFlacTag = new string[TagAmount];
            index += 4;
            for (int i = 0; i < TagAmount; i++)
            {
                distance = SmallDecToHex(index, read);
                index += 4;
                byte[] currTag = new byte[distance];
                Array.Copy(read, index + 1, currTag, 0, distance);
                allFlacTag[i] = Encoding.UTF8.GetString(currTag);
                index += distance;
            }

            flacTags = new string[TagAmount,2];
            for (int i=0;i<TagAmount;i++)
            {
                string[] tmp = allFlacTag[i].Split("=");
                flacTags[i, 0] = tmp[0];
                flacTags[i, 1] = tmp[1];
            }
            index = 4;
            while (read[index]!=0)
            {
                index++;
            }
            index += 14;
            hexFlacBasicInfo = new byte[8];
            Array.Copy(read, index, hexFlacBasicInfo, 0, 8);
        }

        static long BigDecToHex(long ptr, byte[] flac)
        /*read 3 block*/
        {
            double result = 0;
            result = (flac[ptr += 1] / 16) * Math.Pow(16, 5) + (flac[ptr] % 16) * Math.Pow(16, 4)
                   + (flac[ptr += 1] / 16) * Math.Pow(16, 3) + (flac[ptr] % 16) * Math.Pow(16, 2)
                   + (flac[ptr += 1] / 16) * 16 + (flac[ptr] % 16);
            long final = Convert.ToInt32(result);
            return final;
        }

        static long SmallDecToHex(long ptr, byte[] flac)
        /*read 4 block*/
        {
            double result = 0;
            result = (flac[ptr += 1] / 16) * 16 + (flac[ptr] % 16)
                   + (flac[ptr += 1] / 16) * Math.Pow(16, 3) + (flac[ptr] % 16) * Math.Pow(16, 2)
                   + (flac[ptr += 1] / 16) * Math.Pow(16, 5) + (flac[ptr] % 16) * Math.Pow(16, 4)
                   + (flac[ptr += 1] / 16) * Math.Pow(16, 7) + (flac[ptr] % 16) * Math.Pow(16, 6);
            long final = Convert.ToInt64(result);
            return final;
        }
    }
}
