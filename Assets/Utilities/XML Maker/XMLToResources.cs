using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
using Gazzotto.Controller;
using Gazzotto.Managers;
using Gazzotto.Stats;

namespace Gazzotto.Utilities
{
    [ExecuteInEditMode]
    public class XMLToResources : MonoBehaviour
    {
        public bool load;

        public ResourcesManager resourcesManager;
        public string weaponFileName = "item_database.xml";

        private void Update()
        {
            if (!load)
                return;
            load = false;

            resourcesManager.weaponList = new List<Weapon>();
            LoadWeaponData(resourcesManager);
        }

        public void LoadWeaponData(ResourcesManager rm)
        {
            string filePath = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            filePath += weaponFileName;

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach(XmlNode w in doc.DocumentElement.SelectNodes("//weapon"))
            {
                Weapon _w = new Weapon();
                _w.actions = new List<Controller.Action>();
                _w.two_handedActions = new List<Controller.Action>();

                XmlNode weaponId = w.SelectSingleNode("weaponId");
                _w.weaponId = weaponId.InnerText;

                XmlNode oh_idle = w.SelectSingleNode("oh_idle");
                _w.oh_idle = oh_idle.InnerText;
                XmlNode th_idle = w.SelectSingleNode("th_idle");
                _w.th_idle = th_idle.InnerText;

                XmlNode parryMultiplier = w.SelectSingleNode("parryMultiplier");
                float.TryParse(parryMultiplier.InnerText, out _w.parryMultiplier);
                XmlNode backstabMultiplier = w.SelectSingleNode("backstabMultiplier");
                float.TryParse(backstabMultiplier.InnerText, out _w.backstabMultiplier);

                XmlToActions(doc, "actions", ref _w);
                XmlToActions(doc, "two_handed", ref _w);

                XmlNode leftHandMirror = w.SelectSingleNode("leftHandMirror");
                bool.TryParse(leftHandMirror.InnerText, out _w.leftHandMirror);

                resourcesManager.weaponList.Add(_w);
            }
        }

        void XmlToActions(XmlDocument d, string nodeName, ref Weapon weapon)
        {
            foreach (XmlNode a in d.DocumentElement.SelectNodes("//" + nodeName))
            {
                Controller.Action _a = new Controller.Action();

                XmlNode actionInput = a.SelectSingleNode("ActionInput");
                _a.input = (ActionInput)Enum.Parse(typeof(ActionInput), actionInput.InnerText);

                XmlNode actionType = a.SelectSingleNode("ActionType");
                _a.type = (ActionType)Enum.Parse(typeof(ActionType), actionType.InnerText);

                XmlNode targetAnim = a.SelectSingleNode("targetAnim");
                _a.targetAnim = targetAnim.InnerText;

                XmlNode mirror = a.SelectSingleNode("mirror");
                bool.TryParse(mirror.InnerText, out _a.mirror);
                XmlNode canBeParried = a.SelectSingleNode("canBeParried");
                bool.TryParse(canBeParried.InnerText, out _a.canBeParried);
                XmlNode changeSpeed = a.SelectSingleNode("changeSpeed");
                bool.TryParse(changeSpeed.InnerText, out _a.changeSpeed);

                XmlNode animSpeed = a.SelectSingleNode("animSpeed");
                float.TryParse(animSpeed.InnerText, out _a.animSpeed);

                XmlNode canParry = a.SelectSingleNode("canParry");
                bool.TryParse(canParry.InnerText, out _a.canParry);
                XmlNode canBackstab = a.SelectSingleNode("canBackstab");
                bool.TryParse(canBackstab.InnerText, out _a.canBackstab);
                XmlNode overrideDamangeAnim = a.SelectSingleNode("overrideDamageAnim");
                bool.TryParse(overrideDamangeAnim.InnerText, out _a.overrideDamageAnim);

                XmlNode damageAnim = a.SelectSingleNode("damageAnim");
                _a.damageAnim = damageAnim.InnerText;

                _a.weaponStats = new WeaponStats();

                XmlNode physical = a.SelectSingleNode("physical");
                int.TryParse(physical.InnerText, out _a.weaponStats.physical);
                XmlNode strike = a.SelectSingleNode("strike");
                int.TryParse(strike.InnerText, out _a.weaponStats.strike);
                XmlNode slash = a.SelectSingleNode("slash");
                int.TryParse(slash.InnerText, out _a.weaponStats.slash);
                XmlNode thrust = a.SelectSingleNode("thrust");
                int.TryParse(thrust.InnerText, out _a.weaponStats.thrust);
                XmlNode magic = a.SelectSingleNode("magic");
                int.TryParse(magic.InnerText, out _a.weaponStats.magic);
                XmlNode fire = a.SelectSingleNode("fire");
                int.TryParse(fire.InnerText, out _a.weaponStats.fire);
                XmlNode lighting = a.SelectSingleNode("lighting");
                int.TryParse(lighting.InnerText, out _a.weaponStats.lighting);
                XmlNode dark = a.SelectSingleNode("dark");
                int.TryParse(dark.InnerText, out _a.weaponStats.dark);

                if (nodeName.Equals("actions"))
                {
                    weapon.actions.Add(_a);
                }
                else
                {
                    weapon.two_handedActions.Add(_a);
                }
            }
        }
    }
}