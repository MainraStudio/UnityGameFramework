using Ami.BroAudio;
using BroAudio.Demo.Scripts.InteractiveComponents;
using UnityEngine;
namespace BroAudio.Demo.Scripts
{
    public class SparkleObject : InteractiveComponent
    {
        [SerializeField] SoundID _sound = default;

        protected override bool IsTriggerOnce => true;

        public override void OnInZoneChanged(bool isInZone)
        {
            base.OnInZoneChanged(isInZone);
            Ami.BroAudio.BroAudio.Play(_sound);
            Destroy(gameObject);
        }
    } 
}
