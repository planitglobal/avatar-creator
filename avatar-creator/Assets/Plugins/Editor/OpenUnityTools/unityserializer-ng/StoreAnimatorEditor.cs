using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StoreAnimator))]
public class StoreAnimatorEditor : Editor {
    private StoreAnimator script;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        script = (StoreAnimator)target;

        if (!script.e_storeAllLayers) {
            Animator animator = script.GetComponent<Animator>();
            PrepareLayerMask(animator);
            if (animator) {
                for (int i = 0; i < animator.layerCount; i++) {
                    using (new Horizontal()) {
                        GUILayout.Label("      >       ");
                        bool before = GetLayerActive(i);
                        bool after;
                        if (GUILayout.Toggle(before, animator.GetLayerName(i))) {
                            after = true;
                            if (before != after) {
                                Undo.RecordObject(script, "usng: Store Animator layer mask");
                                SetLayerActive(i);
                            }
                        }
                        else {
                            after = false;
                            if (before != after) {
                                Undo.RecordObject(script, "usng: Store Animator layer mask");
                                SetLayerInactive(i);
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                }

                for (int i = 0; i < script.e_layerMask.Length; i++) {
                    string s = "";
                    for (int i2 = 0; i2 < 8; i2++) {
                        s += (((script.e_layerMask[i] >> i2) & 1).ToString());
                    }
                    EditorGUILayout.LabelField(s);
                }
            }
        }
    }

    /// <summary>
    /// Makes sure that there are enough layermask segments in the layermask array.
    /// </summary>
    /// <param name="animator">The Animator component.</param>
    private void PrepareLayerMask(Animator animator) {
        int size = animator.layerCount / 8 + 1;
        if (script.e_layerMask.Length != size) {
            byte[] newMask = new byte[size];
            for (int i = 0; i < script.e_layerMask.Length; i++) {
                newMask[i] = script.e_layerMask[i];
            }
            script.e_layerMask = newMask;
        }
    }

    /// <summary>
    /// Checks whether we want to store a specific layer.
    /// </summary>
    /// <param name="i">The layer's index.</param>
    /// <returns>True, if we want to store it.</returns>
    private bool GetLayerActive(int i) {
        byte maskSegment = script.e_layerMask[i / 8];
        byte offset = (byte)(i % 8);
        return (maskSegment >> offset & 1) == 1;
    }

    /// <summary>
    /// Marks a specific layer to be stored in the layermask.
    /// </summary>
    /// <param name="i">The layer's index.</param>
    private void SetLayerActive(int i) {
        byte offset = (byte)(i % 8);
        script.e_layerMask[i / 8] |= (byte)(1 << offset);
    }

    /// <summary>
    /// Clears a specific layer in the layermask, so it doesn't get stored.
    /// </summary>
    /// <param name="i">The layer's index.</param>
    private void SetLayerInactive(int i) {
        byte offset = (byte)(i % 8);
        script.e_layerMask[i / 8] &= (byte)(~(1 << offset));
    }
}