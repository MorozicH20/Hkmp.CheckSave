using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hkmp.Networking.Packet;
using static Hkmp.CheckSave.Models.AllowedSave;
using HKMirror;
using System.Reflection;
using System.Diagnostics;
using HKMirror.Hooks.ILHooks;

namespace Hkmp.CheckSave.Models
{
    public class PlayerSave
    {
        //[JsonProperty("maxHealth")]
        public int maxHealth;

        //[JsonProperty("maxMP")]
        public int maxMP;

        //[JsonProperty("geo")]
        public int geo;

        //[JsonProperty("Charms")]
        public List<Charm> Charms;

        //[JsonProperty("Skills")]
        public List<Skill> Skills;

        public void LoadData()
        {
            maxHealth = PlayerDataAccess.maxHealth;
            maxMP = PlayerDataAccess.maxMP;
            geo = PlayerDataAccess.geo;

            FileEdit logs = new FileEdit();

            Skills = new List<Skill>() { };
            Skills.Clear();
            Charms = new List<Charm>() { };
            Charms.Clear();

            PropertyInfo[] fieldInfos = new PropertyInfo[] { };
            try { fieldInfos = typeof(PlayerDataAccess).GetProperties(); } catch { }
            string result = "find Charms/Skills:\n";

            Dictionary<Charm, bool> CharmValues = new Dictionary<Charm, bool>() { };
            Dictionary<Skill, bool> SkillValues = new Dictionary<Skill, bool>() { };
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                PropertyInfo f = fieldInfos[i];
                if (f.Name.Contains("gotCharm_"))
                {
                    if (f.GetValue(null) == null)
                        continue;
                    CharmValues.Add((Charm)Convert.ToInt32(f.Name.Replace("gotCharm_", "")), Convert.ToBoolean(f.GetValue(typeof(bool))));

                }

                switch (f.Name)
                {
                    case "fireballLevel":
                        switch (Convert.ToInt32(f.GetValue(typeof(int))))
                        {
                            case 0:
                                SkillValues.Add(Skill.VengefulSpirit, false);
                                SkillValues.Add((Skill)3, false);
                                break;

                            case 1:
                                SkillValues.Add(Skill.VengefulSpirit, true);
                                SkillValues.Add((Skill)3, false);
                                break;

                            case 2:
                                SkillValues.Add(Skill.VengefulSpirit, true);
                                SkillValues.Add((Skill)3, true);
                                break;
                        }

                        break;
                    case "quakeLevel":
                        switch (Convert.ToInt32(f.GetValue(typeof(int))))
                        {
                            case 0:
                                SkillValues.Add((Skill)1, false);
                                SkillValues.Add((Skill)4, false);
                                break;

                            case 1:
                                SkillValues.Add((Skill)1, true);
                                SkillValues.Add((Skill)4, false);
                                break;

                            case 2:
                                SkillValues.Add((Skill)1, true);
                                SkillValues.Add((Skill)4, true);
                                break;
                        }
                        break;
                    case "screamLevel":
                        switch (Convert.ToInt32(f.GetValue(typeof(int))))
                        {
                            case 0:
                                SkillValues.Add((Skill)2, false);
                                SkillValues.Add((Skill)5, false);
                                break;

                            case 1:
                                SkillValues.Add((Skill)2, true);
                                SkillValues.Add((Skill)5, false);
                                break;

                            case 2:
                                SkillValues.Add((Skill)2, true);
                                SkillValues.Add((Skill)5, true);
                                break;
                        }
                        break;
                    case "hasDash":
                        SkillValues.Add((Skill)6, true);
                        break;
                    case "hasWallJump":
                        SkillValues.Add((Skill)7, true);
                        break;
                    case "hasSuperDash":
                        SkillValues.Add((Skill)8, true);
                        break;
                    case "hasDoubleJump":
                        SkillValues.Add((Skill)9, true);
                        break;
                    case "hasAcidArmour":
                        SkillValues.Add((Skill)10, true);
                        break;
                    case "hasShadowDash":
                        SkillValues.Add((Skill)11, true);
                        break;
                    case "hasDreamNail":
                        SkillValues.Add((Skill)12, true);
                        break;
                    case "hasCyclone":
                        SkillValues.Add((Skill)13, true);
                        break;
                    case "hasDashSlash":
                        SkillValues.Add((Skill)14, true);
                        break;
                    case "hasUpwardSlash":
                        SkillValues.Add((Skill)15, true);
                        break;

                }



            }
            result += $"\tCharmsValues:\n {CharmValues.Count}\n";
            foreach (var charm in CharmValues)
            {
                if (charm.Value)
                {
                    Charms.Add(charm.Key);
                }
                result += $"\t\t{charm.Key}: {charm.Value}\n";
            }
            result += $"\tSkillsValues:\n {SkillValues.Count}\n";

            foreach (var skill in SkillValues)
            {
                if (skill.Value)
                {
                    Skills.Add(skill.Key);
                }
                result += $"\t\t{skill.Key}: {skill.Value}\n";

            }
            logs.Write(result);
            logs.Write($"Skill List: {Skills.Count}\nCharms List: {Charms.Count}");

            StackTrace stackTrace = new StackTrace();
            logs.Write(stackTrace.ToString());
        }

        ~PlayerSave()
        {
            Charms = null;
            Skills = null;

            new FileEdit().Write("call Destructor");

        }
    }
}
