using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using Codice.LogWrapper;
using System.Runtime.Remoting.Contexts;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;
using System.Collections.Generic;

public class Export : Editor
{
    const string PATH = @"E:\ONIModding\ModsSource\UnityUI";


    static Dictionary<string,string> predefinedPaths = new Dictionary<string, string>()
    {
        {"clustergenerationsettingsmanager_menuassets", @"E:\ONIModding\ModsSource\ModsSolution\ClusterTraitGenerationManager\ModAssets\assets\" },
        {"dss_uiassets",                                @"E:\ONIModding\ModsSource\ModsSolution\SetStartDupes\ModAssets\assets\"},
        {"rocketryexpanded_ui_assets",                  @"E:\ONIModding\ModsSource\ModsSolution\Rockets-TinyYetBig\ModAssets\assets\"},
        {"blueprints_ui",                               @"E:\ONIModding\ModsSource\ModsSolution\BlueprintsV2\ModAssets\assets\"},
        {"mpm_ui",                               @"E:\ONIModding\ModsSource\ModsSolution\ModProfileManager_Addon\ModAssets\assets\"},

    };

    static string WIN = Path.Combine(PATH, "windows");
    static string MAC = Path.Combine(PATH, "mac");
    static string LINUX = Path.Combine(PATH, "linux");

    [MenuItem("Assets/Windows export")]
    static void BuildAllAssetBundles2()
    {
        var winBundles = BuildPipeline.BuildAssetBundles(WIN, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        MoveToTargetFolder(winBundles, WIN);
    }
    static void MoveToTargetFolder(AssetBundleManifest target, string bundlePath)
    {
        foreach (var bundle in target.GetAllAssetBundles())
        {
            if (predefinedPaths.ContainsKey(bundle))
            {
                string targetSubPath = predefinedPaths[bundle];
                Debug.Log("copying " + bundle + " to predefined folder: " + targetSubPath);
                string subpath = bundlePath.Replace(PATH,string.Empty).Replace("\\",string.Empty);
                string bundlePathSource = Path.Combine(bundlePath, bundle);
                string bundlePathTarget = Path.Combine(Path.Combine(targetSubPath, subpath),bundle);

                Directory.CreateDirectory(Directory.GetParent(bundlePathTarget).FullName);

                File.Copy(bundlePathSource, bundlePathTarget, true);
            }
        }
    }

    [MenuItem("Assets/Export (all platforms)")]
    static void BuildAllAssetBundles()
    {
        Debug.Log("Windows Bundles...");
        var winBundles = BuildPipeline.BuildAssetBundles(WIN, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        MoveToTargetFolder(winBundles, WIN);

        Debug.Log("MAC Bundles...");
        var macBundles =  BuildPipeline.BuildAssetBundles(MAC, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
        MoveToTargetFolder(macBundles, MAC);

        Debug.Log("Linux Bundles...");
        var linuxBundles = BuildPipeline.BuildAssetBundles(LINUX, BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux64);
        MoveToTargetFolder(linuxBundles, LINUX);        
    }

    [Serializable]
    public class TMPSettings
    {
        [JsonProperty]
        public bool Skip { get; set; }

        [JsonProperty]
        public string Font { get; set; } = "NotoSans-Regular";
        [JsonProperty]
        public FontStyles FontStyle { get; set; }
        [JsonProperty]
        public float FontSize { get; set; }
        [JsonProperty]
        public TextAlignmentOptions Alignment { get; set; }
        [JsonProperty]
        public int MaxVisibleLines { get; set; }
        [JsonProperty]
        public bool EnableWordWrapping { get; set; }
        [JsonProperty]
        public bool AutoSizeTextContainer { get; set; }
        [JsonProperty]
        public string Content { get; set; }
        [JsonProperty]
        public float X { get; set; }
        [JsonProperty]
        public float Y { get; set; }
        [JsonProperty]
        public float[] Color { get; set; }
        [JsonProperty]
        public bool VariableFontSize { get; set; }
        [JsonProperty]
        public float VariableFontSizeMaximum { get; set; }
        [JsonProperty]
        public float VariableFontSizeMinimum { get; set; }
    }

    public static string GetGameObjectPath(GameObject obj, GameObject parent)
    {
        string stop = parent.name;
        string prefix = "STRINGS.UI";

        //if (parent.gameObject.TryGetComponent(out ModInfo modInfo))
        //{
        //    prefix = modInfo.modName + "." + prefix;
        //}

        string path = "." + obj.name.ToUpper();
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            string parentName = obj.name;
            if (parentName == stop) break;

            path = "." + parentName.ToUpper() + path;
        }
        return prefix + path;
    }

    static void SetButtonRefs(GameObject obj)
    {
        if(obj.TryGetComponent(out SettingsDialog settings))
        {
            var dataHolder = new GameObject("SettingsDialogData");
            dataHolder.transform.parent = obj.transform;
            dataHolder.AddComponent<Text>().text = settings.GenerateJson();
            UnityEngine.Object.DestroyImmediate(settings);
        }
    }

    [MenuItem("Assets/Graystroke")]
    static void GrayStrokeAdd()
    {
        StrokeMeSomeGray(Selection.activeGameObject, Selection.activeGameObject.name);
    }


    [MenuItem("Assets/Convert TMP")]
    static void BuildTMP()
    {
        BuildObject(Selection.activeGameObject, Selection.activeGameObject.name);
    }
    [MenuItem("Assets/Revert TMP Conversion")]
    static void UnBuildTMP()
    {
        ReplaceAllText(Selection.activeGameObject);
    }

    [MenuItem("Assets/Export Selected UI (Convert TMP)")]
    static void BuildAllAssetBundlesTMP()
    {
        BuildObject(Selection.activeGameObject, Selection.activeGameObject.name);
        BuildBundles();
    }

    public static TMP_FontAsset NotoSans => AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/NotoSans-Regular SDF.asset");
    public static TMP_FontAsset GrayStroke => AssetDatabase.LoadAssetAtPath<TMP_FontAsset>( "Assets/Fonts/GRAYSTROKE REGULAR SDF.asset");

    static void ReplaceAllText(GameObject parent, bool realign = true)
    {
        var textComponents = parent.GetComponentsInChildren(typeof(Text), true);

        foreach (Text text in textComponents)
        {
            if (text.gameObject.name == "SettingsDialogData") continue;

            string TMPData = text.text;
            GameObject obj = text.gameObject;
            TMPSettings data = ExtractTMPData(TMPData, text);

            if (data != null)
            {
                var LT = obj.AddComponent<TextMeshProUGUI>();
                LT.font = data.Font.ToUpperInvariant().Contains("GRAYSTROKE") ? GrayStroke : NotoSans;
                LT.fontStyle = data.FontStyle;
                LT.fontSize = data.FontSize;
                LT.maxVisibleLines = data.MaxVisibleLines;
                LT.enableWordWrapping = data.EnableWordWrapping;
                LT.autoSizeTextContainer = data.AutoSizeTextContainer;
                LT.text = "";
                LT.color = new Color(data.Color[0], data.Color[1], data.Color[2]);
                LT.text = data.Content;
                // alignment isn't carried over instantiation, so it's applied later
                if (realign)
                {//
                   // LT.gameObject.AddComponent<TMPFixer>().alignment = data.Alignment;
                }
            }
        }
    }
    private static bool isValidJSon(string data)
    {
        if (string.IsNullOrWhiteSpace(data)) return false;
        return data.StartsWith("{") && data.EndsWith("}");
    }
    private static TMPSettings ExtractTMPData(string TMPData, Text text)
    {
        TMPSettings data = null;

        if (isValidJSon(TMPData))
        {
            try
            {
                data = JsonConvert.DeserializeObject<TMPSettings>(TMPData);
            }
            catch (JsonReaderException e)
            {
            }

            UnityEngine.Object.DestroyImmediate(text);
        }

        return data;
    }

    private static void BuildObject(GameObject original, string name)
    {
        GameObject obj = Instantiate(original, original.transform.parent);
        SetButtonRefs(obj);
        obj.name = name;
        var tmps = obj.GetComponentsInChildren<TextMeshProUGUI>(true);

        if (obj.transform.parent == null)
        {
            Debug.Log("Missing parent!: "+obj+", "+name);
            return;
        }

        foreach (TextMeshProUGUI tmp in tmps)
        {
            if (tmp == null)
            {
                continue;
            }
            var rect = tmp.GetComponent<RectTransform>();

            string path = GetGameObjectPath(tmp.gameObject, obj.transform.parent.gameObject);

            if (tmp.font == null)
                tmp.font = GrayStroke;
            if (tmp.fontStyle == default)
                tmp.fontStyle = FontStyles.Normal;

            Debug.Assert(path != null, "Path was null?!");
            Debug.Assert(rect != null, "rect was null?!");
            Debug.Assert(tmp != null, "tmp was null?!");
            Debug.Assert(tmp.font != null, "font was null?!");
            //Debug.Assert(tmp.fontStyle != default, "fontStyle was default?!");
            Debug.Assert(tmp.color != default, "color was default?!");
            Debug.Assert(tmp.alignment != default, "alignment was null?!");
            Debug.Assert(rect.sizeDelta != null, "rect.sizeDelta was null?!");



            TMPSettings settings = new TMPSettings
            {
               
            };
            settings.Alignment = tmp.alignment != default ? tmp.alignment : TextAlignmentOptions.Center;
            settings.AutoSizeTextContainer = tmp.autoSizeTextContainer;
            settings.Content = path;
            settings.EnableWordWrapping = tmp.enableWordWrapping;
            settings.FontSize = tmp.fontSize;
            settings.FontStyle = tmp.fontStyle != default ? tmp.fontStyle : FontStyles.Normal;
            settings.MaxVisibleLines = tmp.maxVisibleLines;
            settings.Font = tmp.font != null ? tmp.font.name : "GRAYSTROKE REGULAR SDF";
            settings.X = rect.sizeDelta.x;
            settings.Y = rect.sizeDelta.y;
            settings.Color = tmp.color != null ? new float[] { tmp.color.r, tmp.color.g, tmp.color.b } : new float[] { 1f, 1f, 1f };
            settings.VariableFontSize = tmp.autoSizeTextContainer;
            settings.VariableFontSizeMinimum = tmp.fontSizeMin;
            settings.VariableFontSizeMaximum = tmp.fontSizeMax;


            var textCmp = obj.transform.parent.gameObject.GetComponent<Text>();

            if (textCmp == null)
            {
                textCmp = obj.transform.parent.gameObject.AddComponent<Text>();
            }

            textCmp.text += path + ", " + tmp.text;

            string jsonData;
            jsonData = JsonConvert.SerializeObject(settings, Formatting.Indented);

            GameObject parent = tmp.gameObject;
            DestroyImmediate(tmp);

            Text text = parent.AddComponent<Text>();
            text.text = jsonData;
        }

        var prefab = PrefabUtility.SaveAsPrefabAsset(obj, $"Assets/UIs/{obj.name}.prefab");
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(obj.transform.parent.name, "");
        Debug.Log(obj.name + " updated in asset bundle " + obj.transform.parent.name);
        
        //Debug.Log($"Finished exporting assetBundle {PATH}/{obj.transform.parent.name}");
        DestroyImmediate(obj);
    }
    private static void StrokeMeSomeGray(GameObject obj, string name)
    {
        var tmps = obj.GetComponentsInChildren<TextMeshProUGUI>(true);

        if (obj.transform.parent == null)
        {
            Debug.Log("Missing parent!: " + obj + ", " + name);
            return;
        }

        foreach (TextMeshProUGUI tmp in tmps)
        {
            if(GrayStroke == null)
            {
                Debug.LogError("Graystroke was null!");
                continue;
            }
            tmp.font = GrayStroke;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.UpdateFontAsset();
            tmp.ForceMeshUpdate();
        }

      
        //DestroyImmediate(obj);
    }


    private static void BuildBundles()
    {
        BuildPipeline.BuildAssetBundles(WIN, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        BuildPipeline.BuildAssetBundles(MAC, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);
        BuildPipeline.BuildAssetBundles(LINUX, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneLinux64);
    }
}
