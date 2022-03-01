using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HardpointList : MonoBehaviour
{
    [Serializable]
    public class HardpointItem {
        [SerializeField] public Image bg;
        [SerializeField] public TextMeshProUGUI number;
        [SerializeField] public Image icon;
    }

    [SerializeField] public List<HardpointItem> hardpointItems = new List<HardpointItem>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(HardpointList))]
    public class HardpointListEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            HardpointList script = (HardpointList)target;

            if (GUILayout.Button("ADD ALL"))
            {
                script.hardpointItems.Clear();

                //get all children one level down
                Transform[] children = script.transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in children)
                {
                    if (child.parent == script.transform)
                    {
                        HardpointItem item = new HardpointItem();
                        Image[] images = child.GetComponentsInChildren<Image>();
                        item.bg = images[0];
                        item.number = child.GetComponentInChildren<TextMeshProUGUI>();
                        item.icon = images[1];

                        script.hardpointItems.Add(item);
                    }
                }

                //save changes
                EditorUtility.SetDirty(target);
            }
        }
    }
    #endif
}
