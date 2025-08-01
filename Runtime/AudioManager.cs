/*
 * Author: 吴雁涛(Yantao Wu)
 * Date: 2025-7-31
 * Description: 声音管理器
 */

using Cysharp.Threading.Tasks;
using KuonTools.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace KuonTools.Audio
{
    /// <summary>
    /// 声音管理器
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        /// <summary>
        /// 音量
        /// </summary>
        public float Volume { get; private set; }

        /// <summary>
        /// 静音
        /// </summary>
        public bool Mute { get; private set; }

        /// <summary>
        /// 暂停
        /// </summary>
        public bool IsPause { get; private set; }

        /// <summary>
        /// 声音播放器游戏对象池
        /// </summary>
        public ObjectPool<AudioController> Pool;

        /// <summary>
        /// 正在播放的控制器
        /// </summary>
        public List<AudioController> Controllers;

        /// <summary>
        /// awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Volume = PlayerPrefs.GetFloat("KuonAudioVolume", 0.5f);
            Mute = PlayerPrefs.GetInt("KuonAudioMute", 0) == 1;
            Pool = new ObjectPool<AudioController>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 10);
        }

        /// <summary>
        /// On Destroy
        /// </summary>
        protected override void OnDestroy()
        {
            PlayerPrefs.SetFloat("KuonAudioVolume", Volume);
            PlayerPrefs.SetInt("KuonAudioMute", Mute ? 1 : 0);
            base.OnDestroy();
        }

        #region 对象池相关

        private AudioController CreatePooledItem()
        {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            AudioSource audio = go.AddComponent<AudioSource>();
            AudioController controller = go.AddComponent<AudioController>();
            controller.Init(audio, this);
            return controller;
        }

        void OnReturnedToPool(AudioController controller)
        {
            controller.Stop();
            controller.gameObject.SetActive(false);
        }

        void OnTakeFromPool(AudioController controller)
        {
            controller.gameObject.SetActive(true);
        }

        void OnDestroyPoolObject(AudioController controller)
        {
            Destroy(controller.gameObject);
        }

        #endregion

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="clip">声音源</param>
        public void Play(AudioClip clip)
        {
            AudioController controller = Pool.Get();
            controller.PlayAsync(clip).Forget();
        }

        /// <summary>
        /// 异步播放声音
        /// </summary>
        /// <param name="clip">声音源</param>
        /// <returns>UniTask</returns>
        public async UniTask PlayAsync(AudioClip clip)
        {
            AudioController controller = Pool.Get();
            await controller.PlayAsync(clip);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="clip">声音源</param>
        /// <param name="key">关键字，用于停止声音</param>
        /// <param name="loop">是否循环，默认循环播放</param>
        public void Play(AudioClip clip, string key, bool loop = true)
        {
            Stop(key);
            AudioController controller = Pool.Get();
            controller.PlayAsync(clip, key, loop).Forget();
        }

        /// <summary>
        /// 停止声音
        /// </summary>
        /// <param name="key">关键字</param>
        public void Stop(string key)
        {
            var list = Controllers.Where(x => x.Key.Equals(key));
            foreach (var controller in list)
            {
                controller.Stop();
            }
        }

        /// <summary>
        /// 停止所有声音
        /// </summary>
        public void StopAll()
        {
            foreach (var controller in Controllers)
            {
                controller.Stop();
            }
        }

        /// <summary>
        /// 暂停所有声音
        /// </summary>
        public void PauseAll()
        {
            IsPause = true;
            foreach (var controller in Controllers)
            {
                controller.Pause();
            }
        }

        /// <summary>
        /// 恢复所有声音播放
        /// </summary>
        public void UnPauseAll()
        {
            IsPause = false;
            foreach (var controller in Controllers)
            {
                controller.UnPause();
            }
        }

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="volume">音量</param>
        public void SetVolume(float volume)
        {
            foreach (var controller in Controllers)
            {
                controller.SetVolume(volume);
            }
        }

        /// <summary>
        /// 设置静音
        /// </summary>
        /// <param name="mute">静音</param>
        public void SetMute(bool mute)
        {
            foreach (var controller in Controllers)
            {
                controller.SetMute(mute);
            }
        }
    }
}