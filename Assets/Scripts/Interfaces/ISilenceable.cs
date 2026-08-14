public interface ISilenceable
{
    bool IsSilenced { get; }
    void Silence(float duration);
    void UnSilence();
}
