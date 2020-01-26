using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Use flat shading ?")]
    public bool flatshading;
    [Tooltip("The resolution of the terrain. Between 2 and 100. Dont go past 100 or it will bug out")]
    public int resolution = 2;
    [Tooltip("Regenerate a new mesh with a random seed on start")]
    public bool generate_on_start;
    private Texture2D texture;
    public Mesh mesh;
    private Vector3[] vertices;
    private int[] tris;
    private Vector2[] uvs;
    private Vector3 dotproduct = Vector3.up * 10.0f;
    [Header("Colors")]
    [Tooltip("Divison number of the color gradiant")]
    public float div;
    [Tooltip("Color gradiant that selects color on terrain slope")]
    public Gradient grad;    
    [Header("Noise")]
    [Tooltip("Main Scale value")]
    public float scalem;
    [Tooltip("Main Steepness value")]
    public float steepnessm;
    [Tooltip("Should we use the curve-remap ?")]
    public bool usecurve;
    [Tooltip("The noise layers. Two types of noise : Perlin And Voronoi")]
    public Layers[] layersvar;
    [Tooltip("The remap-curve to remap the output value")]
    public AnimationCurve curveremap;
    [Header("Masks")]
    [Tooltip("Use Circle Mask ?")]
    public bool UseMask;
    [Tooltip("Show only debug mask ?")]
    public bool debugMask;
    [Tooltip("Multiplicator value of the mask")]
    public float maskmult;
    [Tooltip("Circle mask radius")]
    public float maskradius;
    [Tooltip("Fading circle curve")]
    public AnimationCurve fade;
    private Vector2 pos;
    private Vector2 offset;
    [System.Serializable]
    public struct Layers
    {
        public int octaves;
        public float scale;
        public float steepness;
        public float lacunarity;//Increase in frequency
        public float persistence;//Decrease in amplitude
        public enum NoiseType
        {
            PerlinNoise, Voronoi
        }
        public NoiseType noise_type;
    }
    private void Start()
    {
        if (generate_on_start)
        {
            generateRandomTerrain();
        }        
    }
    private void OnValidate()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }
        createmesh();        
        updatemesh();
        maketexture();
        if (flatshading) FlatShading();
        updatemesh();
    }
    
    public void generateRandomTerrain()
    {
        offset = new Vector2(UnityEngine.Random.Range(-100000, 100000), UnityEngine.Random.Range(-100000, 100000));
        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }
        createmesh();
        updatemesh();
        maketexture();
        if (flatshading) FlatShading();
        updatemesh();
    }
    private void createmesh()
    {        
        pos = new Vector2(0 - (resolution / 2), resolution / 2);
        texture = new Texture2D(resolution, resolution);
        vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        uvs = new Vector2[vertices.Length];
        for (int z = 0, i = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float y = advancenoise(x + offset.x, z + offset.y);
                if (UseMask && !debugMask)
                {
                    y *= island_mask(x + pos.x, z - pos.y);
                }
                if (debugMask)
                {
                    y = island_mask(x + pos.x, z - pos.y);
                }
                vertices[i] = new Vector3(x, y, z);
                uvs[i] = new Vector2(x / (float)resolution, z / (float)resolution);
                i++;
            }
        }
        
        texture.filterMode = FilterMode.Point;
        int vert = 0;
        int tri = 0;
        tris = new int[resolution * resolution * 6];
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                tris[tri] = vert;
                tris[tri+1] = vert + resolution + 1;
                tris[tri+2] = vert + 1;
                tris[tri+3] = vert+1;
                tris[tri+4] = vert+resolution+1;
                tris[tri+5] = vert+resolution+2;
                vert++;
                tri += 6;
            }
            vert++;
        }       
    }
    private void updatemesh()
    {             
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        mesh.uv = uvs;
        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", texture);
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    private void maketexture()
    {        
        Vector3[] normals = mesh.normals;
        for (int z = 0, i = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float normal = Vector3.Dot(normals[i], dotproduct);
                texture.SetPixel(x, z, grad.Evaluate(normal / div));
                i++;
            }
        }
        texture.Apply();
    }
    private float advancenoise(float x, float z)
    {
        float value = 0;
        Vector2 position = new Vector2(x, z);
        for (int i = 0; i < layersvar.Length; i++)
        {
            float scale = layersvar[i].scale * scalem;
            float steepness = layersvar[i].steepness * steepnessm;
            float scaleo;
            float steepnesso;
            for (int j = 0; j < layersvar[i].octaves; j++)
            {
                scaleo = Mathf.Pow(layersvar[i].lacunarity, j) * scale;
                steepnesso = Mathf.Pow(layersvar[i].persistence, j) * steepness;
                if (layersvar[i].noise_type == Layers.NoiseType.PerlinNoise)
                {
                    value += noise.snoise(position * scaleo) * steepnesso;
                }
                if (layersvar[i].noise_type == Layers.NoiseType.Voronoi)
                {
                    value += noise.cellular(new Vector2(x, z) * scaleo).x * steepnesso;
                }
            }
        }
        if (usecurve)
        {
            return curveremap.Evaluate(value);
        }
        return value;
    }
    private float island_mask(float x, float z)
    {
        Vector2 vec2 = new Vector2(x, z);
        return fade.Evaluate(vec2.magnitude + maskradius) * maskmult;
    }
    void FlatShading()//Code from https://www.youtube.com/watch?v=V1vL9yRA_eM He makes extremly good videos !
    {
        Vector3[] flatShadedVertices = new Vector3[tris.Length];
        Vector2[] flatShadedUvs = new Vector2[tris.Length];

        for (int i = 0; i < tris.Length; i++)
        {
            flatShadedVertices[i] = vertices[tris[i]];
            flatShadedUvs[i] = uvs[tris[i]];
            tris[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }
}
