using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class OverworldEffectCondition
{
    public enum VARIABLE {SHARD, CURRENT_HEALTH, COMP_OWNED}
    public enum TYPE {DEFAULT_TRUE, EQUAL, BIGGER, LESSER}
    public enum VARIABLE_VALUE { STATIC, CURRENT, MIN, MAX, INDEX}

    public VARIABLE variable;
    public TYPE type;
    public int v1;
    public VARIABLE_VALUE v1Type;
    public int v2;
    public VARIABLE_VALUE v2Type;

    private int GetValue(int baseV, VARIABLE_VALUE type)
    {
        if(type == VARIABLE_VALUE.STATIC) { return baseV; }
        switch (variable)
        {
            case VARIABLE.SHARD:
                switch (type)
                {
                    case VARIABLE_VALUE.CURRENT:
                        return PlayerInfos.Instance.CurrentShards;
                }
                break;
            case VARIABLE.CURRENT_HEALTH:
                switch (type)
                {
                    case VARIABLE_VALUE.MIN:
                        return PlayerInfos.Instance.compagnions.Select(x => x.CurrentHealth).Min();
                    case VARIABLE_VALUE.MAX:
                        return PlayerInfos.Instance.compagnions.Select(x => x.CurrentHealth).Max();
                }
                break;
            case VARIABLE.COMP_OWNED:
                switch (type)
                {
                    case VARIABLE_VALUE.INDEX:
                        return PlayerInfos.Instance.compagnions.FindAll(x => x.id == baseV).Count();
                }break;
        }
        return baseV;
    }

    public bool Test()
    {
        switch (type)
        {
            case TYPE.EQUAL:
                return GetValue(v1, v1Type) == GetValue(v2, v2Type);

            case TYPE.BIGGER:
                return GetValue(v1, v1Type) > GetValue(v2, v2Type);

            case TYPE.LESSER:
                return GetValue(v1, v1Type) < GetValue(v2, v2Type);

        }
        return true;
    }

    private string GetVariableDescription()
    {
        switch (variable)
        {
            case VARIABLE.CURRENT_HEALTH:
                return "health";
            case VARIABLE.SHARD:
                return "shards";
        }
        return "";
    }

    public List<string> GetDescripton()
    {
        if (variable != VARIABLE.COMP_OWNED)
        {
            return new List<string> { "You do not have enough " + GetVariableDescription() + "." };
        }
        else
        {
            return new List<string> { "You already have this unit with you." };
        }
    }

}