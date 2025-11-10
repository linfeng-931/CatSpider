using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class CustomRuleTileBase : RuleTile<CustomRuleTileBase.Neighbor>
{
    [Header("Custom Settings")]
    public string groupID = ""; // 子類別設定 BR / BL / GR

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Nothing = 3;
        public const int Anything = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            // 🟥 Nothing = 空格 或 不同群組
            case Neighbor.Nothing:
                if (tile == null) return true;
                if (tile is CustomRuleTileBase otherN)
                    return this.groupID != otherN.groupID;
                return true; // 非本類型的 Tile 也算 Nothing
                

            // 🟩 Anything = 同群組的 Tile
            case Neighbor.Anything:
                if (tile is CustomRuleTileBase otherA)
                    return this.groupID == otherA.groupID;
                return false;
        }

        // 🟦 普通匹配邏輯：只有同群組的才算匹配
        if (tile is CustomRuleTileBase otherTile)
        {
            return this.groupID == otherTile.groupID;
        }

        return false;
    }
}
