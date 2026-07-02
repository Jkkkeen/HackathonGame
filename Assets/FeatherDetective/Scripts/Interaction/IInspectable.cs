namespace FeatherDetective
{
    public interface IInspectable
    {
        string PromptText { get; }
        void Inspect();
    }
}
