using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public struct CharData
{
    #region CharacterInfo
    public VertexData initialMesh;
    public VertexData currentMesh;

    public readonly int index;
    public readonly int wordFirstIndex;
    public readonly int wordLen;

    public readonly float pointSize;
    public readonly float scale;

    public readonly char character;

    public readonly bool isVisible;

    public readonly int lineNumber;
    public readonly int pageNumber;

    // public readonly Color32 baseColor;
    #endregion

    // This data would be made redundant if passing with Span
    public int segmentIndex;
    public int segmentLength;

    public float passedTime;

    public VisibilityState visibilityState
    {
        get => _visibilityState;
        set
        {
            _stateTime = Time.time; // TODO appropriate way to get time?
            _visibilityState = value;
        }
    }
    public float stateTime => _stateTime;
    private VisibilityState _visibilityState;
    private float _stateTime;
    //public bool hidden;

    public CharData(TMP_CharacterInfo characterInfo, TMP_WordInfo wordInfo) : this(characterInfo)
    {
        this.wordFirstIndex = wordInfo.firstCharacterIndex;
        this.wordLen = wordInfo.characterCount;
    }

    public CharData(TMP_CharacterInfo characterInfo)
    {
        VertexData vData = new VertexData(characterInfo);
        initialMesh = vData;
        currentMesh = vData;

        index = characterInfo.index;
        isVisible = characterInfo.isVisible;
        this.wordFirstIndex = -1;
        this.wordLen = -1;

        lineNumber = characterInfo.lineNumber;
        pageNumber = characterInfo.pageNumber;

        segmentIndex = -1;
        segmentLength = -1;

        pointSize = characterInfo.pointSize;
        scale = characterInfo.scale;
        character = characterInfo.character;

        passedTime = 0;
        _visibilityState = VisibilityState.Shown;
        _stateTime = -1;
        visibilityState = VisibilityState.Shown;

    }

    public enum VisibilityState : int
    {
        Shown = 10,
        Hidden = -10,
        ShowAnimation = 5,
        HideAnimation = -5
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
}
