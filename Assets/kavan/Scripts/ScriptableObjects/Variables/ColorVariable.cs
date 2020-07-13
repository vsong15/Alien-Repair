using UnityEngine;

[CreateAssetMenu]
public class ColorVariable: ScriptableObject
{
#if UNITY_EDITOR
	[Multiline]
	public string DeveloperDescription = "";
#endif

	public Color DefaultValue;
	public Color currentValue;

	public Color CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	private void OnEnable()
	{
		currentValue = DefaultValue;
	}

	//SET VALUE
	public void SetValue(Color value)
	{
		CurrentValue = value;
	}

	public void SetValue(ColorVariable value)
	{
		CurrentValue = value.CurrentValue;
	}
}

