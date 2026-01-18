using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Animated tile that randomizes animation start times based on tile position.
/// Prevents all tiles from animating in sync by giving each tile a deterministic random offset.
/// </summary>
[CreateAssetMenu(
    fileName = "NewDesyncedAnimatedTile",
    menuName = "Tiles/Desynced Animated Tile"
)]
public class DesyncedAnimatedTile : AnimatedTile
{
    public override bool GetTileAnimationData(
        Vector3Int position,
        ITilemap tilemap,
        ref TileAnimationData tileAnimationData)
    {
        // Let AnimatedTile populate the default data
        bool hasAnimation = base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
        if (!hasAnimation)
            return false;

        // Deterministic "random" seed based on tile position
        int seed = position.x * 73856093 ^ position.y * 19349663;
        Random.InitState(seed);

        // Random offset in seconds
        float randomOffset = Random.Range(0f, 1f);

        tileAnimationData.animationStartTime = Time.time + randomOffset;

        return true;
    }
}