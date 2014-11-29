using UnityEngine;
using System.Collections;

namespace Gem
{

    public class UITextfield : UIButton
    {
        private int m_MaxCharacter = 0;


        protected override void Start()
        {
            base.Start();
            label.textChanged += OnTextChanged;
            label.text = string.Empty;
        }

        protected override void Update()
        {
            base.Update();
            if(m_Toggle != null && m_Toggle.isSelected)
            {
                string currentText = text;
                string inputString = Input.inputString;
                if(inputString.Length > 0 && inputString[0] != 8)
                {
                    currentText += inputString[0];
                }
                if(Input.GetKeyDown(KeyCode.Backspace) && currentText.Length >= 1)
                {
                    currentText = currentText.Substring(0, currentText.Length - 1);
                }
                if(m_MaxCharacter > 0  && currentText.Length > 0)
                {
                    text = currentText.Substring(0, m_MaxCharacter);
                }
                else
                {
                    text = currentText;
                }
            }
        }

        protected virtual void OnTextChanged(string aText)
        {
            if(autoAdjust == true)
            {
                UpdateComponents();
            }

        }
        public string text
        {
            get { return label == null ? string.Empty : label.text; }
            set { if (label != null) { label.text = value; } }
        }
        public int maxCharacter
        {
            get { return m_MaxCharacter; }
            set { m_MaxCharacter = value; }
        }
    }
}