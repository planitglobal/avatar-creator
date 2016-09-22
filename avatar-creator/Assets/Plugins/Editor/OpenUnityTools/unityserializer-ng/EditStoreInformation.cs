using UnityEngine;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(StoreInformation))]
public class EditStoreInformation : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var t = target as StoreInformation;

        if (!t.StoreAllComponents) {
            GUILayout.Label("  Components to store");
            var cs = t.GetComponents<Component>().Where(c => !c.GetType().IsDefined(typeof(DontStoreAttribute), false) && (c.hideFlags & HideFlags.HideInInspector) == 0);
            foreach (var c in cs) {
                var typeName = c.GetType().FullName;

                if (typeName == "UnityEngine.Animator") {
                    GUI.enabled = false;
                    using (new Horizontal()) {
                        GUILayout.Label("      >       ");
                        GUILayout.Toggle(false, "Use the StoreAnimator component for Animators!");
                        GUILayout.FlexibleSpace();
                    }
                    t.Components.Remove(typeName);
                    GUI.enabled = true;
                    continue;
                }

                using (new Horizontal()) {
                    GUILayout.Label("      >       ");
                    if (GUILayout.Toggle(t.Components.Contains(typeName), ObjectNames.NicifyVariableName(typeName))) {
                        if (!t.Components.Contains(typeName)) {
                            t.Components.Add(typeName);
                        }
                        EditorUtility.SetDirty(target);
                    }
                    else {
                        t.Components.Remove(typeName);
                        EditorUtility.SetDirty(target);
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }
    }
}
#endif