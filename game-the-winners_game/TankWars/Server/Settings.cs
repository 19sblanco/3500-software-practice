using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TankWars
{
    /// <summary>
    ///  Reads the Setting.xml file sets world state based off of that.
    /// </summary>
    class Settings
    {
        public int UniverseSize { get; } = 2000;
        public int MSPerFram { get; } = 17;
        public int FramesPerShot { get; set; }
        public int RespawnRate { get; set; }

        public HashSet<Wall> Walls { get; } = new HashSet<Wall>();
        public Settings(string filePath)
        {
            ReadSettings(filePath);
        }
        /// <summary>
        /// Reads setting.xml file
        /// </summary>
        /// <param name="path">filepath</param>
        /// <returns></returns>
        private void ReadSettings(string path)
        {
            using (XmlReader reader = XmlReader.Create(path))
            {
                double x = 0, y = 0;
                Vector2D p1 = new Vector2D(), p2 = new Vector2D();

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "FramesPerShot":
                                reader.Read();
                                Int32.TryParse(reader.Value, out int FPS);
                                FramesPerShot = FPS;
                                break;

                            case "RespawnRate":
                                reader.Read();
                                Int32.TryParse(reader.Value, out int RR);
                                RespawnRate = RR;
                                break;

                            case "x":
                                reader.Read();
                                x = int.Parse(reader.Value);
                                break;

                            case "y":
                                reader.Read();
                                y = int.Parse(reader.Value);
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "p1":
                                p1 = new Vector2D(x, y);
                                break;

                            case "p2":
                                p2 = new Vector2D(x, y);
                                break;

                            case "Wall":
                                Walls.Add(new Wall(p1, p2));
                                break;
                        }
                    }
                }
            }
        }
    }
}
