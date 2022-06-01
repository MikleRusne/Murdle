using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;
using Tiles;
using UnityEngine;
using UnityEngine.Events;
namespace Characters
{

    
// Under construction
// Uses Spawner and ManhattanDistance instead

public class AStar : MonoBehaviour
{
    public UnityEvent ProcessingInitialized;
    public UnityEvent<bool> ProcessingFinished;

    public Queue<Node> Open = new Queue<Node>();
    public List<GameObject> OpenObjects = new List<GameObject>();
    public List<(int y, int x)> OpenLocations = new List<(int y, int x)>();
    private Priority_Queue.SimplePriorityQueue<Node> OpenPQueue = new SimplePriorityQueue<Node>();
    
    public List<(int y, int x)> Closed = new List<(int, int)>();

    public (int, int) Target;

    public bool PathFound;
    public List<Node> Path = new List<Node>();

    public List<(int, int)>PathLocs = new List<(int, int)>();

    public float Distance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }
    public void Initialize((int, int) Start, (int, int) Target)
    {
        // Debug.Log("From " + Start.transform.name + " , setting target as: "+ Target.transform.name);
        this.Target = Target;
        reset();
        // PathFound = false;
        var FirstNode = new Node(Spawner.inst.GetTileFromLocation(Start).BL, null,0,
            Spawner.ManhattanDistance(Start, Target));
        
        Open.Enqueue(FirstNode);
        OpenPQueue.Enqueue(FirstNode, FirstNode.f);
        // OpenObjects.Add(Start);
        FindPath();
    }

    public void PrintQueues()
    {
        Debug.Log("Open list of length:" + Open.Count);
        
        StringBuilder sb = new StringBuilder();
        foreach (var node in Open)
        {
            // sb.AppendLine(node.TileObject.name);
        }
        Debug.Log(sb.ToString());
        Debug.Log("Closed list");
        sb = new StringBuilder();
        foreach (var node in Closed)
        {
            // sb.AppendLine(node.transform.name);
        }
        Debug.Log(sb.ToString());

    }

    public void reset()
    {
        PathLocs = new List<(int, int)>();
        Open = new Queue<Node>();
        OpenPQueue = new SimplePriorityQueue<Node>();
        OpenObjects = new List<GameObject>();
        OpenLocations = new List<(int y, int x)>();
        Closed= new List<(int, int)>();
        PathFound = false;
        Path= new List<Node>();

    }

    public async Task FindPath()
    {
        PathFound = false;
        while (!Progress())
        {
            // Debug.Log("Waiting for processing");
            await Task.Yield();
        } 
        if (ProcessingFinished != null)
        {
            Debug.Log("Processing Finished");
            ProcessingFinished.Invoke(PathFound);
        }
        // else
        // {
        //     Debug.Log("Nothing Subscribed to Processing finished");
        // }
        
    }
    
    //Returns true if processing finished, false if more needed
    public bool Progress()
    {
        if (PathFound)
        {
            return true;
        }
        
        
        if (OpenPQueue.Count == 0)
        {
            PathFound = false;
            return true;
        }

        // Node CurrentNode = Open.Dequeue();
        Node CurrentNode = OpenPQueue.Dequeue();

        if (CurrentNode.loc == Target)
        {
            PathFound = true;
            // Debug.Log("Found path to be:");
            StringBuilder sb = new StringBuilder();
            while (CurrentNode != null)
            {
                sb.Append(CurrentNode.loc + ", ");
                Path.Add(CurrentNode);
                PathLocs.Add(CurrentNode.loc);
                CurrentNode = CurrentNode.PrevNode;
            }

            PathLocs.Reverse();
            Path.Reverse();
            // Debug.Log(sb.ToString());
            PathFound = true;
            return true;
        }
        
        //Under construction
        // Debug.Log("Analyzing "+ CurrentNode.loc);
        BlockLinks bl = CurrentNode.TileBlockLinks;
        foreach (BlockLinks.ELinkDirection direction in Enum.GetValues(typeof(BlockLinks.ELinkDirection)))
        {
            //Adds neighbors of current node to open list
            
            //Check if
            // It is connected in that direction
            // The connection is a tile
            // It is not in closed list
            var targetnebber = Spawner.inst.DirectionOntoLocation(bl.MyID, direction);
            if (!bl.connected[direction]|| !(bl.Neighbors[direction].Type == Spawner.ETileType.Tile)||
                Closed.Contains(targetnebber))
            {
                continue;
            }
            
            if (!OpenLocations.Contains(targetnebber))
            {
                // Debug.Log("Enqueing: ");
                OpenLocations.Add(targetnebber);

                var newNode = new Node(Spawner.inst.GetTileFromLocation(targetnebber).BL, CurrentNode, CurrentNode.g+1, 
                    Spawner.ManhattanDistance(targetnebber, Target));
                OpenPQueue.Enqueue(newNode, newNode.f);
            }
            else
            {
                // Debug.Log("Open locations already contains " + targetnebber);
            }
        }
        Closed.Add(bl.MyID);
        return false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public BlockLinks[] PrintPath()
    {
        // Debug.Log("Path was found to be:");
        StringBuilder sb = new StringBuilder();

        foreach (var gObj in Path)
        {
            // sb.AppendLine(gObj.TileObject.name);
        }

        // Debug.Log(sb.ToString());
        // Debug.Log("It costs g:" + Path.Last().g);
        // Debug.Log("Path object list is " + Path.Skip(0).Select((node)=>node.TileObject).ToList());
        // return Path.Select((node)=>node.TileObject).ToArray();
        // return null;
        // Debug.Log("Returning Path");
        foreach (var node in Path)
        {
            // Debug.Log(node.loc);
        }
        return Path.Select((node)=>node.TileBlockLinks).ToArray();
    }
}

}

