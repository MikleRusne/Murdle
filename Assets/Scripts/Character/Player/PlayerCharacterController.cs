using System.Collections;
using System.Collections.Generic;
using Characters;
using GameActions;
using Tiles;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    private Character PlayerCharacter;
    // Start is called before the first frame update
    void Start()
    {
        PlayerCharacter = transform.GetComponent<Character>();
        PlayerCharacter.TurnFaction = EFaction.Player;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                Debug.Log(hit.transform.name);
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject.name == "TileMesh")
                {
                    GameObject hitTile = hitObject.transform.parent.gameObject;
                    Debug.Log(hitTile);
                    BlockLinks LinkInfo = (BlockLinks) hitTile.GetComponent<BlockLinks>();
                    
                    Debug.Log(LinkInfo.MyID);
                    Spawner.inst.ChangeToBlock(LinkInfo.MyID);  
                }
                else
                {
                    GameObject hitTile = hitObject.transform.gameObject;
                    Debug.Log(hitTile);
                    BlockLinks LinkInfo = (BlockLinks) hitTile.GetComponent<BlockLinks>();
                    
                    Debug.Log(LinkInfo.MyID);
                    Spawner.inst.ChangeToTile(LinkInfo.MyID);  
                }
            }
            // Debug.Log("Clicked");
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                // Debug.Log(hit.transform.name);
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject.name == "TileMesh")
                {
                    GameObject hitTile = hitObject.transform.parent.gameObject;
                    // PlayerCharacter.MoveTo(hitTile);
                    BlockLinks bl = hitTile.GetComponent<BlockLinks>();
                    Debug.Log("Trying to get to " + bl.MyID);
                    PlayerCharacter.MoveTo(bl.MyID);
                    // Orchestrator.inst.EnqueueAction(new PathAndMoveToAction(PlayerCharacter, EFaction.Player, bl.MyID));
                }
            }

        }

        if (Input.GetKeyDown("space"))
        {
            // Pathfinder.Progress();

        }

        if (Input.GetKeyDown("x"))
        {
            // MoveOnPath();

        }

    }
}
