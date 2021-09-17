using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SiblingTileThing : RuleTile<SiblingTileThing.Neighbor>
{
    public List<TileBase> siblings = new List<TileBase>();

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Sibling = 3;
        //public const int Null = 4;
        //public const int NotNull = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.Sibling: return siblings.Contains(tile);
                //case Neighbor.Null: return tile == null;
                //case Neighbor.NotNull: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }


    //public int siblingGroup;
    //public class Neighbor : RuleTile.TilingRule.Neighbor
    //{
    //    public const int Sibing = 3;
    //}
    //public override bool RuleMatch(int neighbor, TileBase tile)
    //{
    //    SiblingTileThing myTile = tile as SiblingTileThing;
    //    switch (neighbor)
    //    {
    //        case Neighbor.Sibing: return myTile && myTile.siblingGroup == siblingGroup;
    //    }
    //    return base.RuleMatch(neighbor, tile);
    //}
}