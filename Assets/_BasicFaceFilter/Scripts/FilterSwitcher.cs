using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARFaceManager))]
public class FilterSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class FilterProp
    {
        public GameObject prefab;
        public Vector3 positionOffset = Vector3.zero;
        public Vector3 rotationOffset = Vector3.zero;
        public float scale = 1f;
    }

    [System.Serializable]
    public class Filter
    {
        public string name;
        public List<FilterProp> props = new();
    }

    [Tooltip("Themed filters. Each is a name + a list of prop prefabs (with per-prop position/rotation/scale offsets).")]
    public List<Filter> filters = new();

    [Tooltip("Index of the filter selected at startup. -1 = no filter (clean face).")]
    public int initialFilterIndex = 0;

    ARFaceManager m_FaceManager;
    int m_CurrentIndex = -1;
    readonly List<GameObject> m_SpawnedProps = new();
    ARFace m_ActiveFace;

    void Awake()
    {
        m_FaceManager = GetComponent<ARFaceManager>();
        Debug.Log($"FilterSwitcher: Awake. Filters configured: {filters?.Count ?? 0}");
    }

    void OnEnable()
    {
        m_FaceManager.trackablesChanged.AddListener(OnFacesChanged);
        m_CurrentIndex = initialFilterIndex;

        foreach (var face in m_FaceManager.trackables)
        {
            m_ActiveFace = face;
            ApplyCurrentFilter();
            break;
        }
    }

    void OnDisable() => m_FaceManager.trackablesChanged.RemoveListener(OnFacesChanged);

    void OnFacesChanged(ARTrackablesChangedEventArgs<ARFace> args)
    {
        foreach (var face in args.added)
        {
            m_ActiveFace = face;
            ApplyCurrentFilter();
        }
        foreach (var face in args.removed)
        {
            if (m_ActiveFace == face.Value) { ClearProps(); m_ActiveFace = null; }
        }
    }

    public void SetFilter(int index)
    {
        m_CurrentIndex = index;
        ApplyCurrentFilter();
    }

    void ApplyCurrentFilter()
    {
        ClearProps();
        if (m_ActiveFace == null) return;
        if (m_CurrentIndex < 0 || m_CurrentIndex >= filters.Count) return;

        var filter = filters[m_CurrentIndex];
        if (filter?.props == null) return;
        foreach (var p in filter.props)
        {
            if (p?.prefab == null) continue;
            var instance = Instantiate(p.prefab, m_ActiveFace.transform);
            instance.transform.localPosition = p.positionOffset;
            instance.transform.localRotation = Quaternion.Euler(p.rotationOffset);
            instance.transform.localScale = Vector3.one * Mathf.Max(0.0001f, p.scale);
            m_SpawnedProps.Add(instance);
        }
    }

    void ClearProps()
    {
        foreach (var p in m_SpawnedProps)
            if (p != null) Destroy(p);
        m_SpawnedProps.Clear();
    }

    GUIStyle m_BtnStyle;

    void OnGUI()
    {
        if (filters == null || filters.Count == 0) return;

        if (m_BtnStyle == null)
        {
            m_BtnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(Screen.height * 0.025f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
        }

        int count = filters.Count + 1;
        float pad = Screen.width * 0.02f;
        float btnW = (Screen.width - pad * (count + 1)) / count;
        float btnH = Screen.height * 0.08f;
        float y = Screen.height - btnH - pad;

        for (int i = 0; i < filters.Count; i++)
        {
            float x = pad + i * (btnW + pad);
            string label = filters[i].name;
            if (i == m_CurrentIndex) label = "▶ " + label;
            if (GUI.Button(new Rect(x, y, btnW, btnH), label, m_BtnStyle))
                SetFilter(i);
        }

        float clearX = pad + filters.Count * (btnW + pad);
        string clearLabel = m_CurrentIndex == -1 ? "▶ Clear" : "Clear";
        if (GUI.Button(new Rect(clearX, y, btnW, btnH), clearLabel, m_BtnStyle))
            SetFilter(-1);
    }
}
