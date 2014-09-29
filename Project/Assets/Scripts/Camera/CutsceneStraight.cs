using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CutsceneStraight : CutsceneAction
{


    public CutsceneStraight()
        : base()
    {

    }
    public CutsceneStraight(Vector3 aStart, Vector3 aEnd)
        : base()
    {
        startPosition = aStart;
        endPosition = aEnd;
    }

    public override Vector3[] getPath()
    {
        return new Vector3[] { startPosition, endPosition };
    }
}
