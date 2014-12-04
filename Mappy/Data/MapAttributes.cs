namespace Mappy.Data
{
    using System;
    using System.Drawing;
    using System.IO;

    using TAUtil.Tdf;

    using Util;

    /// <summary>
    /// Contains all the metadata about a map.
    /// </summary>
    public class MapAttributes : Notifier
    {
        private readonly Point?[] startPositions = new Point?[10];

        private string name;
        private string description;
        private string planet;
        private int gravity;
        private string numPlayers;
        private string memory;
        private string aiProfile;

        private int surfaceMetal;
        private int mohoMetal;

        private int tidalStrength;
        private int solarStrength;

        private int minWindSpeed;
        private int maxWindSpeed;

        private bool lavaWorld;

        private bool waterDoesDamage;
        private int waterDamage;

        private string meteorWeapon;
        private int meteorRadius;
        private double meteorDensity;
        private int meteorDuration;
        private int meteorInterval;

        public event EventHandler<StartPositionChangedEventArgs> StartPositionChanged;

        public string Name
        {
            get { return this.name; }
            set { this.SetField(ref this.name, value, "Name"); }
        }

        public string Description
        {
            get { return this.description; }
            set { this.SetField(ref this.description, value, "Description"); }
        }

        public string Planet
        {
            get { return this.planet; }
            set { this.SetField(ref this.planet, value, "Planet"); }
        }

        public int Gravity
        {
            get { return this.gravity; }
            set { this.SetField(ref this.gravity, value, "Gravity"); }
        }

        public string Memory
        {
            get { return this.memory; }
            set { this.SetField(ref this.memory, value, "Memory"); }
        }

        public string NumPlayers
        {
            get { return this.numPlayers; }
            set { this.SetField(ref this.numPlayers, value, "NumPlayers"); }
        }

        public string AiProfile
        {
            get { return this.aiProfile; }
            set { this.SetField(ref this.aiProfile, value, "AiProfile"); }
        }

        public int SurfaceMetal
        {
            get { return this.surfaceMetal; }
            set { this.SetField(ref this.surfaceMetal, value, "SurfaceMetal"); }
        }

        public int MohoMetal
        {
            get { return this.mohoMetal; }
            set { this.SetField(ref this.mohoMetal, value, "MohoMetal"); }
        }

        public int TidalStrength
        {
            get { return this.tidalStrength; }
            set { this.SetField(ref this.tidalStrength, value, "TidalStrength"); }
        }

        public int SolarStrength
        {
            get { return this.solarStrength; }
            set { this.SetField(ref this.solarStrength, value, "SolarStrength"); }
        }

        public int MinWindSpeed
        {
            get { return this.minWindSpeed; }
            set { this.SetField(ref this.minWindSpeed, value, "MinWindSpeed"); }
        }

        public bool LavaWorld
        {
            get { return this.lavaWorld; }
            set { this.SetField(ref this.lavaWorld, value, "LavaWorld"); }
        }

        public bool WaterDoesDamage
        {
            get { return this.waterDoesDamage; }
            set { this.SetField(ref this.waterDoesDamage, value, "WaterDoesDamage"); }
        }

        public int MaxWindSpeed
        {
            get { return this.maxWindSpeed; }
            set { this.SetField(ref this.maxWindSpeed, value, "MaxWindSpeed"); }
        }

        public int WaterDamage
        {
            get { return this.waterDamage; }
            set { this.SetField(ref this.waterDamage, value, "WaterDamage"); }
        }

        public string MeteorWeapon
        {
            get { return this.meteorWeapon; }
            set { this.SetField(ref this.meteorWeapon, value, "MeteorWeapon"); }
        }

        public int MeteorRadius
        {
            get { return this.meteorRadius; }
            set { this.SetField(ref this.meteorRadius, value, "MeteorRadius"); }
        }

        public int MeteorDuration
        {
            get { return this.meteorDuration; }
            set { this.SetField(ref this.meteorDuration, value, "MeteorDuration"); }
        }

        public double MeteorDensity
        {
            get { return this.meteorDensity; }
            set { this.SetField(ref this.meteorDensity, value, "MeteorDensity"); }
        }

        public int MeteorInterval
        {
            get { return this.meteorInterval; }
            set { this.SetField(ref this.meteorInterval, value, "MeteorInterval"); }
        }

        public static MapAttributes Load(TdfNode n)
        {
            TdfNode r = n.Keys["globalheader"];

            TdfNode schema = r.Keys["schema 0"];

            MapAttributes m = new MapAttributes();

            m.Name = r.Entries.GetOrDefault("missionname", string.Empty);
            m.Description = r.Entries.GetOrDefault("missiondescription", string.Empty);
            m.Planet = r.Entries.GetOrDefault("planet", string.Empty);
            m.TidalStrength = TdfConvert.ToInt32(r.Entries.GetOrDefault("tidalstrength", "0"));
            m.SolarStrength = TdfConvert.ToInt32(r.Entries.GetOrDefault("solarstrength", "0"));
            m.LavaWorld = TdfConvert.ToBool(r.Entries.GetOrDefault("lavaworld", "0"));
            m.MinWindSpeed = TdfConvert.ToInt32(r.Entries.GetOrDefault("minwindspeed", "0"));
            m.MaxWindSpeed = TdfConvert.ToInt32(r.Entries.GetOrDefault("maxwindspeed", "0"));
            m.Gravity = TdfConvert.ToInt32(r.Entries.GetOrDefault("gravity", "0"));
            m.NumPlayers = r.Entries.GetOrDefault("numplayers", string.Empty);
            m.Memory = r.Entries.GetOrDefault("memory", string.Empty);
            m.AiProfile = schema.Entries.GetOrDefault("aiprofile", string.Empty);
            m.SurfaceMetal = TdfConvert.ToInt32(schema.Entries.GetOrDefault("surfacemetal", "0"));
            m.MohoMetal = TdfConvert.ToInt32(schema.Entries.GetOrDefault("mohometal", "0"));
            m.MeteorWeapon = schema.Entries.GetOrDefault("meteorweapon", string.Empty);
            m.MeteorRadius = TdfConvert.ToInt32(schema.Entries.GetOrDefault("meteorradius", "0"));
            m.MeteorDensity = TdfConvert.ToDouble(schema.Entries.GetOrDefault("meteordensity", "0"));
            m.MeteorDuration = TdfConvert.ToInt32(schema.Entries.GetOrDefault("meteorduration", "0"));
            m.MeteorInterval = TdfConvert.ToInt32(schema.Entries.GetOrDefault("meteorinterval", "0"));

            if (schema.Keys.ContainsKey("specials"))
            {
                TdfNode specials = schema.Keys["specials"];

                foreach (var special in specials.Keys.Values)
                {
                    var type = special.Entries.GetOrDefault("specialwhat", string.Empty);
                    if (type.Length < "startposx".Length || !type.StartsWith("startpos"))
                    {
                        continue;
                    }

                    int id = TdfConvert.ToInt32(type.Substring(8));
                    int x = TdfConvert.ToInt32(special.Entries.GetOrDefault("xpos", "0"));
                    int y = TdfConvert.ToInt32(special.Entries.GetOrDefault("zpos", "0"));
                    m.SetStartPosition(id - 1, new Point(x, y));
                }
            }

            return m;
        }

        public Point? GetStartPosition(int i)
        {
            return this.startPositions[i];
        }

        public void SetStartPosition(int i, Point? coordinates)
        {
            if (this.startPositions[i] != coordinates)
            {
                this.startPositions[i] = coordinates;
                this.OnStartPositionChanged(new StartPositionChangedEventArgs(i));
            }
        }

        public void WriteOta(Stream st)
        {
            TdfNode r = new TdfNode("GlobalHeader");

            TdfNode s = new TdfNode("Schema 0");
            r.Keys["Schema 0"] = s;

            r.Entries["missionname"] = this.Name;
            r.Entries["missiondescription"] = this.Description;
            r.Entries["planet"] = this.Planet;
            r.Entries["missionhint"] = string.Empty;
            r.Entries["brief"] = string.Empty;
            r.Entries["narration"] = string.Empty;
            r.Entries["glamour"] = string.Empty;
            r.Entries["lineofsight"] = "0";
            r.Entries["mapping"] = "0";
            r.Entries["tidalstrength"] = TdfConvert.ToString(this.TidalStrength);
            r.Entries["solarstrength"] = TdfConvert.ToString(this.SolarStrength);
            r.Entries["lavaworld"] = TdfConvert.ToString(this.lavaWorld);
            r.Entries["killmul"] = "0";
            r.Entries["timemul"] = "0";
            r.Entries["minwindspeed"] = TdfConvert.ToString(this.MinWindSpeed);
            r.Entries["maxwindspeed"] = TdfConvert.ToString(this.MaxWindSpeed);
            r.Entries["gravity"] = TdfConvert.ToString(this.Gravity);
            r.Entries["numplayers"] = this.numPlayers;
            r.Entries["size"] = string.Empty; // TODO
            r.Entries["memory"] = this.memory;
            r.Entries["useonlyunits"] = string.Empty;
            r.Entries["SCHEMACOUNT"] = "1";

            s.Entries["Type"] = "Network 1";
            s.Entries["aiprofile"] = this.AiProfile;
            s.Entries["SurfaceMetal"] = TdfConvert.ToString(this.SurfaceMetal);
            s.Entries["MohoMetal"] = TdfConvert.ToString(this.MohoMetal);
            s.Entries["HumanMetal"] = "1000";
            s.Entries["ComputerMetal"] = "1000";
            s.Entries["HumanEnergy"] = "1000";
            s.Entries["ComputerEnergy"] = "1000";
            s.Entries["MeteorWeapon"] = this.MeteorWeapon;
            s.Entries["MeteorRadius"] = TdfConvert.ToString(this.MeteorRadius);
            s.Entries["MeteorDensity"] = TdfConvert.ToString(this.MeteorDensity);
            s.Entries["MeteorDuration"] = TdfConvert.ToString(this.MeteorDuration);
            s.Entries["MeteorInterval"] = TdfConvert.ToString(this.MeteorInterval);

            TdfNode specials = new TdfNode("specials");
            s.Keys["specials"] = specials;

            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                Point? p = this.GetStartPosition(i);
                if (!p.HasValue)
                {
                    continue;
                }

                TdfNode spec = new TdfNode("special" + count);
                spec.Entries["specialwhat"] = "StartPos" + (i + 1);
                spec.Entries["XPos"] = TdfConvert.ToString(p.Value.X);
                spec.Entries["ZPos"] = TdfConvert.ToString(p.Value.Y);

                specials.Keys[spec.Name] = spec;

                count++;
            }

                r.WriteTdf(st);
        }

        protected virtual void OnStartPositionChanged(StartPositionChangedEventArgs e)
        {
            EventHandler<StartPositionChangedEventArgs> h = this.StartPositionChanged;
            if (h != null)
            {
                h(this, e);
            }
        }
    }
}
