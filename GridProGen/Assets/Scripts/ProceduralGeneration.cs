using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    const int size = 100;
    Cell[,] grid;
    public Material m;
    public float noiseScale = 0.1f;
    public float waterValue;
    public Material edgeMaterial;
    public GameObject tree;

    // Start is called before the first frame update
    void Start()
    {
        InitCell();
        DrawTerrainMesh(grid);
        DrawTexture(grid);
        DrawEdgeMesh(grid);
        SpawnTree(grid);
    }

    private void DrawTexture(Cell[,] grid)
    {
        Texture2D t = new Texture2D(size, size);
        Color[] map = new Color[size * size];
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                Cell cell = grid[i, j];
                if (cell.isWater)
                {
                    Debug.Log("water");
                    map[j * size + i] = Color.blue;
                }
                else
                {
                    map[j * size + i] = Color.green;
                }
            }
        }
        t.filterMode = FilterMode.Point;
        t.SetPixels(map);
        t.Apply();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = m;
        meshRenderer.material.mainTexture = t;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitCell()
    {
        grid = new Cell[size, size];
        float[,] noiseMap = CreateNoiseMap();
        float[,] fallOffMap = CreateFallOffMap();

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                noiseMap[i, j] -= fallOffMap[i, j];
            }
        }
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                Cell c = new Cell();
                if (noiseMap[i, j] < waterValue)
                {
                    c.isWater = true;
                    grid[i, j] = c;
                }
                else
                {
                    c.isWater = false;
                    grid[i, j] = c;
                }
            }
        }
    }
    private float[,] CreateFallOffMap()
    {
        float[,] fallOffMap = new float[size, size];
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                float fallOff_A = i / (float)size * 2 - 1;
                float fallOff_B = j / (float)size * 2 - 1;
                float val = Mathf.Max(Mathf.Abs(fallOff_A), Mathf.Abs(fallOff_B));
                val = Evaluate(val);
                fallOffMap[i, j] = MathUtil.RadialFallOff(val, 90, i, j, size / 2f, size / 2f);
            }
        }
        return fallOffMap;
    }
    private float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
    private float[,] CreateNoiseMap()
    {
        int offsetX = Random.Range(-10000, 10000);
        int offsetY = Random.Range(-10000, 10000);
        float[,] noiseMap;
        noiseMap = new float[size, size];
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                noiseMap[i, j] = Mathf.PerlinNoise(i * noiseScale + offsetX, j * noiseScale + offsetY);
            }
        }

        return noiseMap;
    }

    private void DrawTerrainMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (!cell.isWater)
                {
                    Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                    Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                    Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                    Vector3 d = new Vector3(x + .5f, 0, y - .5f);
                    Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
                    Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                    Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                    Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[k]);
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }
    private void DrawEdgeMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                Cell cell = grid[i, j];
                if (cell.isWater == false)
                {
                    if (i > 0)
                    {
                        Cell left = grid[i - 1, j];
                        if (left.isWater)
                        {
                            Vector3 a = new Vector3(i - .5f, 0, j + .5f);
                            Vector3 b = new Vector3(i - .5f, 0, j - .5f);
                            Vector3 c = new Vector3(i - .5f, -1, j + .5f);
                            Vector3 d = new Vector3(i - .5f, -1, j - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }

                    }
                    if (i < size - 1)
                    {
                        Cell right = grid[i + 1, j];
                        if (right.isWater)
                        {
                            Vector3 a = new Vector3(i + .5f, 0, j - .5f);
                            Vector3 b = new Vector3(i + .5f, 0, j + .5f);
                            Vector3 c = new Vector3(i + .5f, -1, j - .5f);
                            Vector3 d = new Vector3(i + .5f, -1, j + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (j > 0)
                    {
                        Cell down = grid[i, j - 1];
                        if (down.isWater)
                        {
                            Vector3 a = new Vector3(i - .5f, 0, j - .5f);
                            Vector3 b = new Vector3(i + .5f, 0, j - .5f);
                            Vector3 c = new Vector3(i - .5f, -1, j - .5f);
                            Vector3 d = new Vector3(i + .5f, -1, j - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (j < size - 1)
                    {
                        Cell up = grid[i, j + 1];
                        if (up.isWater)
                        {
                            Vector3 a = new Vector3(i + .5f, 0, j + .5f);
                            Vector3 b = new Vector3(i - .5f, 0, j + .5f);
                            Vector3 c = new Vector3(i + .5f, -1, j + .5f);
                            Vector3 d = new Vector3(i - .5f, -1, j + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject edge = new GameObject("Edge");
        edge.transform.SetParent(transform);

        MeshFilter filter = edge.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer r = edge.AddComponent<MeshRenderer>();
        r.material = edgeMaterial;

    }
    private void SpawnTree(Cell[,] grid)
    {
        for(int j = 0; j < size; j++)
        {
            for(int i = 0; i < size; i++)
            {
                if(grid[i,j].isWater == false)
                {
                    var a = Random.Range(0f, 1.0f);
                    if (a < 0.05)
                    {
                        Instantiate(tree, new Vector3(i, 0, j), Quaternion.identity);
                    }
                }
            }
        }
    }
    /*
    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            return;
        }
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {

                Cell c = grid[i, j];
                if (c.isWater)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(new Vector3(i, 0, j), Vector3.one);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(new Vector3(i, 0, j), Vector3.one);
                }
            }
        }

    }
    */
}
    

