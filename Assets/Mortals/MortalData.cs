using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class MoodElement
{
    public float Value { get { return value; } set { this.value = Mathf.Clamp01(value); } }
    [SerializeField] private float value;
    [HideInInspector] public float moodImpactMultiplier;
    public MoodElement(float value, float moodImpactMultiplier)
    {
        this.moodImpactMultiplier = moodImpactMultiplier;
        Value = value;
    }     
}
[System.Serializable]
public class Mood
{
    public class ImpactMultipliers
    {
        public const float safety = 10f;
        public const float food = 1.5f;
        public const float companionship = 1.1f;
        public const float fun = 1f;
        public const float energy = 1f;
    }
    public MoodElement safety;
    public MoodElement food;
    public MoodElement companionship;
    public MoodElement fun;
    public MoodElement energy;
    [HideInInspector] public bool isSitting;
    [HideInInspector] public Vector3 position;
    private Dictionary<string, MoodElement> elements = new Dictionary<string, MoodElement>();
    public Mood()
    {        
        safety = new MoodElement(1f,ImpactMultipliers.safety);
        food = new MoodElement(1f,ImpactMultipliers.food);
        companionship = new MoodElement(1f,ImpactMultipliers.companionship);
        fun = new MoodElement(1f,ImpactMultipliers.fun);
        energy = new MoodElement(0f,ImpactMultipliers.energy);
        isSitting = false;
        position = new Vector3();

        elements.Add("safety", safety);
        elements.Add("food", food);
        elements.Add("companionship", companionship);
        elements.Add("fun", fun);
        elements.Add("energy", energy);
    }
    public MoodElement GetNeed(string key)
    {
        return elements[key];
    }
    public List<KeyValuePair<string,float>> GetBiggestNeeds()
    {        
        List<KeyValuePair<string, float>> needs = new List<KeyValuePair<string, float>>();
        needs.Add( new KeyValuePair<string, float>("safety", (1f - safety.Value) * safety.moodImpactMultiplier));
        needs.Add(new KeyValuePair<string, float>("food", (1f - food.Value) * food.moodImpactMultiplier));
        needs.Add(new KeyValuePair<string, float>("companionship", (1f - companionship.Value) * companionship.moodImpactMultiplier));
        needs.Add(new KeyValuePair<string, float>("fun", (1f - fun.Value) * fun.moodImpactMultiplier));
        needs.Add(new KeyValuePair<string, float>("energy", (1f - energy.Value) * energy.moodImpactMultiplier));

        // sort decreasingly - biggest need will be in the first place
        needs.Sort((KeyValuePair<string, float> x, KeyValuePair<string, float> y) => { return y.Value.CompareTo(x.Value); });
        
        return needs;
    }
    public Dictionary<string, object> ToStates()
    {
        Dictionary<string, object> states = new Dictionary<string, object>();
        states.Add("safety", safety.Value);
        states.Add("food", food.Value);
        states.Add("companionship", companionship.Value);
        states.Add("fun", fun.Value);
        states.Add("energy", energy.Value);
        states.Add("isSitting", isSitting);
        states.Add("position", position);
        return states;
    }
    public void FromStates(Dictionary<string,object> states)
    {
        if (states.TryGetValue("safety", out object safety)) this.safety.Value = (float)safety;
        if (states.TryGetValue("food", out object food)) this.food.Value = (float)food;
        if (states.TryGetValue("companionship", out object companionship)) this.companionship.Value = (float)companionship;
        if (states.TryGetValue("fun", out object fun)) this.fun.Value = (float)fun;
        if (states.TryGetValue("energy", out object energy)) this.energy.Value = (float)energy;
        if (states.TryGetValue("isSitting", out object isSitting)) this.isSitting = (bool)isSitting;
        if (states.TryGetValue("position", out object position)) this.position = (Vector3)position;
    }

    public void UpdateMood(float deltaTime)
    {
        companionship.Value -= deltaTime * 0.01f;
        //safety.Value -= deltaTime * 0.01f;
        food.Value -= deltaTime * 0.01f;
        energy.Value -= deltaTime * 0.01f;
        fun.Value -= deltaTime * 0.01f;
    }
}
