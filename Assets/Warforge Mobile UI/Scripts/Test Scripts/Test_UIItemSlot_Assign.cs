using UnityEngine;

namespace DuloGames.UI
{
    public class Test_UIItemSlot_Assign : MonoBehaviour
    {

        public UIItemSlot slot;
        public UIItemDatabase itemDatabase;
        public int assignItem;

        void Awake()
        {
            if (this.slot == null)
                this.slot = this.GetComponent<UIItemSlot>();
        }

        void Start()
        {
            if (this.slot == null || this.itemDatabase == null)
            {
                this.Destruct();
                return;
            }

            this.slot.Assign(PlayerInfos.Instance.collection.cards[0]);
            this.Destruct();
        }

        private void Destruct()
        {
            DestroyImmediate(this);
        }
    }
}