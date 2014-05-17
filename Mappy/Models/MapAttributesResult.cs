namespace Mappy.Models
{
    public class MapAttributesResult
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Memory { get; set; }

        public string AiProfile { get; set; }

        public string Planet { get; set; }

        public string Players { get; set; }

        public int MinWindSpeed { get; set; }

        public int MaxWindSpeed { get; set; }

        public int TidalStrength { get; set; }

        public int SolarStrength { get; set; }

        public int SeaLevel { get; set; }

        public int Gravity { get; set; }

        public int SurfaceMetal { get; set; }

        public int MohoMetal { get; set; }

        public string MeteorWeapon { get; set; }

        public int MeteorRadius { get; set; }

        public int MeteorDuration { get; set; }

        public double MeteorDensity { get; set; }

        public int MeteorInterval { get; set; }

        public int WaterDamage { get; set; }

        public bool ImpassibleWater { get; set; }

        public bool WaterDoesDamage { get; set; }

        public static MapAttributesResult FromModel(IMapModel map)
        {
            var attrs = map.Attributes;
            return new MapAttributesResult
                {
                    AiProfile = attrs.AiProfile,
                    Description = attrs.Description,
                    Gravity = attrs.Gravity,
                    ImpassibleWater = attrs.LavaWorld,
                    MaxWindSpeed = attrs.MaxWindSpeed,
                    Memory = attrs.Memory,
                    MeteorDensity = attrs.MeteorDensity,
                    MeteorDuration = attrs.MeteorDuration,
                    MeteorInterval = attrs.MeteorInterval,
                    MeteorRadius = attrs.MeteorRadius,
                    MeteorWeapon = attrs.MeteorWeapon,
                    MinWindSpeed = attrs.MinWindSpeed,
                    MohoMetal = attrs.MohoMetal,
                    Name = attrs.Name,
                    Planet = attrs.Planet,
                    Players = attrs.NumPlayers,
                    SeaLevel = map.SeaLevel,
                    SolarStrength = attrs.SolarStrength,
                    SurfaceMetal = attrs.SurfaceMetal,
                    TidalStrength = attrs.TidalStrength,
                    WaterDamage = attrs.WaterDamage,
                    WaterDoesDamage = attrs.WaterDoesDamage,
                };
        }

        public void MergeInto(IMapModel map)
        {
            var attrs = map.Attributes;

            attrs.AiProfile = this.AiProfile;
            attrs.Description = this.Description;
            attrs.Gravity = this.Gravity;
            attrs.LavaWorld = this.ImpassibleWater;
            attrs.MaxWindSpeed = this.MaxWindSpeed;
            attrs.Memory = this.Memory;
            attrs.MeteorDensity = this.MeteorDensity;
            attrs.MeteorDuration = this.MeteorDuration;
            attrs.MeteorInterval = this.MeteorInterval;
            attrs.MeteorRadius = this.MeteorRadius;
            attrs.MeteorWeapon = this.MeteorWeapon;
            attrs.MinWindSpeed = this.MinWindSpeed;
            attrs.MohoMetal = this.MohoMetal;
            attrs.Name = this.Name;
            attrs.Planet = this.Planet;
            attrs.NumPlayers = this.Players;
            map.SeaLevel = this.SeaLevel;
            attrs.SolarStrength = this.SolarStrength;
            attrs.SurfaceMetal = this.SurfaceMetal;
            attrs.TidalStrength = this.TidalStrength;
            attrs.WaterDamage = this.WaterDamage;
            attrs.WaterDoesDamage = this.WaterDoesDamage;
        }
    }
}