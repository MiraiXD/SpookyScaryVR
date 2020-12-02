using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void FSMState(FSM fsm, GameObject gameObject);
public class FSM 
{
    private Stack<FSMState> states = new Stack<FSMState>();
    public void Update(GameObject go)
    {
        if (states.Count > 0) states.Peek().Invoke(this, go);
    }
    public void PushState(FSMState state)
    {
        states.Push(state);
    }
    public FSMState PopState()
    {
        return states.Pop();
    }
}
