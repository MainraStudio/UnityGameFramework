using Ami.BroAudio;
using BroAudio.Demo.Scripts.InteractiveComponents;
using UnityEngine;
namespace BroAudio.Demo.Scripts
{
	public class EffectField : InteractiveComponent
	{
#pragma warning disable 414
        [SerializeField] SoundID _enterExitSound = default;
		[SerializeField, Frequency] float _lowPassFreq = 800f;
		[SerializeField] float _fadeTime = 1f;
#pragma warning restore 414
        public override void OnInZoneChanged(bool isInZone)
		{
			Ami.BroAudio.BroAudio.Play(_enterExitSound);

#if !UNITY_WEBGL
			if (isInZone)
			{
				Ami.BroAudio.BroAudio.SetEffect(Effect.LowPass(_lowPassFreq, _fadeTime));
			}
			else
			{
                Ami.BroAudio.BroAudio.SetEffect(Effect.ResetLowPass(_fadeTime));
			} 
#endif
		}
	}
}