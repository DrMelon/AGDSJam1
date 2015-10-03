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

        // Player


        public PlayState()
        {



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

            // Create station
            AddGraphic(floorTiles);
            AddGraphic(wallTiles);

            // Move camera to start point
            TiledObjectGroup mapObjects = (TiledObjectGroup)mapProject.Layers[2];
            TiledObject strt = mapObjects.Objects[0];

            CameraX = strt.X - 320;
            CameraY = strt.Y - 240;
        }


        // Funcs
        public override void Update()
        {
            base.Update();

            starFieldFar.X -= 0.1f;
            starFieldMid.X -= 0.5f;
            starFieldClose.X -= 1.0f;

            // bounce zoom?
            CameraZoom = 1.8f + (float)Math.Sin(Global.theGame.Timer * 0.01f) * 0.2f;
            CameraAngle = 0.0f + (float)Math.Sin(Global.theGame.Timer * 0.02f) * 4.0f;

        }

        public override void UpdateLast()
        {
            base.UpdateLast();
        }

        public override void Render()
        {
            base.Render();
        }


    }
}
