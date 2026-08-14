public interface IBlindable
{
    bool IsBlind { get; }
    void Blind(float duration);
    void UnBlind();
}
