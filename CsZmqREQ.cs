using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System.Diagnostics;


namespace musicRenamer
{
    public class MusicBasicInfo
        /***
         * musicBasicInfo: decode music freq. & bps(bit per sec,)
         * warning: Init brefore use data is required.
         * 
         ***/
    {
        private string hzRate, channles, bitPerSec;
        private bool isInit;
        public MusicBasicInfo()
        {
            hzRate = "";
            channles = "";
            bitPerSec = "";
            isInit = false;
        }
        public string HzRate
        {
            get
            {
                if (isInit)
                {
                    return hzRate;
                }
                else
                {
                    return "err: hasn't been init.";
                }
            }
        }

        public string Channles
        {
            get
            {
                if (isInit)
                {
                    return channles;
                }
                else
                {
                    return "err: hasn't been init.";
                }
            }
        }

        public string BitPerSec
        {
            get
            {
                if (isInit)
                {
                    return bitPerSec;
                }
                else
                {
                    return "err: hasn't been init.";
                }
            }
        }

        public void Init(string MSG)
        {
            StartPY();
            var clientCtx = new RequestSocket("tcp://localhost:5556");
            clientCtx.SendFrame(MSG);
            string[] recvStr = clientCtx.ReceiveFrameString().Split(',');
            hzRate = recvStr[0];
            channles = recvStr[1];
            bitPerSec = recvStr[2];
            isInit = true;
            return;
        }
        private async void StartPY()
        {
            Process.Start(@"C:\Python310\python.exe", @"..\..\..\zmqFlacInfoExtractor.py");
        }
    }
}
