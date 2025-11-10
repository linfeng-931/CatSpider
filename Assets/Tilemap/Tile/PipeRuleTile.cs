using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Pipe Rule Tile")]
public class CustomRuleTile : RuleTile<CustomRuleTile.Neighbor>
{
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Nothing = 3;  // 空格或不同類型的 Tile
        public const int Anything = 4; // 同類型的 Tile
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            // 🟥 Nothing = 空格 或 不是 CustomRuleTile
            case Neighbor.Nothing:
                if (tile == null) return true;
                if (tile is CustomRuleTile) return false; // 同類型不算 Nothing
                return true; // 其他類型算 Nothing

            // 🟩 Anything = 同類型的 Tile
            case Neighbor.Anything:
                if (tile is CustomRuleTile) return true;
                return false;
        }

        // 🟦 預設邏輯：只有相同類型才匹配
        if (tile is CustomRuleTile)
        {
            return true;
        }

        return false;
    }
}
