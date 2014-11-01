using UnityEngine;
using System.Collections;
using Gem;

/// <summary>
/// This class demonstrates how events could be used.
/// Notice the inheritence from MonoGameEventHandler
/// </summary>
public class ExampleEventHandler : MonoGameEventHandler
{

	/// <summary>
	/// Here I register for events
	/// </summary>
	void Start () 
    {
        RegisterEvent(GameEventID.GAME_LEVEL_LOAD_FINISH);
	}

    /// <summary>
    /// Here I Override the OnGameEvent method as it will be invoked by the event system to let us know of the event occuring
    /// </summary>
    /// <param name="aEventType"></param>
    protected override void OnGameEvent(GameEventID aEventType)
    {
        switch(aEventType)
        {
            case GameEventID.GAME_LEVEL_LOAD_FINISH:
                {
                    GameScene loadedScene = (GameScene)eventData.triggeringObject;
                    Debug.Log("Loaded " + loadedScene.sceneName + "!");
                }
                break;
        }
    }
}
