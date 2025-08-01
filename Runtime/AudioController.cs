/*
 * Author: 吴雁涛(Yantao Wu)
 * Date: 2025-8-1
 * Description: 声音管控制器
 */

using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KuonTools.Audio
{
    /// <summary>
    /// 声音播放控制器
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {
        /// <summary>
        /// 关键字，用于停止播放
        /// </summary>
        public string Key;

        /// <summary>
        /// 播放器
        /// </summary>
        private AudioSource source;

        /// <summary>
        /// 管理器
        /// </summary>
        private AudioManager manager;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="audioSource">播放器</param>
        /// <param name="audioManager">管理器</param>
        public void Init(AudioSource audioSource, AudioManager audioManager)
        {
            manager = audioManager;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0;
            Key = string.Empty;
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="clip">声音源</param>
        /// <returns>UniTask</returns>
        public async UniTask PlayAsync(AudioClip clip)
        {
            source.clip = clip;
            source.volume = manager.Volume;
            source.mute = manager.Mute;
            source.Play();
            manager.Controllers.Add(this);
            await UniTask.WaitUntil(() => source.isPlaying == false && manager.IsPause == false);
            manager.Controllers.Remove(this);
            Key = string.Empty;
            manager.Pool.Release(this);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="clip">声音源</param>
        /// <param name="key">关键字，用于停止声音</param>
        /// <param name="loop">是否循环，默认循环播放</param>
        /// <returns>UniTask</returns>
        public async UniTask PlayAsync(AudioClip clip, string key, bool loop = true)
        {
            Key = key;
            source.loop = loop;
            await PlayAsync(clip);
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            source.Stop();
        }

        /// <summary>
        /// 暂停声音
        /// </summary>
        public void Pause()
        {
            source.Pause();
        }

        /// <summary>
        /// 恢复声音播放
        /// </summary>
        public void UnPause()
        {
            source.UnPause();
        }

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="volume">音量</param>
        public void SetVolume(float volume)
        {
            source.volume = volume;
        }

        /// <summary>
        /// 设置静音
        /// </summary>
        /// <param name="mute">静音</param>
        public void SetMute(bool mute)
        {
            source.mute = mute;
        }
    }
}