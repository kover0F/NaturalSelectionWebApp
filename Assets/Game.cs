using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    [SerializeField] public bool genView = false;
    [SerializeField] [Range (0.0001f, 10f)] float speed = 0.5f;
    [SerializeField][Range(0f, 1f)] static public float energyGot = 0.1f;
    [SerializeField][Range(0f, 1f)] static public float energyLoss = 0.2f;

    protected abstract class Entity
    {
        private readonly ETile tile;
        
        
        protected Entity(ref ETile tile)
        {
            this.tile = tile;
        }
    }
    protected class Cell : Entity
    {
        private int[,] Gens;
        protected Cell(ref ETile tile) : base( ref tile)
        {
            
        }
    }
    protected class Gen
    {
        private delegate void Function();
    }
    protected class ETile
    {
        public Entity ContainedEntity;
        public ETile[] Neighbours
        {
            get
            {
                ETile[] neighbours = new ETile[4];
                if (Coords[1] - 1 < 0) { neighbours[0] = enviroment[Coords[0], enviroment.GetLength(0) - 1]; } else { neighbours[0] = enviroment[Coords[0], Coords[1] - 1]; }
                if (Coords[1] + 1 == enviroment.GetLength(0)) { neighbours[1] = enviroment[Coords[0], 0]; } else { neighbours[1] = enviroment[Coords[0], Coords[1] + 1]; }
                if (Coords[0] - 1 < 0) { neighbours[2] = enviroment[enviroment.GetLength(0) - 1, Coords[1]]; } else { neighbours[2] = enviroment[Coords[0] - 1, Coords[1]]; }
                if (Coords[0] + 1 == enviroment.GetLength(0)) { neighbours[3] = enviroment[0, Coords[1]]; } else { neighbours[3] = enviroment[Coords[0] + 1, Coords[1]]; }
                return neighbours;
            } }
        public int[] Coords { get; private set; }

        private ETile[,] enviroment; 
        
        public ETile(int[] coords, ETile[,] enviroment)
        {
            Coords = coords;
            this.enviroment = enviroment;
        }
    }
    class WorldTile
    {

        WorldTile[,] enviroment;
        public int[] Coords { get; set; }
        public int[,] Gens;
        public int GenLength = 50;
        public float Id;
        public int Energy;
        public int MaxEnergy = 100;
        public int ActiveGen;
        int energy;
        WorldTile[] neighbours;
        public enum TileType
        {
            Wood,
            Plant,
            None
        }
        public  TileType Type { get; set; } = TileType.None;

        public WorldTile(int[] coords, ref WorldTile[,] enviroment)
        {
            Type = TileType.None;
            Coords = coords;
            this.enviroment = enviroment;
            neighbours = new WorldTile[4];
            
            
        }
        public void BornPlant(float id, int[,] gens, int activeGen)
        {
            Type = TileType.Plant;
            Energy = (int)(MaxEnergy * 0.75);
            Id = id;
            Gens = gens;
            ActiveGen = activeGen;
        }
        void BornWood()
        {
            Type = TileType.Wood;
        }

        private void FindNeighbours()
        {
            if (Coords[1] - 1 < 0) { neighbours[0] = enviroment[Coords[0], enviroment.GetLength(0) - 1]; } else { neighbours[0] = enviroment[Coords[0], Coords[1] - 1]; }
            if (Coords[1] + 1 == enviroment.GetLength(0)) { neighbours[1] = enviroment[Coords[0], 0]; } else { neighbours[1] = enviroment[Coords[0], Coords[1] + 1]; }
            if (Coords[0] - 1 < 0) { neighbours[2] = enviroment[enviroment.GetLength(0) - 1, Coords[1]]; } else { neighbours[2] = enviroment[Coords[0] - 1, Coords[1]]; }
            if (Coords[0] + 1 == enviroment.GetLength(0)) { neighbours[3] = enviroment[0, Coords[1]]; } else { neighbours[3] = enviroment[Coords[0] + 1, Coords[1]]; }
        }
        private void PlantLifeCycle()
        {

        }
        public void LifeCycle()
        {
            FindNeighbours();

            if (Type == TileType.Plant && Coords[0] >= 1 && Coords[0] < enviroment.GetLength(0) - 1 && Coords[1] >= 1 && Coords[1] < enviroment.GetLength(0) - 1)
            {
                energy += (int)(MaxEnergy * energyGot);
                bool born = false;

                for(int i = 0; i < neighbours.Length; i++)
                {
                    if (neighbours[i].Type == TileType.None && Gens[ActiveGen, 0] < GenLength * 2 && energy > (int)(MaxEnergy * 0.2))
                    {
                        neighbours[i].BornPlant(Id, Gens, Gens[ActiveGen, i]);
                        born = true;
                    }
                }
                if (born) Type = TileType.Wood;

            }

            if(Type == TileType.Wood) energy -= (int)(MaxEnergy * energyLoss);

            if (energy <= 0) { Type = TileType.None; }

            for(int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i].energy < energy && neighbours[i].Type == TileType.Wood && Id == neighbours[0].Id)
                { neighbours[i].energy += (energy - neighbours[i].energy) / 2; energy -= (energy - neighbours[i].energy) / 2; }
            }

        }
        
    }

    GameObject plane;
    int WorldSize = 100;
    WorldTile[,] tiles;
    Texture2D image;

    // Start is called before the first frame update
    void Start()
    {
        image = new Texture2D(WorldSize, WorldSize);
        image.filterMode = FilterMode.Point;
        tiles = new WorldTile[WorldSize, WorldSize];

        for(int i = 0; i < WorldSize; i++)
        {
            for(int j = 0; j < WorldSize; j++)
            {
                tiles[i, j] = new WorldTile(new int[] { i, j }, ref tiles);
                if(Random.value < 0.001) {
                    int[,] gens = new int[tiles[i, j].GenLength, 4];
                    for(int k = 0; k < tiles[i, j].GenLength; k++)
                    {
                        for(int l = 0; l < 4; l++)
                        {
                            gens[k, l] = (int)(Random.Range(0, tiles[i, j].GenLength));
                        }
                    }
                    tiles[i, j].BornPlant(Random.value, gens, Random.Range(0, tiles[i, j].GenLength));
                }
            }
        }
        
        for(int i = 0; i < WorldSize; i++)
        {
            for(int j = 0; j < WorldSize; j++)
            {
                if (tiles[i, j].Type == WorldTile.TileType.Plant) { image.SetPixel(i, j, Color.green); }
                
            }
        }
        image.Apply();
        plane = GameObject.FindGameObjectWithTag("Map");

        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = image;

        Renderer renderer = plane.GetComponent<Renderer>();
        renderer.sharedMaterial = material;

        InvokeRepeating("GameTick", 0.0f, speed);
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    void GameTick()
    {
        int plants = 0;
        int woods = 0;
        int nones = 0;
        for (int i = 0; i < WorldSize; i++)
        {
            for (int j = 0; j < WorldSize; j++)
            {
                tiles[i, j].LifeCycle();
                if (tiles[i, j].Type == WorldTile.TileType.Plant) {
                    plants++;
                    if (genView)
                    {
                        image.SetPixel(i, j, new Color(Mathf.SmoothStep(0.2f, 0.8f, tiles[i, j].Id), 0.5f, tiles[i, j].Id));

                    } else image.SetPixel(i, j, Color.green);

                }
                else if (tiles[i, j].Type == WorldTile.TileType.Wood) {
                    woods++;
                    if (genView)
                    {
                        image.SetPixel(i, j, new Color(Mathf.SmoothStep(0.2f, 0.8f, tiles[i, j].Id), 0.5f, tiles[i, j].Id));

                    }
                    else image.SetPixel(i, j, Color.black);

                }
                else if (tiles[i, j].Type == WorldTile.TileType.None) { image.SetPixel(i, j, Color.white); nones++; }


            }
        }
        image.Apply();
        plane = GameObject.FindGameObjectWithTag("Map");

        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = image;

        Renderer renderer = plane.GetComponent<Renderer>();
        renderer.sharedMaterial = material;

        Debug.Log($"plants: {plants}; woods: {woods}; nones: {nones}");

    }
}
