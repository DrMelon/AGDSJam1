using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;

namespace AGDSJam1
{
    class Program
    {
        static void Main(string[] args)
        {

            Global.theGame = new Game("AGDSJam1", 640, 480, 60, false);
            
            Global.thePlayerSession = Global.theGame.AddSession("PlayerOne");
            Global.thePlayerSession.Controller = new ControllerXbox360(0);
            Global.controllerPlayerOne = Global.thePlayerSession.GetController<ControllerXbox360>();
            Global.controllerPlayerOne.LeftStick.AddKeys(new Key[] { Otter.Key.W, Otter.Key.D, Otter.Key.S, Otter.Key.A });
            Global.controllerPlayerOne.Start.AddKey(Otter.Key.Return);
            Global.controllerPlayerOne.A.AddKey(Otter.Key.Space);
            Global.controllerPlayerOne.X.AddKey(Otter.Key.E);

            
            Global.theGame.AddScene(new PlayState());
            Global.theGame.Start();

        }
    }
}
