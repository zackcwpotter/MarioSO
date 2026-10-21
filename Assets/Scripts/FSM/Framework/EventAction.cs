[System.Serializable]
public struct EventAction
{
    public Action action;
    public ActionType type;
}

public enum ActionType
{
    Attack = 0,
    Default = -1
}
