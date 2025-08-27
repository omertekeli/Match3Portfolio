using Match3.Scripts.Enums;

namespace Match3.Scripts.Core.Interfaces
{
    public interface IAudioManager: IService
    {
        void SetMusicEnabled(bool isEnabled);
        void SetSfxEnabled(bool isEnabled);
        void PlaySfx(SfxType audioType);
        bool IsMusicEnabled { get; }
        bool IsSfxEnabled { get; }
    }
}