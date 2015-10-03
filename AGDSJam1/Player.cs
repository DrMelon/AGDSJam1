using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace AGDSJam1
{
    class Player : Entity
    {
        Spritemap<string> mySprite;

        public Entity crossHair;


        public float maxSpeed = 5.0f;

        public ControllerXbox360 myController;

        public Speed mySpeed;

        public Player(float x, float y)
        {
            
            X = x;
            Y = y;
            // load player sheet
            mySprite = new Spritemap<string>(Assets.GFX_PLAYER, 32, 32);
            mySprite.Add("idle", new int[] { 0 }, new float[] { 60.0f });
            mySprite.Add("run", new int[] { 0, 1, 2, 3, 4, 5 }, new float[] { 6f, 6f, 6f, 6f, 6f, 6f });
            mySprite.Play("run");
            mySprite.CenterOrigin();
            AddGraphic(mySprite);
            mySpeed = new Speed(5);

            AddCollider(new CircleCollider(8, 2));
            Collider.CenterOrigin();

            crossHair = new Entity(X, Y, new Image(Assets.XHAIR));
            crossHair.Graphic.CenterOrigin();
            
        }

        public void HandleInput()
        {
            myController = Global.thePlayerSession.GetController<ControllerXbox360>();
            Vector2 moveDelta = new Vector2();
            moveDelta.X = myController.LeftStick.X * 0.1f;
            moveDelta.Y = myController.LeftStick.Y * 0.1f;
            
            MoveInDirection(moveDelta);


        }

        public void MoveInDirection(Vector2 moveDelta)
        {
            mySpeed.X += moveDelta.X;
            mySpeed.Y += moveDelta.Y;
            
            mySpeed.Max = 3.0f;
            
            if (Math.Abs(moveDelta.X) <= 0.1)
            {
                mySpeed.X *= 0.92f;

            }
            
            if (Math.Abs(moveDelta.Y) <= 0.1)
            {
                mySpeed.Y *= 0.92f;
            }


        }

        public override void Update()
        { 
            base.Update();

            HandleInput();

            if (Overlap(X + mySpeed.X, Y + mySpeed.Y, 3))
            {
                //try just xspeed
                if(Overlap(X + mySpeed.X, Y, 3))
                {
                    mySpeed.X = 0;
                }
                if (Overlap(X, mySpeed.Y + Y, 3))
                {
                    mySpeed.Y = 0;
                }

            }
           // else
           // {
                X += mySpeed.X;
                Y += mySpeed.Y;
           // }

            crossHair.X = X + (Input.GameMouseX - 320) / 2;
            crossHair.Y = Y + (Input.GameMouseY - 240) / 2;

            // rotate to face crosshair
            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
            directionToXHair.Normalize();
            Graphic.Angle = 270 + (float)Math.Atan2(-directionToXHair.Y, directionToXHair.X) * 180.0f / (float)Math.PI;

            

        }

        public override void UpdateLast()
        {
            base.UpdateLast();

            // animate if moving
            if ((Math.Abs(mySpeed.X) > 0.3f) || (Math.Abs(mySpeed.Y) > 0.3f))
            {
                if (mySprite.CurrentAnim != "run")
                {
                    mySprite.Play("run");
                }
                
            }
            else
            {
                mySprite.Play("idle");
            }
        }


    }
}
