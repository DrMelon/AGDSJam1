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

        public float HP;
        public float O2Level;
        public float Hunger;
        public bool NoShoes;
        public bool InsideShip;
        public bool Dead = false;

        // Fixed Inventory?
        public int Wrench = 0;
        public int Boots = 1;
        public int Crisps = 0;
        public int Circuit = 0;
        public int Donut = 0;
        public int Battery = 0;
        public int O2Tank = 0;
        public int FireExtinguisher = 0;
        public int FloorTile = 0;



        public int SelectedItem = 0; // 0 = hands

        // Inventory Images
        Image hand;
        Image boots;
        Image circuit;
        Image wrench;
        Image crisps;
        Image donut;
        Image o2tank;
        Image fireextinguisher;
        Image battery;
        Image tiles;


        // leftmouse : use
        // e: pick up, inspect
        // q: radial menu

        public bool RenderRadial = false;
        public Machine inspectMachine = null;
        public bool RenderDetail = false;
        public float InspectTime = 300;
        public float StartInspect;

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

            // Load inventory images
            hand = new Image(Assets.GFX_HAND);
            boots = new Image(Assets.GFX_BOOTS);
            circuit = new Image(Assets.GFX_CIRCUIT);
            wrench = new Image(Assets.GFX_WRENCH);
            crisps = new Image(Assets.GFX_CRISPS);
            donut = new Image(Assets.GFX_DONUT);
            o2tank = new Image(Assets.GFX_O2TANK);
            fireextinguisher = new Image(Assets.GFX_EXTINGUISHER);
            battery = new Image(Assets.GFX_BATTERY);
            tiles = new Image(Assets.GFX_TILE);

            StartInspect = Global.theGame.Timer;

            AddCollider(new CircleCollider(8, 2));
            Collider.CenterOrigin();

            crossHair = new Entity(X, Y, new Image(Assets.XHAIR));
            crossHair.Graphic.CenterOrigin();
            crossHair.AddCollider(new CircleCollider(8, 4));
            crossHair.Collider.CenterOrigin();


            // Default vars
            NoShoes = false;
            HP = 100;
            O2Level = 100;
            Hunger = 0;
            InsideShip = true;

        }

        public void HandleInput()
        {
            myController = Global.thePlayerSession.GetController<ControllerXbox360>();
            Vector2 moveDelta = new Vector2();
            if(InsideShip)
            {
                moveDelta.X = myController.LeftStick.X * 0.1f;
                moveDelta.Y = myController.LeftStick.Y * 0.1f;
            }

            // if LB down, render radial menu
            RenderRadial = myController.LB.Down;

            // if use item key is pressed, use an item -- select from radial if radial is used
            if (myController.X.Pressed)
            {
                // Check for radial, select if it's on.
                if (RenderRadial)
                {
                    // get crosshair angle
                    Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                    directionToXHair.Normalize();
                    float AtanResult = (float)Math.Atan2(directionToXHair.X, directionToXHair.Y);
                    if (AtanResult < 0)
                    {
                        AtanResult += 2 * (float)Math.PI;
                    }
                    float Ang = MathHelper.ToDegrees(AtanResult);
                    Otter.Debugger.Instance.Log(Ang);
                    // Get closest to ang
                    float res = Ang / 360.0f;
                    Otter.Debugger.Instance.Log(res * 10);
                    int index = (int)Math.Round(res * 10);

                    if(index == 10)
                    {
                        index = 0;
                    }
                    SelectedItem = index;


                }
                else
                {
                    // use the currently-held item
                    switch (SelectedItem)
                    {
                        case 0:
                            // Hands - Used to inspect an object.
                            // Check for inspection target nearby, inspecting if necessary.
                            if (crossHair.Overlap(crossHair.X, crossHair.Y, 6))
                            {
                                //machine, display text in box.
                                Collider target = crossHair.Collide(crossHair.X, crossHair.Y, 6);
                                if (target.Entity.GetType().Name == "Machine")
                                {
                                    Machine theMachine = (Machine)target.Entity;
                                    Global.ResetBox = true;
                                    Global.MsgString = theMachine.Name + ":\n" + theMachine.Description;
                                    if(theMachine.Status == 2)
                                    {
                                        Global.MsgString += "\n" + theMachine.FlavBroke;
                                    }
                                    if(theMachine.Status == 3)
                                    {
                                        Global.MsgString += "\n" + theMachine.FlavFixing;
                                    }

                                    inspectMachine = theMachine;
                                    RenderDetail = true;
                                    StartInspect = Global.theGame.Timer;
                                }
                            }
                            // Pick up other items if there is room. 
                            if(crossHair.Overlap(crossHair.X, crossHair.Y, 1))
                            {
                                Collider target = crossHair.Collide(crossHair.X, crossHair.Y, 1);
                                if (target.Entity.GetType().Name == "Item")
                                {
                                    Item theItem = (Item)target.Entity;

                                    // get item based on type
                                    switch(theItem.itemType)
                                    {
                                        case 1:
                                            if(Boots < 1)
                                            {
                                                theItem.RemoveSelf();
                                                Boots = 1;
                                                
                                                
                                            }
                                            else
                                            {
                                                //can't pick up
                                            }
                                            break;
                                        case 2:
                                            if (Circuit < 5)
                                            {
                                                theItem.RemoveSelf();
                                                Circuit += 1;

                                            }
                                            else
                                            {
                                                //can't pick up
                                            }
                                            break;
                                        case 3:
                                            if (FloorTile < 5)
                                            {
                                                theItem.RemoveSelf();
                                                FloorTile += 1;

                                            }
                                            else
                                            {
                                                //can't pick up
                                            }
                                            break;
                                        case 4:
                                            if (Wrench < 1)
                                            {
                                                theItem.RemoveSelf();
                                                Wrench = 1;

                                            }
                                            else
                                            {
                                                //can't pick up
                                            }
                                            break;
                                        case 5:
                                            if (Battery < 1)
                                            {
                                                theItem.RemoveSelf();
                                                Battery = 1;

                                            }
                                            else
                                            {
                                                //can't pick up
                                            }
                                            break;
                                        case 6:
                                            if (Donut < 1)
                                            {
                                                theItem.RemoveSelf();
                                                Donut = 1;

                                            }
                                            else
                                            {
                                                //can't pick up
                                            }
                                            break;
                                        case 7:
                                            if (Crisps < 1)
                                            {
                                                theItem.RemoveSelf();
                                                Crisps = 1;

                                            }
                                            else
                                            {
                                                //can't pick up
                                            }
                                            break;
                                        case 8:
                                            if (FireExtinguisher < 1)
                                            {
                                                theItem.RemoveSelf();
                                                FireExtinguisher = 1;

                                            }
                                            else
                                            {
                                                //can't pick up
                                            }
                                            break;
                                        case 9:
                                            if (O2Tank < 1)
                                            {
                                                theItem.RemoveSelf();
                                                O2Tank = 1;

                                            }
                                            else
                                            {
                                                //can't pick up
                                            }
                                            break;
                                    }

                                   
                                }

                            }

                            break;
                        default:
                            //nada
                            break;
                    }
                }

            }

            // Item is thrown away or hands strike object
            if(myController.B.Pressed)
            {
                switch(SelectedItem)
                {
                    case 0:
                        // Percussive force! Wham!
                        // Check for machine, shorten break  time.
                        // Chance to fix?
                        // If vending machine, chance to vend item instead of hurting it. 80% chance. 


                        break;
                    case 1:
                        // Take off your hat, kick off your shoes...
                        if (Boots > 0)
                        {
                            Boots = 0;
                            Item newItem = new Item(X, Y, SelectedItem);
                            this.Scene.Add(newItem);
                            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                            directionToXHair.Normalize();
                            newItem.Throw(directionToXHair);
                            if(!InsideShip)
                            {
                                mySpeed.X = -directionToXHair.X;
                                mySpeed.Y = -directionToXHair.Y;
                            }
                            SelectedItem = 0;
                        }
                        // ... i know you ain't goin anywhere
                        break;
                    case 2:
                        // Take off your hat, kick off your shoes...
                        if (Circuit > 0)
                        {
                            Circuit -= 1;
                            Item newItem = new Item(X, Y, SelectedItem);
                            this.Scene.Add(newItem);
                            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                            directionToXHair.Normalize();
                            newItem.Throw(directionToXHair);
                            if (!InsideShip)
                            {
                                mySpeed.X = -directionToXHair.X;
                                mySpeed.Y = -directionToXHair.Y;
                            }
                            SelectedItem = 0;
                        }
                        // ... i know you ain't goin anywhere
                        break;
                    case 3:
                        // Take off your hat, kick off your shoes...
                        if (FloorTile > 0)
                        {
                            FloorTile -= 1;
                            Item newItem = new Item(X, Y, SelectedItem);
                            this.Scene.Add(newItem);
                            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                            directionToXHair.Normalize();
                            newItem.Throw(directionToXHair);
                            if (!InsideShip)
                            {
                                mySpeed.X = -directionToXHair.X;
                                mySpeed.Y = -directionToXHair.Y;
                            }
                            SelectedItem = 0;
                        }
                        // ... i know you ain't goin anywhere
                        break;
                    case 4:
                        // Take off your hat, kick off your shoes...
                        if (Wrench > 0)
                        {
                            Wrench = 0;
                            Item newItem = new Item(X, Y, SelectedItem);
                            this.Scene.Add(newItem);
                            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                            directionToXHair.Normalize();
                            newItem.Throw(directionToXHair);
                            if (!InsideShip)
                            {
                                mySpeed.X = -directionToXHair.X;
                                mySpeed.Y = -directionToXHair.Y;
                            }
                            SelectedItem = 0;
                        }
                        // ... i know you ain't goin anywhere
                        break;
                    case 5:
                        // Take off your hat, kick off your shoes...
                        if (Battery > 0)
                        {
                            Battery = 0;
                            Item newItem = new Item(X, Y, SelectedItem);
                            this.Scene.Add(newItem);
                            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                            directionToXHair.Normalize();
                            newItem.Throw(directionToXHair);
                            if (!InsideShip)
                            {
                                mySpeed.X = -directionToXHair.X;
                                mySpeed.Y = -directionToXHair.Y;
                            }
                            SelectedItem = 0;
                        }
                        // ... i know you ain't goin anywhere
                        break;
                    case 6:
                        // Take off your hat, kick off your shoes...
                        if (Donut > 0)
                        {
                            Donut = 0;
                            Item newItem = new Item(X, Y, SelectedItem);
                            this.Scene.Add(newItem);
                            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                            directionToXHair.Normalize();
                            newItem.Throw(directionToXHair);
                            if (!InsideShip)
                            {
                                mySpeed.X = -directionToXHair.X;
                                mySpeed.Y = -directionToXHair.Y;
                            }
                            SelectedItem = 0;
                        }
                        // ... i know you ain't goin anywhere
                        break;
                    case 7:
                        // Take off your hat, kick off your shoes...
                        if (Crisps > 0)
                        {
                            Crisps = 0;
                            Item newItem = new Item(X, Y, SelectedItem);
                            this.Scene.Add(newItem);
                            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                            directionToXHair.Normalize();
                            newItem.Throw(directionToXHair);
                            if (!InsideShip)
                            {
                                mySpeed.X = -directionToXHair.X;
                                mySpeed.Y = -directionToXHair.Y;
                            }
                            SelectedItem = 0;
                        }
                        // ... i know you ain't goin anywhere
                        break;
                    case 8:
                        // Take off your hat, kick off your shoes...
                        if (FireExtinguisher > 0)
                        {
                            FireExtinguisher = 0;
                            Item newItem = new Item(X, Y, SelectedItem);
                            this.Scene.Add(newItem);
                            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                            directionToXHair.Normalize();
                            newItem.Throw(directionToXHair);
                            if (!InsideShip)
                            {
                                mySpeed.X = -directionToXHair.X;
                                mySpeed.Y = -directionToXHair.Y;
                            }
                            SelectedItem = 0;
                        }
                        // ... i know you ain't goin anywhere
                        break;
                    case 9:
                        // Take off your hat, kick off your shoes...
                        if (O2Tank > 0)
                        {
                            O2Tank = 0;
                            Item newItem = new Item(X, Y, SelectedItem);
                            this.Scene.Add(newItem);
                            Vector2 directionToXHair = new Vector2(crossHair.X - X, crossHair.Y - Y);
                            directionToXHair.Normalize();
                            newItem.Throw(directionToXHair);
                            if (!InsideShip)
                            {
                                mySpeed.X = -directionToXHair.X;
                                mySpeed.Y = -directionToXHair.Y;
                            }
                            SelectedItem = 0;
                        }
                        // ... i know you ain't goin anywhere
                        break;
                    default:
                        break;
                }
            }

            
            MoveInDirection(moveDelta);


        }

        public void MoveInDirection(Vector2 moveDelta)
        {
            mySpeed.X += moveDelta.X;
            mySpeed.Y += moveDelta.Y;
            
            mySpeed.Max = 3.0f;
            
            if (Math.Abs(moveDelta.X) <= 0.1)
            {
                if (InsideShip)
                {
                    mySpeed.X *= 0.92f;
                }

            }
            
            if (Math.Abs(moveDelta.Y) <= 0.1)
            {
                if(InsideShip)
                {
                    mySpeed.Y *= 0.92f;
                }
                
            }


        }

        public override void Update()
        { 
            base.Update();

            HandleInput();

            // not on floor, not inside!
            InsideShip = Overlap(X, Y, 8);

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

                InsideShip = true;

            }

            X += mySpeed.X;
            Y += mySpeed.Y;
 

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

        public override void Render()
        {
            base.Render();
            // if inspect is open, render it
            if (RenderDetail && inspectMachine != null)
            {
                if(Global.theGame.Timer >= InspectTime + StartInspect)
                {
                    RenderDetail = false;
                }

                // Draw 64x64 box at right edge
                Draw.Rectangle(X + 30, Y - 30, 64, 64, Color.Black, Color.White, 1);
                // Draw Image
                if(inspectMachine.Status == 1)
                {
                    inspectMachine.gfxDetail.Play("normal");
                }
                if (inspectMachine.Status == 2)
                {
                    inspectMachine.gfxDetail.Play("broken");
                }
                if (inspectMachine.Status == 3)
                {
                    inspectMachine.gfxDetail.Play("fixing");
                }
                inspectMachine.gfxDetail.X = 30 + X;
                inspectMachine.gfxDetail.Y = Y - 30;
                inspectMachine.gfxDetail.Render();
            }


            // if radial is open, render it
            if (RenderRadial)
            {
                for(int i = 0; i < 10; i++)
                {
                    // Draw boxes, then items
                    float Ang = (float)i / 10.0f * 360.0f;
                    Ang = MathHelper.ToRadians(Ang);
                    Vector2 newPos = new Vector2((float)Math.Sin(Ang) * 50.0f, (float)Math.Cos(Ang) * 50.0f);
                    newPos.X += X;
                    newPos.Y += Y;
                    
                    if(SelectedItem == i)
                    {
                        Draw.Line(X, Y, newPos.X, newPos.Y, Color.Yellow);
                        Draw.Rectangle(newPos.X - 8, newPos.Y - 8, 16, 16, Color.Gray, Color.Yellow, 1);
                    }
                    else
                    {
                        Draw.Line(X, Y, newPos.X, newPos.Y, Color.White);
                        Draw.Rectangle(newPos.X - 8, newPos.Y - 8, 16, 16, Color.Black, Color.White, 1);
                    }
                    
                    // render images
                    if(i == 0)
                    {
                        hand.X = newPos.X - 8;
                        hand.Y = newPos.Y - 8;
                        hand.Render();
                    }
                    if (i == 1 && Boots > 0)
                    {
                        boots.X = newPos.X - 8;
                        boots.Y = newPos.Y - 8;
                        boots.Render();
                    }
                    if (i == 2 && Circuit > 0)
                    {
                        circuit.X = newPos.X - 8;
                        circuit.Y = newPos.Y - 8;
                        circuit.Render();
                    }
                    if (i == 3 && FloorTile > 0)
                    {
                        tiles.X = newPos.X - 8;
                        tiles.Y = newPos.Y - 8;
                        tiles.Render();
                    }
                    if (i == 4 && Wrench > 0)
                    {
                        wrench.X = newPos.X - 8;
                        wrench.Y = newPos.Y - 8;
                        wrench.Render();
                    }
                    if (i == 5 && Battery > 0)
                    {
                        battery.X = newPos.X - 8;
                        battery.Y = newPos.Y - 8;
                        battery.Render();
                    }
                    if (i == 6 && Donut > 0)
                    {
                        donut.X = newPos.X - 8;
                        donut.Y = newPos.Y - 8;
                        donut.Render();
                    }
                    if (i == 7 && Crisps > 0)
                    {
                        crisps.X = newPos.X - 8;
                        crisps.Y = newPos.Y - 8;
                        crisps.Render();
                    }
                    if (i == 8 && FireExtinguisher > 0)
                    {
                        fireextinguisher.X = newPos.X - 8;
                        fireextinguisher.Y = newPos.Y - 8;
                        fireextinguisher.Render();
                    }
                    if (i == 9 && O2Tank > 0)
                    {
                        o2tank.X = newPos.X - 8;
                        o2tank.Y = newPos.Y - 8;
                        o2tank.Render();
                    }


                }
            }
            
        }


    }
}
