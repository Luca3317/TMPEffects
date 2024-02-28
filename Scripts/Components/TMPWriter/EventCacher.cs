using TMPEffects.Components.Writer;
using TMPEffects.Tags;
using TMPEffects.TMPEvents;

namespace TMPEffects.Components.Writer
{
    internal class EventCacher : ITagCacher<CachedEvent>
    {
        private TMPEvent tmpEvent;
        //private IList<CharData> charData;

        public EventCacher(/*IList<CharData> charData,*/ TMPEvent tmpEvent)
        {
            //this.charData = charData;
            this.tmpEvent = tmpEvent;
        }

        public CachedEvent CacheTag(EffectTag tag, EffectTagIndices indices)
        {
            int endIndex = indices.StartIndex + 1;
            CachedEvent ce = new CachedEvent(new TMPEventArgs(tag, new EffectTagIndices(indices.StartIndex, endIndex, indices.OrderAtIndex)), tmpEvent);
            return ce;
        }
    }
}