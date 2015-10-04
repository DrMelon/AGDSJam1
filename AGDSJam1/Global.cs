using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace AGDSJam1
{
    class Global
    {
        public static Game theGame;
        public static Session thePlayerSession;
        public static ControllerXbox360 controllerPlayerOne;
        public static string MsgString = "Subject awakened.";
        public static bool ResetBox = true;

        public static void NewWords(string msg)
        {
            MsgString = msg;
            ResetBox = true;
        }
    }
}
