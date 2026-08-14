public interface IVulnerable
{
    void ApplyVulnerability(float percentage, float duration);
    void UndoVulnerability();
}
