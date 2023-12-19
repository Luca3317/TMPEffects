using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System;
using UnityEditor;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

public struct CharData
{
    public VisibilityState visibilityState
    {
        get => _visibilityState;
    }
    public float stateTime => _stateTime;
    public float visibleTime => _visibleTime;

    public bool positionDirty => position != info.initialPosition;
    public bool rotationDirty => rotation != info.initialRotation;
    public bool scaleDirty => scale != Info.defaultScale;

    public Vector3 Position => position;
    public Vector3 Scale => scale;
    public Quaternion Rotation => rotation;
    public Vector3 RotationPivot => pivot;

    #region Fields
    public readonly Info info;
    public int segmentIndex;
    public VertexData currentMesh;

    private VisibilityState _visibilityState;
    private float _stateTime;
    private float _visibleTime;

    private Vector3 position;
    private Vector3 scale;
    private Quaternion rotation;
    private Vector3 pivot;
    #endregion

    public CharData(TMP_CharacterInfo cInfo)
    {
        info = new Info(cInfo);

        position = info.initialPosition;
        rotation = info.initialRotation;
        scale = info.initialScale;
        pivot = info.initialPosition;

        VertexData vData = new VertexData(cInfo);
        currentMesh = vData;

        _stateTime = -1;
        _visibleTime = -1;
        _visibilityState = default;

        segmentIndex = -1;

        SetVisibilityState(VisibilityState.Shown, -1);
    }
    public CharData(TMP_CharacterInfo cInfo, TMP_WordInfo? wInfo = null)
    {
        info = wInfo == null ? new Info(cInfo) : new Info(cInfo, wInfo.Value);

        position = info.initialPosition;
        rotation = info.initialRotation;
        scale = info.initialScale;
        pivot = info.initialPosition;

        VertexData vData = new VertexData(cInfo);
        currentMesh = vData;

        _stateTime = -1;
        _visibleTime = -1;
        _visibilityState = default;

        segmentIndex = -1;

        SetVisibilityState(VisibilityState.Shown, -1);
    }

    public void SetVisibilityState(VisibilityState newState, float currentTime)
    {
        _stateTime = currentTime;
        if (_visibilityState == VisibilityState.Hidden && newState != VisibilityState.Hidden)
        {
            //Debug.Log("Updating my time to " + currentTime + "!");
            _visibleTime = _stateTime;
        }
        _visibilityState = newState;
    }


    /// <summary>
    /// Set the position of the vertex at the given index.
    /// </summary>
    /// <param name="index">The index of the vertex.</param>
    /// <param name="position">The new position of the vertex.</param>
    public void SetVertex(int index, Vector3 position)
    {
        currentMesh.SetPosition(index, position);
    }

    /// <summary>
    /// Add a positon delta to the vertex at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="delta"></param> 
    public void AddVertexDelta(int index, Vector3 delta)
    {
        currentMesh.SetPosition(index, currentMesh.GetPosition(index) + delta);
    }

    /// <summary>
    /// Set the position of the character.
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(Vector3 position)
    {
        this.position = position;
    }

    /// <summary>
    /// Add a delta to the position of the character.
    /// </summary>
    /// <param name="delta"></param>
    public void AddPositionDelta(Vector3 delta)
    {
        position += delta;
    }

    /// <summary>
    /// Set the pivot of this character. Note that the pivot is independent of the character's position, scale and rotation.
    /// </summary>
    /// <param name="pivot"></param>
    public void SetPivot(Vector3 pivot)
    {
        this.pivot = pivot;
    }
    /// <summary>
    /// Add a delta to the pivot of the character. Note that the pivot is independent of the character's position, rotation and scale.
    /// </summary>
    /// <param name="delta"></param>
    public void AddPivotDelta(Vector3 delta)
    {
        pivot += delta;
    }


    /// <summary>
    /// Set the rotation of this character.
    /// </summary>
    /// <param name="rotation">The new rotation of this character.</param>
    public void SetRotation(Quaternion rotation)
    {
        this.rotation = rotation;
        this.pivot = info.initialPosition;
    }
    /// <summary>
    /// Set the scale of this character. 
    /// </summary>
    /// <param name="scale">The new scale of this character.</param>
    public void SetScale(Vector3 scale)
    {
        this.scale = scale;
    }

    public void Reset()
    {
        ResetPosition();
        ResetRotation();
        ResetScale();
        ResetPivot();
        ResetVertices();
    }
    /// <summary>
    /// Reset the character's scale.
    /// </summary>
    public void ResetScale() => this.scale = info.initialScale;
    /// <summary>
    /// Reset the character's position.
    /// </summary>
    public void ResetPosition() => this.position = info.initialPosition;
    /// <summary>
    /// Reset the character's rotation.
    /// </summary>
    public void ResetRotation() => this.rotation = info.initialRotation;
    /// <summary>
    /// Reset the character's pivot.
    /// </summary>
    public void ResetPivot() => this.pivot = info.initialPosition;
    public void ResetVertices()
    {
        currentMesh.vertex_BL = info.initialMesh.vertex_BL;
        currentMesh.vertex_TL = info.initialMesh.vertex_TL;
        currentMesh.vertex_TR = info.initialMesh.vertex_TR;
        currentMesh.vertex_BR = info.initialMesh.vertex_BR;
    }

    public struct Info
    {
        public Vector3 initialScale => defaultScale;
        public static readonly Vector3 defaultScale = new Vector3(1, 1, 1);

        public readonly int index;
        public readonly int wordFirstIndex;
        public readonly int wordLen;

        public readonly float pointSize;

        public readonly char character;

        public readonly bool isVisible;

        public readonly int lineNumber;
        public readonly int pageNumber;

        public readonly float ascender;
        public readonly float descender;
        public readonly float xAdvance;

        public readonly Vector3 initialPosition;
        public readonly Quaternion initialRotation;
        public readonly float referenceScale;
        public readonly ReadOnlyVertexData initialMesh;

        public Info(TMP_CharacterInfo cInfo)
        {
            ReadOnlyVertexData vData = new ReadOnlyVertexData(cInfo);
            initialMesh = vData;

            index = cInfo.index;
            isVisible = cInfo.isVisible;

            wordFirstIndex = -1;
            wordLen = -1;

            lineNumber = cInfo.lineNumber;
            pageNumber = cInfo.pageNumber;

            pointSize = cInfo.pointSize;
            character = cInfo.character;

            ascender = cInfo.ascender;
            descender = cInfo.descender;
            xAdvance = cInfo.xAdvance;

            referenceScale = cInfo.scale;
            initialPosition = default;
            initialRotation = Quaternion.identity;

            initialPosition = GetCenter(ref initialMesh);
        }

        public Info(TMP_CharacterInfo cInfo, TMP_WordInfo wInfo) : this(cInfo)
        {
            wordFirstIndex = wInfo.firstCharacterIndex;
            wordLen = wInfo.characterCount;
        }

        private Vector3 GetCenter(ref ReadOnlyVertexData data)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < 4; i++)
            {
                center += data.GetPosition(i);
            }
            return center / 4;
        }
    }

    public struct VertexData
    {
        public TMP_Vertex vertex_TL;
        public TMP_Vertex vertex_TR;
        public TMP_Vertex vertex_BR;
        public TMP_Vertex vertex_BL;

        public TMP_Vertex this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return vertex_BL;
                    case 1: return vertex_TL;
                    case 2: return vertex_TR;
                    case 3: return vertex_BR;
                    default: throw new System.ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (i)
                {
                    case 0: vertex_BL = value; break;
                    case 1: vertex_TL = value; break;
                    case 2: vertex_TR = value; break;
                    case 3: vertex_BR = value; break;
                    default: throw new System.ArgumentOutOfRangeException();
                }
            }
        }

        public VertexData(TMP_Vertex bl, TMP_Vertex tl, TMP_Vertex tr, TMP_Vertex br)
        {
            this.vertex_BL = bl;
            this.vertex_TL = tl;
            this.vertex_TR = tr;
            this.vertex_BR = br;
        }

        public VertexData(TMP_CharacterInfo info)
        {
            this.vertex_BL = info.vertex_BL;
            this.vertex_TL = info.vertex_TL;
            this.vertex_TR = info.vertex_TR;
            this.vertex_BR = info.vertex_BR;
        }

        public Vector3 GetPosition(int i)
        {
            switch (i)
            {
                case 0: return vertex_BL.position;
                case 1: return vertex_TL.position;
                case 2: return vertex_TR.position;
                case 3: return vertex_BR.position;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public void SetPosition(int i, Vector3 value)
        {
            switch (i)
            {
                case 0: vertex_BL.position = value; break;
                case 1: vertex_TL.position = value; break;
                case 2: vertex_TR.position = value; break;
                case 3: vertex_BR.position = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public Color32 GetColor(int i)
        {
            switch (i)
            {
                case 0: return vertex_BL.color;
                case 1: return vertex_TL.color;
                case 2: return vertex_TR.color;
                case 3: return vertex_BR.color;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public void SetColor(int i, Color32 value)
        {
            switch (i)
            {
                case 0: vertex_BL.color = value; break;
                case 1: vertex_TL.color = value; break;
                case 2: vertex_TR.color = value; break;
                case 3: vertex_BR.color = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public Vector2 GetUV0(int i)
        {
            switch (i)
            {
                case 0: return vertex_BL.uv;
                case 1: return vertex_TL.uv;
                case 2: return vertex_TR.uv;
                case 3: return vertex_BR.uv;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public void SetUV(int i, Vector2 value)
        {
            switch (i)
            {
                case 0: vertex_BL.uv = value; break;
                case 1: vertex_TL.uv = value; break;
                case 2: vertex_TR.uv = value; break;
                case 3: vertex_BR.uv = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public Vector2 GetUV2(int i)
        {
            switch (i)
            {
                case 0: return vertex_BL.uv2;
                case 1: return vertex_TL.uv2;
                case 2: return vertex_TR.uv2;
                case 3: return vertex_BR.uv2;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public void SetUV2(int i, Vector2 value)
        {
            switch (i)
            {
                case 0: vertex_BL.uv2 = value; break;
                case 1: vertex_TL.uv2 = value; break;
                case 2: vertex_TR.uv2 = value; break;
                case 3: vertex_BR.uv2 = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public Vector2 GetUV4(int i)
        {
            switch (i)
            {
                case 0: return vertex_BL.uv4;
                case 1: return vertex_TL.uv4;
                case 2: return vertex_TR.uv4;
                case 3: return vertex_BR.uv4;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public void SetUV4(int i, Vector2 value)
        {
            switch (i)
            {
                case 0: vertex_BL.uv4 = value; break;
                case 1: vertex_TL.uv4 = value; break;
                case 2: vertex_TR.uv4 = value; break;
                case 3: vertex_BR.uv4 = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }
    }

    public struct ReadOnlyVertexData
    {
        public TMP_Vertex vertex_TL => _vertex_TL;
        public TMP_Vertex vertex_TR => _vertex_TR;
        public TMP_Vertex vertex_BL => _vertex_BL;
        public TMP_Vertex vertex_BR => _vertex_BR;

        private TMP_Vertex _vertex_TL;
        private TMP_Vertex _vertex_TR;
        private TMP_Vertex _vertex_BR;
        private TMP_Vertex _vertex_BL;

        public TMP_Vertex this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return _vertex_BL;
                    case 1: return _vertex_TL;
                    case 2: return _vertex_TR;
                    case 3: return _vertex_BR;
                    default: throw new System.ArgumentOutOfRangeException();
                }
            }
        }

        public ReadOnlyVertexData(TMP_Vertex bl, TMP_Vertex tl, TMP_Vertex tr, TMP_Vertex br)
        {
            this._vertex_BL = bl;
            this._vertex_TL = tl;
            this._vertex_TR = tr;
            this._vertex_BR = br;
        }

        public ReadOnlyVertexData(TMP_CharacterInfo info)
        {
            this._vertex_BL = info.vertex_BL;
            this._vertex_TL = info.vertex_TL;
            this._vertex_TR = info.vertex_TR;
            this._vertex_BR = info.vertex_BR;
        }

        public Vector3 GetPosition(int i)
        {
            switch (i)
            {
                case 0: return _vertex_BL.position;
                case 1: return _vertex_TL.position;
                case 2: return _vertex_TR.position;
                case 3: return _vertex_BR.position;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public Color32 GetColor(int i)
        {
            switch (i)
            {
                case 0: return _vertex_BL.color;
                case 1: return _vertex_TL.color;
                case 2: return _vertex_TR.color;
                case 3: return _vertex_BR.color;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public Vector2 GetUV0(int i)
        {
            switch (i)
            {
                case 0: return _vertex_BL.uv;
                case 1: return _vertex_TL.uv;
                case 2: return _vertex_TR.uv;
                case 3: return _vertex_BR.uv;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public Vector2 GetUV2(int i)
        {
            switch (i)
            {
                case 0: return _vertex_BL.uv2;
                case 1: return _vertex_TL.uv2;
                case 2: return _vertex_TR.uv2;
                case 3: return _vertex_BR.uv2;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public Vector2 GetUV4(int i)
        {
            switch (i)
            {
                case 0: return _vertex_BL.uv4;
                case 1: return _vertex_TL.uv4;
                case 2: return _vertex_TR.uv4;
                case 3: return _vertex_BR.uv4;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }
    }

    public enum VisibilityState : int
    {
        Shown = 10,
        Hidden = -10,
        ShowAnimation = 5,
        HideAnimation = -5
    }
}

//public struct CharDataOld
//{
//    public static readonly Vector3 defaultScale = new Vector3(1, 1, 1);
//    public Vector3 initialScale => defaultScale;

//    public bool positionDirty => position != initialPosition;
//    public bool rotationDirty => rotation != initialRotation;
//    public bool scaleDirty => scale != defaultScale;

//    #region CharacterInfo
//    public VertexData initialMesh;
//    public VertexData currentMesh;

//    public readonly int index;
//    public readonly int wordFirstIndex;
//    public readonly int wordLen;

//    public readonly float pointSize;
//    //public readonly float scale;

//    public readonly char character;

//    public readonly bool isVisible;

//    public readonly int lineNumber;
//    public readonly int pageNumber;

//    public readonly float ascender;
//    public readonly float descender;
//    public readonly float xAdvance;
//    // public readonly Color32 baseColor;
//    #endregion

//    public Vector3 position;
//    public readonly Vector3 initialPosition;
//    public Quaternion rotation;
//    public readonly Quaternion initialRotation;
//    public Vector3 scale;
//    public readonly float referenceScale;

//    public Vector3 rotationPivot;


//    // This data would be made redundant if passing with Span
//    public int segmentIndex;
//    public int segmentLength;

//    public VisibilityState visibilityState
//    {
//        get => _visibilityState;
//        set
//        {
//            _stateTime = Time.time; // TODO appropriate way to get time?
//            if (_visibilityState == VisibilityState.Hidden && value != VisibilityState.Hidden) _visibleTime = _stateTime;
//            _visibilityState = value;
//        }
//    }
//    public float stateTime => _stateTime;
//    public float visibleTime => _visibleTime;
//    private VisibilityState _visibilityState;
//    private float _stateTime;
//    private float _visibleTime;
//    //public bool hidden;

//    public CharDataOld(TMP_CharacterInfo characterInfo, TMP_WordInfo wordInfo) : this(characterInfo)
//    {
//        this.wordFirstIndex = wordInfo.firstCharacterIndex;
//        this.wordLen = wordInfo.characterCount;
//    }

//    public CharDataOld(TMP_CharacterInfo characterInfo)
//    {
//        VertexData vData = new VertexData(characterInfo);
//        initialMesh = vData;
//        currentMesh = vData;

//        index = characterInfo.index;
//        isVisible = characterInfo.isVisible;
//        this.wordFirstIndex = -1;
//        this.wordLen = -1;

//        lineNumber = characterInfo.lineNumber;
//        pageNumber = characterInfo.pageNumber;

//        segmentIndex = -1;
//        segmentLength = -1;

//        pointSize = characterInfo.pointSize;
//        character = characterInfo.character;

//        rotationPivot = default;
//        initialPosition = default;
//        position = default;

//        referenceScale = characterInfo.scale;
//        scale = Vector3.one;

//        initialRotation = Quaternion.identity;
//        rotation = initialRotation;

//        ascender = characterInfo.ascender;
//        descender = characterInfo.descender;
//        xAdvance = characterInfo.xAdvance;

//        _visibilityState = VisibilityState.Shown;
//        _stateTime = -1;
//        _visibleTime = -1;
//        visibilityState = VisibilityState.Shown;

//        initialPosition = GetCenter(ref initialMesh);
//        position = initialPosition;
//        rotationPivot = initialPosition;
//    }

//    private Vector3 GetCenter(ref VertexData data)
//    {
//        Vector3 center = Vector3.zero;
//        for (int i = 0; i < 4; i++)
//        {
//            center += data.GetPosition(i);
//        }
//        return center / 4;
//    }

//    public enum VisibilityState : int
//    {
//        Shown = 10,
//        Hidden = -10,
//        ShowAnimation = 5,
//        HideAnimation = -5
//    }
//    public struct VertexData
//    {
//        public TMP_Vertex vertex_TL;
//        public TMP_Vertex vertex_TR;
//        public TMP_Vertex vertex_BR;
//        public TMP_Vertex vertex_BL;

//        public TMP_Vertex this[int i]
//        {
//            get
//            {
//                switch (i)
//                {
//                    case 0: return vertex_BL;
//                    case 1: return vertex_TL;
//                    case 2: return vertex_TR;
//                    case 3: return vertex_BR;
//                    default: throw new System.ArgumentOutOfRangeException();
//                }
//            }
//            set
//            {
//                switch (i)
//                {
//                    case 0: vertex_BL = value; break;
//                    case 1: vertex_TL = value; break;
//                    case 2: vertex_TR = value; break;
//                    case 3: vertex_BR = value; break;
//                    default: throw new System.ArgumentOutOfRangeException();
//                }
//            }
//        }

//        public VertexData(TMP_Vertex bl, TMP_Vertex tl, TMP_Vertex tr, TMP_Vertex br)
//        {
//            this.vertex_BL = bl;
//            this.vertex_TL = tl;
//            this.vertex_TR = tr;
//            this.vertex_BR = br;
//        }

//        public VertexData(TMP_CharacterInfo info)
//        {
//            this.vertex_BL = info.vertex_BL;
//            this.vertex_TL = info.vertex_TL;
//            this.vertex_TR = info.vertex_TR;
//            this.vertex_BR = info.vertex_BR;
//        }

//        public Vector3 GetPosition(int i)
//        {
//            switch (i)
//            {
//                case 0: return vertex_BL.position;
//                case 1: return vertex_TL.position;
//                case 2: return vertex_TR.position;
//                case 3: return vertex_BR.position;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }

//        public void SetPosition(int i, Vector3 value)
//        {
//            switch (i)
//            {
//                case 0: vertex_BL.position = value; break;
//                case 1: vertex_TL.position = value; break;
//                case 2: vertex_TR.position = value; break;
//                case 3: vertex_BR.position = value; break;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }

//        public Color32 GetColor(int i)
//        {
//            switch (i)
//            {
//                case 0: return vertex_BL.color;
//                case 1: return vertex_TL.color;
//                case 2: return vertex_TR.color;
//                case 3: return vertex_BR.color;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }

//        public void SetColor(int i, Color32 value)
//        {
//            switch (i)
//            {
//                case 0: vertex_BL.color = value; break;
//                case 1: vertex_TL.color = value; break;
//                case 2: vertex_TR.color = value; break;
//                case 3: vertex_BR.color = value; break;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }

//        public Vector2 GetUV0(int i)
//        {
//            switch (i)
//            {
//                case 0: return vertex_BL.uv;
//                case 1: return vertex_TL.uv;
//                case 2: return vertex_TR.uv;
//                case 3: return vertex_BR.uv;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }

//        public void SetUV(int i, Vector2 value)
//        {
//            switch (i)
//            {
//                case 0: vertex_BL.uv = value; break;
//                case 1: vertex_TL.uv = value; break;
//                case 2: vertex_TR.uv = value; break;
//                case 3: vertex_BR.uv = value; break;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }

//        public Vector2 GetUV2(int i)
//        {
//            switch (i)
//            {
//                case 0: return vertex_BL.uv2;
//                case 1: return vertex_TL.uv2;
//                case 2: return vertex_TR.uv2;
//                case 3: return vertex_BR.uv2;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }

//        public void SetUV2(int i, Vector2 value)
//        {
//            switch (i)
//            {
//                case 0: vertex_BL.uv2 = value; break;
//                case 1: vertex_TL.uv2 = value; break;
//                case 2: vertex_TR.uv2 = value; break;
//                case 3: vertex_BR.uv2 = value; break;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }

//        public Vector2 GetUV4(int i)
//        {
//            switch (i)
//            {
//                case 0: return vertex_BL.uv4;
//                case 1: return vertex_TL.uv4;
//                case 2: return vertex_TR.uv4;
//                case 3: return vertex_BR.uv4;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }

//        public void SetUV4(int i, Vector2 value)
//        {
//            switch (i)
//            {
//                case 0: vertex_BL.uv4 = value; break;
//                case 1: vertex_TL.uv4 = value; break;
//                case 2: vertex_TR.uv4 = value; break;
//                case 3: vertex_BR.uv4 = value; break;
//                default: throw new System.ArgumentOutOfRangeException();
//            }
//        }
//    }
//}
