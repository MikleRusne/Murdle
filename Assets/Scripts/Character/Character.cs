using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameActions;
using Tiles;
using UnityEngine;


public enum EFaction
{
    Player,            //Hurts enemies and neutral, acts on Player Turn only
    Enemy,             //Hurts player and neutrak, acts on Enemy Turn only
    Neutral,           //Hurts regardless of faction, acts every turn
    Chaos              //Also hurts regardless of faction, acts every turn, might phase out later
}



namespace Characters
{
    #region State Enumerator
    public enum ECharacterState
    {
        Moving,
        Stationary
    }
    
    public enum EPFState
    {
        Idle,
        Searching,
        PathFound
    }
    #endregion

    
    public class Character : MonoBehaviour
{
    // [HideInInspector]
    
    #region Main components
    public BlockLinks CurrentBlockLinks;
    public GameObject _CurrentTile;
    public GameObject CurrentTile
    {
        get { return _CurrentTile;}
        set
        {
            CurrentBlockLinks.isOccupied = false;
            _CurrentTile = value;
            CurrentBlockLinks = value.GetComponent<BlockLinks>();
            CurrentBlockLinks.isOccupied = true;
            CurrentBlockLinks.OccupiedBy = this;
        }
    }

    [SerializeField]
    public GameObject StartingTile;

    
    public MovementComponent MovementComponent { get; set; }

    public GameObject NextTile = null;
    public AStar Pathfinder;

    public IEnumerator Path = null;

    public ECharacterState CharacterState = ECharacterState.Stationary;
    
    #endregion

    public LinkedList<Directive> CurrentDirectives = new LinkedList<Directive>();
    void Start()
    
    {
        if(StartingTile!= null)
        {
            // _CurrentTile = StartingTile;
            CurrentBlockLinks = StartingTile.GetComponent<BlockLinks>();
            CurrentTile = StartingTile;
            // Pathfinder = CurrentTile.GetComponent<AStar>();
            Vector3 AlignedPosition = CurrentTile.transform.GetChild(5).position;
            MovementComponent = GetComponent<MovementComponent>();
        
            this.transform.position = AlignedPosition;
        }

        Pathfinder = this.GetComponent<AStar>();
        Orchestrator.inst.TurnMade.AddListener(OnTurnMade);
        // Spawner.inst.TileChangedEvent.AddListener(OnTileChange);
    }

    void PushMyNextAction()
    {
        if (CurrentDirectives.Count > 0)
        {
            var DirectiveToBeChallenged = CurrentDirectives.First.Value;
            if (DirectiveToBeChallenged.Dirty)
            {
                DirectiveToBeChallenged.FixDirty();
            }
            GameAction MyNextAction = DirectiveToBeChallenged.GetNextAction();
            DirectiveToBeChallenged.IncrementAction();
            Orchestrator.inst.EnqueueAction(MyNextAction);
            //If it is complete now then remove
            if (DirectiveToBeChallenged.isComplete)
            {
                CurrentDirectives.RemoveFirst();
            }
        }
    }
    
    #region Event listeners
    void OnTurnMade(bool PlayerTurn, int TotalTicks)
    {
        //Push the action on the queue to the orchestrator

        switch (TurnFaction)
        {
            case EFaction.Player:
                if(PlayerTurn) {PushMyNextAction(); }
                break;
            case EFaction.Enemy:
                if(!PlayerTurn) {PushMyNextAction();}
                break;
            case EFaction.Chaos:
            case EFaction.Neutral:
                PushMyNextAction();
                break;
        }
       
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void OnMovementFinished()
    {
        CurrentTile = NextTile;
        CharacterState = ECharacterState.Stationary;
        // Debug.Log("At tile " + CurrentTile.name);
        //If there was an older move queued up, now is the time to do it
    }
   #endregion 
    
   # region Pathfinding and movement
   // async Task<bool> MoveOnPath()
   //  {
   //      //Get next tile
   //      //Move to the next tile
   //      NextTile = (GameObject) Path.Current;
   //      CharacterState = ECharacterState.Moving;
   //      await MovementComponent.StartMoving(this.transform.position, this.transform.rotation,
   //          NextTile.transform.GetChild(5).transform.position);
   //      if (Path == null || !Path.MoveNext())
   //      {
   //          // Debug.Log("Path is null or no more steps");
   //          MovementComponent.MovementFinished.RemoveListener(OnMovementFinished);
   //          return false;
   //      }
   //      return true;
   //  }
    
    

    public BlockLinks[] PathList;

    #endregion
    // public LinkedList<GameAction> PendingActions = new LinkedList<GameAction>();
    // public void OnProcessingFinished(bool Success)
    // {
    //     if (!Success)
    //     {
    //         return;
    //     }
    //     Pathfinder.ProcessingFinished.RemoveListener(OnProcessingFinished);
    //     PathList = Pathfinder.PrintPath();
    //     Path = PathList.GetEnumerator();
    //     MoveAction[] actions = SplitPathIntoActions(PathList);
    //     foreach (var action in actions)
    //     {
    //         // PendingActions.AddLast(action);
    //     }
    //     
    //     CurrentDirectives.AddFirst(new DeliberateMoveDirective(this, PathList, null));
    // }

    //If already moving, complete current move, then go to new target
    
    public void MoveTo((int y, int x) targetloc)
    {
        Spawner.FInstantiatedTile Target = Spawner.inst.GetTileFromLocation(targetloc).IT;
        if (Target.Type != Spawner.ETileType.Tile)
        {
            return;
        }

        
        switch (CharacterState)
        {
            case ECharacterState.Stationary:
                // Debug.Log("Moving from "+ CurrentTile.name + " to " + Target.Tile.name);
                Path = null;
                CurrentDirectives.AddFirst(new DeliberateMoveDirective(this, CurrentBlockLinks.MyID, targetloc));
                // PendingActions = new LinkedList<GameAction>();
                break;
            case ECharacterState.Moving:
                // PendingActions = new LinkedList<GameAction>();
                // PendingActions.AddFirst(new PathAndMoveToAction(this, TurnFaction, targetloc));
                break;
        }
        
    }
    
    
    public EFaction TurnFaction = EFaction.Chaos;

    public (int, int)[] loclist = new (int, int)[0];
    MoveAction[] SplitPathIntoActions(BlockLinks[] PathArray)
    {
        int TotalActions = PathArray.Length - 1;
        loclist = (from p in PathArray
            select p.MyID).ToArray();
        MoveAction[] temp = new MoveAction[TotalActions];
        for (int i = 0; i < TotalActions; ++i)
        {
            temp[i] = new MoveAction(this, TurnFaction, PathArray[i], PathArray[i + 1]);
            // Debug.Log(temp[i].ToString());
        }
        return temp;
    }
    
    
    // async Task MoveToGoal()
    // {
    //     // Debug.Log("Moving to goal");
    //     if (!Path.MoveNext()|| !Path.MoveNext())
    //     {
    //         Debug.Log("Path empty");
    //         return;
    //     }
    //
    //     MovementComponent.MovementFinished.AddListener(OnMovementFinished);
    //     bool isPathLeft = await MoveOnPath();
    //     while (isPathLeft)
    //     {
    //         isPathLeft = await MoveOnPath();
    //     }
    //     
    //     // Pathfinder = CurrentTile.GetComponent<AStar>();
    // }

    public void OnTileChange((int, int) location, Spawner.ETileType newType)
    {
        if (loclist.Contains(location))
        {
            Debug.Log("My path might be invalid");
            //Check which of my directives are movement related
            foreach (Directive currentDirective in CurrentDirectives)
            {
                if (currentDirective.DirectiveType == EDirectiveType.Movement)
                {
                    currentDirective.MarkDirty();
                } 
            }
        }
        else
        {
            Debug.Log("My path is still valid.");
        }
    }
}

}
