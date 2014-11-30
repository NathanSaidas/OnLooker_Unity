using UnityEngine;
using System.Collections;

namespace Gem
{
    public class MenuEventListener : UIEventListener
    {
        private enum MenuState
        {
            MAIN,
            SINGLE_PLAYER,
            ONLINE,
            OPTIONS
        }
        [SerializeField]
        private string m_LoadScene = "Level_01";

        private UIButton m_Singleplayer = null;
        private UIButton m_Online = null;
        private UIButton m_Options = null;
        private UIButton m_Quit = null;
        private UIButton m_Back = null;
        private UITextfield m_Textfield = null;
        private MenuState m_State = MenuState.MAIN;

        /// <summary>
        /// Call the base method start to do any initialization on the base class
        /// </summary>
        protected override void Start()
        {
 	        base.Start();
            ///Invoke a coroutine to get a late start.
            StartCoroutine(LateStart());
        }


        IEnumerator LateStart()
        {
            yield return new WaitForEndOfFrame();
            //Late Start Code Goes Here.
            m_Singleplayer = UIManager.Find<UIButton>("SinglePlayer");
            m_Online = UIManager.Find<UIButton>("Online");
            m_Options = UIManager.Find<UIButton>("Options");
            m_Quit = UIManager.Find<UIButton>("Quit");
            m_Back = UIManager.Find<UIButton>("Back");
            m_Textfield = UIManager.Find<UITextfield>("Textfield");

            if(m_Singleplayer != null)
            {
                m_Singleplayer.eventListener = this;
            }
            if(m_Online != null)
            {
                m_Online.eventListener = this;
            }
            if(m_Options != null)
            {
                m_Options.eventListener = this;
            }
            if(m_Quit != null)
            {
                m_Quit.eventListener = this;
            }
            if(m_Textfield != null)
            {
                m_Textfield.eventListener = this;
            }
            if(m_Back != null)
            {
                m_Back.eventListener = this;
            }
            UpdateMenuItems();
        }

        public override void OnRelayEvent(UIEvent aEvent, UIEventListener aListener)
        {
            if(aListener == null)
            {
                return;
            }
            switch(aEvent)
            {
                case UIEvent.MOUSE_CLICK:
                case UIEvent.MOUSE_DOUBLE_CLICK:
                    if(aListener == m_Singleplayer)
                    {
                        SinglePlayerClicked();
                    }
                    else if(aListener == m_Online)
                    {
                        OnlineClicked();
                    }
                    else if(aListener == m_Options)
                    {
                        OptionsClicked();
                    }
                    else if(aListener == m_Quit)
                    {
                        QuitClicked();
                    }
                    else if(aListener == m_Back)
                    {
                        BackClicked();
                    }
                    break;
            }
        }

        private void SinglePlayerClicked()
        {
            m_State = MenuState.SINGLE_PLAYER;
            //UpdateMenuItems();
            Game.LoadLevel(m_LoadScene);
        }
        private void OnlineClicked()
        {
            m_State = MenuState.ONLINE;
            UpdateMenuItems();
        }
        private void OptionsClicked()
        {
            m_State = MenuState.OPTIONS;
            UpdateMenuItems();
        }
        private void QuitClicked()
        {
            Game.Quit();
        }
        private void BackClicked()
        {
            m_State = MenuState.MAIN;
            UpdateMenuItems();
        }

        private void UpdateMenuItems()
        {
            switch(m_State)
            {
                case MenuState.MAIN:
                    {
                        m_Singleplayer.uiToggle.gameObject.SetActive(true);
                        m_Online.uiToggle.gameObject.SetActive(true);
                        m_Options.uiToggle.gameObject.SetActive(true);
                        m_Quit.uiToggle.gameObject.SetActive(true);
                        m_Back.uiToggle.gameObject.SetActive(false);
                        m_Textfield.uiToggle.gameObject.SetActive(false);
                    }
                    break;
                case MenuState.SINGLE_PLAYER:
                    {
                        m_Singleplayer.uiToggle.gameObject.SetActive(false);
                        m_Online.uiToggle.gameObject.SetActive(false);
                        m_Options.uiToggle.gameObject.SetActive(false);
                        m_Quit.uiToggle.gameObject.SetActive(false);
                        m_Back.uiToggle.gameObject.SetActive(true);
                        m_Textfield.uiToggle.gameObject.SetActive(true);
                        m_Textfield.text = string.Empty;
                    }
                    break;
                case MenuState.ONLINE:
                    {
                        m_Singleplayer.uiToggle.gameObject.SetActive(false);
                        m_Online.uiToggle.gameObject.SetActive(false);
                        m_Options.uiToggle.gameObject.SetActive(false);
                        m_Quit.uiToggle.gameObject.SetActive(false);
                        m_Back.uiToggle.gameObject.SetActive(true);
                        m_Textfield.uiToggle.gameObject.SetActive(true);
                        m_Textfield.label.text = string.Empty;
                        m_Textfield.text = string.Empty;
                    }
                    break;
                case MenuState.OPTIONS:
                    {
                        m_Singleplayer.uiToggle.gameObject.SetActive(false);
                        m_Online.uiToggle.gameObject.SetActive(false);
                        m_Options.uiToggle.gameObject.SetActive(false);
                        m_Quit.uiToggle.gameObject.SetActive(false);
                        m_Back.uiToggle.gameObject.SetActive(true);
                        m_Textfield.uiToggle.gameObject.SetActive(true);
                        m_Textfield.label.text = string.Empty;
                        m_Textfield.text = string.Empty;
                    }
                    break;
            }
        }
    }
}