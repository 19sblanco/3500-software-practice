using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace TankWars
{
    /// <summary>
    /// Beams class provides logic for Beams.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beams
    {
        private static int nextId = 0;

        public Beams(Vector2D origin, Vector2D direction, int owner)
        {
            ID = nextId++;
            this.origin = origin;
            this.direction = direction;
            this.owner = owner;
        }

        [JsonProperty(PropertyName = "beam")]
        public int ID { get; private set; }

        [JsonProperty(PropertyName = "org")]
        public Vector2D origin { get; private set; }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D direction { get; private set; }

        [JsonProperty]
        public int owner { get; private set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this) + "\n";
        }
    }
}
