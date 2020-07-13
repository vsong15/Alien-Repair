using UnityEngine;

[CreateAssetMenu]
public class SOVariable: ScriptableObject
{
#if UNITY_EDITOR
	[Multiline]
	public string DeveloperDescription = "";
#endif

	public ScriptableObject DefaultValue;
	public ScriptableObject currentValue;

	public ScriptableObject CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	private void OnEnable()
	{
		currentValue = DefaultValue;
	}

	//SET VALUE
	public void SetValue(ScriptableObject value)
	{
		CurrentValue = value;
	}

	public void SetValue(SOVariable value)
	{
		CurrentValue = value.CurrentValue;
	}
}

