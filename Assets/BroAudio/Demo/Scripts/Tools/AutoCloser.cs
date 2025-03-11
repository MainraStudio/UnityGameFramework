using UnityEngine;
namespace BroAudio.Demo.Scripts.Tools
{
	public class AutoCloser : MonoBehaviour
	{
		void Start()
		{
			gameObject.SetActive(false);
		}
	} 
}
