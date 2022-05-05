using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace TankWars
{
    /// <summary>
    /// Powerups class provides logic for powerups.
    /// </summary>
    public class Powerups
    {
        private static int nextId = 0;
        public Powerups()
        {
            ID = nextId++;
        }

        [JsonProperty(PropertyName = "power")]
        public int ID { get; private set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location { get; internal set; }

        [JsonProperty]
        public bool died { get; internal set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this) + "\n";
        }
    }
}
