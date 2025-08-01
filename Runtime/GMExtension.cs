/*
 * Author: 吴雁涛(Yantao Wu)
 * Date: 2025-7-31
 * Description: 统一入口扩展
 */
using KuonTools.Base;
using UnityEngine;

namespace KuonTools.Audio
{
    /// <summary>
    /// 统一入口扩展
    /// </summary>
    public static class GMExtension
    {
        /// <summary>
        /// 声音
        /// </summary>
        /// <param name="gm">统一入口</param>
        /// <returns>声音管理器</returns>
        public static AudioManager Audio(this GM gm)
        {
            if (AudioManager.Instance == null)
            {
                var go = new GameObject("AudioManager");
                return go.AddComponent<AudioManager>();
            }
            else
            {
                return AudioManager.Instance;
            }
        }
    }
}