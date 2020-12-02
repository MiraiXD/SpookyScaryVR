using UnityEngine;
using System.Collections.Generic;
public class MoodElement
{
    public float value;
    public MoodElement()
    {
        value = 0f;
    }     
}

public class Mood
{
    public MoodElement safety;
    public MoodElement food;
    public MoodElement companionship;
    public MoodElement fun;
    public MoodElement energy;
    public bool isSitting;
    public Vector3 position;
    public Mood()
    {        
        safety = new MoodElement();
        food = new MoodElement();
        companionship = new MoodElement();
        fun = new MoodElement();
        energy = new MoodElement();
        isSitting = false;
        position = new Vector3();
    }
    public Dictionary<string, object> ToStates()
    {
        Dictionary<string, object> states = new Dictionary<string, object>();
        states.Add("safety", safety.value);
        states.Add("food", food.value);
        states.Add("companionship", companionship.value);
        states.Add("fun", fun.value);
        states.Add("energy", energy.value);
        states.Add("isSitting", isSitting);
        states.Add("position", position);
        return states;
    }
    public void FromStates(Dictionary<string,object> states)
    {
        if (states.TryGetValue("safety", out object threat)) this.safety.value = Mathf.Clamp01((float)threat);
        if (states.TryGetValue("food", out object hunger)) this.food.value = Mathf.Clamp01((float)hunger);
        if (states.TryGetValue("companionship", out object companionship)) this.companionship.value = Mathf.Clamp01((float)companionship);
        if (states.TryGetValue("fun", out object boredom)) this.fun.value = Mathf.Clamp01((float)boredom);
        if (states.TryGetValue("energy", out object energy)) this.energy.value = Mathf.Clamp01((float)energy);
        if (states.TryGetValue("isSitting", out object isSitting)) this.isSitting = (bool)isSitting;
        if (states.TryGetValue("position", out object position)) this.position = (Vector3)position;
    }
}
