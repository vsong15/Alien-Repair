// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// Ryan Hipple
// ----------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu]
public class FloatVariable : ScriptableObject
{
#if UNITY_EDITOR
	[Multiline]
	public string DeveloperDescription = "";
#endif

	public float DefaultValue;
	public float currentValue;

	public float CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	private void OnEnable()
	{
		currentValue = DefaultValue;
	}

	//Set Value
	public void SetValue(float value)
	{
		currentValue = value;
	}

	public void SetValue(FloatVariable value)
	{
		currentValue = value.currentValue;
	}

	//Manipulate Value
	public void ApplyChange(float amount)
	{
		currentValue += amount;
	}

	public void ApplyChange(FloatVariable amount)
	{
		currentValue += amount.currentValue;
	}
}
