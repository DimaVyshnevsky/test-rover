using System.Collections.Generic;
using Core;
using Core.Services.Audio;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using UnityEngine;
using AudioType = Core.Services.Audio.AudioType;

namespace Application.Services.Audio
{
    public class AudioService : IAudioService
    {
        private readonly IAssetProvider _assetProvider;

        private Dictionary<string, AudioClip> _clips;

        public AudioService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask Initialize()
        {
            var clips = await _assetProvider.LoadByLabel<AudioClip>(ConstAudio.AudioLabel);
            _clips = new Dictionary<string, AudioClip>(clips.Count);

            foreach (var clip in clips)
                _clips.TryAdd(clip.name, clip);
        }

        public void Dispose()
        {
            _clips?.Clear();
        }

        public void PlayMusic(string clipId)
        {
            if (_clips.TryGetValue(clipId, out var clip))
                EazySoundManager.PlayMusic(clip);
        }

        public void PlayMusic(AudioClip clip)
        {
            EazySoundManager.PlayMusic(clip);
        }

        public void PlaySound(string clipId)
        {
            if (_clips.TryGetValue(clipId, out var clip))
                EazySoundManager.PlaySound(clip);
        }

        public void PlaySound(string clipId, bool loop)
        {
            if (_clips.TryGetValue(clipId, out var clip))
                EazySoundManager.PlaySound(clip, loop);
        }

        public void PauseAll()
        {
            EazySoundManager.PauseAll();
        }

        public void ResumeAll()
        {
            EazySoundManager.ResumeAll();
        }

        public void ResumeSounds()
        {
            EazySoundManager.ResumeAllSounds();
        }

        public bool IsPlaying(string clipId)
        {
            if (_clips.TryGetValue(clipId, out var clip))
                return EazySoundManager.IsPlaying(clip);

            return false;
        }

        public void StopMusic()
        {
            EazySoundManager.StopAllMusic();
        }

        public void StopAllSounds()
        {
            EazySoundManager.StopAllSounds();
        }

        public void StopAll()
        {
            EazySoundManager.StopAll();
        }

        public void SetVolume(AudioType audioType, float volume)
        {
            switch (audioType)
            {
                case AudioType.Music:
                    EazySoundManager.GlobalMusicVolume = volume;
                    break;
                case AudioType.Sound:
                    EazySoundManager.GlobalSoundsVolume = volume;
                    break;
                default:
                    throw new KeyNotFoundException($"{nameof(AudioService)}: {audioType} handler not found");
            }
        }
    }
}