using UnityEngine;

namespace NeoDragonCP
{
	public class EventRaiser : MonoBehaviour
	{
		public GameEvent eventToRaise;
		public void RaiseEvent()
		{
			if (eventToRaise == null)
			{
				Debug.Log("Event was not set for Event Raiser on GameObject named:" + gameObject.name);
				return;
			}

			eventToRaise.Raise();
			Debug.Log("Raised Event on GameObject named:" + gameObject.name);
		}
	}
}