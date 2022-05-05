using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TankWars
{
    /// <summary>
    /// Contains information for the worlds state of the game and provides logic for
    /// updating each frame.
    /// </summary>
    public class World
    {
        public const int MaxPowerUps = 2;
        public const int MaxFramesTillPowerup = 1650;
        public int FramesTillPowerup = 0;
        public int Powerups = 0;
        private Random random = new Random();

        public int WorldSize { get; set; }
        public Dictionary<int, Wall> walls { get; } = new Dictionary<int, Wall>();
        public Dictionary<int, Tank> tanks { get; } = new Dictionary<int, Tank>();
        public Dictionary<int, Projectile> projectiles { get; } = new Dictionary<int, Projectile>();
        public Dictionary<int, Powerups> powerups { get; } = new Dictionary<int, Powerups>();
        public Dictionary<int, Beams> beams { get; } = new Dictionary<int, Beams>();
        public Dictionary<int, Control_Commands> ctrlCmds = new Dictionary<int, Control_Commands>();
        public World()
        { }
        public World(int size)
        {
            WorldSize = size;
        }
        /// <summary>
        /// finds an unoccupied spot in the map and respawns the tank
        /// </summary>
        /// <param name="tankID"></param>
        private void RespawnTank(Tank tank)
        {
            Vector2D tankLoc;
            while (true)
            {
                bool collisionDetected = false;

                int x = random.Next(-WorldSize / 2, WorldSize / 2);
                int y = random.Next(-WorldSize / 2, WorldSize / 2);
                tankLoc = new Vector2D(x, y);
                foreach (Wall walls in walls.Values)
                {
                    if (walls.CollidesTank(tankLoc))
                    {
                        collisionDetected = true;
                        break;
                    }
                }
                if (!collisionDetected) break;
            }
            tank.hitPoints = Tank.MaxHP;
            tank.location = tankLoc;
        }
        /// <summary>
        /// finds an unoccupied spot in the map and spawns a powerup
        /// </summary>
        private void GeneratePowerups()
        {
            Vector2D powerupLoc;
            while (true)
            {
                bool collisionDetected = false;

                int x = random.Next(-WorldSize / 2, WorldSize / 2);
                int y = random.Next(-WorldSize / 2, WorldSize / 2);
                powerupLoc = new Vector2D(x, y);
                foreach (Wall walls in walls.Values)
                {
                    if (walls.CollidesPowerup(powerupLoc))
                    {
                        collisionDetected = true;
                        break;
                    }
                }
                if (!collisionDetected) break;
            }
            Powerups powerup = new Powerups();
            powerup.location = powerupLoc;
            powerups.Add(powerup.ID, powerup);
            Powerups++;
        }
        /// <summary>
        /// if tank is out of bound it will wrap around the map
        /// </summary>
        /// <param name="tankLoc"></param>
        /// <returns></returns>
        private Vector2D WrapAround(Vector2D tankLoc)
        {
            double tX = tankLoc.GetX();
            double tY = tankLoc.GetY();

            if (tX < -WorldSize / 2)
                return new Vector2D(WorldSize / 2, tY);

            else if (tX > WorldSize / 2)
                return new Vector2D(-WorldSize / 2, tY);

            else if (tY < -WorldSize / 2)
                return new Vector2D(tX, WorldSize / 2);

            else if (tY > WorldSize / 2)
                return new Vector2D(tX, -WorldSize / 2);

            else
                return tankLoc;
        }
        /// <summary>
        /// if projectiles are out of bound return true
        /// so we know to mark them as dead
        /// </summary>
        /// <param name="projLoc"></param>
        /// <returns></returns>
        private bool CleanUp(Vector2D projLoc)
        {
            double pX = projLoc.GetX();
            double pY = projLoc.GetY();

            if (pX < -WorldSize / 2 ||
                pX > WorldSize / 2 ||
                pY < -WorldSize / 2 ||
                pY > WorldSize / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// removes all dead objects
        /// </summary>
        public void cleanupDead()
        {
            foreach (Tank tank in tanks.Values.ToArray())
            {
                if (tank.disconnected)
                {
                    tanks.Remove(tank.ID);
                }
            }
            foreach (Projectile proj in projectiles.Values.ToArray())
            {
                if (proj.died)
                    projectiles.Remove(proj.ID);
            }
            foreach (Beams beam in beams.Values.ToArray())
            {
                beams.Remove(beam.ID);
            }

            foreach (Powerups powerup in powerups.Values.ToArray())
            {
                if (powerup.died)
                {
                    powerups.Remove(powerup.ID);
                }
            }
            foreach (Beams beam in beams.Values.ToArray())
            {
                beams.Remove(beam.ID);
            }
        }

        /// <summary>
        /// this contains the logic for each frame
        /// updating the state of all things contained in the world
        /// </summary>
        public void Update()
        {
            if (FramesTillPowerup <= 0 && Powerups < 2)
            {
                GeneratePowerups();
                FramesTillPowerup = random.Next(0, MaxFramesTillPowerup);
            }
            else
            {
                FramesTillPowerup--;
            }

            foreach (KeyValuePair<int, Control_Commands> ctrlCmd in ctrlCmds)
            {
                Tank tank = tanks[ctrlCmd.Key];
                if (tank.hitPoints == 0) continue;

                switch (ctrlCmd.Value.moving)
                {
                    case "up":
                        tank.Velocity = new Vector2D(0, -1);
                        tank.orientation = new Vector2D(0, -1);
                        break;

                    case "down":
                        tank.Velocity = new Vector2D(0, 1);
                        tank.orientation = new Vector2D(0, 1);
                        break;

                    case "left":
                        tank.Velocity = new Vector2D(-1, 0);
                        tank.orientation = new Vector2D(-1, 0);
                        break;

                    case "right":
                        tank.Velocity = new Vector2D(1, 0);
                        tank.orientation = new Vector2D(1, 0);
                        break;

                    default:
                        tank.Velocity = new Vector2D(0, 0);
                        break;

                }
                switch (ctrlCmd.Value.fire)
                {
                    case "main":
                        if (tank.framesToFire == 0)
                        {
                            Projectile proj = new Projectile(tank.location, tank.aiming, tank.ID);
                            projectiles.Add(proj.ID, proj);
                            tank.framesToFire = tank.maxFramesToFire;
                        }
                        break;

                    case "alt":
                        if (tank.powerups > 0)
                        {
                            Beams beam = new Beams(tank.location, tank.aiming, tank.ID);
                            beams.Add(beam.ID, beam);
                            tank.powerups--;
                        }
                        break;

                }
                tank.aiming = ctrlCmd.Value.aiming;
                tank.Velocity *= Tank.EnginePower;
            }
            ctrlCmds.Clear();

            foreach (Tank tank in tanks.Values)
            {
                if (tank.disconnected)
                {
                    tank.hitPoints = 0;
                    tank.died = true;
                }

                if (tank.framesToFire > 0)
                    tank.framesToFire--;

                if (tank.framesToRespawn > 0)
                {
                    tank.hitPoints = 0;
                    tank.framesToRespawn--;
                    tank.died = false;
                    continue;
                }

                if ((tank.hitPoints == 0 && tank.framesToRespawn <= 0) || tank.joined)
                {
                    RespawnTank(tank);
                    tank.joined = false;
                }

                foreach (Projectile proj in projectiles.Values)
                {
                    if (tank.CollidesProjectile(proj.location) && proj.owner != tank.ID)
                    {
                        proj.died = true;
                        tank.hitPoints--;
                        if (tank.hitPoints == 0)
                        {
                            tank.died = true;
                            tank.framesToRespawn = Tank.maxFramesToRespawn;
                            tanks[proj.owner].score++;
                        }
                    }
                }
                
                foreach(Beams beam in beams.Values)
                {
                    if (tank.CollidesBeam(beam.origin, beam.direction, tank.location,Tank.Size/2) && beam.owner != tank.ID)
                    {
                        tank.hitPoints = 0;
                        if (tank.hitPoints == 0)
                        {
                            tank.died = true;
                            tank.framesToRespawn = Tank.maxFramesToRespawn;
                            tanks[beam.owner].score++;
                        }
                    }
                }

                if (tank.Velocity.Length() == 0)
                {
                    continue;
                }
                Vector2D newLoc = tank.location + tank.Velocity;
                bool collision = false;

                foreach (Wall wall in walls.Values)
                {
                    if (wall.CollidesTank(newLoc))
                    {
                        collision = true;
                        tank.Velocity = new Vector2D(0, 0);
                        break;
                    }
                }

                if (!collision)
                {
                    tank.location = WrapAround(newLoc);
                }

                foreach (Powerups powerup in powerups.Values)
                {
                    if (tank.CollidesPowerup(powerup.location))
                    {
                        powerup.died = true;
                        tank.powerups++;
                        Powerups--;
                    }
                }
            }

            foreach (Projectile proj in projectiles.Values)
            {
                proj.orientation.Normalize();
                Vector2D projVelocity = proj.orientation * Projectile.projectileSpeed;
                Vector2D newLoc = proj.location + projVelocity;

                if (CleanUp(newLoc))
                {
                    proj.died = true;
                    continue;
                }
                bool collision = false;

                foreach (Wall wall in walls.Values)
                {
                    if (wall.CollidesProjectile(newLoc))
                    {
                        collision = true;
                        proj.died = true;
                        break;
                    }
                }

                if (!collision)
                {
                    proj.location = newLoc;
                }
            }
        }
    }
}
