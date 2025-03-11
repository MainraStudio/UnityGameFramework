using BroAudio.Demo.Scripts.UI;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace BroAudio.Demo.Scripts.InteractiveComponents
{
	public class HierarchyLocator : InteractiveComponent
	{
		[SerializeField] GameObject _target = null;

		protected override bool ListenToInteractiveZone() => false;

		private void Update()
		{
			if(!PauseMenu.Instance.IsOpen && InteractiveZone.IsInZone && Input.GetKeyDown(KeyCode.Tab))
			{
				Selection.activeObject = _target;
				EditorGUIUtility.PingObject(_target);
			}
		}
	}
} 
#endif