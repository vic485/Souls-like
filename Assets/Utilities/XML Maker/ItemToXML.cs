using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Gazzotto.Controller;
using Gazzotto.Items;
using Gazzotto.Managers;
using Gazzotto.Stats;

namespace Gazzotto.Utilities
{
    [ExecuteInEditMode]
    public class ItemToXML : MonoBehaviour
    {
        public bool make;
        public List<ItemInstance> candidates = new List<ItemInstance>();
        public string xml_version;
        public string targetName;

        private void Update()
        {
            if (!make)
                return;
            make = false;

            string xml = xml_version + "\n"; // <?xml version="1.0" encoding="utf-8"?>
            xml += "<root>\n";

            foreach (ItemInstance i in candidates)
            {
                Weapon w = i.instance;

                xml += "<weapon>\n";
                xml += "<weaponId>" + w.weaponId + "</weaponId>\n";
                xml += "<oh_idle>" + w.oh_idle + "</oh_idle>\n";
                xml += "<th_idle>" + w.th_idle + "</th_idle>\n";

                xml += ActionListToString(w.actions, "actions");
                xml += ActionListToString(w.two_handedActions, "two_handed");

                xml += "<parryMultiplier>" + w.parryMultiplier + "</parryMultiplier>\n";
                xml += "<backstabMultiplier>" + w.backstabMultiplier + "</backstabMultiplier>\n";
                xml += "<leftHandMirror>" + w.leftHandMirror + "</leftHandMirror>\n";

                xml += "<mp_x>" + w.model_pos.x + "</mp_x>\n";
                xml += "<mp_y>" + w.model_pos.y + "</mp_y>\n";
                xml += "<mp_z>" + w.model_pos.z + "</mp_z>\n";

                xml += "<me_x>" + w.model_eulers.x + "</me_x>\n";
                xml += "<me_y>" + w.model_eulers.y + "</me_y>\n";
                xml += "<me_z>" + w.model_eulers.z + "</me_z>\n";

                xml += "<ms_x>" + w.model_scale.x + "</ms_x>\n";
                xml += "<ms_y>" + w.model_scale.y + "</ms_y>\n";
                xml += "<ms_z>" + w.model_scale.z + "</ms_z>\n";

                xml += "</weapon>\n";
            }

            xml += "</root>\n";

            string path = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            if (string.IsNullOrEmpty(targetName))
            {
                targetName = "item_database.xml";
            }

            path += targetName;

            File.WriteAllText(path, xml);
        }

        string ActionListToString(List<Action> l, string nodeName)
        {
            string r = "";
            
			foreach (Action a in l)
			{
				r += "<" + nodeName + ">\n";
				r += "<ActionInput>" + a.input.ToString() + "</ActionInput>\n";
				r += "<ActionType>" + a.type.ToString() + "</ActionType>\n";
				r += "<targetAnim>" + a.targetAnim + "</targetAnim>\n";
				r += "<mirror>" + a.mirror + "</mirror>\n";
				r += "<canBeParried>" + a.canBeParried + "</canBeParried>\n";
				r += "<changeSpeed>" + a.changeSpeed + "</changeSpeed>\n";
				r += "<animSpeed>" + a.animSpeed.ToString() + "</animSpeed>\n";
				r += "<canParry>" + a.canParry + "</canParry>\n";
				r += "<canBackstab>" + a.canBackstab + "</canBackstab>\n";
				r += "<overrideDamageAnim>" + a.overrideDamageAnim + "</overrideDamageAnim>\n";
				r += "<damageAnim>" + a.damageAnim + "</damageAnim>\n";

				WeaponStats s = a.weaponStats;

				r += "<physical>" + s.physical + "</physical>\n";
				r += "<strike>" + s.strike + "</strike>\n";
				r += "<slash>" + s.slash + "</slash>\n";
				r += "<thrust>" + s.thrust + "</thrust>\n";
				r += "<magic>" + s.magic + "</magic>\n";
				r += "<fire>" + s.fire + "</fire>\n";
				r += "<lighting>" + s.lighting + "</lighting>\n";
				r += "<dark>" + s.dark + "</dark>\n";

				r += "</" + nodeName + ">\n";
			}
            return r;
        }
    }
}