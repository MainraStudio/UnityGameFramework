using Ami.BroAudio;
using BroAudio.Demo.Scripts.InteractiveComponents;
using UnityEngine;
using UnityEngine.Playables;
namespace BroAudio.Demo.Scripts
{
	public class CutScenePlayer : InteractiveComponent
	{
		[SerializeField] PlayableDirector _director = null;
		[SerializeField] SoundID _backgroundMusic = default;
		[SerializeField, Volume(true)] float _maxBgmVolumeDuringCutScene = default;

		protected override bool IsTriggerOnce => true;

		public override void OnInZoneChanged(bool isInZone)
		{
			base.OnInZoneChanged(isInZone);

			_director.Play();
			_director.stopped += OnCutSceneStopped;
            Ami.BroAudio.BroAudio.Play(_backgroundMusic)
                .AsBGM()
                .SetVolume(_maxBgmVolumeDuringCutScene);
		}

        private void OnCutSceneStopped(PlayableDirector director)
		{
			_director.stopped -= OnCutSceneStopped;
			Ami.BroAudio.BroAudio.SetVolume(_backgroundMusic,1f,2f);
		}
	}
}