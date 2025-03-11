using Ami.BroAudio;
using UnityEngine;
using UnityEngine.UI;
namespace BroAudio.Demo.Scripts.UI
{
    public class SongNameUI : MonoBehaviour
    {
        [SerializeField] Text _title = null;

        void Start()
        {
            Ami.BroAudio.BroAudio.OnBGMChanged += OnBGMChanged;
        }

        private void OnDestroy()
        {
            Ami.BroAudio.BroAudio.OnBGMChanged -= OnBGMChanged;
        }

        private void OnBGMChanged(IAudioPlayer player)
        {
            if(player == null)
            {
                return;
            }

            if(!player.IsPlaying)
            {
                player.OnStart(SetClipName);
            }
            else
            {
                SetClipName(player);
            }
            
        }

        private void SetClipName(IAudioPlayer player)
        {
            _title.text = player.AudioSource.clip.name;
        }
    }
}