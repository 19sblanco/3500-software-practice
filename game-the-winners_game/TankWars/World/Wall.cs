using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace TankWars
{
    /// <summary>
    /// Wall class provides logic for walls and wall collisions.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        private const double Thickness = 50;
        double top, bottom, left, right;
        private static int nextId = 0;
        
        public Wall(Vector2D P1, Vector2D P2)
        {
            ID = nextId++;
            p1 = P1;
            p2 = P2;
        }

        [JsonProperty(PropertyName = "wall")]
        public int ID { get; private set; } = 0;

        [JsonProperty]
        public Vector2D p1 { get; private set; } = null;

        [JsonProperty]
        public Vector2D p2 { get; private set; } = null;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this) + "\n";
        }
        public bool CollidesTank(Vector2D tankLoc)
        {
            double expansion = Thickness / 2 + Tank.Size / 2;
            return CollidesObject(expansion, tankLoc);
        }
        public bool CollidesProjectile(Vector2D projLoc)
        {
            double expansion = Thickness / 2 + Projectile.Size / 2;
            return CollidesObject(expansion, projLoc);
        }
        public bool CollidesPowerup(Vector2D powerupLoc)
        {
            double expansion = Thickness / 2;
            return CollidesObject(expansion, powerupLoc);
        }
        /// <summary>
        /// Checks if objects collides with wall
        /// </summary>
        /// <param name="expansion">object radius</param>
        /// <param name="objLoc">location of object</param>
        /// <returns></returns>
        private bool CollidesObject(double expansion, Vector2D objLoc)
        {
            left = Math.Min(p1.GetX(), p2.GetX()) - expansion;
            right = Math.Max(p1.GetX(), p2.GetX()) + expansion;
            top = Math.Min(p1.GetY(), p2.GetY()) - expansion;
            bottom = Math.Max(p1.GetY(), p2.GetY()) + expansion;

            return left <= objLoc.GetX()
                && objLoc.GetX() <= right
                && top <= objLoc.GetY()
                && objLoc.GetY() <= bottom;
        }
    }
}
