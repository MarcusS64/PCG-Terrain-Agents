using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SubdivideHelper : MonoBehaviour
{
    public MeshFilter MF;

    // Start is called before the first frame update
    void Start()
    {
        //string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        //if (string.IsNullOrEmpty(path)) return;

        //path = FileUtil.GetProjectRelativePath(path);

        //Mesh m = MF.mesh;
        //MeshHelper.Subdivide(m);
        //MeshHelper.Subdivide(m);  // two times :D
        //MF.mesh = m;

        //AssetDatabase.CreateAsset( MF.mesh, path );
        //AssetDatabase.SaveAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
