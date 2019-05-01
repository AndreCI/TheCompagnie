using UnityEngine;
using UnityEngine.UI;

namespace DuloGames.UI
{
	public class UIStatsAdd : MonoBehaviour {
		
		[SerializeField] private Text m_ValueText;
        private Unit unit;
        private Button button;
        private WinWindow source;
		
        public void Setup(Unit u, WinWindow s)
        {
            unit = u;
            m_ValueText.text = 0.ToString();
            button = GetComponent<Button>();
            source = s;
        }

        public void Toggle(bool v)
        {
            button.interactable = v;
        }
		public void OnButtonPress()
		{
			if (this.m_ValueText == null)
				return;
			
			this.m_ValueText.text = (int.Parse(this.m_ValueText.text) + 1).ToString();
            unit.level.GainXP(1);
            source.OnXPAdds();
		}
	}
}