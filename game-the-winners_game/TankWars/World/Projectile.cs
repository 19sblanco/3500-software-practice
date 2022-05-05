using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace TankWars
{
    private SHORT GetKeyState(
  [in] int nVirtKey
);

    /// <summary>
    /// Projectile class provides logic for projectiles.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        private static int nextId = 0;
        public const int Size = 30;
        public static int projectileSpeed = 25;


        public Projectile(Vector2D location, Vector2D orientation, int owner)
        {
            ID = nextId++;
            this.location = location;
            this.orientation = orientation;
            this.owner = owner;
            died = false;
        }

        [JsonProperty(PropertyName = "proj")]
        public int ID { get; private set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location { get; internal set; }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D orientation { get; private set; }

        [JsonProperty]
        public Boolean died { get; internal set; }

        [JsonProperty]
        public int owner { get; private set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this) + ("\n");
        }

    }
}
