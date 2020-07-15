using UnityEngine;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AI
{
    [CreateAssetMenu(menuName = "Assets/AI/Utils/Planner debugger")]
    public class PlannerDebugger : ScriptableObject
    {
        public ScriptableHTNFactory HTNToDebug;
        public WorldDefinition World;

        private Planner planner = new Planner();
        private StringBuilder debugPrinter = new StringBuilder();

        public void FindPlan( ITaskNode root, WorldStateDefinition world, int[] ws )
        {
            PlannerResult result = planner.FindPlan( root, world, ws );
            if( result.Found )
            {
                AI.Utils.PrintMTR(debugPrinter, result.MTRNodes);
                Debug.LogWarningFormat("Plan: {0}", debugPrinter.ToString());
            }
            else
            {
                Debug.LogWarningFormat("No Plan");
            }
        }
    }

#if UNITY_EDITOR

    [CustomEditor( typeof( PlannerDebugger ) )]
    public class PlannerDebuggerInspector : Editor
    {
        private string serializedWS = "";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedWS = EditorGUILayout.TextArea(serializedWS);

            if ( GUILayout.Button("Debug Plan") )
            {
                PlannerDebugger pdbg = (target as PlannerDebugger);
                Dictionary<string, object> status = MiniJSON.Json.Deserialize(serializedWS) as Dictionary<string,object>;
                int[] ws = pdbg.World.World.BuildWorldStateFromJSON(status);
                pdbg.FindPlan( pdbg.HTNToDebug.Build(), pdbg.World.World, ws );
            }
        }
    }

#endif
}

