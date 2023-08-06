using System;
using System.Collections.Generic;
using System.IO;
using DigitalHuarongRoad.Share;
using LFrame.Core.Tools;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.UI.Image;

namespace LFrame.Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        public static AudioManager Instance
        {
            get
            {
                return instance;
            }
        }

        private static Transform root;

        private IObjectPool<AudioItem> pool;

        private Dictionary<string, AudioClip> audioClipDict;

        private List<AudioItem> playingAudio;
        
        public float MaxMusicVolume
        {
            get
            {
              return  Mathf.Clamp01(PlayerInfoModule.MusicVolume);
            }
           
        }
        
        private float maxClipVolume;
        public float MaxClipVolume
        {
            get
            {
                return maxClipVolume;
            }
            set
            {
                maxClipVolume = Mathf.Clamp01(value);
            }
        }
        
        public static  void Initialise(Transform rootTransform)
        {
            root = rootTransform;
            root.gameObject.AddComponent<AudioManager>();
        }

        private AudioItem OnPoolItemCreate()
        {
            var go = new GameObject("AudioItem PoolItem");
            go.transform.SetParent(root);
            go.AddComponent<AudioSource>();
            var audioItem = go.AddComponent<AudioItem>();
            playingAudio.Add(audioItem);
            return audioItem;
        }

        private void OnPoolItemTake(AudioItem audioItem)
        {
            playingAudio.Add(audioItem);
            Helper.SetActiveState(audioItem, true);
        }

        private void OnPoolItemReturn(AudioItem audioItem)
        {
            playingAudio.Remove(audioItem);
            audioItem.ResetStatus();
            Helper.SetActiveState(audioItem, false);
        }

        private void OnPoolItemDestroy(AudioItem audioItem)
        {
            playingAudio.Remove(audioItem);
            Destroy(audioItem.gameObject);
        }

        public void ReturnToPool(AudioItem audioItem)
        {
            pool.Release(audioItem);
        }

        private void Awake()
        {
            instance = this;
            audioClipDict = new Dictionary<string, AudioClip>();
            pool = new LinkedPool<AudioItem>(OnPoolItemCreate, OnPoolItemTake, OnPoolItemReturn, OnPoolItemDestroy,
                true, 50);
            playingAudio = new List<AudioItem>(50);

            var allAudio = Resources.LoadAll<AudioClip>(Constants.AUDIO_PATH);
            if (allAudio != null)
            {
                foreach (var audioClip in allAudio)
                {
                    audioClipDict.Add(audioClip.name, audioClip);
                }
            }
        }


        private AudioClip GetClip(string name)
        {
            if (audioClipDict.ContainsKey(name))
            {
                return audioClipDict[name];
            }

            Helper.LogError($"不存在名为{name}的AudioClip");
            return null;
        }


        public void PlayClip(string audioName, Vector3 origin, bool loop, Single volume, float pitch)
        {
            AudioClip audioClip = GetClip(audioName);
            if (audioClip != null)
            {
                var audioItem = pool.Get();
                audioItem.PlayClip(audioClip, origin, loop, volume, pitch);
            }
        }

        public void PlayClip(string audioName, Vector3 origin)
        {
            PlayClip(audioName, origin, false, Single.MaxValue, 1);
        }
        public void PlayClip(string audioName)
        {
            PlayClip(audioName, Camera.main.transform.position, false, Single.MaxValue, 1);
        }


        public void PlayMusic(string[] audioNames)
        {
            List<AudioClip> audioClips = new List<AudioClip>(audioNames.Length);
            for (int i = 0; i < audioNames.Length; i++)
            {
                var tempClip = GetClip(audioNames[i]);
                if (tempClip == null) continue;
                audioClips.Add(tempClip);
            }

            if (audioClips.Count != 0)
            {
                var audioItem = pool.Get();
                audioItem.PlayMusic(audioClips.ToArray());
            }
        }


        public void StopPlay(string audioName)
        {
            for (int i = 0; i < playingAudio.Count; i++)
            {
                if(playingAudio[0] == null ) continue;
                if (playingAudio[0].CurPlayAudio.name.Equals(audioName))
                {
                    playingAudio[0].Stop();
                    return;
                }
                
            }
        }

        public void StopAll()
        {
            for (int i = 0; i < playingAudio.Count; i++)
            {
                if(playingAudio[i] == null ) continue;
                playingAudio[i].Stop();
            }
        }
        

        public void RemoveFromPlayingAudio(AudioItem item)
        {
            playingAudio.Remove(item);
        }
    }
}
