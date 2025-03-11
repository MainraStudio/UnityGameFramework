using Ami.BroAudio;
using BroAudio.Demo.Scripts.InteractiveComponents;
using UnityEngine;
namespace BroAudio.Demo.Scripts
{
    public class TunnelSoundModifier : InteractiveComponent
    {
        [SerializeField] BroAudioType _targetType = default;
        [SerializeField, Volume] float _absorbedvolume = 0.3f;
        [SerializeField] float _transitionTime = 2f;

        private bool _hasStarted = false;

        public override void OnInZoneChanged(bool isInZone)
        {
            if(isInZone)
            {
                Ami.BroAudio.BroAudio.SetVolume(_targetType, _absorbedvolume, _hasStarted ? _transitionTime : 0f);
                _hasStarted = true;
            }
            else
            {
                Ami.BroAudio.BroAudio.SetVolume(_targetType, Ami.Extension.AudioConstant.FullVolume, _transitionTime);
			}
        }
    } 
}
