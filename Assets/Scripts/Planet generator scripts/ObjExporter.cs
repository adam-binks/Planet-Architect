using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

struct ObjMaterial
{
    public string name;
    public string textureName;
    public float r;
    public float g;
    public float b;
    public float opacity;
}

public class ObjExporter : ScriptableObject
{
    private static int vertexOffset = 0;
    private static int normalOffset = 0;
    private static int uvOffset = 0;

    private static string targetFolder = "ExportedObj";


    private static string MeshToString(MeshFilter mf, Dictionary<string, ObjMaterial> materialList)
    {
        Mesh m = mf.sharedMesh;
        Material[] mats = mf.GetComponent<MeshRenderer>().materials; // or sharedmaterials

        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mf.name).Append("\n");
        foreach (Vector3 lv in m.vertices)
        {
            Vector3 wv = mf.transform.TransformPoint(lv);

            //This is sort of ugly - inverting x-component since we're in
            //a different coordinate system than "everyone" is "used to".
            sb.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
        }
        sb.Append("\n");

        foreach (Vector3 lv in m.normals)
        {
            Vector3 wv = mf.transform.TransformDirection(lv);

            sb.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
        }
        sb.Append("\n");

        foreach (Vector3 v in m.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }

        for (int material = 0; material < m.subMeshCount; material++)
        {
            sb.Append("\n");
            sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            sb.Append("usemap ").Append(mats[material].name).Append("\n");

            //See if this material is already in the materiallist.
            try
            {
                ObjMaterial objMaterial = new ObjMaterial();

                objMaterial.name = mats[material].name;
                objMaterial.r = mats[material].color.r;
                objMaterial.g = mats[material].color.g;
                objMaterial.b = mats[material].color.b;
                objMaterial.opacity = mats[material].color.a;

                //if (mats[material].mainTexture)
                //    objMaterial.textureName = EditorUtility.GetAssetPath(mats[material].mainTexture);
                //else
                    objMaterial.textureName = null;

                materialList.Add(objMaterial.name, objMaterial);
            }
            catch (ArgumentException)
            {
                //Already in the dictionary
            }


            int[] triangles = m.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                //Because we inverted the x-component, we also needed to alter the triangle winding.
                sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                    triangles[i] + 1 + vertexOffset, triangles[i + 1] + 1 + normalOffset, triangles[i + 2] + 1 + uvOffset));
            }
        }

        vertexOffset += m.vertices.Length;
        normalOffset += m.normals.Length;
        uvOffset += m.uv.Length;

        return sb.ToString();
    }

    private static void Clear()
    {
        vertexOffset = 0;
        normalOffset = 0;
        uvOffset = 0;
    }

    private static Dictionary<string, ObjMaterial> PrepareFileWrite()
    {
        Clear();

        return new Dictionary<string, ObjMaterial>();
    }

    private static void MaterialsToFile(Dictionary<string, ObjMaterial> materialList, string folder, string filename)
    {
        using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".mtl"))
        {
            foreach (KeyValuePair<string, ObjMaterial> kvp in materialList)
            {
                sw.Write("\n");
                sw.Write("newmtl {0}\n", kvp.Key);
                sw.Write("Ka  0.4 0.4 0.4\n");
                sw.Write("Kd  "+ kvp.Value.r + " " + kvp.Value.g + " " + kvp.Value.b + "\n");
                sw.Write("Ks  0.0 0.0 0.0\n");
                sw.Write("d  " + kvp.Value.opacity +"\n");
                sw.Write("Ns  0.0\n");
                sw.Write("illum 2\n");

                if (kvp.Value.textureName != null)
                {
                    string destinationFile = kvp.Value.textureName;


                    int stripIndex = destinationFile.LastIndexOf('/');//FIXME: Should be Path.PathSeparator;

                    if (stripIndex >= 0)
                        destinationFile = destinationFile.Substring(stripIndex + 1).Trim();


                    string relativeFile = destinationFile;

                    destinationFile = folder + "/" + destinationFile;

                    Debug.Log("Copying texture from " + kvp.Value.textureName + " to " + destinationFile);

                    try
                    {
                        //Copy the source file
                        File.Copy(kvp.Value.textureName, destinationFile);
                    }
                    catch
                    {

                    }


                    sw.Write("map_Kd {0}", relativeFile);
                }

                sw.Write("\n\n\n");
            }
        }
    }

    private static void MeshToFile(MeshFilter mf, string folder, string filename)
    {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

        using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
        {
            sw.Write("mtllib ./" + filename + ".mtl\n");

            sw.Write(MeshToString(mf, materialList));
        }

        MaterialsToFile(materialList, folder, filename);
    }

    public static void MeshesToFile(MeshFilter[] mf, string folder, string filename)
    {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

        using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
        {
            sw.Write("mtllib ./" + filename + ".mtl\n");

            for (int i = 0; i < mf.Length; i++)
            {
                sw.Write(MeshToString(mf[i], materialList));
            }
        }

        MaterialsToFile(materialList, folder, filename);
    }
}