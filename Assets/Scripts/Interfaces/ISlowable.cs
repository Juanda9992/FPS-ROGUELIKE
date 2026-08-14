public interface ISlowable
{
    void ApplySlowEffect(float duration, float strength);
    void RemoveSlowEffect();
}