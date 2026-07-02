using System.Collections;

namespace FeatherDetective
{
    public interface IMemoryEffect
    {
        BirdSpecies Species { get; }

        IEnumerator Play(FeatherDefinition feather, MemoryContext context);
    }
}
