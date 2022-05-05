using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace TankWars
{   
    /// <summary>
    /// Tank class provides logic for tank and tank collisions.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        public const int MaxHP = 3;
        public const double EnginePower = 3;
        public const int Size = 60;
        public int maxFramesToFire;
        public const int maxFramesToRespawn = 300;
        public int currentPowerups = 0;

        public Tank(int id, string Name)
        {
            ID = id;
            location = new Vector2D(0, 0);
            orientation = new Vector2D(0, -1);
            aiming = orientation;
            Name = name;
            hitPoints = MaxHP;
            score = 0;
            died = false;
            disconnected = false;
            joined = true;
            Velocity = new Vector2D(0, 0);
        }
        public int framesToFire { get; internal set; } = 0;
        public int powerups { get; internal set; } = 0;
        public int framesToRespawn { get; internal set; } = 0;

        [JsonProperty(PropertyName = "tank")]
        public int ID { get; private set; }
        [JsonProperty(PropertyName = "loc")]
        public Vector2D location { get; internal set; }
        [JsonProperty(PropertyName = "bdir")]
        public Vector2D orientation { get; internal set; }
        [JsonProperty(PropertyName = "tdir")]
        public Vector2D aiming { get; internal set; }
        [JsonProperty(PropertyName = "name")]
        public string name { get; internal set; }
        [JsonProperty(PropertyName = "hp")]
        public int hitPoints = MaxHP;
        [JsonProperty(PropertyName = "score")]
        public int score { get; internal set; }
        [JsonProperty(PropertyName = "died")]
        public bool died { get; internal set; }
        [JsonProperty(PropertyName = "dc")]
        public bool disconnected { get; set; }
        [JsonProperty(PropertyName = "join")]
        public bool joined { get; internal set; }
        public Vector2D Velocity { get; internal set; }
       
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this)+ "\n"; 
        }
        public bool CollidesProjectile(Vector2D projLoc)
        {
            return (location - projLoc).Length() <= Size;
        }
        public bool CollidesPowerup(Vector2D powerupLoc)
        {
            return (location - powerupLoc).Length() <= Size;
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public bool CollidesBeam(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);

        }
    }
}
