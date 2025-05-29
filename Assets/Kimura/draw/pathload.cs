//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

//public class PathLoader : MonoBehaviour
//{
//    public string csvFileName = "compass_30fps_log.csv";
//    public EnergyMapDrawer_BrailleHighlight_FrontBack10m energyMapDrawer;

//    void Start()
//    {
//        List<Vector3> positions = new List<Vector3>();
//        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);
//        Debug.Log($"[PathLoader] Looking for CSV at: {path}");

//        if (File.Exists(path))
//        {
//            string[] lines = File.ReadAllLines(path);
//            Debug.Log($"[PathLoader] Found CSV with {lines.Length} lines");

//            for (int i = 1; i < lines.Length; i++) // Skip header line
//            {
//                string[] parts = lines[i].Split(',');
//                if (parts.Length < 7)
//                {
//                    Debug.LogWarning($"[PathLoader] Skipping line {i} (not enough columns): {lines[i]}");
//                    continue;
//                }

//                if (float.TryParse(parts[4], out float posX) && float.TryParse(parts[6], out float posZ))
//                {
//                    positions.Add(new Vector3(posX, 0f, posZ));
//                }
//                else
//                {
//                    Debug.LogWarning($"[PathLoader] Failed to parse line {i}: {lines[i]}");
//                }
//            }

//            energyMapDrawer.pathPositions = positions;
//            Debug.Log($"[PathLoader] Loaded {positions.Count} positions from CSV.");
//        }
//        else
//        {
//            Debug.LogWarning($"[PathLoader] CSV file not found at: {path}");
//        }
//    }
//}
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathLoader : MonoBehaviour
{
    public string csvFileName = "compass_30fps_log.csv";
    public PathLineRenderer pathLineRenderer;

    void Start()
    {
        List<Vector3> positions = new List<Vector3>();
        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            Debug.Log($"[PathLoader] Loaded {lines.Length} lines from CSV.");

            for (int i = 1; i < lines.Length; i++) // Skip header
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length < 7)
                {
                    Debug.LogWarning($"[PathLoader] Skipping line {i} (not enough columns): {lines[i]}");
                    continue;
                }

                if (float.TryParse(parts[4], out float posX) && float.TryParse(parts[6], out float posZ))
                {
                    Vector3 position = new Vector3(posX, 0f, posZ);
                    positions.Add(position);
                    Debug.Log($"[PathLoader] Loaded position: {position}");
                }
                else
                {
                    Debug.LogWarning($"[PathLoader] Failed to parse line {i}: {lines[i]}");
                }
            }

            pathLineRenderer.pathPositions = positions;
        }
        else
        {
            Debug.LogWarning($"[PathLoader] CSV file not found at: {path}");
        }
    }
}
