using UnityEngine;

[CreateAssetMenu]
public class MaterialVariable: ScriptableObject
{
#if UNITY_EDITOR
	[Multiline]
	public string DeveloperDescription = "";
#endif

	public Material DefaultValue;
	public Material currentValue;

	public Material CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	private void OnEnable()
	{
		currentValue = DefaultValue;
	}

	//SET VALUE
	public void SetValue(Material value)
	{
		CurrentValue = value;
	}

	public void SetValue(MaterialVariable value)
	{
		CurrentValue = value.CurrentValue;
	}
}

