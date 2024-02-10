using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public struct EffectTagTuple : IEquatable<EffectTagTuple>
{
    public readonly EffectTag Tag;
    public readonly EffectTagIndices Indices;

    public EffectTagTuple(EffectTag tag, EffectTagIndices indices)
    {
        Tag = tag;
        Indices = indices;
    }

    public bool Equals(EffectTagTuple other)
    {
        return Tag.Equals(other.Tag) && Indices.Equals(other.Indices);
    }
}

public sealed class EffectTag
{
    public string Name => name;
    public char Prefix => prefix;
    public ReadOnlyDictionary<string, string> Parameters => parameters;

    private readonly string name;
    private readonly char prefix;
    private readonly ReadOnlyDictionary<string, string> parameters;

    public EffectTag(string name, char prefix, IDictionary<string, string> parameters)
    {
        this.name = name;
        this.prefix = prefix;
        if (parameters == null)
            this.parameters = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        else
            this.parameters = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(parameters));
    }
}

/// <summary>
/// The indices of an <see cref="EffectTag"/>.
/// The indices can be regarded as a half-open interval of [<see cref="StartIndex"/>, <see cref="EndIndex"/>).
/// For example, an instance of this with <see cref="StartIndex"/> == 5 and <see cref="EndIndex"/> == 10 "contains" the indices 5, 6, 7, 8 and 9.
/// The decision to make the indices a half open interval was made to represent the indices that naturally emerge when parsing them from text.
/// </summary>
public struct EffectTagIndices : IComparable<EffectTagIndices>, IEquatable<EffectTagIndices>
{
    public int StartIndex => startIndex;
    public int EndIndex => endIndex;
    public int OrderAtIndex => orderAtIndex;

    public bool IsOpen => endIndex == -1;
    public int Length => IsOpen ? endIndex : endIndex - startIndex + 1;
    public bool Contains(int index) => index >= startIndex && index < endIndex;
    public IEnumerable<int> ContainedIndices
    {
        get
        {
            for (int i = startIndex; i < EndIndex; i++)
            {
                yield return i;
            }
        }
    }

    private int startIndex;
    private int endIndex;
    private int orderAtIndex;

    public EffectTagIndices(int startIndex, int endIndex, int orderAtIndex)
    {
        // TODO Make endindex = -1 representing open some constant or smth; also used ie in tagcollection and animator
        if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (endIndex < -1) throw new ArgumentOutOfRangeException(nameof(endIndex));

        this.startIndex = startIndex;
        this.endIndex = endIndex;
        this.orderAtIndex = orderAtIndex;
    }

    public int CompareTo(EffectTagIndices other)
    {
        int res = startIndex.CompareTo(other.startIndex);
        if (res == 0) return orderAtIndex.CompareTo(other.orderAtIndex);
        return res;
    }

    public static bool operator ==(EffectTagIndices c1, EffectTagIndices c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(EffectTagIndices c1, EffectTagIndices c2)
    {
        return !c1.Equals(c2);
    }

    public static bool operator >(EffectTagIndices c1, EffectTagIndices c2)
    {
        return c1.CompareTo(c2) > 0;
    }

    public static bool operator <(EffectTagIndices c1, EffectTagIndices c2)
    {
        return c1.CompareTo(c2) < 0;
    }

    public override bool Equals(object obj)
    {
        if (obj is EffectTagIndices)
        {
            return Equals((EffectTagIndices)obj);
        }
        return false;
    }
    public bool Equals(EffectTagIndices other)
    {
        return startIndex == other.startIndex && endIndex == other.endIndex && orderAtIndex == other.orderAtIndex;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(startIndex, endIndex, orderAtIndex);
    }
}