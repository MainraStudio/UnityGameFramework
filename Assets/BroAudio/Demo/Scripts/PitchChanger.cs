using Ami.BroAudio;
using UnityEngine;
namespace BroAudio.Demo.Scripts
{
	public class PitchChanger : MonoBehaviour
	{
		[SerializeField, Pitch] float _pitch = 1f;
		[SerializeField] BroAudioType _targetAudioType = BroAudioType.All;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                Ami.BroAudio.BroAudio.SetPitch(_targetAudioType, _pitch);
            }
        }
    }
}
