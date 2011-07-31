using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Navigation
{
    /// <summary>
    /// Состояние узла навигации
    /// </summary>
    public class NavigationNodeState
    {
        public static implicit operator NavigationNodeState[](NavigationNodeState state)
        {
            return new NavigationNodeState[] { state };
        }

        public const string openingIdBracket = "(";
        public const string closingIdBracket = ")";

        public const string openingBracket = "[";
        public const string closingBracket = "]";

        public const string equal = "=";
        public const string delimeter = ",";

        /// <summary>
        /// Идентифицирующие значения состояния
        /// </summary>
        public Dictionary<string, string> IdValues = new Dictionary<string, string>();

        /// <summary>
        /// Значения состояния
        /// </summary>
        public Dictionary<string, string> Values = new Dictionary<string, string>();

        /// <summary>
        /// Идентифицирующие флаги состояния
        /// </summary>
        public List<string> IdFlags = new List<string>();

        /// <summary>
        /// Флаги состояния
        /// </summary>
        public List<string> Flags = new List<string>();

        /// <summary>
        /// Идентификатор узла, вместе с идентифицирующими флагами и значениями идентифицирует контрол
        /// </summary>
        public string NodeId;

        /// <summary>
        /// Идентификатор контрола
        /// </summary>
        public string ControlId
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(NodeId);

                var idVals = IdFlags.Concat(IdValues.Select(n => n.Key + equal + n.Value)).ToArray();
                if (idVals.Length > 0)
                {
                    sb.Append(openingIdBracket);
                    sb.Append(String.Join(delimeter, idVals.ToArray()));
                    sb.Append(closingIdBracket);
                }
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(ControlId);

            var vals = Flags.Concat(Values.Select(n => n.Key + equal + n.Value)).ToArray();
            if(vals.Length > 0)
            {
                sb.Append(openingBracket);
                sb.Append(String.Join(delimeter, vals.ToArray()));
                sb.Append(closingBracket);
            }
            return sb.ToString();
        }

        public static NavigationNodeState FromString(string state)
        {
            var nodeState = new NavigationNodeState();

            string operandsString = null;
            if (state.Contains(openingBracket))
            {
                var openingIndex = state.IndexOf(openingBracket);
                var closingIndex = state.IndexOf(closingBracket);
                nodeState.NodeId = state.Substring(0, openingIndex);
                operandsString = state.Substring(openingIndex + 1, closingIndex - openingIndex - 1);
            }
            else
            {
                nodeState.NodeId = state;
            }

            string operandsIdString = null;
            if (nodeState.NodeId.Contains(openingIdBracket))
            {
                var openingIndex = nodeState.NodeId.IndexOf(openingIdBracket);
                var closingIndex = nodeState.NodeId.IndexOf(closingIdBracket);
                operandsIdString = nodeState.NodeId.Substring(openingIndex + 1, closingIndex - openingIndex - 1);
                nodeState.NodeId = nodeState.NodeId.Substring(0, openingIndex);
            }

            if (!String.IsNullOrEmpty(operandsIdString))
            {
                var operands = operandsIdString.Split(delimeter[0]);

                foreach (var operand in operands)
                {
                    if (operand.Contains(equal))
                    {
                        int pos = operand.IndexOf(equal);
                    	string name = operand.Substring(0, pos);
                    	string value = operand.Substring(pos+1, operand.Length - pos - 1);
                        nodeState.IdValues[name.Trim()] = value.Trim();
                    }
                    else
                    {
                        nodeState.IdFlags.Add(operand.Trim());
                    }
                }
            }

            if (!String.IsNullOrEmpty(operandsString))
            {
                var operands = operandsString.Split(delimeter[0]);

                foreach (var operand in operands)
                {
                    if (operand.Contains(equal))
                    {
                    	int pos = operand.IndexOf(equal);
                    	string name = operand.Substring(0, pos);
                    	string value = operand.Substring(pos + 1, operand.Length - pos - 1);
                        nodeState.Values[name.Trim()] = value.Trim();
                    }
                    else
                    {
                        nodeState.Flags.Add(operand.Trim());
                    }
                }
            }

            return nodeState;
        }
    }
}
