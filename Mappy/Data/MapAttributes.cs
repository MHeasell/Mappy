namespace Mappy.Data
{
    using System;
    using System.Drawing;
    using System.IO;

    using TAUtil.Tdf;

    using Util;

    public class MapAttributes : Notifier
    {
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
        private int meteorDensity;
        private int meteorDuration;
        private int meteorInterval;

        private Point?[] startPositions = new Point?[10];

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

        public int MeteorDensity
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
            TdfNode r = n.Keys["GlobalHeader"];

            TdfNode schema = r.Keys["Schema 0"];

            MapAttributes m = new MapAttributes();

            m.Name = r.Entries["missionname"];
            m.Description = r.Entries["missiondescription"];
            m.Planet = r.Entries["planet"];
            m.TidalStrength = Convert.ToInt32(r.Entries["tidalstrength"]);
            m.SolarStrength = Convert.ToInt32(r.Entries["solarstrength"]);
            m.LavaWorld = EvalBoolString(r.Entries["lavaworld"]);
            m.MinWindSpeed = Convert.ToInt32(r.Entries["minwindspeed"]);
            m.MaxWindSpeed = Convert.ToInt32(r.Entries["maxwindspeed"]);
            m.Gravity = Convert.ToInt32(r.Entries["gravity"]);
            m.NumPlayers = r.Entries["numplayers"];
            m.Memory = r.Entries["memory"];
            m.AiProfile = schema.Entries["aiprofile"];
            m.SurfaceMetal = Convert.ToInt32(schema.Entries["SurfaceMetal"]);
            m.MohoMetal = Convert.ToInt32(schema.Entries["MohoMetal"]);
            m.MeteorWeapon = schema.Entries["MeteorWeapon"];
            m.MeteorRadius = Convert.ToInt32(schema.Entries["MeteorRadius"]);
            m.MeteorDensity = Convert.ToInt32(schema.Entries["MeteorDensity"]);
            m.MeteorDuration = Convert.ToInt32(schema.Entries["MeteorDuration"]);
            m.MeteorInterval = Convert.ToInt32(schema.Entries["MeteorInterval"]);

            TdfNode specials = schema.Keys["specials"];

            foreach (var special in specials.Keys.Values)
            {
                var type = special.Entries["specialwhat"];
                if (!type.StartsWith("StartPos"))
                {
                    continue;
                }

                int id = Convert.ToInt32(type.Substring(8));
                int x = Convert.ToInt32(special.Entries["XPos"]);
                int y = Convert.ToInt32(special.Entries["ZPos"]);
                m.SetStartPosition(id - 1, new Point(x, y));
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
            r.Entries["tidalstrength"] = Convert.ToString(this.TidalStrength);
            r.Entries["solarstrength"] = Convert.ToString(this.SolarStrength);
            r.Entries["lavaworld"] = EvalToString(this.lavaWorld);
            r.Entries["killmul"] = "0";
            r.Entries["timemul"] = "0";
            r.Entries["minwindspeed"] = Convert.ToString(this.MinWindSpeed);
            r.Entries["maxwindspeed"] = Convert.ToString(this.MaxWindSpeed);
            r.Entries["gravity"] = Convert.ToString(this.Gravity);
            r.Entries["numplayers"] = this.numPlayers;
            r.Entries["size"] = string.Empty; // TODO
            r.Entries["memory"] = this.memory;
            r.Entries["useonlyunits"] = string.Empty;
            r.Entries["SCHEMACOUNT"] = "1";

            s.Entries["Type"] = "Network 1";
            s.Entries["aiprofile"] = this.AiProfile;
            s.Entries["SurfaceMetal"] = Convert.ToString(this.SurfaceMetal);
            s.Entries["MohoMetal"] = Convert.ToString(this.MohoMetal);
            s.Entries["HumanMetal"] = "1000";
            s.Entries["ComputerMetal"] = "1000";
            s.Entries["HumanEnergy"] = "1000";
            s.Entries["ComputerEnergy"] = "1000";
            s.Entries["MeteorWeapon"] = this.MeteorWeapon;
            s.Entries["MeteorRadius"] = Convert.ToString(this.MeteorRadius);
            s.Entries["MeteorDensity"] = Convert.ToString(this.MeteorDensity);
            s.Entries["MeteorDuration"] = Convert.ToString(this.MeteorDuration);
            s.Entries["MeteorInterval"] = Convert.ToString(this.MeteorInterval);

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
                spec.Entries["XPos"] = Convert.ToString(p.Value.X);
                spec.Entries["ZPos"] = Convert.ToString(p.Value.Y);

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

        private static string EvalToString(bool b)
        {
            return b ? "1" : "0";
        }

        private static bool EvalBoolString(string s)
        {
            return !(string.IsNullOrEmpty(s) || s == "0");
        }
    }
}
