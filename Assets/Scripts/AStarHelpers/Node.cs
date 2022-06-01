using System.Collections;
using System.Collections.Generic;
using Tiles;
using UnityEngine;

namespace Tiles
{
    public class Node
    {
        public float f;
        public float g;
        public (int, int) loc;
        public Node PrevNode;

        public BlockLinks TileBlockLinks;
        public Vector3 position;
        public Node(BlockLinks BL, Node prevNode, float g, float h=0.0f)
        {
            this.TileBlockLinks = BL;
            this.PrevNode = prevNode;
            this.g = g;
            this.f = g + h;
            this.loc = BL.MyID;
        }


    }

}
