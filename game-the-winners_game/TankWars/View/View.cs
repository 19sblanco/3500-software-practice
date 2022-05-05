using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GC;
using TankWars;


namespace View
{
    /// <summary>
    /// provides the form for having a name and connecting to a server
    /// 
    /// gets told by the controller when to draw a frame
    /// then draws the world states
    /// </summary>
    public partial class View : Form
    {
        // The controller handles updates from the "server"
        // and notifies us via an event
        public GameController controller = new GameController();
        // World is a simple container for Players and Powerups
        // The controller owns the world, but we have a reference to it
        private World TheWorld;
        private int ImageSize = 900;
        DrawingPanel drawingPanel;


        public View()
        {
            InitializeComponent();


            TheWorld = controller.TheWorld;
            controller.UpdateArrived += OnFrame;
            controller.Error += ShowError;

            // Place and add the drawing panel
            drawingPanel = new DrawingPanel(TheWorld, controller);
            drawingPanel.Location = new Point(0, 45);
            drawingPanel.Size = new Size(ImageSize, ImageSize);
            drawingPanel.Visible = true;
            this.Controls.Add(this.drawingPanel);

            // Set up key and mouse handlers
            this.KeyDown += HandleKeyDown;
            this.KeyUp += HandleKeyUp;
            drawingPanel.MouseDown += HandleMouseDown;
            drawingPanel.MouseUp += HandleMouseUp;
            drawingPanel.MouseMove += HandleMouseMove;

        }



        /// <summary>
        /// button that connects to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (playerNameBox.Text.Length <= 16)
            {
                connectButton.Enabled = false;
                serverAddressBox.Enabled = false;
                playerNameBox.Enabled = false;
                KeyPreview = true;
                controller.Connect(playerNameBox.Text, serverAddressBox.Text);
            }
            else
            {
                MessageBox.Show("Name must be less than 16 characters");
                playerNameBox.Text = "";
            }
        }

        /// <summary>
        /// Handler for the controller's UpdateArrived event
        /// </summary>
        private void OnFrame()
        {
            try
            {
                Invoke(new MethodInvoker(() => Invalidate(true)));
            }
            catch { }
        }

        /// <summary>
        /// Error handling
        /// </summary>
        /// <param name="message">message</param>
        private void ShowError(string message)
        {
            MessageBox.Show(message);

            this.Invoke(new MethodInvoker(
            () =>
            {
                serverAddressBox.Text = "localhost";
                connectButton.Enabled = true;
                serverAddressBox.Enabled = true;
            }));
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.Q:
                    Application.Exit();
                    break;
                case Keys.W:
                    controller.HandleMoveRequest("up");
                    break;
                case Keys.S:
                    controller.HandleMoveRequest("down");
                    break;
                case Keys.A:
                    controller.HandleMoveRequest("left");
                    break;
                case Keys.D:
                    controller.HandleMoveRequest("right");
                    break;
            }

            // Prevent other key handlers from running
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        /// <summary>
        /// Key up handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    controller.CancelMoveRequest("up");
                    break;
                case Keys.S:
                    controller.CancelMoveRequest("down");
                    break;
                case Keys.A:
                    controller.CancelMoveRequest("left");
                    break;
                case Keys.D:
                    controller.CancelMoveRequest("right");
                    break;
            }

        }

        /// <summary>
        /// Handle mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                controller.HandleMouseRequest("main");
            if (e.Button == MouseButtons.Right)
                controller.HandleMouseRequest("alt");
        }

        /// <summary>
        /// Handle mouse up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                controller.HandleMouseRequest("none");
        }

        /// <summary>
        /// tell controller that the mouse 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int translatedX = e.X - (ImageSize / 2);
            int translatedY = e.Y - (ImageSize / 2);
            controller.HandleTurretDirection(translatedX, translatedY);
        }

        /// <summary>
        /// the part of the view thats draws things onto the screen
        /// uses resources from the model (world)
        /// </summary>
        private class DrawingPanel : Panel
        {
            private World TheWorld;
            private GameController controller;
            int BeamCount = 0;
            int ExplosionCount = 0;
            ///Wall background and Explosion images
            Image wall = Image.FromFile("../../../Resources/Images/WallSprite.png");
            Image background = Image.FromFile("../../../Resources/Images/Background.png");
            Image explosionImage = Image.FromFile("../../../Resources/Images/explosion.png");
            ///Tank images
            Image tankImage0 = Image.FromFile("../../../Resources/Images/BlueTank.png");
            Image tankImage1 = Image.FromFile("../../../Resources/Images/DarkTank.png");
            Image tankImage2 = Image.FromFile("../../../Resources/Images/GreenTank.png");
            Image tankImage3 = Image.FromFile("../../../Resources/Images/LightGreenTank.png");
            Image tankImage4 = Image.FromFile("../../../Resources/Images/OrangeTank.png");
            Image tankImage5 = Image.FromFile("../../../Resources/Images/PurpleTank.png");
            Image tankImage6 = Image.FromFile("../../../Resources/Images/RedTank.png");
            Image tankImage7 = Image.FromFile("../../../Resources/Images/YellowTank.png");
            ///Turret images
            Image turretImage0 = Image.FromFile("../../../Resources/Images/BlueTurret.png");
            Image turretImage1 = Image.FromFile("../../../Resources/Images/DarkTurret.png");
            Image turretImage2 = Image.FromFile("../../../Resources/Images/GreenTurret.png");
            Image turretImage3 = Image.FromFile("../../../Resources/Images/LightGreenTurret.png");
            Image turretImage4 = Image.FromFile("../../../Resources/Images/OrangeTurret.png");
            Image turretImage5 = Image.FromFile("../../../Resources/Images/PurpleTurret.png");
            Image turretImage6 = Image.FromFile("../../../Resources/Images/RedTurret.png");
            Image turretImage7 = Image.FromFile("../../../Resources/Images/YellowTurret.png");
            ///Shot images
            Image shotImage0 = Image.FromFile("../../../Resources/Images/shot-blue.png");
            Image shotImage1 = Image.FromFile("../../../Resources/Images/shot-brown.png");
            Image shotImage2 = Image.FromFile("../../../Resources/Images/shot-green.png");
            Image shotImage3 = Image.FromFile("../../../Resources/Images/shot-white.png");
            Image shotImage4 = Image.FromFile("../../../Resources/Images/shot-grey.png");
            Image shotImage5 = Image.FromFile("../../../Resources/Images/shot-violet.png");
            Image shotImage6 = Image.FromFile("../../../Resources/Images/shot-red.png");
            Image shotImage7 = Image.FromFile("../../../Resources/Images/shot-yellow.png");
            /// <summary>
            /// Constructor used by the view to draw.
            /// </summary>
            /// <param name="w">The world</param>
            /// <param name="controller">The object that controlles your tank</param>
            public DrawingPanel(World w, GameController controller)
            {
                DoubleBuffered = true;
                TheWorld = w;
                this.controller = controller;
            }

            // A delegate for DrawObjectWithTransform
            // Methods matching this delegate can draw whatever they want using e  
            public delegate void ObjectDrawer(object o, PaintEventArgs e);

            /// <summary>
            /// This method performs a translation and rotation to draw an object in the world.
            /// </summary>
            /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
            /// <param name="o">The object to draw</param>
            /// <param name="worldX">The X coordinate of the object in world space</param>
            /// <param name="worldY">The Y coordinate of the object in world space</param>
            /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
            /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
            private void DrawObjectWithTransform(PaintEventArgs e, object o, double worldX, double worldY, double angle, ObjectDrawer drawer)
            {
                // "push" the current transform
                System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

                e.Graphics.TranslateTransform((int)worldX, (int)worldY);
                e.Graphics.RotateTransform((float)angle);
                drawer(o, e);

                // "pop" the transform
                e.Graphics.Transform = oldMatrix;
            }

            /// <summary>
            /// This method is invoked when the DrawingPanel needs to be re-drawn
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaint(PaintEventArgs e)
            {
                // Center the view on the middle of the world,
                // since the image and world use different coordinate systems
                int viewSize = Size.Width; // view is square, so we can just use width
                int Id = controller.Id;

                lock (TheWorld)
                {
                    if (!TheWorld.tanks.ContainsKey(Id))
                    {
                        return;
                    }
                    double playerX = TheWorld.tanks[Id].location.GetX();
                    double playerY = TheWorld.tanks[Id].location.GetY();

                    e.Graphics.TranslateTransform((float)(-playerX + (viewSize / 2)), (float)(-playerY + (viewSize / 2)));

                    DrawObjectWithTransform(e, TheWorld, 0, 0, 0, WorldDrawer);
                    for (int i = 0; i < TheWorld.walls.Count; i++)
                    {
                        // calcualte the number of blocks in a wall
                        // then call draw object with tranfrom on each block, with new x and y coordinates
                        double width = TheWorld.walls[i].p1.GetX() - TheWorld.walls[i].p2.GetX();
                        double height = TheWorld.walls[i].p1.GetY() - TheWorld.walls[i].p2.GetY();

                        if (width < 0)
                        {
                            width *= -1;
                            for (int j = 0; j <= width / 50; j++)
                            {
                                DrawObjectWithTransform(e, TheWorld.walls[i], TheWorld.walls[i].p1.GetX() + (j * 50), TheWorld.walls[i].p1.GetY(), 0, WallDrawer);
                            }
                        }
                        else if (width > 0)
                        {
                            for (int j = 0; j <= width / 50; j++)
                            {
                                DrawObjectWithTransform(e, TheWorld.walls[i], TheWorld.walls[i].p2.GetX() + (j * 50), TheWorld.walls[i].p2.GetY(), 0, WallDrawer);
                            }
                        }
                        else if (height < 0)
                        {
                            height *= -1;
                            for (int j = 0; j <= height / 50; j++)
                            {
                                DrawObjectWithTransform(e, TheWorld.walls[i], TheWorld.walls[i].p1.GetX(), TheWorld.walls[i].p1.GetY() + (j * 50), 0, WallDrawer);
                            }

                        }
                        else if (height > 0)
                        {
                            for (int j = 0; j <= height / 50; j++)
                            {
                                DrawObjectWithTransform(e, TheWorld.walls[i], TheWorld.walls[i].p2.GetX(), TheWorld.walls[i].p2.GetY() + (j * 50), 0, WallDrawer);
                            }
                        }
                    }
                    // draw tank and turret
                    foreach (Tank tank in TheWorld.tanks.Values)
                    {
                        DrawObjectWithTransform(e, tank, tank.location.GetX(), tank.location.GetY(), tank.orientation.ToAngle(), TankDrawer);
                        DrawObjectWithTransform(e, tank, tank.location.GetX(), tank.location.GetY(), tank.aiming.ToAngle(), TurretDrawer);
                        DrawObjectWithTransform(e, tank, tank.location.GetX(), tank.location.GetY(), 0, explosionDrawer);
                        DrawObjectWithTransform(e, tank, tank.location.GetX(), tank.location.GetY(), 0, NameDrawer);
                        DrawObjectWithTransform(e, tank, tank.location.GetX(), tank.location.GetY(), 0, HealthDrawer);

                    }
                    foreach (Projectile shot in TheWorld.projectiles.Values)
                    {
                        DrawObjectWithTransform(e, shot, shot.location.GetX(), shot.location.GetY(), shot.orientation.ToAngle(), ShotDrawer);
                    }
                    foreach (Powerups pow in TheWorld.powerups.Values)
                    {
                        DrawObjectWithTransform(e, pow, pow.location.GetX(), pow.location.GetY(), 0, PowerupDrawer);
                    }
                    ArrayList beamsToRemove = new ArrayList();
                    foreach (Beams beam in TheWorld.beams.Values)
                    {
                        if (BeamCount < 60)
                        {
                            Tuple<Beams, int> beamAndSize = new Tuple<Beams, int>(beam, BeamCount);
                            DrawObjectWithTransform(e, beamAndSize, beam.origin.GetX(), beam.origin.GetY(), beam.direction.ToAngle() + 180, BeamDrawer);
                            BeamCount++;
                        }
                        else
                        {
                            beamsToRemove.Add(beam.ID);
                        }
                    }
                    foreach (int beam in beamsToRemove)
                    {
                        controller.removeBeam(beam);
                        BeamCount = 0;
                    }
                }

                base.OnPaint(e);
            }

            /// <summary>
            /// draws the background of the game
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void WorldDrawer(object o, PaintEventArgs e)
            {
                World w = o as World;
                int width = TheWorld.WorldSize;
                int height = width;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
                e.Graphics.DrawImage(background, r);
            }

            /// <summary>
            /// draws the walls on the map
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void WallDrawer(object o, PaintEventArgs e)
            {
                Wall w = o as Wall;
                int width = 50;
                int height = 50;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                // Rectangles are drawn starting from the top-left corner.
                // So if we want the rectangle centered on the player's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
                e.Graphics.DrawImage(wall, r);
            }

            private void explosionDrawer(object o, PaintEventArgs e)
            {
                Tank t = o as Tank;
                int tankWidth = 60;
                if (t.hitPoints == 0)
                {
                    if (ExplosionCount < 120)
                    {
                        Rectangle explosionRectangle = new Rectangle(-(tankWidth / 2) - ExplosionCount, -(tankWidth / 2) - ExplosionCount, tankWidth + ExplosionCount, tankWidth + ExplosionCount);
                        e.Graphics.DrawImage(explosionImage, explosionRectangle);
                        return;
                    }
                    else
                    {
                        ExplosionCount = 0;
                    }
                }

                ExplosionCount++;
            }
            /// <summary>
            /// Draws the name of the player.
            /// </summary>
            /// <param name="o">Object being drawn</param>
            /// <param name="e"></param>
            private void NameDrawer(object o, PaintEventArgs e)
            {
                Tank t = o as Tank;
                int tankId = t.ID % 8;
                Font drawFont = new Font("Arial", 16);
                SolidBrush drawBrush = new SolidBrush(Color.Black);

                float x = 30f;
                float y = 30.0F;

                // Set format of string.
                StringFormat drawFormat = new StringFormat();
                drawFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;

                // Draw string to screen.
                e.Graphics.DrawString(t.name + ": " + t.score, drawFont, drawBrush, x, y, drawFormat);
            }
            /// <summary>
            /// Draws the healthBar
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void HealthDrawer(object o, PaintEventArgs e)
            {
                Tank t = o as Tank;
                int tankWidth = 60;

                using (System.Drawing.SolidBrush greenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green))
                using (System.Drawing.SolidBrush yellowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow))
                using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                {
                    if (t.hitPoints == 3)
                    {
                        Rectangle healthRectangle = new Rectangle(-(tankWidth - 25), -(tankWidth), 60, 5);
                        e.Graphics.FillRectangle(greenBrush, healthRectangle);
                    }
                    else if (t.hitPoints == 2)
                    {
                        Rectangle healthRectangle = new Rectangle(-(tankWidth - 25), -(tankWidth), 40, 5);
                        e.Graphics.FillRectangle(yellowBrush, healthRectangle);
                    }
                    else if (t.hitPoints == 1)
                    {
                        Rectangle healthRectangle = new Rectangle(-(tankWidth - 25), -(tankWidth), 20, 5);
                        e.Graphics.FillRectangle(redBrush, healthRectangle);

                    }
                    else
                    {
                        return;
                    }
                }
            }
            /// <summary>
            /// draws a tank image
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void TankDrawer(object o, PaintEventArgs e)
            {
                Tank t = o as Tank;
                int tankWidth = 60;
                int tankId = t.ID % 8;
                Image tankImage;
                Rectangle tankRectangle = new Rectangle(-(tankWidth / 2), -(tankWidth / 2), tankWidth, tankWidth); ;

                switch (tankId)
                {
                    case 0:
                        tankImage = tankImage0;
                        break;
                    case 1:
                        tankImage = tankImage1;
                        break;
                    case 2:
                        tankImage = tankImage2;
                        break;
                    case 3:
                        tankImage = tankImage3;
                        break;
                    case 4:
                        tankImage = tankImage4;
                        break;
                    case 5:
                        tankImage = tankImage5;
                        break;
                    case 6:
                        tankImage = tankImage6;
                        break;
                    default:
                        tankImage = tankImage7;
                        break;
                }



                e.Graphics.DrawImage(tankImage, tankRectangle);
            }


            /// <summary>
            /// draws a TurretImage image
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void TurretDrawer(object o, PaintEventArgs e)
            {
                Tank t = o as Tank;
                int turretWidth = 50;
                int tankId = t.ID % 8;
                Image turretImage;
                switch (tankId)
                {
                    case 0:
                        turretImage = turretImage0;
                        break;
                    case 1:
                        turretImage = turretImage1;
                        break;
                    case 2:
                        turretImage = turretImage2;

                        break;
                    case 3:
                        turretImage = turretImage3;
                        break;
                    case 4:
                        turretImage = turretImage4;
                        break;
                    case 5:
                        turretImage = turretImage5;
                        break;
                    case 6:
                        turretImage = turretImage6;
                        break;
                    default:
                        turretImage = turretImage7;
                        break;
                }

                if (t.hitPoints == 0)
                {
                    return;
                }

                Rectangle turretRectangle = new Rectangle(-(turretWidth / 2), -(turretWidth / 2), turretWidth, turretWidth);
                e.Graphics.DrawImage(turretImage, turretRectangle);
            }

            /// <summary>
            /// draws a TurretImage image
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void ShotDrawer(object o, PaintEventArgs e)
            {
                Projectile projectile = o as Projectile;
                int shotWidth = 30;
                int projectileOwner = projectile.owner % 8;
                Image shotImage;
                switch (projectileOwner)
                {
                    case 0:
                        shotImage = shotImage0;
                        break;
                    case 1:
                        shotImage = shotImage1;
                        break;
                    case 2:
                        shotImage = shotImage2;
                        break;
                    case 3:
                        shotImage = shotImage3;
                        break;
                    case 4:
                        shotImage = shotImage4;
                        break;
                    case 5:
                        shotImage = shotImage5;
                        break;
                    case 6:
                        shotImage = shotImage6;
                        break;
                    default:
                        shotImage = shotImage7;
                        break;
                }

                Rectangle shotRectangle = new Rectangle(-(shotWidth / 2), -(shotWidth / 2), shotWidth, shotWidth);
                e.Graphics.DrawImage(shotImage, shotRectangle);
            }
            /// <summary>
            /// Acts as a drawing delegate for DrawObjectWithTransform
            /// After performing the necessary transformation (translate/rotate)
            /// DrawObjectWithTransform will invoke this method
            /// </summary>
            /// <param name="o">The object to draw</param>
            /// <param name="e">The PaintEventArgs to access the graphics</param>
            private void PowerupDrawer(object o, PaintEventArgs e)
            {
                Powerups p = o as Powerups;
                int width = 8;
                int height = 8;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))

                {
                    // Circles are drawn starting from the top-left corner.
                    // So if we want the circle centered on the powerup's location, we have to offset it
                    // by half its size to the left (-width/2) and up (-height/2)
                    Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
                    e.Graphics.FillEllipse(redBrush, r);
                }
            }
            private void BeamDrawer(object o, PaintEventArgs e)
            {
                Tuple<Beams, int> beamAndSize = o as Tuple<Beams, int>;
                int width = 7; // animate this
                int height = TheWorld.WorldSize * 2;
                int size = beamAndSize.Item2;

                Rectangle r = new Rectangle(0, 0, width - (size / 10), height);

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                {
                    e.Graphics.FillRectangle(redBrush, r);
                }
            }
        }
    }
}
