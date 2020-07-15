using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace AI
{
    /**/
    public static class Utils
    {
        private static Dictionary<System.Type, string> VISITOR_SYMBOLS = new Dictionary<System.Type, string>() { { typeof( SelectorVisitor ), "(?)"  }, { typeof(SequenceVisitor), "(>)" }  };

        public static void PrintTreeStructure(StringBuilder stringBuilder, ITaskNode root, int numTabs)
        {
            Stack<KeyValuePair<ITaskNode,int>> nodes = new Stack<KeyValuePair<ITaskNode, int>>();
            nodes.Push( new KeyValuePair<ITaskNode, int>( root, numTabs ) );
            while ( nodes.Count > 0 )
            {
                KeyValuePair<ITaskNode, int> entry = nodes.Pop();
                ITaskNode node = entry.Key;
                int padding = entry.Value;
                for (int i = 0; i < padding; i++)
                {
                    stringBuilder.Append('\t');
                }

                if (node.IsComposite())
                {
                    stringBuilder.Append('+');
                    ITaskNodeVisitor visitor = node.CreateVisitor();

                    string symbol = VISITOR_SYMBOLS[visitor.GetType()];

                    stringBuilder.Append(symbol);

                    stringBuilder.Append(node.GetName());

                    padding++;
                    for (int i = node.NumChildrenTasks() - 1; i >= 0; i--)
                    {
                        nodes.Push(  new KeyValuePair<ITaskNode, int>( node.GetChildTask( i ), padding ) );
                    }
                }
                else
                {
                    stringBuilder.Append('-');
                    stringBuilder.Append(node.GetName());
                }

                stringBuilder.Append('\n');
            }
        }

        public static void PrintCondition(StringBuilder stringBuilder, ICondition condition, int numTabs)
        {
        }

#if DEBUG

        public static void PrintMTR(StringBuilder stringBuilder, List<ITaskNode> mtrnodes)
        {
            stringBuilder.Clear();
            stringBuilder.Append("[");
            ITaskNode node = null;
            for (int i = 0; i < mtrnodes.Count - 1; i++)
            {
                node = mtrnodes[i];
                stringBuilder.Append(node.GetName());
                stringBuilder.Append(",");
            }
            node = mtrnodes[mtrnodes.Count - 1];
            stringBuilder.Append(node.GetName());
            stringBuilder.Append("]\n");
        }
#endif
    }
}
