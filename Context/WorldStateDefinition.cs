using System.Collections.Generic;
using System.Text;
using System;

namespace AI
{
    public class WorldStateDefinition
    {
        public Dictionary<string, string[]> enums = new Dictionary<string, string[]>();
        public string[] ints;
        public string[] order;


        public bool IsEnum(string variable)
        {
            return enums.ContainsKey( variable );
        }

        public void SetInts(string[] ints)
        {
            this.ints = ints;
        }

        public void SetOrder(string[] order)
        {
            this.order = order;
        }

        public int FindIndex(string name)
        {
            for (int i = 0; i < order.Length; i++)
            {
                if (order[i] == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public int FindValueInWorldState(string name, int[] worldState)
        {
            for (int i = 0; i < order.Length; i++)
            {
                if ( order[i] == name )
                {
                    return worldState[i];
                }
            }

            return int.MinValue;
        }

        public void SetValueInWorldState(string name, int value, int[] worldState)
        {
            for (int i = 0; i < order.Length; i++)
            {
                if (order[i] == name)
                {
                    worldState[i] = value;
                    return;
                }
            }
        }

        public void SetEnumValueInWorldState(string name, string value, int[] worldState)
        {
            for (int i = 0; i < order.Length; i++)
            {
                if (order[i] == name)
                {
                    if ( enums.ContainsKey( name ) )
                    {
                        for (int j = 0; j < enums[name].Length; j++)
                        {
                            if (enums[name][j] == value)
                            {
                                worldState[i] = j;
                                return;
                            }
                        }
                    }
                    return;
                }
            }
        }

        public int[] CreateWorldState()
        {
            int[] ws = new int[ order.Length ];
            return ws;
        }

        public int[] CopyWorldState(int[] ws)
        {
            int[] copy = new int[ws.Length];
            System.Array.Copy(ws, copy, ws.Length);
            return copy;
        }

        public bool IsSameWorldState(int[] ws1, int[] ws2)
        {
            if ( ws1.Length != ws2.Length )
            {
                return false;
            }

            for (int i = 0; i < ws1.Length; i++)
            {
                if ( ws1[i] != ws2[i] )
                {
                    return false;
                }
            }

            return true;
        }

        public string WorldStatePrettyPrint(int[] ws)
        {
            StringBuilder sb = new StringBuilder();
            string variable;

            sb.Append( "\n{" );
            for (int i = 0; i < order.Length - 1; i++)
            {
                variable = order[i];
                if (IsEnum(variable))
                {
                    sb.AppendFormat("\"{0}\" : \"{1}\",\n", variable, enums[variable][ws[i]]  );
                }
                else
                {
                    sb.AppendFormat( "\"{0}\" : {1},\n", variable, ws[i] );
                }
            }

            int last = order.Length - 1;
            variable = order[last];
            if (IsEnum(variable))
            {
                sb.AppendFormat("\"{0}\" : \"{1}\"\n", variable, enums[variable][last]);
            }
            else
            {
                sb.AppendFormat("\"{0}\" : {1}\n", variable, ws[last]);
            }

            sb.Append("}\n");

            return sb.ToString();
        }

        public void WorldStatePrettyPrint(StringBuilder sb, int[] ws)
        {
            string variable;

            sb.Append("\n{");
            for (int i = 0; i < order.Length - 1; i++)
            {
                variable = order[i];
                if (IsEnum(variable))
                {
                    sb.AppendFormat("\"{0}\" : \"{1}\",\n", variable, enums[variable][ws[i]]);
                }
                else
                {
                    sb.AppendFormat("\"{0}\" : {1},\n", variable, ws[i]);
                }
            }

            int last = order.Length - 1;
            variable = order[last];
            if (IsEnum(variable))
            {
                sb.AppendFormat("\"{0}\" : \"{1}\"\n", variable, enums[variable][last]);
            }
            else
            {
                sb.AppendFormat("\"{0}\" : {1}\n", variable, ws[last]);
            }

            sb.Append("}\n");
        }

        public int[] BuildWorldStateFromJSON(Dictionary<string,object> json)
        {
            int[] ws = CreateWorldState();
            foreach (var item in json)
            {
                int idx = FindIndex( item.Key );

                if (IsEnum(item.Key))
                {
                    ws[idx] = 0;
                    for (int i = 0; i < enums[item.Key].Length; i++)
                    {
                        if ( (string)item.Value == enums[item.Key][i] )
                        {
                            ws[idx] = i;
                            break;
                        }
                    }
                }
                else
                {
                    ws[idx] = Convert.ToInt32( item.Value );
                }
            }
            return ws;
        }
    }
}
