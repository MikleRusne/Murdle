using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Tiles;
using UnityEngine;

public class TurnGreenOnPlayerTurn : MonoBehaviour
{
    public Orchestrator _orchestrator;

    public MeshRenderer _mesh;

    public Color PlayerTurnColor;

    public Color DefaultColor;

    public Material TileMaterial;
    
    public Color IsInPathMaterial;

    public BlockLinks[] PathList;

    public Character Player;

    public BlockLinks bl;
    public bool isInPath = false;
    void Start()
    {
        _orchestrator = Orchestrator.inst;
        _orchestrator.TurnMade.AddListener(ChangeColor);
        _mesh = transform.Find("TileMesh").GetComponent<MeshRenderer>();
        TileMaterial = _mesh.material;
        bl = this.GetComponent<BlockLinks>();
        //Store a reference to the player and its pathlist
         Player = Spawner.inst.Player;
    }

    

    public void ChangeColor(bool PlayerTurn, int TotalTicks)
    {
        Debug.Log("Changing color maybe");
        PathList = Player.PathList;
        bool changed = false;
        //First assign player turn material
        if (PlayerTurn)
        {
            TileMaterial.SetFloat("_PlayerTurn",1.0f);
            changed = true;
        }
        else
        {
            TileMaterial.SetFloat("_PlayerTurn",0.0f);
            
        }
        if (isInPath)
        {
            TileMaterial.SetFloat("_PlayerPath",1.0f);
            changed = true;
        }
        else
        {
            TileMaterial.SetFloat("_PlayerPath", 0.0f);
        }
        // if(!changed){
        // _mesh.material = DefaultMaterial;
        // }
        
        isInPath = false;
    }
}
