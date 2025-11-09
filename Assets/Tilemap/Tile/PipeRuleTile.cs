using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Pipe Rule Tile")]
public class CustomRuleTile : RuleTile<CustomRuleTile.Neighbor> {
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Nothing = 3;
        public const int Anything = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Nothing: return tile == null;
            case Neighbor.Anything: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }
}