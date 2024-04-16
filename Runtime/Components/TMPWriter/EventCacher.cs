using TMPEffects.Components.Writer;
using TMPEffects.Tags;
using TMPEffects.TMPEvents;

namespace TMPEffects.Components.Writer
{
    internal class EventCacher : ITagCacher<CachedEvent>
    {
        private TMPEvent tmpEvent;
        private TMPWriter writer;
        //private IList<CharData> charData;

        public EventCacher(TMPWriter writer, TMPEvent tmpEvent)
        {
            //this.charData = charData;
            this.tmpEvent = tmpEvent;
            this.writer = writer;
        }

        public CachedEvent CacheTag(TMPEffectTag tag, TMPEffectTagIndices indices)
        {
            int endIndex = indices.StartIndex + 1;
            CachedEvent ce = new CachedEvent(new TMPEventArgs(tag, new TMPEffectTagIndices(indices.StartIndex, endIndex, indices.OrderAtIndex), writer), tmpEvent);
            return ce;
        }
    }
}