using System;
using System.Text.RegularExpressions;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TankWars;

namespace GC
{
    /// <summary>
    /// recieves updates from the server. parses information
    /// then updates the world and tells the view
    /// </summary>
    public class GameController
    {
        private string PlayerName;
        public int Id;
        bool receivedJson = true;
        public World TheWorld = new World();
        public Control_Commands control_Commands = new Control_Commands();
        public SocketState state;
        public delegate void ErrorHandler(string err);
        public event ErrorHandler Error;
        private bool playerWantsUp, playerWantsDown, playerWantsLeft, playerWantsRight;

        public event Action UpdateArrived;

        /// <summary>
        /// connect to the server
        /// </summary>
        /// <param name="player"></param>
        /// <param name="hostname"></param>
        public void Connect(string player, string hostname)
        {
            PlayerName = player;
            Networking.ConnectToServer(OnConnect, hostname, 11000);
        }
        /// <summary>
        /// call back for connect, send name to server
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                Error(state.ErrorMessage);
                return;
            }

            this.state = state;
            Networking.Send(state.TheSocket, PlayerName + '\n');
            state.OnNetworkAction = StartupInfo;
            Networking.GetData(state);
        }

        /// <summary>
        /// call back for onconnect
        /// recieve starting information from server, such as name and world size
        /// </summary>
        /// <param name="state"></param>
        private void StartupInfo(SocketState state)
        {
            if (state.ErrorOccurred == true)
            {
                Error(state.ErrorMessage);
                return;
            }

            string info = state.GetData();
            string[] parts = Regex.Split(info, @"(?<=[\n])");
            if (parts.Length < 2 || !parts[1].EndsWith("\n"))
            {
                Networking.GetData(state);
                return;
            }

            if (state.ErrorOccurred)
            {
                Error(state.ErrorMessage);
                return;
            }
            receivedJson = true;
            Id = int.Parse(parts[0]);
            TheWorld.WorldSize = int.Parse(parts[1]);
            state.RemoveData(0, parts[0].Length + parts[1].Length);
            state.OnNetworkAction = ReceiveJson;
            Networking.GetData(state);
        }
        /// <summary>
        /// revieve json from server, parse information and update the world (model)
        /// then tells view to draw it
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveJson(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                Error(state.ErrorMessage);
                return;
            }

            string message = state.GetData();
            string[] parts = Regex.Split(message, @"(?<=[\n])");
            lock (TheWorld)
            {
                foreach (string part in parts)
                {
                    // Ignore
                    if (part.Length == 0)
                    {
                        continue;
                    }
                    if (part[part.Length - 1] != '\n')
                    {
                        break;
                    }

                    JObject obj = JObject.Parse(part);
                    JToken token = obj["wall"];
                    if (token != null)
                    {
                        Wall wall = JsonConvert.DeserializeObject<Wall>(part);
                        state.RemoveData(0, part.Length);
                        TheWorld.walls[wall.ID] = wall;
                        continue;
                    }

                    // comeback for tank fields
                    if (obj["tank"] != null)
                    {
                        Tank tank = JsonConvert.DeserializeObject<Tank>(part);
                        state.RemoveData(0, part.Length);
                        if (tank.disconnected || tank.died)
                        {
                            TheWorld.tanks.Remove(tank.ID);
                        }
                        else
                        {
                            TheWorld.tanks[tank.ID] = tank;
                        }
                    }
                    else if (obj["proj"] != null)
                    {
                        Projectile proj = JsonConvert.DeserializeObject<Projectile>(part);
                        state.RemoveData(0, part.Length);
                        if (proj.died)
                        {
                            TheWorld.projectiles.Remove(proj.ID);
                        }
                        else
                        {
                            TheWorld.projectiles[proj.ID] = proj;
                        }
                    }
                    else if (obj["power"] != null)
                    {

                        Powerups powerup = JsonConvert.DeserializeObject<Powerups>(part);
                        state.RemoveData(0, part.Length);
                        if (powerup.died)
                        {
                            TheWorld.powerups.Remove(powerup.ID);
                        }
                        else
                        {
                            TheWorld.powerups[powerup.ID] = powerup;
                        }

                    }
                    else if (obj["beam"] != null)
                    {
                        Beams beam = JsonConvert.DeserializeObject<Beams>(part);
                        state.RemoveData(0, part.Length);
                        TheWorld.beams[beam.ID] = beam;
                    }
                }
            }

            UpdateArrived?.Invoke();
            SendCommand_Control();
            Networking.GetData(state);
        }

        /// <summary>
        /// removes a beam from the model
        /// </summary>
        /// <param name="ID"></param>
        public void removeBeam(int ID)
        {
            TheWorld.beams.Remove(ID);
        }


        /// <summary>
        /// After user input serialize and send the command control
        /// </summary>
        private void SendCommand_Control()
        {
            lock (control_Commands)
            {
                if (playerWantsLeft)
                    control_Commands.moving = "left";
                else if (playerWantsRight)
                    control_Commands.moving = "right";
                else if (playerWantsUp)
                    control_Commands.moving = "up";
                else if (playerWantsDown)
                    control_Commands.moving = "down";
                else
                    control_Commands.moving = "none";
                string message = JsonConvert.SerializeObject(control_Commands);
                Networking.Send(state.TheSocket, message + '\n');
            }
        }

        /// <summary>
        /// Handling movement
        /// </summary>
        public void HandleMoveRequest(string key)
        {
            if (receivedJson)
            {
                switch (key)
                {
                    case "up":
                        playerWantsUp = true;
                        break;
                    case "down":
                        playerWantsDown = true;
                        break;
                    case "left":
                        playerWantsLeft = true;
                        break;
                    case "right":
                        playerWantsRight = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Handling movement
        /// </summary>
        public void CancelMoveRequest(string key)
        {
            if (receivedJson)
            {
                switch (key)
                {
                    case "up":
                        playerWantsUp = false;
                        break;
                    case "down":
                        playerWantsDown = false;
                        break;
                    case "left":
                        playerWantsLeft = false;
                        break;
                    case "right":
                        playerWantsRight = false;
                        break;
                }
            }
        }

        /// <summary>
        /// handling mouse fire request
        /// </summary>
        public void HandleMouseRequest(string fireType)
        {
            if (receivedJson)
            {
                lock (control_Commands)
                {
                    control_Commands.fire = fireType;
                }
            }
        }

        /// <summary>
        /// normalize and set direction of turret to be direction of mouse relative to tank
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void HandleTurretDirection(int x, int y)
        {
            if (receivedJson)
            {
                lock (control_Commands)
                {
                    Vector2D turretDirection = new Vector2D(x, y);
                    turretDirection.Normalize();
                    control_Commands.aiming = turretDirection;
                }
            }
        }
    }
}
