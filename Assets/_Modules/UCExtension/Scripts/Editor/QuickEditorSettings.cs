using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UCExtension
{
    public class QuickEditorSettings : ScriptableObject
    {
        #region Instance
        public const string FileFolder = "_Project/Editor";

        public const string FileName = "QuickEditorSettings.asset";

        public static string AssetPath => Path.Combine("Assets", FileFolder, FileName);

        public static string FilePath => Path.Combine(Application.dataPath, FileFolder, FileName);

        public static string FolderPath => Path.Combine(Application.dataPath, FileFolder);

        public static QuickEditorSettings Instance
        {
            get
            {
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                if (!File.Exists(FilePath))
                {
                    QuickEditorSettings newConfiguration = ScriptableObject.CreateInstance<QuickEditorSettings>();
                    AssetDatabase.CreateAsset(newConfiguration, AssetPath);
                    EditorUtility.SetDirty(newConfiguration);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorGUIUtility.PingObject(newConfiguration);
                    return newConfiguration;
                }
                QuickEditorSettings instance = AssetDatabase.LoadAssetAtPath<QuickEditorSettings>(AssetPath);
                return instance;
            }
        }
        #endregion

        [FoldoutGroup("Materials")]
        [HorizontalGroup("Materials/Get")]
        public Object MaterialFolder;

        [FoldoutGroup("Materials")]
        public Shader Shader;

        [FoldoutGroup("FBXs")]
        public List<Object> Fbxs;

        [FoldoutGroup("Find Animation")]
        public List<AnimatorOverrideController> Animators;

        [FoldoutGroup("Find Animation")]
        public Object AnimatorsFolder;

        [FoldoutGroup("Find Animation")]
        public Object AnimationClipsFolder;

        [FoldoutGroup("Find Animation")]
        public FindAnimationSettings FindAnimationSettings;
    }
}