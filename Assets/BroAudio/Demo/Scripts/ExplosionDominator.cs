using System.Collections;
using Ami.BroAudio;
using BroAudio.Demo.Scripts.InteractiveComponents;
using UnityEngine;
namespace BroAudio.Demo.Scripts
{
	public class ExplosionDominator : InteractiveComponent
	{
		[SerializeField] ParticleSystem _particle = null;
		[SerializeField] ParticleSystem _fog = null;
		[SerializeField] float _playInterval = default;
		[SerializeField] SoundID _explosion = default;
#pragma warning disable 414
        [SerializeField] float dominateFadeOut = default;
        [SerializeField, Frequency] float _lowPassFrequency = default;
#pragma warning restore 414
        private Coroutine _coroutine;
		private IAudioPlayer _explosionPlayer = null;

        private void PlayAudio()
        {
            _explosionPlayer = Ami.BroAudio.BroAudio.Play(_explosion);
#if !UNITY_WEBGL
            _explosionPlayer.AsDominator().LowPassOthers(_lowPassFrequency, new Fading(BroAdvice.FadeTime_Quick, dominateFadeOut, EffectType.LowPass)); 
#endif
		}

        public override void OnInZoneChanged(bool isInZone)
		{
			if(_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}

			if(isInZone)
			{
				_coroutine = StartCoroutine(KeepPlaying());
			}
			else
			{
				_particle.Stop();
				_fog.gameObject.SetActive(true);
				_fog.Play();
			}
		}

		private IEnumerator KeepPlaying()
		{
			while(true)
            {
                if (_particle.isPlaying)
                {
                    yield return new WaitWhile(() => _particle.isPlaying);
                }

                _particle.Play();
                _fog.Stop();
                _fog.gameObject.SetActive(false);

                PlayAudio();
                
                yield return new WaitForSeconds(_playInterval);
            }
        }
    }
}