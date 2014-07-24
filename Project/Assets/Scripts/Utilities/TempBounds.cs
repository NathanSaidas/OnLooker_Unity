using UnityEngine;
using System.Collections;

using OnLooker.UI;

public class TempBounds : MonoBehaviour {

    [SerializeField]
    private UILabel m_Label;

	// Use this for initialization
	void Start () 
    {
        if (m_Label != null)
        {
            m_Label.registerEvent(onUIEvent);
            m_Label.registerTextChangedEvent(onTextChanged);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_Label != null)
        {
            if (m_Label.textComponent.isFocused == true)
            {
                string inputString = Input.inputString;

                string verifiedString = string.Empty;

                if (inputString.Length > 0)
                {
                    if (inputString[0] > 32 && inputString[0] <= 127)
                    {
                        verifiedString += inputString[0];
                    }
                }

                string currentText = m_Label.text;

                if (Input.GetKeyDown(KeyCode.Backspace) && currentText.Length > 0)
                {
                    currentText = currentText.Substring(0, currentText.Length - 1);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentText += ' ';
                }

                currentText += verifiedString;

                m_Label.text = currentText;

                
            }
        }
	}
    void LateUpdate()
    {

    }

    void onUIEvent(UIToggle aSender, UIEventArgs aArgs)
    {
        //Debug.Log(aArgs.eventType);
    }
    string onTextChanged(UIText aSender, string aText)
    {
        Debug.Log(aText);
        return aText;
    }

}
