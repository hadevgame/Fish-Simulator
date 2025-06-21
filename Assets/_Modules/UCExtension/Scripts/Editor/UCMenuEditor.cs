using System.Collections;
using System.Collections.Generic;
using UCExtension.Audio;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Sirenix.OdinInspector;
using System.Linq;
using UCExtension.GUI;
using UnityEditor.SceneManagement;

namespace UCExtension
{
    public class UCMenuEditor
    {
        [MenuItem("GameObject/UI/Button Scale")]
        public static void CreateButtonScale()
        {
            if (Selection.transforms.Length == 0) return;
            Vector2 buttonSize = new Vector2(100, 100);
            GameObject newButton = new GameObject("Button");
            newButton.transform.SetParent(Selection.transforms[0]);
            newButton.transform.localPosition = Vector2.zero;
            //
            var rectTransform = newButton.AddComponent<RectTransform>();
            rectTransform.sizeDelta = buttonSize;
            //
            var rootImg = newButton.AddComponent<Image>();
            rootImg.SetAlpha(0);
            var buttonScale = newButton.AddComponent<ButtonScale>();
            buttonScale.transition = Selectable.Transition.ColorTint;
            //
            GameObject child = new GameObject("Graphic");
            RectTransform childRect = child.AddComponent<RectTransform>();
            childRect.sizeDelta = buttonSize;
            childRect.anchorMax = new Vector2(1, 1);
            childRect.anchorMin = new Vector2(0, 0);
            childRect.SetParent(newButton.transform);
            childRect.transform.localPosition = Vector2.zero;
            var backgroundImage = child.AddComponent<Image>();
            buttonScale.targetGraphic = backgroundImage;
            EditorGUIUtility.PingObject(newButton);
        }

        [MenuItem("GameObject/UCExtension/AudioManager")]
        public static void CreateAudioManager()
        {
            var exist = GameObject.FindObjectOfType<AudioManager>();
            if (exist)
            {
                EditorGUIUtility.PingObject(exist);
            }
            else
            {
                var path = "Assets/UCExtension/Packages/UCExtension/Audio/Prefabs/AudioManager.prefab";
                CreatePrefab(path);
            }
        }

        [MenuItem("GameObject/UI/UCJoystick")]
        public static void CreateJoystick()
        {
            var path = "Assets/UCExtension/Prefabs/UCJoystick.prefab";
            CreatePrefab(path);
        }

        public static void CreatePrefab(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var obj = PrefabUtility.InstantiatePrefab(prefab);
            var gameObj = (obj as GameObject);
            if (Selection.transforms.Length > 0)
            {
                gameObj.transform.SetParent(Selection.transforms[0]);
            }
            EditorGUIUtility.PingObject(obj);
            gameObj.transform.localScale = Vector3.one;
            gameObj.transform.SetAsLastSibling();
            EditorUtility.SetDirty(gameObj);
        }


        [MenuItem("UCExtension/Cache/Clear Cache")]
        public static void ClearCache()
        {
            Caching.ClearCache();
        }

        [MenuItem("UCExtension/Add newtonsoft package")]
        public static void Add()
        {
            AddRequest request = Client.Add("com.unity.nuget.newtonsoft-json");
        }

        [MenuItem("GameObject/Edit/Remove Number In Name (Rescursively)")]
        public static void RemoveNumberInName()
        {
            if (Selection.transforms.Length != 1) return;
            string[] splited;
            string last = "";
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in Selection.transforms[0].AllChilds())
            {
                splited = item.name.Split(" ");
                if (splited.Length < 2) continue;
                last = splited[splited.Length - 1];
                if (last.Length < 3) continue;
                if (last[0].Equals('(') && last[last.Length - 1].Equals(')'))
                {
                    strBuilder.Clear();
                    for (int i = 0; i < splited.Length - 1; i++)
                    {
                        if (i == splited.Length - 2)
                        {
                            strBuilder.Append(splited[i]);
                        }
                        else
                        {
                            strBuilder.Append(splited[i] + " ");
                        }
                    }
                    item.name = strBuilder.ToString();
                    EditorUtility.SetDirty(item);
                }
            }
        }

        [MenuItem("GameObject/Edit/Cast Shadow/Turn Off")]
        public static void TurnOffShadowInTrans()
        {
            Debug.Log("Off shadow: " + Selection.transforms.Count());
            foreach (var item in Selection.transforms)
            {
                foreach (var tran in item.AllChilds())
                {
                    Debug.Log("Off shadow: " + tran.name);
                    TurnCastShadow(tran, false);
                }
            }
        }

        [MenuItem("GameObject/Edit/Cast Shadow/Turn On")]
        public static void TurnOnShadowInTrans()
        {
            foreach (var item in Selection.transforms)
            {
                foreach (var tran in item.AllChilds())
                {
                    Debug.Log("On shadow: " + tran.name);
                    TurnCastShadow(tran, true);
                }
            }
        }

        [MenuItem("GameObject/Edit/Receive Shadow/Turn Off")]
        public static void TurnOffReceiveShadowInTrans()
        {
            Debug.Log("Off Receive shadow: " + Selection.transforms.Count());
            foreach (var item in Selection.transforms)
            {
                foreach (var tran in item.AllChilds())
                {
                    TurnReceiveShadow(tran, false);
                }
            }
        }

        [MenuItem("GameObject/Edit/Receive Shadow/Turn On")]
        public static void TurnOnReceiveShadowInTrans()
        {
            Debug.Log("On Receive shadow: " + Selection.transforms.Count());
            foreach (var item in Selection.transforms)
            {
                foreach (var tran in item.AllChilds())
                {
                    TurnReceiveShadow(tran, true);
                }
            }
        }
        [MenuItem("UCExtension/Open First Scene")]
        public static void OpenFirstScene()
        {
            var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
            //var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
            //EditorSceneManager.playModeStartScene = sceneAsset;
            EditorSceneManager.OpenScene(pathOfFirstScene);
        }

        public static void TurnReceiveShadow(Transform transform, bool value)
        {
            var mesh = transform.GetComponent<MeshRenderer>();
            if (mesh)
            {
                mesh.receiveShadows = value;
                mesh.SetDirtyAndSave();
            }
        }

        public static void TurnCastShadow(Transform transform, bool value)
        {
            var mesh = transform.GetComponent<MeshRenderer>();
            if (mesh)
            {
                mesh.shadowCastingMode = value ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
                mesh.SetDirtyAndSave();
            }
        }


        [MenuItem("GameObject/Edit/Remove Missing Scripts")]
        private static void FindAndRemoveMissingInSelected()
        {
            // EditorUtility.CollectDeepHierarchy does not include inactive children
            var deeperSelection = Selection.gameObjects.SelectMany(go => go.GetComponentsInChildren<Transform>(true))
                .Select(t => t.gameObject);
            var prefabs = new HashSet<Object>();
            int compCount = 0;
            int goCount = 0;
            foreach (var go in deeperSelection)
            {
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (count > 0)
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(go))
                    {
                        RecursivePrefabSource(go, prefabs, ref compCount, ref goCount);
                        count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                        // if count == 0 the missing scripts has been removed from prefabs
                        if (count == 0)
                            continue;
                        // if not the missing scripts must be prefab overrides on this instance
                    }

                    Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    compCount += count;
                    goCount++;
                }
            }

            Debug.Log($"Found and removed {compCount} missing scripts from {goCount} GameObjects");
        }

        // Prefabs can both be nested or variants, so best way to clean all is to go through them all
        // rather than jumping straight to the original prefab source.
        private static void RecursivePrefabSource(GameObject instance, HashSet<Object> prefabs, ref int compCount,
            ref int goCount)
        {
            var source = PrefabUtility.GetCorrespondingObjectFromSource(instance);
            // Only visit if source is valid, and hasn't been visited before
            if (source == null || !prefabs.Add(source))
                return;

            // go deep before removing, to differantiate local overrides from missing in source
            RecursivePrefabSource(source, prefabs, ref compCount, ref goCount);

            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(source);
            if (count > 0)
            {
                Undo.RegisterCompleteObjectUndo(source, "Remove missing scripts");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(source);
                compCount += count;
                goCount++;
            }
        }


        [MenuItem("GameObject/Edit/Remove Box Colliders")]
        public static void RemoveBoxCols()
        {
            foreach (var item in Selection.transforms)
            {
                var boxes = item.GetComponents<BoxCollider>();
                foreach (var b in boxes)
                {
                    GameObject.DestroyImmediate(b);
                }
            }
        }

        [MenuItem("GameObject/Edit/Remove Mesh Colliders")]
        public static void RemoveMeshCols()
        {
            foreach (var item in Selection.transforms)
            {
                var boxes = item.GetComponents<MeshCollider>();
                foreach (var b in boxes)
                {
                    GameObject.DestroyImmediate(b);
                }
            }
        }

        [MenuItem("GameObject/Edit/Remove All Components")]
        public static void RemoveAllComponents()
        {
            foreach (var item in Selection.transforms)
            {
                foreach (var comp in item.GetComponents<Component>())
                {
                    //Don't remove the Transform component
                    if (!(comp is Transform))
                    {
                        GameObject.DestroyImmediate(comp);
                    }
                }
                item.SetDirtyAndSave();
            }
        }
    }
}