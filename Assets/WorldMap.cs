using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    [SerializeField] public int WorldHeight = 50;
    [SerializeField] public int WorldWidth = 30;
    [SerializeField] public int WorldLength = 30;
    [SerializeField][Range(0.00001f, 50f)] public float Scale = 0.3f;
    [SerializeField] public int Octaves = 10;
    [SerializeField] public float Persistance = 1;
    [SerializeField] public float Lacunarity = 1;

    public bool AutoUpdate;

    private WorldCell[,,] worldCells;
    private float[,] noiseMap;
    private Vector3[] vertices = new Vector3[]
                    {
                        //front face
                        new Vector3(-0.5f, 0.5f, 0.5f), //left top front 0
                        new Vector3(0.5f, 0.5f, 0.5f), //right top front 1
                        new Vector3(-0.5f, -0.5f, 0.5f), // left bottom front 2
                        new Vector3(0.5f, -0.5f, 0.5f), //right bottom front 3

                        //back face
                        new Vector3(0.5f, 0.5f, -0.5f), //right top back 4
                        new Vector3(-0.5f, 0.5f, -0.5f), //left top back 5
                        new Vector3(0.5f, -0.5f, -0.5f), // right bottom back 6
                        new Vector3(-0.5f, -0.5f, -0.5f), //left bottom back 7

                        //left face
                        new Vector3(-0.5f, 0.5f, -0.5f),//left top back 8
                        new Vector3(-0.5f, 0.5f, 0.5f),//left top front 9
                        new Vector3(-0.5f, -0.5f, -0.5f),//left bottom back 10
                        new Vector3(-0.5f, -0.5f, 0.5f),//left bottom front 11

                        //right face
                        new Vector3(0.5f, 0.5f, 0.5f),//right top front 12
                        new Vector3(0.5f, 0.5f, -0.5f),//right top back 13
                        new Vector3(0.5f, -0.5f, 0.5f),//right bottom front 14
                        new Vector3(0.5f, -0.5f, -0.5f),//right bottom back 15

                        //top face
                        new Vector3(-0.5f, 0.5f, -0.5f),//left top back 16
                        new Vector3(0.5f, 0.5f, -0.5f),//right top back 17
                        new Vector3(-0.5f, 0.5f, 0.5f),//left top front 18
                        new Vector3(0.5f, 0.5f, 0.5f),//right top front 19

                        //bottom face
                        new Vector3(-0.5f, -0.5f, 0.5f),//left bottom front 20
                        new Vector3(0.5f, -0.5f, 0.5f),//right bottom front 21
                        new Vector3(-0.5f, -0.5f, -0.5f),//left bottom back 22
                        new Vector3(0.5f, -0.5f, -0.5f),//right bottom back 23


                    };
    private Vector2[] uvs = new Vector2[]
                    {
                        //front face
                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0)
                    };
    private MeshFilter meshFilter;

    protected class WorldCell
    {
        public float HeightValue {  get; set; }
        public Color Color { get; set; }

    }


    
    void Start()
    {
        this.AddComponent<MeshFilter>();
        this.AddComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        //InvokeRepeating("RenderWorldCombined", 0, 1);
        RenderWorldCombined();
    }
    void Update()
    {
        //float[,] noiseMap = GenerateNoiseMap(WorldWidth, WorldLength, Scale, Octaves, Persistance, Lacunarity);
        //WorldCell[,,] worldCells = GenerateMap(noiseMap);
        //RenderWorldCombined();
    }
    private void FixedUpdate()
    {
        float[,] noiseMap = GenerateNoiseMap(WorldWidth, WorldLength, Scale, Octaves, Persistance, Lacunarity);
        //WorldCell[,,] worldCells = GenerateMap(noiseMap);

        //RenderWorldCombined();
    }

    private static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity)
    {
        if (scale <= 0)
        {
            scale = 0.000001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float[,] noiseMap = new float[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for(int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency + (Time.deltaTime / 10);
                    float sampleY = y / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                } else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                noiseMap[x, y] = Mathf.Exp((noiseMap[x, y] - 1) * 2f) + 0.1f;
            }
        }

        return noiseMap;
    }

    private WorldCell[,,] GenerateMap(float[,] noiseMap)
    {
        WorldCell[,,] worldCells = new WorldCell[WorldWidth, WorldLength, WorldHeight];
        for (int h = 0; h < WorldHeight; h++)
        {
            for (int w = 0; w < WorldWidth; w++)
            {
                for (int l = 0; l < WorldLength; l++)
                {
                    if (h < noiseMap[w, l] * WorldHeight)
                    {
                        Color Color;
                        /*if (noiseMap[w, l] < 0.35)
                        {
                            float cl;
                            if (noiseMap[w, l] / 2 < 0.25)
                            {
                                cl = noiseMap[w, l] / 8;
                            } else
                            {
                                cl = noiseMap[w, l] * 2;
                            }
                            Color = Color.Lerp(new Color(0.184f, 0.729f, 0.843f), new Color(0.149f, 0.298f, 0.404f), cl);
                            
                        } else
                        {
                            Color = new Color(0.831f, 0.851f, 0.322f);
                        }*/
                        Color = Color.Lerp(new Color(0.831f, 0.851f, 0.322f), new Color(0.529f, 0.541f, 0.239f), Mathf.Pow(noiseMap[w, l] + 0.2f, 3) - 0.1f);
                        worldCells[w, l, h] = new WorldCell() {
                            HeightValue = noiseMap[w, l],
                            Color = Color
                        };
                    }
                    else if((float)h < (float)WorldHeight / 2f)
                    {
                        Color Color;
                        Color = Color.Lerp(new Color(0.184f, 0.729f, 0.843f), new Color(0.149f, 0.298f, 0.404f), noiseMap[w, l]);
                        worldCells[w, l, h] = new WorldCell()
                        {
                            HeightValue = noiseMap[w, l],
                            Color = Color
                        };
                    }
                }
            }
        }
        return worldCells;
    }

    public void RenderWorldCombined()
    {
        float[,] noiseMap = GenerateNoiseMap(WorldWidth, WorldLength, Scale, Octaves, Persistance, Lacunarity);
        WorldCell[,,] worldCells = GenerateMap(noiseMap);

        var combine = new List<CombineInstance>();

        Mesh mesh = new Mesh();

        Texture2D texture = new Texture2D(WorldWidth, WorldLength);

        for (int h = 0; h < WorldHeight; h++)
        {
            for (int w = 0; w < WorldWidth; w++)
            {
                for (int l = 0; l < WorldLength; l++)
                {
                    if(worldCells[w, l, h] == null)
                    {
                        continue;
                    }
                    bool hasNeighborH = (h > 0 && h < WorldHeight - 1 && worldCells[w, l, h - 1] != null && worldCells[w, l, h + 1] != null);
                    bool hasNeighborW = (w > 0 && w < WorldWidth - 1 && worldCells[w - 1, l, h] != null && worldCells[w + 1, l, h] != null);
                    bool hasNeighborL = (l > 0 && l < WorldLength - 1 && worldCells[w, l - 1, h] != null && worldCells[w, l + 1, h] != null);

                    if (hasNeighborH && hasNeighborW && hasNeighborL)
                    {
                        continue;
                    }

                    //Color color = Color.Lerp(Color.black, Color.white, noiseMap[w, l]);
                    /*Color color = Color.black;
                    
                    if (h <  WorldHeight / 2)
                    {
                        color = Color.Lerp(new Color(0.184f, 0.729f, 0.843f), new Color(0.149f, 0.298f, 0.404f), noiseMap[w, l]);
                    }*/

                    texture.SetPixel(w, l, worldCells[w, l, h].Color);

                    Vector3 position = new Vector3(w, h, l);

                    List<int> trianglesList = new List<int>();

                    List<Vector2> uvs = new List<Vector2>();

                    float xStart = (w) / (float)WorldWidth;
                    float xEnd = (w + 1f) / (float)WorldWidth;
                    float yStart = (l) / (float)WorldLength;
                    float yEnd = (l + 1f) / (float)WorldLength;

                    Debug.Log(xStart + "  " + yStart + "  " + Color.Lerp(Color.black, Color.white, noiseMap[w, l]));
                    

                    List<Vector2> uvAdd = new List<Vector2>()
                    {
                        /*new Vector2(xStart, xEnd),
                        new Vector2(yStart, yEnd),
                        new Vector2(xStart, xEnd),
                        new Vector2(yStart, yEnd)*/
                        /*new Vector2(xStart, yEnd),
                        new Vector2(yStart, yEnd),
                        new Vector2(xStart, xEnd),
                        new Vector2(yStart, xEnd)*/
                        new Vector2(xStart, yStart),
                        new Vector2(xStart, yStart),
                        new Vector2(xStart, yStart),
                        new Vector2(xStart, yStart)
                        /*new Vector2(yStart, xStart),
                        new Vector2(yStart, xStart),
                        new Vector2(yStart, xStart),
                        new Vector2(yStart, xStart)*/
                    };

                    if (w == 0 || w - 1 >= 0 && worldCells[w - 1, l, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 8, 10, 11, 11, 9, 8 });
                    }
                    if (w == WorldWidth - 1 || w + 1 < WorldWidth && worldCells[w + 1, l, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 12, 14, 15, 15, 13, 12 });
                    }
                    if (l == 0 || l - 1 >= 0 && worldCells[w, l - 1, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 4, 6, 7, 7, 5, 4 });
                    }
                    if (l == WorldLength - 1 || l + 1 < WorldLength && worldCells[w, l + 1, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 0, 2, 3, 3, 1, 0 });
                    }
                    if (h == WorldHeight - 1 || h + 1 < WorldHeight && worldCells[w, l, h + 1] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 16, 18, 19, 19, 17, 16 });
                    }

                    int[] triangles = trianglesList.ToArray();

                    for(int i = 0; i < 6; i++)
                    {
                        uvs.AddRange(uvAdd);
                    }
                    
                    Mesh meshToCombine = new Mesh();
                    meshToCombine.Clear();
                    meshToCombine.vertices = vertices;
                    meshToCombine.triangles = triangles;
                    meshToCombine.uv = uvs.ToArray();
                    meshToCombine.Optimize();
                    meshToCombine.RecalculateNormals();

                    combine.Add(new CombineInstance()
                    {
                        mesh = meshToCombine,
                        transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one)
                    });
                }

            }
        }

        texture.Apply();
        
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine.ToArray());
        meshFilter.mesh = mesh;
        Material material = new Material(Shader.Find("Custom/MapShader"));
        material.mainTexture = texture;
        this.GetComponent<Renderer>().material = material;
    }

    public void RenderWorld()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GameObject wordlCellPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(wordlCellPrefab.GetComponent<BoxCollider>());

        float[,] noiseMap = GenerateNoiseMap(WorldWidth, WorldLength, Scale, Octaves, Persistance, Lacunarity);
        WorldCell[,,] worldCells = GenerateMap(noiseMap);
        

        for (int h = 0; h < WorldHeight; h++)
        {
            for (int w = 0; w < WorldWidth; w++)
            {
                for (int l = 0; l < WorldLength; l++)
                {
                    bool hasNeighborH = (h > 0 && h < WorldHeight - 1 && worldCells[w, l, h - 1] != null && worldCells[w, l, h + 1] != null);
                    bool hasNeighborW = (w > 0 && w < WorldWidth - 1 && worldCells[w - 1, l, h] != null && worldCells[w + 1, l, h] != null);
                    bool hasNeighborL = (l > 0 && l < WorldLength - 1 && worldCells[w, l - 1, h] != null && worldCells[w, l + 1, h] != null);

                    if (hasNeighborH && hasNeighborW && hasNeighborL || worldCells[w, l, h] == null)
                    {
                        continue;
                    }

                    Material material = new Material(Shader.Find("Standard"));
                    material.color = Color.Lerp(Color.black, Color.white, noiseMap[w, l]);
                    wordlCellPrefab.GetComponent<Renderer>().material = material;

                    Vector3 position = new Vector3(w, h, l);
                    GameObject worldCell = Instantiate(wordlCellPrefab, position, Quaternion.identity);
                    worldCell.transform.parent = this.transform;
                    Destroy(worldCell.GetComponent<BoxCollider>());
                    MeshRenderer meshRenderer = worldCell.GetComponent<MeshRenderer>();
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    meshRenderer.receiveShadows = false;
                    meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                    



                    Vector3[] vertices = new Vector3[]
                    {
                        //front face
                        new Vector3(-0.5f, 0.5f, 0.5f), //left top front 0
                        new Vector3(0.5f, 0.5f, 0.5f), //right top front 1
                        new Vector3(-0.5f, -0.5f, 0.5f), // left bottom front 2
                        new Vector3(0.5f, -0.5f, 0.5f), //right bottom front 3

                        //back face
                        new Vector3(0.5f, 0.5f, -0.5f), //right top back 4
                        new Vector3(-0.5f, 0.5f, -0.5f), //left top back 5
                        new Vector3(0.5f, -0.5f, -0.5f), // right bottom back 6
                        new Vector3(-0.5f, -0.5f, -0.5f), //left bottom back 7

                        //left face
                        new Vector3(-0.5f, 0.5f, -0.5f),//left top back 8
                        new Vector3(-0.5f, 0.5f, 0.5f),//left top front 9
                        new Vector3(-0.5f, -0.5f, -0.5f),//left bottom back 10
                        new Vector3(-0.5f, -0.5f, 0.5f),//left bottom front 11

                        //right face
                        new Vector3(0.5f, 0.5f, 0.5f),//right top front 12
                        new Vector3(0.5f, 0.5f, -0.5f),//right top back 13
                        new Vector3(0.5f, -0.5f, 0.5f),//right bottom front 14
                        new Vector3(0.5f, -0.5f, -0.5f),//right bottom back 15

                        //top face
                        new Vector3(-0.5f, 0.5f, -0.5f),//left top back 16
                        new Vector3(0.5f, 0.5f, -0.5f),//right top back 17
                        new Vector3(-0.5f, 0.5f, 0.5f),//left top front 18
                        new Vector3(0.5f, 0.5f, 0.5f),//right top front 19

                        //bottom face
                        new Vector3(-0.5f, -0.5f, 0.5f),//left bottom front 20
                        new Vector3(0.5f, -0.5f, 0.5f),//right bottom front 21
                        new Vector3(-0.5f, -0.5f, -0.5f),//left bottom back 22
                        new Vector3(0.5f, -0.5f, -0.5f),//right bottom back 23


                    };
                    /*int[] triangles = new int[]
                    {
                        //front face
                        0, 2, 3, //first triangle
                        3, 1, 0, //second triangle

                        //back face
                        4, 6, 7, //first triangle
                        7, 5, 4, //second triangle

                        //left face
                        8, 10, 11, //first triangle
                        11, 9, 8, //second triangle

                        //right face
                        12, 14, 15, //first triangle
                        15, 13, 12, //second triangle

                        //top face
                        16, 18, 19, //first triangle
                        19, 17, 16, //second triangle

                        //bottom face
                        20, 22, 23, //first triangle
                        23, 21, 20 //second triangle

                    };*/

                    List<int> trianglesList = new List<int>();

                    if (w == 0 || w - 1 >= 0 && worldCells[w - 1, l, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 8, 10, 11, 11, 9, 8 });
                    }
                    if(w == WorldWidth - 1 || w + 1 < WorldWidth && worldCells[w + 1, l, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 12, 14, 15, 15, 13, 12 });
                    }
                    if (l == 0 || l - 1 >= 0 && worldCells[w, l - 1, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 4, 6, 7, 7, 5, 4 });

                    }
                    if (l == WorldLength - 1 || l + 1 < WorldLength && worldCells[w, l + 1, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 0, 2, 3, 3, 1, 0 });
                    }
                    if (h == WorldHeight - 1 || h + 1 < WorldHeight && worldCells[w, l, h + 1] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 16, 18, 19, 19, 17, 16 });
                    }

                    int[] triangles = trianglesList.ToArray(); 


                    //UVs
                    Vector2[] uvs = new Vector2[]
                    {
                        //front face
                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0)
                    };

                    Mesh mesh = new Mesh();
                    mesh.Clear();
                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.uv = uvs;
                    mesh.Optimize();
                    mesh.RecalculateNormals();

                    worldCell.GetComponent<MeshFilter>().mesh = mesh;

                }
            }
        }
    }

    private void RenderBatches()
    {
        for (int h = 0; h < WorldHeight; h++)
        {
            for (int w = 0; w < WorldWidth; w++)
            {
                for (int l = 0; l < WorldLength; l++)
                {
                    bool hasNeighborH = (h > 0 && h < WorldHeight - 1 && worldCells[w, l, h - 1] != null && worldCells[w, l, h + 1] != null);
                    bool hasNeighborW = (w > 0 && w < WorldWidth - 1 && worldCells[w - 1, l, h] != null && worldCells[w + 1, l, h] != null);
                    bool hasNeighborL = (l > 0 && l < WorldLength - 1 && worldCells[w, l - 1, h] != null && worldCells[w, l + 1, h] != null);

                    if (hasNeighborH && hasNeighborW && hasNeighborL || worldCells[w, l, h] == null)
                    {
                        continue;
                    }

                    Vector3[] vertices = new Vector3[]
                    {
                        //front face
                        new Vector3(-0.5f, 0.5f, 0.5f), //left top front 0
                        new Vector3(0.5f, 0.5f, 0.5f), //right top front 1
                        new Vector3(-0.5f, -0.5f, 0.5f), // left bottom front 2
                        new Vector3(0.5f, -0.5f, 0.5f), //right bottom front 3

                        //back face
                        new Vector3(0.5f, 0.5f, -0.5f), //right top back 4
                        new Vector3(-0.5f, 0.5f, -0.5f), //left top back 5
                        new Vector3(0.5f, -0.5f, -0.5f), // right bottom back 6
                        new Vector3(-0.5f, -0.5f, -0.5f), //left bottom back 7

                        //left face
                        new Vector3(-0.5f, 0.5f, -0.5f),//left top back 8
                        new Vector3(-0.5f, 0.5f, 0.5f),//left top front 9
                        new Vector3(-0.5f, -0.5f, -0.5f),//left bottom back 10
                        new Vector3(-0.5f, -0.5f, 0.5f),//left bottom front 11

                        //right face
                        new Vector3(0.5f, 0.5f, 0.5f),//right top front 12
                        new Vector3(0.5f, 0.5f, -0.5f),//right top back 13
                        new Vector3(0.5f, -0.5f, 0.5f),//right bottom front 14
                        new Vector3(0.5f, -0.5f, -0.5f),//right bottom back 15

                        //top face
                        new Vector3(-0.5f, 0.5f, -0.5f),//left top back 16
                        new Vector3(0.5f, 0.5f, -0.5f),//right top back 17
                        new Vector3(-0.5f, 0.5f, 0.5f),//left top front 18
                        new Vector3(0.5f, 0.5f, 0.5f),//right top front 19

                        //bottom face
                        new Vector3(-0.5f, -0.5f, 0.5f),//left bottom front 20
                        new Vector3(0.5f, -0.5f, 0.5f),//right bottom front 21
                        new Vector3(-0.5f, -0.5f, -0.5f),//left bottom back 22
                        new Vector3(0.5f, -0.5f, -0.5f),//right bottom back 23
                    };

                    List<int> trianglesList = new List<int>();

                    if (w == 0 || w - 1 >= 0 && worldCells[w - 1, l, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 8, 10, 11, 11, 9, 8 });
                    }
                    if (w == WorldWidth - 1 || w + 1 < WorldWidth && worldCells[w + 1, l, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 12, 14, 15, 15, 13, 12 });
                    }
                    if (l == 0 || l - 1 >= 0 && worldCells[w, l - 1, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 4, 6, 7, 7, 5, 4 });

                    }
                    if (l == WorldLength - 1 || l + 1 < WorldLength && worldCells[w, l + 1, h] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 0, 2, 3, 3, 1, 0 });
                    }
                    if (h == WorldHeight - 1 || h + 1 < WorldHeight && worldCells[w, l, h + 1] == null)
                    {
                        trianglesList.AddRange(new List<int>() { 16, 18, 19, 19, 17, 16 });
                    }

                    int[] triangles = trianglesList.ToArray();


                    //UVs
                    Vector2[] uvs = new Vector2[]
                    {
                        //front face
                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),

                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0)
                    };

                    Mesh mesh = new Mesh();
                    mesh.Clear();
                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.uv = uvs;
                    mesh.Optimize();
                    mesh.RecalculateNormals();

                    Material material = new Material(Shader.Find("Standard"));
                    material.color = Color.Lerp(Color.black, Color.white, noiseMap[w, l]);
                    material.enableInstancing = true;

                    Graphics.DrawMeshInstanced(mesh, 0, material, new List<Matrix4x4>() { Matrix4x4.TRS(new Vector3(w, h, l), Quaternion.identity, Vector3.one) });
                }
            }
        }
    }

}