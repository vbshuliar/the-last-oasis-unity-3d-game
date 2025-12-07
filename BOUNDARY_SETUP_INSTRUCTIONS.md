# Map Boundary Setup Instructions

## Why Barriers Might Not Work

Since your player uses **NavMeshAgent** for movement, simple colliders might not be enough. This improved tool uses multiple methods to ensure boundaries work:

1. **NavMeshObstacle Components** - Block NavMesh paths
2. **Boundary Checker Script** - Teleports player back if they somehow get out
3. **Physical Colliders** - Physical barriers as backup

## How to Use the Improved Tool

1. Open **Tools → Map Boundary Generator**
2. Set your map size (default 30x30)
3. Enable these options:
   - ✅ **Use NavMesh Obstacle** - Blocks NavMesh paths
   - ✅ **Create Boundary Checker Script** - Teleports player back
   - Optional: **Show Barriers in Scene** - For debugging (shows red transparent walls)
4. Click **"Generate Map Boundaries"**

## Important Notes

### After Generating Barriers:

1. **Rebake Your NavMesh!**
   - Go to **Window → AI → Navigation**
   - Click the **Bake** tab
   - Click **Bake** button
   - This is CRITICAL - NavMeshObstacles need a rebaked NavMesh to work

2. **Check the Boundary Checker**
   - A GameObject called "MapBoundaryChecker" will be created
   - Make sure it's active in your scene
   - It automatically finds the player and teleports them back if they go out of bounds

3. **Verify Barriers**
   - In Scene view, select "MapBoundaries" parent
   - You should see 4 walls (North, South, East, West)
   - Each wall should have:
     - BoxCollider component
     - NavMeshObstacle component (if enabled)

## Troubleshooting

**Player still goes through barriers:**
- ✅ Did you rebake the NavMesh? (This is usually the issue!)
- ✅ Is "MapBoundaryChecker" GameObject active in the scene?
- ✅ Check that NavMeshObstacle components are on each wall
- ✅ Try enabling "Show Barriers in Scene" to visualize them

**Boundary checker not working:**
- Make sure your player GameObject has the "Player" tag
- Check that MapBoundaryChecker is in the scene
- Verify map size matches your actual map size

**NavMesh issues:**
- Make sure barriers are NOT marked as "Navigation Static"
- Rebuild NavMesh after creating barriers
- Barriers should carve out the NavMesh automatically

## Testing

1. Enter Play Mode
2. Try to move player to map boundaries
3. Player should stop at barriers
4. If player somehow gets through, boundary checker will teleport them back

