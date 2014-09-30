using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class CutsceneWindow : EditorWindow
{

    [MenuItem("Tools/CutScene Window")]
    static void Init()
    {
        CutsceneWindow window = EditorWindow.GetWindow<CutsceneWindow>("Cutscene Editor");
    }

    private void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }
    private void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }



    private enum State
    {
        NONE,
        BUILD,
        EDIT_MOVEMENT,
        EDIT_LOOK_AT
    }


    private State m_State = State.NONE;
    private GameObject m_Root = null;
    private string m_CurrentScene = string.Empty;
    //private CameraManager m_Manager = null;



    /// <summary>
    /// The scroll position on the build menu
    /// </summary>
    private Vector2 m_BuildScrollPosition = Vector2.zero;


    [SerializeField]
    private bool m_ShowCreateMenu = true;
    /// <summary>
    /// A variable to hold the name of the cutscene to be created
    /// </summary>
    [SerializeField]
    private string m_CutsceneName = string.Empty;
    /// <summary>
    /// A variable to hold the new name of the cutscene to be renamed
    /// </summary>
    private string m_RenameField = string.Empty;

    /// <summary>
    /// A local list of cutscenes. This way the window doesnt have to do a search every frame
    /// </summary>
    [SerializeField]
    List<Cutscene> m_Cutscenes = new List<Cutscene>();
    /// <summary>
    /// A list of indexs of the cutscene list to remove after processing
    /// </summary>
    List<int> m_DeleteList = new List<int>();
    [SerializeField]
    private string m_InstructionName;
    private CutsceneActionMode m_CutsceneAction = CutsceneActionMode.BEZIER;


    [SerializeField]
    private int m_CutSceneToEdit = -1;
    [SerializeField]
    private int m_InstructionToEdit = -1;
    [SerializeField]
    private int m_InstructionControlPointToEdit = -1;

    [SerializeField]
    private int m_LookAtPointInstruction = -1;
    [SerializeField]
    private int m_LookAtPoint = -1;

    [SerializeField]
    private Vector2 m_ScrollViewPos = Vector2.zero;
    //Messages
    private void OnInspectorUpdate()
    {
        //If the last scene was not equal too the current scene there was a new scene loaded.
        if(m_CurrentScene != EditorApplication.currentScene)
        {
            //scene changed
            m_CurrentScene = EditorApplication.currentScene;
            m_Cutscenes = new List<Cutscene>();
            updateCutsceneList();
        }
    }
    private void OnSceneGUI(SceneView sceneView)
    {
        //Get the current cutscene to edit
        if (m_CutSceneToEdit <= -1 || m_State != State.EDIT_MOVEMENT)
        {
            m_InstructionControlPointToEdit = -1;
            m_InstructionToEdit = -1;
            return;
        }

        Cutscene triggeringCutscene = null;
        //m_Cutscenes = m_Manager.cutscenes;
        if (m_Cutscenes != null && m_CutSceneToEdit < m_Cutscenes.Count)
        {
            triggeringCutscene = m_Cutscenes[m_CutSceneToEdit];

        }
        if (triggeringCutscene == null)
        {
            Debug.Log("Cutscene being editted is a null reference.");
            return;
        }


        //Get the current instruction to edit
        List<CutsceneInstruction> instructions = triggeringCutscene.instructions;

        if (instructions == null || instructions.Count == 0)
        {
            Debug.LogWarning("Missing instructions");
            return;
        }


        Handles.matrix = Matrix4x4.TRS(triggeringCutscene.transform.position, triggeringCutscene.transform.rotation, Vector3.one);

        float size = 0.05f;
        Camera editorCam = Camera.current;
        if (editorCam != null)
        {
            float distance = Vector3.Distance(editorCam.transform.position, triggeringCutscene.transform.position);
            size *= distance;
        }

        for (int i = 0; i < instructions.Count; i++)
        {
            if (instructions[i] == null)
            {
                return;
            }
            Handles.color = instructions[i].controlPointColor;
            if (instructions[i].edit == false)
            {
                continue;
            }
            switch (instructions[i].actionMode)
            {

                //Draw 4 Handles
                case CutsceneActionMode.BEZIER:
                    if (instructions[i].bezierAction == null)
                    {
                        break;
                    }
                    if (Handles.Button(instructions[i].bezierAction.startPosition, Quaternion.identity, size, size, Handles.DotCap))
                    {
                        m_InstructionToEdit = i;
                        m_InstructionControlPointToEdit = 0;
                        EditorApplication.RepaintProjectWindow();
                    }
                    else if (Handles.Button(instructions[i].bezierAction.controlPointA, Quaternion.identity, size, size, Handles.DotCap))
                    {
                        m_InstructionToEdit = i;
                        m_InstructionControlPointToEdit = 1;
                    }
                    else if (Handles.Button(instructions[i].bezierAction.controlPointB, Quaternion.identity, size, size, Handles.DotCap))
                    {
                        m_InstructionToEdit = i;
                        m_InstructionControlPointToEdit = 2;
                    }
                    else if (Handles.Button(instructions[i].bezierAction.endPosition, Quaternion.identity, size, size, Handles.DotCap))
                    {
                        m_InstructionToEdit = i;
                        m_InstructionControlPointToEdit = 3;
                    }
                    break;
                //Draw 2 Handles
                case CutsceneActionMode.STRAIGHT:
                    if (instructions[i].straightAction == null)
                    {
                        break; ;
                    }
                    if (Handles.Button(instructions[i].straightAction.startPosition, Quaternion.identity, size, size, Handles.DotCap))
                    {
                        m_InstructionToEdit = i;
                        m_InstructionControlPointToEdit = 0;
                    }
                    else if (Handles.Button(instructions[i].straightAction.endPosition, Quaternion.identity, size, size, Handles.DotCap))
                    {
                        m_InstructionToEdit = i;
                        m_InstructionControlPointToEdit = 1;
                    }
                    break;
            }



        }

        CutsceneInstruction targetInstruction = null;

        if (m_InstructionToEdit >= 0 && m_InstructionToEdit < instructions.Count)
        {
            targetInstruction = instructions[m_InstructionToEdit];
        }

        if (targetInstruction != null)
        {
            Vector3 positionHandle = Vector3.zero;

            if (targetInstruction.actionMode == CutsceneActionMode.BEZIER && m_InstructionControlPointToEdit >= 0 && m_InstructionControlPointToEdit < 4)
            {
                switch (m_InstructionControlPointToEdit)
                {
                    case 0:
                        positionHandle = Handles.PositionHandle(targetInstruction.bezierAction.startPosition, Quaternion.identity);
                        if (positionHandle != targetInstruction.bezierAction.startPosition)
                        {
                            targetInstruction.bezierAction.startPosition = positionHandle;
                            targetInstruction.getPath();
                            Undo.RegisterUndo(triggeringCutscene, "Move Handle");
                            //Undo.RecordObject(triggeringCutscene, "Move Handle");
                        }
                        break;
                    case 1:
                        positionHandle = Handles.PositionHandle(targetInstruction.bezierAction.controlPointA, Quaternion.identity);
                        if (positionHandle != targetInstruction.bezierAction.controlPointA)
                        {
                            targetInstruction.bezierAction.controlPointA = positionHandle;
                            targetInstruction.getPath();
                            Undo.RegisterUndo(triggeringCutscene, "Move Handle");
                            //Undo.RecordObject(triggeringCutscene, "Move Handle");
                        }
                        break;
                    case 2:
                        positionHandle = Handles.PositionHandle(targetInstruction.bezierAction.controlPointB, Quaternion.identity);
                        if (positionHandle != targetInstruction.bezierAction.controlPointB)
                        {
                            targetInstruction.bezierAction.controlPointB = positionHandle;
                            targetInstruction.getPath();
                            Undo.RegisterUndo(triggeringCutscene, "Move Handle");
                            //Undo.RecordObject(triggeringCutscene, "Move Handle");
                        }
                        break;
                    case 3:
                        positionHandle = Handles.PositionHandle(targetInstruction.bezierAction.endPosition, Quaternion.identity);
                        if (positionHandle != targetInstruction.bezierAction.endPosition)
                        {
                            targetInstruction.bezierAction.endPosition = positionHandle;
                            targetInstruction.getPath();
                            Undo.RegisterUndo(triggeringCutscene, "Move Handle");
                            //Undo.RecordObject(triggeringCutscene, "Move Handle");
                        }
                        break;
                    default:
                        Debug.LogError("Something is going horribly wrong");
                        break;
                }
            }
            else if (targetInstruction.actionMode == CutsceneActionMode.STRAIGHT && m_InstructionControlPointToEdit >= 0 && m_InstructionControlPointToEdit < 2)
            {
                switch (m_InstructionControlPointToEdit)
                {
                    case 0:
                        positionHandle = Handles.PositionHandle(targetInstruction.straightAction.startPosition, Quaternion.identity);
                        if (positionHandle != targetInstruction.straightAction.startPosition)
                        {
                            targetInstruction.straightAction.startPosition = positionHandle;
                            targetInstruction.getPath();
                            Undo.RegisterUndo(triggeringCutscene, "Move Handle");
                            //Undo.RecordObject(triggeringCutscene, "Move Handle");
                        }
                        break;
                    case 1:
                        positionHandle = Handles.PositionHandle(targetInstruction.straightAction.endPosition, Quaternion.identity);
                        if (positionHandle != targetInstruction.straightAction.endPosition)
                        {
                            targetInstruction.straightAction.endPosition = positionHandle;
                            targetInstruction.getPath();
                            Undo.RegisterUndo(triggeringCutscene, "Move Handle");
                            //Undo.RecordObject(triggeringCutscene, "Move Handle");
                        }
                        break;
                    default:
                        Debug.LogError("Something is going horribly wrong");
                        break;

                }
            }
        }




    }
    private void OnGUI()
    {
        //GUI.enabled = false;
        //EditorGUILayout.IntField("Instruction To Edit:", m_InstructionToEdit);
        //EditorGUILayout.IntField("Control Point:", m_InstructionControlPointToEdit);
        //GUI.enabled = true;

        //Draw setup menu if were not in a state to build or edit cutscenes
        if(m_Root == null)
        {
            m_State = State.NONE;
        }
        if (m_State == State.NONE)
        {
            drawSetupMenu();
            return;
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build"))
        {
            m_State = State.BUILD;
        }
        if (GUILayout.Button("Edit Movement"))
        {
            m_State = State.EDIT_MOVEMENT;
        }
        if (GUILayout.Button("Edit Look At"))
        {
            m_State = State.EDIT_LOOK_AT;
        }
        EditorGUILayout.EndHorizontal();

        switch (m_State)
        {
            case State.BUILD:
                drawBuildMenu();
                break;
            case State.EDIT_MOVEMENT:
                drawEditMovementMenu();
                break;
            case State.EDIT_LOOK_AT:
                drawEditLookAtMenu();
                break;
        }
    }

    /// <summary>
    /// Draws the setup menu to prompt the user to provide a root game object to get started. If they provide one the 
    /// the CutsceneWindow will process the game object to see if it has any cutscenes
    /// </summary>
    private void drawSetupMenu()
    {
        EditorGUILayout.LabelField("Drag and drop the game object you want to create cutscenes under. If the field is", GUILayout.Width(600.0f));
        EditorGUILayout.LabelField("left empty a default game object will be created when you press the start button.", GUILayout.Width(600.0f));
        m_Root = EditorUtilities.gameObjectField("Cutscene Root:", m_Root);

        if(GUILayout.Button("Start"))
        {
            m_State = State.BUILD;
            if(m_Root == null)
            {
                m_Root = new GameObject("Cutscenes");
            }
            else
            {
                updateCutsceneList();
            }
        }
    }

    private void drawBuildMenu()
    {

        EditorGUILayout.LabelField("Build Menu", EditorStyles.boldLabel);
        if (m_ShowCreateMenu == true)
        {
            drawCreateCutsceneMenu();
        }
        //m_Cutscenes = m_Manager.cutscenes;

        
        if (m_Cutscenes != null && m_Cutscenes.Count > 0)
        {
            drawCutsceneList();
        }
    }


    private void drawEditMovementMenu()
    {
        EditorGUILayout.LabelField("Edit Movement Menu", EditorStyles.boldLabel);
        //m_Cutscenes = m_Manager.cutscenes;
        if (m_Cutscenes != null && m_Cutscenes.Count > 0)
        {
            if (m_CutSceneToEdit >= 0 && m_CutSceneToEdit < m_Cutscenes.Count)
            {
                Cutscene triggeringCutscene = m_Cutscenes[m_CutSceneToEdit];

                List<CutsceneInstruction> instructions = triggeringCutscene.instructions;

                for (int i = 0; i < instructions.Count; i++)
                {
                    drawEditInstructionMovement(instructions[i]);
                }


            }
        }
    }

    private void drawEditInstructionMovement(CutsceneInstruction aInstruction)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Instruction: " + aInstruction.name, EditorStyles.boldLabel); //draw name
        //draw show/hide
        if (aInstruction.showInstruction == true)
        {
            if (GUILayout.Button("Hide"))
            {
                aInstruction.showInstruction = false;
            }
        }
        else
        {
            if (GUILayout.Button("Show"))
            {
                aInstruction.showInstruction = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (aInstruction.showInstruction == false)
        {
            return;
        }

        if (m_State == State.EDIT_MOVEMENT)
        {

            bool valueChanged = false;

            switch (aInstruction.actionMode)
            {
                case CutsceneActionMode.STRAIGHT:
                    EditorGUILayout.LabelField("Straight", EditorStyles.boldLabel);
                    CutsceneStraight straight = aInstruction.straightAction;
                    if (straight != null)
                    {

                        Vector3 startPosition = EditorGUILayout.Vector3Field("Start Position", straight.startPosition);
                        Vector3 endPosition = EditorGUILayout.Vector3Field("End Position", straight.endPosition);

                        if (startPosition != straight.startPosition || endPosition != straight.endPosition)
                        {
                            valueChanged = true;
                            straight.startPosition = startPosition;
                            straight.endPosition = endPosition;
                        }
                    }
                    break;
                case CutsceneActionMode.BEZIER:
                    EditorGUILayout.LabelField("Bezier", EditorStyles.boldLabel);
                    CutsceneBezier bezier = aInstruction.bezierAction;
                    if (bezier != null)
                    {
                        Vector3 startPosition = EditorGUILayout.Vector3Field("Start Position", bezier.startPosition);
                        Vector3 controlA = EditorGUILayout.Vector3Field("Control Point A", bezier.controlPointA);
                        Vector3 controlB = EditorGUILayout.Vector3Field("Control Point B", bezier.controlPointB);
                        Vector3 endPosition = EditorGUILayout.Vector3Field("End Position", bezier.endPosition);
                        int segments = Mathf.Abs(EditorGUILayout.IntField("Segments", bezier.segments));

                        if (startPosition != bezier.startPosition ||
                            controlA != bezier.controlPointA ||
                            controlB != bezier.controlPointB ||
                            endPosition != bezier.endPosition ||
                            segments != bezier.segments)
                        {
                            valueChanged = true;
                            bezier.startPosition = startPosition;
                            bezier.controlPointA = controlA;
                            bezier.controlPointB = controlB;
                            bezier.endPosition = endPosition;
                            bezier.segments = segments;
                        }
                    }
                    break;
            }
            //calculate the path
            if (valueChanged == true)
            {
                //Undo.RegisterUndo(aInstruction.owner, "Instruction");
                aInstruction.getPath();
                //SceneView.RepaintAll();
            }
        }
        else
        {


            //Show the Look At point Editor
            //Control Points have been shown
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Look At Points", EditorStyles.boldLabel);
            if (aInstruction.showLookAtPoints == false)
            {
                if (GUILayout.Button("Show"))
                {
                    aInstruction.showLookAtPoints = true;
                }
            }
            else
            {
                if (GUILayout.Button("Hide"))
                {
                    aInstruction.showLookAtPoints = false;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (aInstruction.showLookAtPoints == false)
            {
                return;
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                aInstruction.addLookAtPoint();
            }
            if (GUILayout.Button("Clear"))
            {
                aInstruction.clearLookAtPoints();
            }
            EditorGUILayout.EndHorizontal();

            List<CutsceneLookAt> lookAtPath = aInstruction.lookAtPath;

            if (lookAtPath == null || lookAtPath.Count == 0)
            {
                return;
            }
            int count = 1;

            for (int i = lookAtPath.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(count.ToString() + ".", EditorStyles.boldLabel);
                count++;
                GUILayout.Space(30.0f);
                if (GUILayout.Button("Remove"))
                {
                    aInstruction.removeLookAtPoint(lookAtPath[i]);
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                EditorGUILayout.EndHorizontal();
                //EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField("Position:");
                //Vector3 position = lookAtPath[i].position;
                //position.x = EditorGUILayout.FloatField("X", lookAtPath[i].position.x);
                //position.y = EditorGUILayout.FloatField("Y", lookAtPath[i].position.y);
                //position.z = EditorGUILayout.FloatField("Z", lookAtPath[i].position.z);
                //lookAtPath[i].position = position;
                //EditorGUILayout.EndHorizontal();

                lookAtPath[i].position = EditorGUILayout.Vector3Field("Position", lookAtPath[i].position);
                lookAtPath[i].startFrame = EditorGUILayout.IntField("Start Frame:", lookAtPath[i].startFrame);
                lookAtPath[i].endFrame = EditorGUILayout.IntField("End Frame:", lookAtPath[i].endFrame);
            }



        }
    }

    private void drawEditLookAtMenu()
    {
        EditorGUILayout.LabelField("Edit Menu", EditorStyles.boldLabel);
        //m_Cutscenes = m_Manager.cutscenes;
        if (m_Cutscenes != null && m_Cutscenes.Count > 0)
        {
            if (m_CutSceneToEdit >= 0 && m_CutSceneToEdit < m_Cutscenes.Count)
            {
                Cutscene triggeringCutscene = m_Cutscenes[m_CutSceneToEdit];

                List<CutsceneInstruction> instructions = triggeringCutscene.instructions;
                if (m_State == State.EDIT_LOOK_AT)
                {
                    m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos);
                }
                for (int i = 0; i < instructions.Count; i++)
                {
                    drawEditInstructionMovement(instructions[i]);
                }

                if (m_State == State.EDIT_LOOK_AT)
                {
                    EditorGUILayout.EndScrollView();
                }

            }
        }


    }

    /// <summary>
    /// Allow the user to create a cutscene which will add it to the list as well as make it a child of the root.
    /// </summary>
    private void drawCreateCutsceneMenu()
    {
        m_CutsceneName = EditorGUILayout.TextField("Cutscene Name", m_CutsceneName);
        if (GUILayout.Button("Create") == true && cutsceneWithNameExists(m_CutsceneName) == false)
        {
            GameObject cutsceneGameObject = new GameObject(m_CutsceneName);
            cutsceneGameObject.transform.parent = m_Root.transform;
            Cutscene cutscene = cutsceneGameObject.AddComponent<Cutscene>();
            cutscene.cutsceneName = m_CutsceneName;
            cutscene.sceneName = EditorApplication.currentScene;
            m_Cutscenes.Add(cutscene);
        }
    }

    /// <summary>
    /// Draws all the cutscenes for the user.
    /// </summary>
    private void drawCutsceneList()
    {
        EditorGUILayout.LabelField("Cutscenes", EditorStyles.boldLabel);
        m_BuildScrollPosition = EditorGUILayout.BeginScrollView(m_BuildScrollPosition);
        m_DeleteList.Clear();
        //Process the cutscene list
        for (int i = 0; i < m_Cutscenes.Count; i++)
        {
            drawCutscene(m_Cutscenes[i], i);
        }
        //Remove any flagged to be deleted
        for (int i = 0; i < m_DeleteList.Count; i++)
        {
            DestroyImmediate(m_Cutscenes[m_DeleteList[i]].gameObject);
            m_Cutscenes.RemoveAt(m_DeleteList[i]);
        }
        EditorGUILayout.EndScrollView();
    }
    private void drawCutscene(Cutscene aCutscene, int aIndex)
    {
        bool deleted = false;
        if (aCutscene != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(aCutscene.cutsceneName, GUILayout.Width(200.0f));
            bool toggle = false;


            if (aCutscene.showCutscene == true)
            {
                if (GUILayout.Button("Hide", GUILayout.Width(65.0f)))
                {
                    aCutscene.showCutscene = false;
                }
            }
            else
            {
                if (GUILayout.Button("Show", GUILayout.Width(65.0f)))
                {
                    aCutscene.showCutscene = true;
                }
            }
            if (GUILayout.Button("Delete", GUILayout.Width(65.0f)))
            {
                m_DeleteList.Add(aIndex);
                deleted = true;
            }

            if (aIndex == m_CutSceneToEdit)
            {
                toggle = EditorGUILayout.Toggle("Edit", true);
            }
            else
            {
                toggle = EditorGUILayout.Toggle("Edit", false);
            }
            if (toggle == true)
            {
                m_CutSceneToEdit = aIndex;
            }
            else if (toggle == false && aIndex == m_CutSceneToEdit)
            {
                m_CutSceneToEdit = -1;
            }

            EditorGUILayout.EndHorizontal();
            if (deleted == true)
            {
                return;
            }
            if (aCutscene.showCutscene == false)
            {
                return;
            }

            //Draw the actual cutscene properties

            GUI.enabled = false; 
            EditorGUILayout.LabelField("Scene Name", aCutscene.sceneName);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            m_RenameField = EditorGUILayout.TextField("Cutscene Name:",m_RenameField);
            if(GUILayout.Button("Rename") && cutsceneWithNameExists(m_RenameField) == false)
            {
                aCutscene.cutsceneName = m_RenameField;
                aCutscene.gameObject.name = m_RenameField;
            }
            EditorGUILayout.EndHorizontal();

            //Create an instruction
            m_InstructionName = EditorGUILayout.TextField("Instruction Name", m_InstructionName);

            EditorGUILayout.BeginHorizontal();
            m_CutsceneAction = (CutsceneActionMode)EditorGUILayout.EnumPopup("Type", m_CutsceneAction);

            if (GUILayout.Button("Create"))
            {
                //Create Instruction
                switch (m_CutsceneAction)
                {
                    case CutsceneActionMode.BEZIER:
                        Debug.Log("BEZIER");
                        aCutscene.addInstruction(m_InstructionName, CutsceneActionMode.BEZIER);
                        break;
                    case CutsceneActionMode.STRAIGHT:
                        Debug.Log("STRIAGHT");
                        aCutscene.addInstruction(m_InstructionName, CutsceneActionMode.STRAIGHT); //addss straight by default
                        break;
                }
                m_InstructionName = string.Empty;
            }
            EditorGUILayout.EndHorizontal();
            //Draw the instructions 
            List<CutsceneInstruction> instructions = aCutscene.instructions;
            if (instructions == null)
            {
                return;
            }
            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                //delete the instruction
                if (drawInstruction(instructions[i]) == true)
                {
                    aCutscene.removeInstruction(instructions[i]);
                }
            }
        }
    }

    public bool drawInstruction(CutsceneInstruction aInstruction)
    {

        if (aInstruction != null)
        {
            bool deleted = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(aInstruction.name, EditorStyles.boldLabel); //draw name
            //draw show/hide
            if (aInstruction.showInstruction == true)
            {
                if (GUILayout.Button("Hide"))
                {
                    aInstruction.showInstruction = false;
                }
            }
            else
            {
                if (GUILayout.Button("Show"))
                {
                    aInstruction.showInstruction = true;
                }
            }
            //draw delete
            if (GUILayout.Button("Delete"))
            {
                deleted = true;
            }
            EditorGUILayout.EndHorizontal();

            //if it was deleted return true to delete it
            if (deleted)
            {
                return true;
            }
            if (aInstruction.showInstruction == false)
            {
                return false;
            }

            //else keep drawing the instruction
            aInstruction.edit = EditorGUILayout.Toggle("Edit", aInstruction.edit);
            //Draw the Action Variables
            switch (aInstruction.actionMode)
            {
                case CutsceneActionMode.STRAIGHT:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Straight", EditorStyles.boldLabel);
                    if (GUILayout.Button("Get Path"))
                    {
                        aInstruction.getPath();
                    }
                    EditorGUILayout.EndHorizontal();
                    CutsceneStraight straight = aInstruction.straightAction;
                    if (straight != null)
                    {
                        straight.startPosition = EditorGUILayout.Vector3Field("Start Position", straight.startPosition);
                        straight.endPosition = EditorGUILayout.Vector3Field("End Position", straight.endPosition);
                    }
                    break;
                case CutsceneActionMode.BEZIER:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Bezier", EditorStyles.boldLabel);
                    if (GUILayout.Button("Get Path"))
                    {
                        aInstruction.getPath();
                    }
                    EditorGUILayout.EndHorizontal();
                    CutsceneBezier bezier = aInstruction.bezierAction;
                    if (bezier != null)
                    {
                        bezier.startPosition = EditorGUILayout.Vector3Field("Start Position", bezier.startPosition);
                        bezier.controlPointA = EditorGUILayout.Vector3Field("Control Point A", bezier.controlPointA);
                        bezier.controlPointB = EditorGUILayout.Vector3Field("Control Point B", bezier.controlPointB);
                        bezier.endPosition = EditorGUILayout.Vector3Field("End Position", bezier.endPosition);
                        bezier.segments = Mathf.Abs(EditorGUILayout.IntField("Segments", bezier.segments));
                    }
                    break;
            }



            aInstruction.distanceClamp = EditorGUILayout.FloatField("Distance Clamp", aInstruction.distanceClamp);
            aInstruction.timeDelay = EditorGUILayout.FloatField("Time Delay", aInstruction.timeDelay);
            aInstruction.moveSpeed = EditorGUILayout.FloatField("Move Speed", aInstruction.moveSpeed);
            aInstruction.moveMode = (CameraMode)EditorGUILayout.EnumPopup("Move Mode", aInstruction.moveMode);
            aInstruction.lookSpeed = EditorGUILayout.FloatField("Look Speed", aInstruction.lookSpeed);
        }
        //return false and keep the instruction
        return false;
    }


    /// <summary>
    /// This method just iterates through all the child gameobjects in the root game objects transform.
    /// If there is a cutscene on the child game object and the list does not currently contain the cutscene 
    /// Then add it to the list
    /// </summary>
    private void updateCutsceneList()
    {
        if(m_Root == null)
        {
            return;
        }

        foreach(Transform child in m_Root.transform)
        {
            Cutscene cutscene = child.GetComponent<Cutscene>();
            if(cutscene != null && m_Cutscenes.Contains(cutscene) == false)
            {
                m_Cutscenes.Add(cutscene);
            }
        }

    }

    private bool cutsceneWithNameExists(string aName)
    {
        for(int i = 0; i < m_Cutscenes.Count; i++)
        {
            if(m_Cutscenes[i].name == aName )
            {
                return true;
            }
        }
        return false;
    }

}