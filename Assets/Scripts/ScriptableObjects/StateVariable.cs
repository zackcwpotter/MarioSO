using UnityEngine;

// Remembers a state (e.g. Mario's powerup state) across scenes
[CreateAssetMenu(fileName = "StateVariable", menuName = "ScriptableObjects/StateVariable", order = 4)]
public class StateVariable : Variable<State>
{
    public override void SetValue(State value)
    {
        _value = value;
    }
}
