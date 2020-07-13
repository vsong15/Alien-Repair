using UnityEngine;

[CreateAssetMenu]
public class BoolVariable: ScriptableObject
{
#if UNITY_EDITOR
	[Multiline]
	public string DeveloperDescription = "";
#endif

	public bool DefaultValue;
	public bool currentValue;

	public bool CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	private void OnEnable()
	{
		currentValue = DefaultValue;
	}

	//SET VALUE
	public void SetValue(bool value)
	{
		currentValue = value;
	}

	public void SetValue(BoolVariable value)
	{
		currentValue = value.currentValue;
	}
}
