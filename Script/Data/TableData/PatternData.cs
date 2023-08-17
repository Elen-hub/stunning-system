using System.Collections.Generic;
namespace Data
{
    public class PatternData
    {
        public readonly int Index;
        public readonly int SkillIndex;
        public readonly eFSMState State;
        public readonly int Priority;
        public readonly float ProbWeight;
        public readonly float CoolTime;
        public readonly PatternCondition[] PatternConditionArray;
        public PatternData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            SkillIndex = int.Parse(dataPair["SkillKey"]);
            string[] stateArray = dataPair["State"].Split('|');
            for (int i = 0; i < stateArray.Length; ++i)
            {
                switch (stateArray[i])
                {
                    case "Idle":
                        State |= eFSMState.Idle;
                        break;
                    case "Patrol":
                        State |= eFSMState.Patrol;
                        break;
                    case "Chase":
                        State |= eFSMState.Chase;
                        break;
                    case "Death":
                        State |= eFSMState.Death;
                        break;
                }
            }
            Priority = int.Parse(dataPair["Priority"]);
            ProbWeight = float.Parse(dataPair["ProbWeight"]);
            CoolTime = float.Parse(dataPair["Cooltime"]);

            int columContainsCounter = 0;
            System.Text.StringBuilder conditionNameBuilder = new System.Text.StringBuilder("Condition0", 10);
            System.Text.StringBuilder conditionVariableBuilder = new System.Text.StringBuilder("ConditionRange0", 15);
            List<PatternCondition> conditionList = new List<PatternCondition>();
            while (true)
            {
                ++columContainsCounter;
                conditionNameBuilder[conditionNameBuilder.Capacity - 1] = char.Parse(columContainsCounter.ToString());
                string currentColumName = conditionNameBuilder.ToString();
                if (string.IsNullOrEmpty(dataPair[currentColumName]))
                    break;

                conditionVariableBuilder[conditionVariableBuilder.Capacity - 1] = char.Parse(columContainsCounter.ToString());
                string currentColumVariable = conditionVariableBuilder.ToString();

                PatternCondition conditionRef = null;
                switch (dataPair[currentColumName])
                {
                    case "HP":
                        {
                            string[] conditionSplit = dataPair[currentColumVariable].Split('|');
                            conditionRef = new HPPatternCondition
                            {
                                Condition = ePatternCondition.HP,
                                MinRangeRate = float.Parse(conditionSplit[0]),
                                MaxRangeRate = float.Parse(conditionSplit[1]),
                            };
                            break;
                        }
                    case "Distance":
                        {
                            string[] conditionSplit = dataPair[currentColumVariable].Split('|');
                            conditionRef = new DistancePatternCondition
                            {
                                Condition = ePatternCondition.Distance,
                                MinRange = float.Parse(conditionSplit[0]),
                                MaxRange = float.Parse(conditionSplit[1]),
                            };
                            break;
                        }
                }
                conditionList.Add(conditionRef);
            }
            PatternConditionArray = conditionList.ToArray();
        }
    }
}