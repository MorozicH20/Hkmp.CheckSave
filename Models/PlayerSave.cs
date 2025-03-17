using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hkmp.Networking.Packet;
using static Hkmp.CheckSave.Models.AllowedSave;
using HKMirror;

namespace Hkmp.CheckSave.Models
{
    public class PlayerSave
    {
        [JsonProperty("maxHealth")]
        public int maxHealth;

        [JsonProperty("maxMP")]
        public int maxMP;

        [JsonProperty("geo")]
        public int geo;

        [JsonProperty("Charms")]
        public List<Charm> Charms { get; set; }

        [JsonProperty("Skills")]
        public List<Skill> Skills { get; set; }

        public PlayerSave()
        {
            maxHealth = PlayerDataAccess.maxHealth;
            maxMP = PlayerDataAccess.maxMP;
            geo = PlayerDataAccess.geo;

            for(int i = 1; i<41;i++)
            {
                if ((bool)typeof(PlayerDataAccess).GetProperty($"charmCost_{i}").GetValue(this))
                {
                    Charms.Add((Charm)i);
                }

            }
            
            for(int i = 0;i <16; i++)
            {
                if ((bool)typeof(PlayerDataAccess).GetProperty($"{(Skill)i}").GetValue(this))
                {
                    Skills.Add((Skill)i);
                }
            }

        }
    }
}
