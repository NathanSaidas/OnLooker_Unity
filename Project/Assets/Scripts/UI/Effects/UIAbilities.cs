using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gem
{

    public class UIAbilities : MonoBehaviour
    {
        [SerializeField]
        private string m_UnitName = string.Empty;
        /// <summary>
        /// The time it takes for an ability to fade.
        /// </summary>
        [SerializeField]
        private float m_FadeTime = 2.0f;
        /// <summary>
        /// The ability to show on the HUD
        /// </summary>
        [SerializeField]
        private int m_SelectedAbilityIndex = -1;
        /// <summary>
        /// The names of the abilities to search for on start
        /// </summary>
        [SerializeField]
        private List<string> m_AbilityNames = new List<string>();
        /// <summary>
        /// A list of abilities to display.
        /// </summary>
        private List<UIImage> m_Abilities = new List<UIImage>();

        
        void Start()
        {
            StartCoroutine(LateStart());
        }
        private IEnumerator LateStart()
        {
            yield return new WaitForEndOfFrame();
            Unit unit = UnitManager.GetUnit(m_UnitName);
            if(unit != null)
            {
                unit.abilityHud = this;
            }

            IEnumerator<string> names = m_AbilityNames.GetEnumerator();
            while(names.MoveNext())
            {
                if(names.Current == null)
                {
                    continue;
                }
                UIImage image = UIManager.Find<UIImage>(names.Current);
                if(image != null)
                {
                    m_Abilities.Add(image);
                }
            }

            m_AbilityNames.Clear();
        }
        // Update is called once per frame
        void Update()
        {
            if(m_Abilities.Count == 0)
            {
                return;
            }
            
            for(int i = 0; i < m_Abilities.Count; i++)
            {
                UIImage current = m_Abilities[i];
                if(i == m_SelectedAbilityIndex)
                {
                    Color color = current.color;
                    color.a = Mathf.Clamp01(color.a + Time.deltaTime * m_FadeTime);
                    current.color = color;
                    current.SetColor();
                }
                else
                {
                    Color color = current.color;
                    color.a = Mathf.Clamp01(color.a - Time.deltaTime * m_FadeTime * 3.0f);
                    current.color = color;
                    current.SetColor();
                }
            }
        }

        public void NextAbility()
        {
            m_SelectedAbilityIndex++;
            if(m_SelectedAbilityIndex >= m_Abilities.Count)
            {
                m_SelectedAbilityIndex = 0;
            }
        }
        public void PreviousAbility()
        {
            m_SelectedAbilityIndex--;
            if (m_SelectedAbilityIndex < 0)
            {
                m_SelectedAbilityIndex = m_Abilities.Count -1;
            }
        }
        public void SelectAbility(Ability aAbility)
        {
            if(aAbility != null)
            {
                string abilityName = "Ability_" + aAbility.abilityName + "_Image";
                for(int i = 0; i < m_Abilities.Count; i++)
                {
                    if(m_Abilities[i].name == abilityName)
                    {
                        m_SelectedAbilityIndex = i;
                        break;
                    }
                }
            }
        }

        public float fadeTime
        {
            get { return m_FadeTime; }
            set { m_FadeTime = value; }
        }
        public int selectedAbilityIndex
        {
            get { return m_SelectedAbilityIndex; }
            set { m_SelectedAbilityIndex = value; }
        }


    }
}