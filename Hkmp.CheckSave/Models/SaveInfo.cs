using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hkmp.CheckSave.Models.AllowedSave;
namespace Hkmp.CheckSave.Models
{
    internal class SaveInfo
    {
        public bool EqualityHealth{ get; set; }
        public bool EqualityMP{ get; set; }
        public bool EqualityGeo{ get; set; }
        public List<Charm> ExtraCharms { get; set; }
        public List<Charm> MissingCharms { get; set; }
        public List<Skill> ExtraSkills { get; set; }
        public List<Skill> MissingSkills { get; set; }
    }
}
