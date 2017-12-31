using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Gazzotto.UI
{
    public class QuickSlot : MonoBehaviour
    {
        public List<QSlots> slots = new List<QSlots>();

        public static QuickSlot singleton;

        public void Init()
        {
            ClearIcons();
        }

        public void ClearIcons()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].icon.gameObject.SetActive(false);
            }
        }

        public void UpdateSlot(QSlotType stype, Sprite i)
        {
            QSlots q = GetSlot(stype);
            q.icon.sprite = i;
            q.icon.gameObject.SetActive(true);
        }

        public QSlots GetSlot(QSlotType t)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].type == t)
                    return slots[i];
            }

            return null;
        }

        private void Awake()
        {
            if (singleton == null)
                singleton = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(this);
        }
    }

    public enum QSlotType
    {
        rh,lh,item,spell
    }

    [System.Serializable]
    public class QSlots
    {
        public Image icon;
        public QSlotType type;
    }
}