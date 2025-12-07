# Level 2 (Jungle) Setup Guide

This guide explains how to set up Level 2 with the automated spawning system for enemies, items, and the boss.

## Overview

Level 2 features:
- **Enemy Spawning**: Enemies spawn continuously with increasing frequency as the game progresses
- **Item Spawning**: Items spawn every 20 seconds randomly (Star +50 score, Speed Potion x2 speed for 10s, Health Potion restore 20% max health, Damage Potion x2 damage for 10s)
- **Boss Spawning**: Big boss spawns at 4 minutes (1 minute before the 5-minute game end)

## Step-by-Step Setup

### Step 1: Use the Level 2 Setup Tool

1. In Unity, go to the top menu: **Tools → Level 2 Setup Tool**
2. Click **"Create Spawn Point Groups"** to automatically create:
   - `EnemySpawnPoints` parent GameObject
   - `ItemSpawnPoints` parent GameObject
   - `BossSpawnPoints` parent GameObject
   - Initial spawn point GameObjects positioned in a circle pattern
3. Click **"Setup Level 2 Scene"** to create the `Level2Manager` GameObject

### Step 2: Position Spawn Points

1. Select the spawn point GameObjects in the Hierarchy
2. Position them around your jungle map where you want enemies/items/boss to spawn
3. You can add more spawn points by:
   - Creating new empty GameObjects
   - Parenting them to the appropriate spawn point group
   - Positioning them where desired

### Step 3: Prepare Item Prefabs

You need to create/configure item prefabs with the `ItemPickup` component:

#### Star Item (for +50 score)
1. If you have `StarCoin.prefab`, duplicate it or create a new prefab
2. Add `ItemPickup` component
3. Set `Item Type` to **Star**
4. Add `Interactable` component
5. Set `Interaction Type` to **Item**
6. Tag as `Interactable`
7. Save as prefab (e.g., `Star.prefab`)

#### Speed Potion
1. Create or use existing potion prefab (e.g., `BluePotion.prefab`)
2. Add `ItemPickup` component
3. Set `Item Type` to **SpeedBoost**
4. Set `Speed Boost Multiplier` to **2**
5. Set `Speed Boost Duration` to **10**
6. Add `Interactable` component (Type: Item, Tag: Interactable)

#### Health Potion
1. Create or use existing health pack prefab
2. Add `ItemPickup` component
3. Set `Item Type` to **HealthPack**
4. Note: Health potion now restores 20% of max health automatically
5. Add `Interactable` component (Type: Item, Tag: Interactable)

#### Damage Potion
1. Create or use existing potion prefab (e.g., `RedPotion.prefab`)
2. Add `ItemPickup` component
3. Set `Item Type` to **DamageBoost**
4. Set `Damage Boost Multiplier` to **2**
5. Set `Damage Boost Duration` to **10**
6. Add `Interactable` component (Type: Item, Tag: Interactable)

### Step 4: Configure Level2Manager

1. Select the `Level2Manager` GameObject in the Hierarchy
2. In the Inspector, configure the `Level2Manager` component:

#### Enemy Spawning
- **Enemy Prefabs**: Drag your enemy prefabs here (e.g., BasicEnemy, RangedEnemy, FastEnemy)
- **Enemy Spawn Points**: Drag all spawn points from `EnemySpawnPoints` parent
- **Initial Enemy Spawn Interval**: `3` (spawns every 3 seconds at start)
- **Min Enemy Spawn Interval**: `0.5` (spawns every 0.5 seconds at end)
- **Max Enemies On Screen**: `30`

#### Item Spawning
- **Item Prefabs**: Drag your item prefabs here:
  - Star prefab
  - Speed Potion prefab
  - Health Potion prefab
  - Damage Potion prefab
- **Item Spawn Points**: Drag all spawn points from `ItemSpawnPoints` parent
- **Item Spawn Interval**: `20` (spawns every 20 seconds)
- **Max Items On Screen**: `10`

#### Boss Spawning
- **Boss Prefab**: Drag your boss enemy prefab (e.g., `BossEnemy.prefab`)
- **Boss Spawn Points**: Drag all spawn points from `BossSpawnPoints` parent
- **Boss Spawn Time**: `240` (4 minutes = 240 seconds)

#### Spawn Settings
- **Spawn Radius**: `50` (used if no spawn points provided)
- **Nav Mesh Layer**: Set to your NavMesh layer

### Step 5: Verify Prefab Configurations

Ensure all enemy prefabs have:
- `Actor` component (with health)
- `NavMeshAgent` component
- `EnemyAI`, `RangedEnemyAI`, `FastEnemyAI`, or `BossAI` component
- `Interactable` component (Type: Enemy)
- Tag: `Interactable` (for player interaction)

Ensure all item prefabs have:
- `ItemPickup` component (with correct Item Type)
- `Interactable` component (Type: Item)
- Tag: `Interactable`

### Step 6: Test the Setup

1. Enter Play Mode
2. Verify that:
   - Enemies spawn and spawn rate increases over time
   - Items spawn every 20 seconds randomly
   - Boss spawns at exactly 4 minutes (check the Console for "Boss spawned" message)
3. Test each item type:
   - Star: Should add +50 score
   - Speed Potion: Should double speed for 10 seconds
   - Health Potion: Should restore 20% of max health
   - Damage Potion: Should double damage for 10 seconds

## Features Explained

### Enemy Spawning
- Spawn rate starts at 1 enemy per 3 seconds
- Gradually increases to 1 enemy per 0.5 seconds by the end of the 5-minute game
- Uses linear interpolation based on elapsed game time
- Respects max enemies on screen limit

### Item Spawning
- Items spawn every 20 seconds randomly from your item prefab array
- Each item type has equal chance to spawn
- Items spawn at random spawn points (or random NavMesh positions)

### Boss Spawning
- Boss spawns exactly at 4:00 (240 seconds into the game)
- Only spawns once
- Spawns at a random boss spawn point

### Item Effects

- **Star**: +50 score (instant)
- **Speed Potion**: 2x movement speed for 10 seconds
- **Health Potion**: Restores 20% of maximum health
- **Damage Potion**: 2x attack damage for 10 seconds

## Troubleshooting

**Enemies not spawning:**
- Check that `Level2Manager` has enemy prefabs assigned
- Check that spawn points are assigned
- Verify `GameManager` is running and state is `Playing`

**Items not spawning:**
- Check that item prefabs are assigned
- Verify item prefabs have `ItemPickup` and `Interactable` components
- Check Console for errors

**Boss not spawning:**
- Check that boss prefab is assigned
- Verify game has been running for at least 4 minutes
- Check Console for "Boss spawned" message

**Items not working:**
- Verify item prefabs have correct `Item Type` set
- Check that items have `Interactable` component with Type: Item
- Ensure items are tagged as `Interactable`

**Damage potion not working:**
- Verify `PlayerController` has the updated damage multiplier system
- Check that `ApplyDamageBoost` is being called correctly

## Notes

- The spawn system automatically cleans up destroyed enemies/items
- If no spawn points are provided, the system will try to spawn on NavMesh around the player
- The enemy spawn rate increase is gradual and smooth over the entire 5-minute game duration
- All spawn points are visualized in the Scene view when `Level2Manager` is selected (red = enemies, green = items, yellow = boss)

