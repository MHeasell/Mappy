﻿namespace Mappy.Data
{
    using System;
    using System.Drawing;
    using System.IO;

    using Mappy.Util;

    using TAUtil.Tdf;

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

        public MapAttributes()
        {
            // set up default values
            this.Name = "Untitled Map";
            this.Description = "A map made with Mappy";
            this.Gravity = 112;
            this.Memory = string.Empty;
            this.NumPlayers = "2";
            this.AiProfile = "DEFAULT";

            this.SurfaceMetal = 3;
            this.MohoMetal = 30;

            this.TidalStrength = 20;
            this.SolarStrength = 20;

            this.MinWindSpeed = 0;
            this.MaxWindSpeed = 3000;

            this.LavaWorld = false;

            this.WaterDoesDamage = false;
            this.WaterDamage = 0;

            this.MeteorWeapon = string.Empty;
            this.MeteorRadius = 0;
            this.MeteorDensity = 0;
            this.MeteorDuration = 0;
            this.MeteorInterval = 0;
        }

        public event EventHandler<StartPositionChangedEventArgs> StartPositionChanged;

        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value, nameof(this.Name));
        }

        public string Description
        {
            get => this.description;
            set => this.SetField(ref this.description, value, nameof(this.Description));
        }

        public string Planet
        {
            get => this.planet;
            set => this.SetField(ref this.planet, value, nameof(this.Planet));
        }

        public int Gravity
        {
            get => this.gravity;
            set => this.SetField(ref this.gravity, value, nameof(this.Gravity));
        }

        public string Memory
        {
            get => this.memory;
            set => this.SetField(ref this.memory, value, nameof(this.Memory));
        }

        public string NumPlayers
        {
            get => this.numPlayers;
            set => this.SetField(ref this.numPlayers, value, nameof(this.NumPlayers));
        }

        public string AiProfile
        {
            get => this.aiProfile;
            set => this.SetField(ref this.aiProfile, value, nameof(this.AiProfile));
        }

        public int SurfaceMetal
        {
            get => this.surfaceMetal;
            set => this.SetField(ref this.surfaceMetal, value, nameof(this.SurfaceMetal));
        }

        public int MohoMetal
        {
            get => this.mohoMetal;
            set => this.SetField(ref this.mohoMetal, value, nameof(this.MohoMetal));
        }

        public int TidalStrength
        {
            get => this.tidalStrength;
            set => this.SetField(ref this.tidalStrength, value, nameof(this.TidalStrength));
        }

        public int SolarStrength
        {
            get => this.solarStrength;
            set => this.SetField(ref this.solarStrength, value, nameof(this.SolarStrength));
        }

        public int MinWindSpeed
        {
            get => this.minWindSpeed;
            set => this.SetField(ref this.minWindSpeed, value, nameof(this.MinWindSpeed));
        }

        public bool LavaWorld
        {
            get => this.lavaWorld;
            set => this.SetField(ref this.lavaWorld, value, nameof(this.LavaWorld));
        }

        public bool WaterDoesDamage
        {
            get => this.waterDoesDamage;
            set => this.SetField(ref this.waterDoesDamage, value, nameof(this.WaterDoesDamage));
        }

        public int MaxWindSpeed
        {
            get => this.maxWindSpeed;
            set => this.SetField(ref this.maxWindSpeed, value, nameof(this.MaxWindSpeed));
        }

        public int WaterDamage
        {
            get => this.waterDamage;
            set => this.SetField(ref this.waterDamage, value, nameof(this.WaterDamage));
        }

        public string MeteorWeapon
        {
            get => this.meteorWeapon;
            set => this.SetField(ref this.meteorWeapon, value, nameof(this.MeteorWeapon));
        }

        public int MeteorRadius
        {
            get => this.meteorRadius;
            set => this.SetField(ref this.meteorRadius, value, nameof(this.MeteorRadius));
        }

        public int MeteorDuration
        {
            get => this.meteorDuration;
            set => this.SetField(ref this.meteorDuration, value, nameof(this.MeteorDuration));
        }

        public double MeteorDensity
        {
            get => this.meteorDensity;
            set => this.SetField(ref this.meteorDensity, value, nameof(this.MeteorDensity));
        }

        public int MeteorInterval
        {
            get => this.meteorInterval;
            set => this.SetField(ref this.meteorInterval, value, nameof(this.MeteorInterval));
        }

        public static MapAttributes Load(TdfNode n)
        {
            var r = n.Keys["GlobalHeader"];

            var schema = r.Keys["Schema 0"];

            var m = new MapAttributes();

            m.Name = r.Entries.GetOrDefault("missionname", string.Empty);
            m.Description = r.Entries.GetOrDefault("missiondescription", string.Empty);
            m.Planet = r.Entries.GetOrDefault("planet", string.Empty);
            m.TidalStrength = TdfConvert.ToInt32(r.Entries.GetOrDefault("tidalstrength", "0"));
            m.SolarStrength = TdfConvert.ToInt32(r.Entries.GetOrDefault("solarstrength", "0"));
            m.LavaWorld = TdfConvert.ToBool(r.Entries.GetOrDefault("lavaworld", "0"));
            m.MinWindSpeed = TdfConvert.ToInt32(r.Entries.GetOrDefault("minwindspeed", "0"));
            m.MaxWindSpeed = TdfConvert.ToInt32(r.Entries.GetOrDefault("maxwindspeed", "0"));
            m.Gravity = TdfConvert.ToInt32(r.Entries.GetOrDefault("gravity", "0"));
            m.WaterDoesDamage = TdfConvert.ToBool(r.Entries.GetOrDefault("waterdoesdamage", "0"));
            m.WaterDamage = TdfConvert.ToInt32(r.Entries.GetOrDefault("waterdamage", "0"));
            m.NumPlayers = r.Entries.GetOrDefault("numplayers", string.Empty);
            m.Memory = r.Entries.GetOrDefault("memory", string.Empty);
            m.AiProfile = schema.Entries.GetOrDefault("aiprofile", string.Empty);
            m.SurfaceMetal = TdfConvert.ToInt32(schema.Entries.GetOrDefault("SurfaceMetal", "0"));
            m.MohoMetal = TdfConvert.ToInt32(schema.Entries.GetOrDefault("MohoMetal", "0"));
            m.MeteorWeapon = schema.Entries.GetOrDefault("MeteorWeapon", string.Empty);
            m.MeteorRadius = TdfConvert.ToInt32(schema.Entries.GetOrDefault("MeteorRadius", "0"));
            m.MeteorDensity = TdfConvert.ToDouble(schema.Entries.GetOrDefault("MeteorDensity", "0"));
            m.MeteorDuration = TdfConvert.ToInt32(schema.Entries.GetOrDefault("MeteorDuration", "0"));
            m.MeteorInterval = TdfConvert.ToInt32(schema.Entries.GetOrDefault("MeteorInterval", "0"));

            if (schema.Keys.ContainsKey("specials"))
            {
                var specials = schema.Keys["specials"];

                foreach (var special in specials.Keys.Values)
                {
                    var type = special.Entries.GetOrDefault("specialwhat", string.Empty);
                    if (type.Length < "StartPosX".Length || !type.StartsWith("StartPos"))
                    {
                        continue;
                    }

                    var id = TdfConvert.ToInt32(type.Substring(8));
                    var x = TdfConvert.ToInt32(special.Entries.GetOrDefault("XPos", "0"));
                    var y = TdfConvert.ToInt32(special.Entries.GetOrDefault("ZPos", "0"));
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

        public void WriteOta(Stream st, int mapWidthIn512Tiles, int mapHeightIn512Tiles)
        {
            var r = new TdfNode("GlobalHeader");

            var s = new TdfNode("Schema 0");
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
            r.Entries["killmul"] = "50";
            r.Entries["timemul"] = "0";
            r.Entries["minwindspeed"] = TdfConvert.ToString(this.MinWindSpeed);
            r.Entries["maxwindspeed"] = TdfConvert.ToString(this.MaxWindSpeed);
            r.Entries["gravity"] = TdfConvert.ToString(this.Gravity);
            r.Entries["waterdoesdamage"] = TdfConvert.ToString(this.WaterDoesDamage);
            r.Entries["waterdamage"] = TdfConvert.ToString(this.WaterDamage);
            r.Entries["numplayers"] = this.numPlayers;
            r.Entries["size"] = $"{mapWidthIn512Tiles} x {mapHeightIn512Tiles}";
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

            var specials = new TdfNode("specials");
            s.Keys["specials"] = specials;

            var count = 0;
            for (var i = 0; i < 10; i++)
            {
                var p = this.GetStartPosition(i);
                if (!p.HasValue)
                {
                    continue;
                }

                var spec = new TdfNode("special" + count);
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
            this.StartPositionChanged?.Invoke(this, e);
        }
    }
}
