using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARFaceManager))]
public class FilterSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class Filter
    {
        public string name;
        public GameObject[] props;
    }

    [Tooltip("Themed filters. Each is a name + a list of prop prefabs to attach to the detected face.")]
    public List<Filter> filters = new();

    [Tooltip("Index of the filter selected at startup. -1 = no filter (clean face).")]
    public int initialFilterIndex = 0;

    ARFaceManager m_FaceManager;
    int m_CurrentIndex = -1;
    readonly List<GameObject> m_SpawnedProps = new();
    ARFace m_ActiveFace;

    void Awake() => m_FaceManager = GetComponent<ARFaceManager>();

    void OnEnable()
    {
        m_FaceManager.trackablesChanged.AddListener(OnFacesChanged);
        m_CurrentIndex = initialFilterIndex;
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
        foreach (var prefab in filter.props)
        {
            if (prefab == null) continue;
            var instance = Instantiate(prefab, m_ActiveFace.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
            m_SpawnedProps.Add(instance);
        }
    }

    void ClearProps()
    {
        foreach (var p in m_SpawnedProps)
            if (p != null) Destroy(p);
        m_SpawnedProps.Clear();
    }
}
