using System;
using System.Collections;
using System.Collections.Generic;
using DigitalHuarongRoad.Share;
using LFrame.Core.Event;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LFrame.Core.Audio
{
    public class AudioItem : MonoBehaviour
    {
        private AudioSource audioSource;
        private Coroutine recycleListenerCR;
        private Coroutine loopPlayMusicCR;
        private List<AudioClip> loopMusics;

        public AudioClip CurPlayAudio
        {
            get
            {
                return audioSource.clip;
            }
        }

        #region unity life

        public void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            loopMusics = new List<AudioClip>(20);
        }

        private void OnEnable()
        {
            EvtManager.RegEvt(GameEvents.MusicVolumeChange, MusicVolumeChangeHandle);
            EvtManager.RegEvt(GameEvents.SoundVolumeChange, SoundVolumeChangeHandle);

            recycleListenerCR = Loom.StartCR(RecycleListenerCR());
        }

        private void MusicVolumeChangeHandle(Evt evt)
        {
            if (loopMusics.Count != 0)
            {
                audioSource.volume = PlayerInfoModule.MusicVolume;
            }
        }

        private void SoundVolumeChangeHandle(Evt evt)
        {
            if (loopMusics.Count == 0)
            {
                audioSource.volume = PlayerInfoModule.SoundVolume;
            }
        }


        private void OnDisable()
        {
            EvtManager.UnRegEvt(GameEvents.MusicVolumeChange, MusicVolumeChangeHandle);
            EvtManager.UnRegEvt(GameEvents.SoundVolumeChange, SoundVolumeChangeHandle);
            Loom.StopCR(recycleListenerCR);
            recycleListenerCR = null;
        }

        private void OnDestroy()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.RemoveFromPlayingAudio(this);
        }

        #endregion

        private IEnumerator RecycleListenerCR()
        {
            float timer = 0;

            while (true)
            {
                if (audioSource.isPlaying == false)
                {
                    timer += Time.deltaTime;
                    if (timer > 0.1f)
                    {
                        AudioManager.Instance.ReturnToPool(this);
                        yield break;
                    }
                }
                else
                {
                    timer = 0;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 播放音乐片段
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="origin"></param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        public void PlayClip(AudioClip clip, Vector3 origin, bool loop, Single volume, float pitch)
        {
            ResetStatus();
            transform.position = origin;
            audioSource.loop = loop;
            audioSource.volume = PlayerInfoModule.SoundVolume;
            audioSource.pitch = pitch;
            audioSource.clip = clip;
            audioSource.Play();
        }

        /// <summary>
        /// 随机循环播放音乐
        /// </summary>
        /// <param name="loopAudioArray"></param>
        public void PlayMusic(AudioClip[] loopAudioArray)
        {
            ResetStatus();
            audioSource.volume = Mathf.Clamp(audioSource.volume, 0, AudioManager.Instance.MaxMusicVolume);
            audioSource.loop = false;
            if (loopPlayMusicCR != null)
            {
                Loom.StopCR(loopPlayMusicCR);
            }

            for (int i = 0; i < loopAudioArray.Length; i++)
            {
                loopMusics.Add(loopAudioArray[i]);
            }

            loopPlayMusicCR = Loom.StartCR(LoopPlayMusicCR());
        }

        private IEnumerator LoopPlayMusicCR()
        {
            while (true)
            {
                yield return new WaitUntil(() => audioSource.isPlaying == false);
                int randomIndex = -1;
                if (loopMusics.Count != 0)
                {
                    randomIndex = Random.Range(0, loopMusics.Count);
                }


                if (randomIndex != -1)
                {
                    audioSource.clip = loopMusics[randomIndex];
                    audioSource.Play();
                }
                else
                {
                    //播放列表中没有音乐时停止播放
                    Loom.StopCR(loopPlayMusicCR);
                    loopPlayMusicCR = null;
                    yield break;
                }


                yield return null;
            }
        }

        public void Stop()
        {
            if (loopPlayMusicCR != null)
            {
                Loom.StopCR(loopPlayMusicCR);
                loopPlayMusicCR = null;
            }

            loopMusics.Clear();
            if (audioSource.isPlaying)
                audioSource.Stop();
        }

        public void ResetStatus()
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            audioSource.clip = null;
            audioSource.loop = false;
            audioSource.volume = Single.MaxValue;
            audioSource.pitch = 1;

            if (loopPlayMusicCR != null)
            {
                Loom.StopCR(loopPlayMusicCR);
                loopPlayMusicCR = null;
            }

            loopMusics.Clear();
        }
    }
}
