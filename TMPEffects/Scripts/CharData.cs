using UnityEngine;
using TMPro;

public class CharData
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
    public bool verticesDirty => mesh.verticesDirty;
    public bool colorsDirty => mesh.colorsDirty;
    public bool uvsDirty => mesh.uvsDirty;

    public Vector3 Position => position;
    public Vector3 Scale => scale;
    public Quaternion Rotation => rotation;
    public Vector3 RotationPivot => pivot;

    #region Fields
    public readonly Info info;
    public int segmentIndex;
    public VertexData mesh;

    private VisibilityState _visibilityState;
    private float _stateTime;
    private float _visibleTime;

    private Vector3 position;
    private Vector3 scale;
    private Quaternion rotation;
    private Vector3 pivot;
    #endregion

    public CharData(int index, TMP_CharacterInfo cInfo)
    {
        VertexData vData = new VertexData(cInfo);
        mesh = vData; 
        info = new Info(index, cInfo, mesh);

        position = info.initialPosition;
        rotation = info.initialRotation;
        scale = info.initialScale;
        pivot = info.initialPosition;

        _stateTime = -1;
        _visibleTime = -1;
        _visibilityState = default;

        segmentIndex = -1;

        SetVisibilityState(VisibilityState.Shown, -1);
    }
    public CharData(int index, TMP_CharacterInfo cInfo, TMP_WordInfo? wInfo = null)
    {
        VertexData vData = new VertexData(cInfo);
        mesh = vData;
        info = wInfo == null ? new Info(index, cInfo, mesh) : new Info(index, cInfo, wInfo.Value, mesh);

        position = info.initialPosition;
        rotation = info.initialRotation;
        scale = info.initialScale;
        pivot = info.initialPosition;

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
        mesh.SetVertex(index, position);
    }

    /// <summary>
    /// Add a positon delta to the vertex at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="delta"></param> 
    public void AddVertexDelta(int index, Vector3 delta)
    {
        mesh.SetVertex(index, mesh.GetPosition(index) + delta);
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
        mesh.Reset();
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
    public void ResetVertices() => mesh.ResetVertices();
    public void ResetUVs() => mesh.ResetUVs();
    public void ResetColors() => mesh.ResetColors();

    public struct Info
    {
        public Vector3 initialScale => defaultScale;
        public static readonly Vector3 defaultScale = new Vector3(1, 1, 1);

        public readonly int index;
        public readonly int rawIndex;
        public readonly int wordFirstIndex;
        public readonly int wordLen;

        public readonly float pointSize;

        public readonly char character;

        public readonly bool isVisible;

        public readonly int lineNumber;
        public readonly int pageNumber;

        public readonly float baseLine;
        public readonly float ascender;
        public readonly float descender;
        public readonly float xAdvance;

        public readonly TMP_FontAsset fontAsset;

        public readonly Vector3 initialPosition;
        public readonly Quaternion initialRotation;
        public readonly float referenceScale;

        //public readonly ReadOnlyVertexData initialMesh;

        internal Info(int index, TMP_CharacterInfo cInfo, VertexData mesh)
        {
            this.index = index; /*cInfo.index;*/
            rawIndex = cInfo.index;
            isVisible = cInfo.isVisible;

            wordFirstIndex = -1;
            wordLen = -1;

            lineNumber = cInfo.lineNumber;
            pageNumber = cInfo.pageNumber;

            pointSize = cInfo.pointSize;
            character = cInfo.character;

            baseLine = cInfo.baseLine;
            ascender = cInfo.ascender;
            descender = cInfo.descender;
            xAdvance = cInfo.xAdvance;

            referenceScale = cInfo.scale;
            initialPosition = default;
            initialRotation = Quaternion.identity;

            fontAsset = cInfo.fontAsset;

            initialPosition = GetCenter(in mesh.initial);
        }

        public Info(int index, TMP_CharacterInfo cInfo, TMP_WordInfo wInfo, VertexData mesh) : this(index, cInfo, mesh)
        {
            wordFirstIndex = wInfo.firstCharacterIndex;
            wordLen = wInfo.characterCount;
        }

        private Vector3 GetCenter(in ReadOnlyVertexData data)
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

        public readonly ReadOnlyVertexData initial;

        public bool verticesDirty { get; private set; }
        public bool colorsDirty { get; private set; }
        public bool uvsDirty { get; private set; }

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

                verticesDirty = true;
            }
        }

        public VertexData(TMP_Vertex bl, TMP_Vertex tl, TMP_Vertex tr, TMP_Vertex br)
        {
            verticesDirty = false;
            uvsDirty = false;
            colorsDirty = false;

            initial = new ReadOnlyVertexData(bl, tl, tr, br);
            this.vertex_BL = bl;
            this.vertex_TL = tl;
            this.vertex_TR = tr;
            this.vertex_BR = br;
        }

        public VertexData(TMP_CharacterInfo info)
        {
            verticesDirty = false;
            uvsDirty = false;
            colorsDirty = false;

            initial = new ReadOnlyVertexData(info);
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

        public void SetVertex(int i, Vector3 value)
        {
            switch (i)
            {
                case 0: vertex_BL.position = value; break;
                case 1: vertex_TL.position = value; break;
                case 2: vertex_TR.position = value; break;
                case 3: vertex_BR.position = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            verticesDirty = true;
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

            colorsDirty = true;
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

            uvsDirty = true;
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

            uvsDirty = true;
        }

        public void Reset()
        {
            ResetColors();
            ResetVertices();
            ResetUVs();
        }

        public void ResetColors()
        {
            if (!colorsDirty) return;
            vertex_BL.color = initial.GetColor(0);
            vertex_TL.color = initial.GetColor(1);
            vertex_TR.color = initial.GetColor(2);
            vertex_BR.color = initial.GetColor(3);
            colorsDirty = false;
        }

        public void ResetVertices()
        {
            if (!verticesDirty) return;
            vertex_BL.position = initial.GetPosition(0);
            vertex_TL.position = initial.GetPosition(1);
            vertex_TR.position = initial.GetPosition(2);
            vertex_BR.position = initial.GetPosition(3);
            verticesDirty = false;
        }

        public void ResetUVs()
        {
            if (!uvsDirty) return;
            vertex_BL.uv = initial.GetUV0(0);
            vertex_TL.uv = initial.GetUV0(1);
            vertex_TR.uv = initial.GetUV0(2);
            vertex_BR.uv = initial.GetUV0(3);

            vertex_BL.uv2 = initial.GetUV2(0);
            vertex_TL.uv2 = initial.GetUV2(1);
            vertex_TR.uv2 = initial.GetUV2(2);
            vertex_BR.uv2 = initial.GetUV2(3);
            uvsDirty = false;
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