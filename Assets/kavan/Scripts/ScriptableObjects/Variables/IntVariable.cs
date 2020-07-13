using UnityEngine;

[CreateAssetMenu]
public class IntVariable : ScriptableObject
{
#if UNITY_EDITOR
	[Multiline]
	public string DeveloperDescription = "";
#endif

	public int DefaultValue;
	public int currentValue;

	public int CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	private void OnEnable()
	{
		currentValue = DefaultValue;
	}

	//SET VALUE
	public void SetValue(int value)
	{
		CurrentValue = value;
	}

	public void SetValue(IntVariable value)
	{
		CurrentValue = value.CurrentValue;
	}

	//Manipulate Value
	public void ApplyChange(int amount)
	{
		CurrentValue += amount;
	}

	public void ApplyChange(IntVariable amount)
	{
		CurrentValue += amount.CurrentValue;
	}
}
