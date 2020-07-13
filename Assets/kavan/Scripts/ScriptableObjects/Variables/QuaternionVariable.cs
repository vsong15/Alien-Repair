using UnityEngine;

[CreateAssetMenu]
public class QuaternionVariable : ScriptableObject
{
#if UNITY_EDITOR
	[Multiline]
	public string DeveloperDescription = "";
#endif

	public Quaternion DefaultValue;
	public Quaternion currentValue;

	public Quaternion CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	private void OnEnable()
	{
		currentValue = DefaultValue;
	}

	//Set Value
	public void SetValue(Quaternion value)
	{
		CurrentValue = value;
	}
	public void SetValue(QuaternionVariable value)
	{
		CurrentValue = value.CurrentValue;
	}
}
