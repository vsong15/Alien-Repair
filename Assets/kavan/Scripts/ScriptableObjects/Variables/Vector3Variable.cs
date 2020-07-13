using UnityEngine;

[CreateAssetMenu]
public class Vector3Variable : ScriptableObject
{
#if UNITY_EDITOR
	[Multiline]
	public string DeveloperDescription = "";
#endif

	public Vector3 DefaultValue;
	public Vector3 currentValue;

	public Vector3 CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	private void OnEnable()
	{
		currentValue = DefaultValue;
	}

	//Set Value
	public void SetValue(Vector3 value)
	{
		CurrentValue = value;
	}
	public void SetValue(Vector3Variable value)
	{
		CurrentValue = value.CurrentValue;
	}
}
