using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UCExtension
{
    public class QuickEditorWindow : OdinEditorWindow
    {
        [MenuItem("UCExtension/Quick Editor")]
        private static void OpenWindow()
        {
            GetWindow<QuickEditorWindow>().Show();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LoadSettings();
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            Debug.Log("Save Changes");
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
            Debug.Log("Save Changes");
        }

        [FoldoutGroup("Materials")]
        [OnValueChanged(nameof(SaveSettings))]
        [SerializeField] Object materialFolder;

        [FoldoutGroup("Materials")]
        [OnValueChanged(nameof(SaveSettings))]
        [SerializeField] Shader shader;

        [Button]
        [FoldoutGroup("Materials")]
        public void ChangeNewShader()
        {
            var materials = EditorExtension.LoadAssets<Material>(materialFolder, FileNameExtension.MATERIAL);
            foreach (var item in materials)
            {
                item.SetShader(shader);
            }
        }

        [FoldoutGroup("FBXs")]
        [OnValueChanged(nameof(SaveSettings))]
        [SerializeField] List<Object> fbxs;

        [FoldoutGroup("FBXs")]
        [Button]
        public void SetLoop()
        {
            foreach (var item in fbxs)
            {
                EditorExtension.SetLoop(item, true);
            }
        }

        [FoldoutGroup("FBXs")]
        [Button]
        public void SetUnloop()
        {
            foreach (var item in fbxs)
            {
                EditorExtension.SetLoop(item, false);
            }
        }

        [FoldoutGroup("Find Animation")]
        [OnValueChanged(nameof(SaveSettings))]
        [SerializeField] List<AnimatorOverrideController> animators;

        [FoldoutGroup("Find Animation")]
        [OnValueChanged(nameof(SaveSettings))]
        [SerializeField] Object animatorsFolder;

        [FoldoutGroup("Find Animation")]
        [OnValueChanged(nameof(SaveSettings))]
        [SerializeField] Object animationClipsFolder;

        [FoldoutGroup("Find Animation")]
        [OnValueChanged(nameof(SaveSettings))]
        [SerializeField] FindAnimationSettings findAnimationSettings;

        [Button]
        [FoldoutGroup("Find Animation")]
        public void FindAnimationClips()
        {
            string path = AssetDatabase.GetAssetPath(animatorsFolder);
            animators = EditorExtension.LoadAssets<AnimatorOverrideController>(path, FileNameExtension.ANIMATOR_OVERRIDE);
            foreach (var item in animators)
            {
                item.FindAnimationClips(animationClipsFolder, findAnimationSettings.Settings);
            }
            this.SetDirtyAndSave();
        }

        public void LoadSettings()
        {
            var settingInstance = QuickEditorSettings.Instance;
            materialFolder = settingInstance.MaterialFolder;
            shader = settingInstance.Shader;
            fbxs = settingInstance.Fbxs.Clone();
            animators = settingInstance.Animators.Clone();
            animatorsFolder = settingInstance.AnimatorsFolder;
            animationClipsFolder = settingInstance.AnimationClipsFolder;
            findAnimationSettings = settingInstance.FindAnimationSettings;
        }

        public void SaveSettings()
        {
            var settingInstance = QuickEditorSettings.Instance;
            settingInstance.MaterialFolder = materialFolder;
            settingInstance.Shader = shader;
            settingInstance.Fbxs = fbxs.Clone();
            settingInstance.Animators = animators.Clone();
            settingInstance.AnimatorsFolder = animatorsFolder;
            settingInstance.AnimationClipsFolder = animationClipsFolder;
            settingInstance.MaterialFolder = materialFolder;
            settingInstance.FindAnimationSettings = findAnimationSettings;
            settingInstance.MarkDirty();
            Debug.Log("Save Settings");
        }
    }
}