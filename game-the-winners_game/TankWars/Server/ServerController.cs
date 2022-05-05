using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NetworkUtil;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Connects clients. Sends world state to each client.
    /// </summary>
    class ServerController
    {
        private Settings Settings;
        private World theWorld;
        private Dictionary<int, SocketState> clients = new Dictionary<int, SocketState>();
        private string startupInfo;
        /// <summary>
        /// Gets world size and walls and sends starting information to client
        /// </summary>
        /// <param name="settings"></param>
        public ServerController(Settings settings)
        {
            this.Settings = settings;
            theWorld = new World(settings.UniverseSize);

            StringBuilder sb = new StringBuilder();
            sb.Append(theWorld.WorldSize);
            sb.Append("\n");

            foreach (Wall wall in settings.Walls)
            {
                theWorld.walls[wall.ID] = wall;
            }
            foreach (Wall wall in theWorld.walls.Values)
            {
                sb.Append(wall.ToString());
            }
            startupInfo = sb.ToString();
        }
        /// <summary>
        /// Starts excepting clients
        /// </summary>
        internal void Start()
        {
            Console.WriteLine("Server is running. Accepting clients.");
            Networking.StartServer(NewClient, 11000);
            Thread t = new Thread(Update);
            t.Start();
        }
        /// <summary>
        /// Tells model to update world state. Serializes it and sends it to each client.
        /// </summary>
        private void Update()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                while (watch.ElapsedMilliseconds < Settings.MSPerFram) ;
                watch.Restart();
                StringBuilder sb = new StringBuilder();
                lock (theWorld)
                {
                    theWorld.Update();

                    foreach (Tank tank in theWorld.tanks.Values)
                    {
                        sb.Append(tank.ToString());
                    }

                    foreach (Projectile proj in theWorld.projectiles.Values)
                    {
                        sb.Append(proj.ToString());
                    }

                    foreach (Powerups powerup in theWorld.powerups.Values)
                    {
                        sb.Append(powerup.ToString());
                    }

                    foreach (Beams beam in theWorld.beams.Values)
                    {
                        sb.Append(beam.ToString());
                    }
                    theWorld.cleanupDead();
                }
                string frame = sb.ToString();
                lock (clients)
                {
                    List<int> disconnectedClients = new List<int>();
                    foreach (SocketState client in clients.Values)
                    {
                        if(!Networking.Send(client.TheSocket, frame))
                        {
                            disconnectedClients.Add((int)client.ID);
                            lock (theWorld)
                            {
                                theWorld.tanks[(int)client.ID].disconnected = true;
                            }
                        }
                    }

                    foreach (int disconnect in disconnectedClients)
                    {
                        RemoveClient(disconnect);
                    }
                }
            }
        }
        /// <summary>
        /// Starts process of excepting new client.
        /// </summary>
        /// <param name="client"></param>
        private void NewClient(SocketState client)
        {
            if (client.ErrorOccurred)
            {
                return;
            }
            client.OnNetworkAction = ReceivePlayerName;
            Networking.GetData(client);
        }
        /// <summary>
        /// Receives player name and creates a tank for them. Then allows the client to start sending comands.
        /// </summary>
        /// <param name="client"></param>
        private void ReceivePlayerName(SocketState client)
        {
            if (client.ErrorOccurred)
            {
                return;
            }
            string name = client.GetData();
            Console.WriteLine("Accepted New Connection:\n"+"New player connected: ID " + client.ID);

            if (!name.EndsWith("\n"))
            {
                client.GetData();
                return;
            }
            client.RemoveData(0, name.Length);
            name = name.Trim();

            Networking.Send(client.TheSocket, client.ID + "\n");
            Networking.Send(client.TheSocket, startupInfo);

            lock (theWorld)
            {
                Tank tank = new Tank((int)client.ID, name);
                theWorld.tanks[(int)client.ID] = tank;
                tank.maxFramesToFire = Settings.FramesPerShot;
            }

            lock (clients)
            {
                clients.Add((int)client.ID, client);
            }
            client.OnNetworkAction = ReceiveControlCommand;
            Networking.GetData(client);


        }
        /// <summary>
        /// Receives the clients commands and informs the world.
        /// </summary>
        /// <param name="client"></param>
        private void ReceiveControlCommand(SocketState client)
        {
            if (client.ErrorOccurred)
            {
                return;
            }

            string totalData = client.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            foreach (string p in parts)
            {
                if (p.Length == 0)
                {
                    continue;
                }

                if (p[p.Length - 1] != '\n')
                {
                    break;
                }
                Control_Commands ctrlCmd = JsonConvert.DeserializeObject<Control_Commands>(p);

                lock (theWorld)
                {
                    theWorld.ctrlCmds[(int)client.ID] = ctrlCmd;
                }
                client.RemoveData(0, p.Length);
            }
            Networking.GetData(client);

        }
        /// <summary>
        /// Removes a client from the clients dictionary
        /// </summary>
        /// <param name="id">The ID of the client</param>
        private void RemoveClient(long id)
        {
            Console.WriteLine("Client " + id + " disconnected");
            lock (clients)
            {
                clients.Remove((int)id);
            }
        }
    }
}
