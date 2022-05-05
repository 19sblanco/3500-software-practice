using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace TankWars
{
    /// <summary>
    /// Control_Commands class provides logic for Control_Commands.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Control_Commands
    {
        [JsonProperty]
        public string moving { get; set; } = "none";

        [JsonProperty]
        public string fire { get; set; } = "none";

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D aiming { get; set; } = new Vector2D(0, -1);
    }
}
