using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace LGamekit.LegacyAssetManagementSystem {

    [CustomEditor(typeof(AssetBundleManifestBuilder))]
    public class AssetBundleManifestBuilderEditor : Editor {

        public override void OnInspectorGUI() {

            AssetBundleManifestBuilder builder = target as AssetBundleManifestBuilder;

            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(60f));

            if (GUILayout.Button(new GUIContent("Output", "Select Outout Folder."), GUILayout.Width(100f), GUILayout.MinHeight(60f))) {
                builder.outputFolder = EditorUtility.OpenFolderPanel("Select Outout Folder", Application.dataPath, string.Empty);
            }

            builder.outputFolder = EditorGUILayout.TextArea(builder.outputFolder, GUILayout.MinHeight(60f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent("Name", "The Output File Name."), GUILayout.Width(100f));
            builder.outputName = EditorGUILayout.TextField(builder.outputName);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(60f));

            if (GUILayout.Button(new GUIContent("Input", "Select Input File."), GUILayout.Width(100f), GUILayout.MinHeight(60f))) {
                builder.inputPath = EditorUtility.OpenFilePanel("Select Input File", Application.dataPath, string.Empty);
            }

            builder.inputPath = EditorGUILayout.TextArea(builder.inputPath, GUILayout.MinHeight(60f));

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(new GUIContent("Export", "Export Output File."), GUILayout.MinHeight(60f))) {
                builder.Execute();
            }

        }

    }

}
