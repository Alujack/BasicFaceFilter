using System.Collections;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public static class FaceTrackingChooser
{
    sealed class Chooser : ConfigurationChooser
    {
        public override Configuration ChooseConfiguration(
            NativeSlice<ConfigurationDescriptor> descriptors,
            Feature requestedFeatures)
        {
            var sb = new StringBuilder("FaceTrackingChooser: requested=").Append(requestedFeatures);
            sb.Append("; descriptors=[");
            for (int i = 0; i < descriptors.Length; i++)
                sb.Append(i == 0 ? "" : ", ").Append(descriptors[i].capabilities);
            sb.Append("]");
            Debug.Log(sb.ToString());

            ConfigurationDescriptor best = default;
            int bestScore = int.MinValue;
            for (int i = 0; i < descriptors.Length; i++)
            {
                var d = descriptors[i];
                int score = 0;
                if (d.capabilities.Any(Feature.FaceTracking))      score += 1000;
                if (d.capabilities.Any(Feature.UserFacingCamera))  score += 500;
                if (d.capabilities.Any(Feature.WorldFacingCamera)) score -= 500;
                if (score > bestScore) { bestScore = score; best = d; }
            }

            var features = (Feature.UserFacingCamera | Feature.FaceTracking).Intersection(best.capabilities);
            Debug.Log($"FaceTrackingChooser: picked capabilities={best.capabilities}, features={features}");
            return new Configuration(best, features);
        }
    }

    sealed class Installer : MonoBehaviour
    {
        IEnumerator Start()
        {
            ARSession session = null;
            while (session == null) { session = Object.FindFirstObjectByType<ARSession>(); yield return null; }
            while (session.subsystem == null) yield return null;
            session.subsystem.configurationChooser = new Chooser();
            Debug.Log("FaceTrackingChooser: installed on ARSession.");
            Destroy(gameObject);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        var go = new GameObject("FaceTrackingChooserInstaller");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<Installer>();
    }
}
