﻿using System;
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
        public static string MsgString = "You wake up.\nPress W, S, A, D to move.\nInteract/Use Item with Left Mouse\nThrow Item with Right Mouse.\nHold Q to open item menu.\nRepair your machines!";
        public static bool ResetBox = true;
        public static bool MachineBroken = true;
        public static float MachineBrokenTime = 0;

        public static void NewWords(string msg)
        {
            MsgString = msg;
            ResetBox = true;
        }
    }
}
