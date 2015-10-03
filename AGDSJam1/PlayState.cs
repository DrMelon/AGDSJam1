using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
using Otter.TiledLoader;

namespace AGDSJam1
{
    class PlayState : Scene
    {
        // Vars
        Entity newTest = new Entity(0, 0, new Image(Assets.SPRITE_TEST));

        // BG Stuf
        // Starfield stuff
        Image starFieldFar;
        Image starFieldMid;
        Image starFieldClose;

        // Map
        TiledProject mapProject = new TiledProject(Assets.MAP_STATION);
        Tilemap floorTiles;
        Tilemap wallTiles;
        GridCollider wallCollision;
        GridCollider floorCollision;
        Entity wallTest;
        Entity floorTest;

        float swayAmt = 1.0f;

        // Shaders
        Shader VHSShader;
        Shader VHSShader2;

        // Player
        Player thePlayer;

        // HUD
        Image msgBox;
        RichText msgText;
        // we need text-reveal mode or sth. edit richtext?
        
        
          

        public PlayState()
        {

            //Load shader
            VHSShader = new Shader(ShaderType.Fragment, Assets.VHS_SHADER);
            VHSShader2 = new Shader(ShaderType.Fragment, Assets.VHS_SHADER2);
            Global.theGame.Surface.AddShader(VHSShader);
            Global.theGame.Surface.AddShader(VHSShader2);

            // Create starfield.
            starFieldFar = new Image(Assets.GFX_STARFIELD);
            starFieldFar.Repeat = true;
            starFieldFar.Scroll = 0.3f;
            starFieldMid = new Image(Assets.GFX_STARFIELD);
            starFieldMid.Repeat = true;
            starFieldMid.Scroll = 0.6f;
            starFieldMid.Scale = 1.5f;
            starFieldClose = new Image(Assets.GFX_STARFIELD);
            starFieldClose.Repeat = true;
            starFieldClose.Scroll = 1.3f;
            starFieldClose.Scale = 3.0f;
            starFieldClose.Alpha = 0.5f;

            

            AddGraphic(starFieldFar);
            AddGraphic(starFieldMid);
            AddGraphic(starFieldClose);


            // Load map
            floorTiles = mapProject.CreateTilemap((TiledTileLayer)mapProject.Layers[0]);
            wallTiles = mapProject.CreateTilemap((TiledTileLayer)mapProject.Layers[1], mapProject.TileSets[1]);

            // Make sure walls have the correct tiles    
            wallCollision = mapProject.CreateGridCollider((TiledTileLayer)mapProject.Layers[1], 3);
            floorCollision = mapProject.CreateGridCollider((TiledTileLayer)mapProject.Layers[0], 8);

            // Move camera to start point
            TiledObjectGroup mapObjects = (TiledObjectGroup)mapProject.Layers[2];
            TiledObject strt = mapObjects.Objects[0];

            CameraX = strt.X - 320;
            CameraY = strt.Y - 240;

            // Add player
            thePlayer = new Player(strt.X, strt.Y);


            // Add station & player to scene
            
            AddGraphic(floorTiles);
            Add(thePlayer);
            AddGraphic(wallTiles);
            Add(thePlayer.crossHair);

            wallTest = new Entity(0, 0, null, wallCollision);
            Add(wallTest);
            floorTest = new Entity(0, 0, null, floorCollision);
            Add(floorTest);

            // Add hud
            msgBox = new Image(Assets.GFX_HUD);
            msgBox.Scroll = 0;
            msgBox.CenterOrigin();
            msgBox.X = 320;
            msgBox.Y = 250 + msgBox.Height + 16;
            AddGraphic(msgBox);

            msgText = new RichText("{shake:0.4}Subject awakened.\nFacility status: {color:00ff00}   [OK]\n{color:ffffff}Subject status: {color:00ff00}    [OK]\n{color:ffffff}HERMES-I stability: {color:ff0000}[POOR]", Assets.FONT_MSG, 8, 270, 50);
            msgText.X = 325 - msgBox.HalfWidth;
            msgText.Y = 250 + msgBox.Height - 8;
            msgText.Scroll = 0;
            AddGraphic(msgText);
            

        }


        // Funcs
        public override void Update()
        {
            base.Update();

            starFieldFar.X -= 0.1f;
            starFieldMid.X -= 0.5f;
            starFieldClose.X -= 1.0f;

            // bounce zoom?
            CameraZoom = 2.0f + (((float)Math.Sin(Global.theGame.Timer * 0.01f) * 0.2f) * swayAmt);
            CameraAngle = 0.0f + (((float)Math.Sin(Global.theGame.Timer * 0.02f) * 4.0f) * swayAmt);

           // VHSShader.SetParameter("time", Global.theGame.Timer);
            VHSShader2.SetParameter("time", Global.theGame.Timer);

        }

        public override void UpdateLast()
        {
            base.UpdateLast();
            CenterCamera(thePlayer.X, thePlayer.Y);
        }

        public override void Render()
        {
            base.Render();
        }


    }
}
