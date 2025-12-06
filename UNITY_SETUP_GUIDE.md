# Complete Unity Setup Guide - The Last Oasis

**This guide provides step-by-step instructions for setting up every aspect of your Unity game. Follow each section in order.**

---

## Table of Contents

1. [Initial Project Setup](#1-initial-project-setup)
2. [Scene Creation](#2-scene-creation)
3. [Splash Screen Setup](#3-splash-screen-setup)
4. [Main Menu Setup](#4-main-menu-setup)
5. [Options Menu Setup](#5-options-menu-setup)
6. [Tutorial Scene Setup](#6-tutorial-scene-setup)
7. [Main Game Scene Setup](#7-main-game-scene-setup)
8. [Second Level Scene Setup](#8-second-level-scene-setup)
9. [UI Setup for Game Scene](#9-ui-setup-for-game-scene)
10. [Player Setup](#10-player-setup)
11. [Enemy Setup](#11-enemy-setup)
12. [Item Prefabs Setup](#12-item-prefabs-setup)
13. [Enemy Spawner Setup](#13-enemy-spawner-setup)
14. [Audio Setup](#14-audio-setup)
15. [Lighting Setup](#15-lighting-setup)
16. [Particle Effects Setup](#16-particle-effects-setup)
17. [NavMesh Setup](#17-navmesh-setup)
18. [Terrain Setup](#18-terrain-setup)
19. [Build Settings](#19-build-settings)
20. [Final Testing](#20-final-testing)

---

## 1. Initial Project Setup

### Step 1.1: Verify Scripts Are Present

1. **Open Unity Editor**

   - Launch Unity Hub
   - Open your project
2. **Check Scripts Folder**

   - In Project window (bottom), navigate to: `Assets/Scripts`
   - Verify all script folders exist:
     - `Core/`
     - `UI/`
     - `Systems/`
     - `AI/`
     - `AI/FSM/`
     - `AI/Steering/`
     - `AI/BehaviourTree/`
     - `AI/Advanced/`
3. **Verify Key Scripts Exist**

   - `Core/GameManager.cs`
   - `Core/SceneTransitionManager.cs`
   - `UI/UIManager.cs`
   - `UI/MainMenuController.cs`
   - `Systems/AudioManager.cs`
   - `Systems/SaveSystem.cs`
   - `PlayerController.cs`
   - `Actor.cs`
   - `EnemyAI.cs`

### Step 1.2: Create Folder Structure

1. **Right-click in Project window** → `Assets` folder
2. **Select:** `Create → Folder`
3. **Name it:** `Scenes`
4. **Repeat to create:**
   - `Prefabs` folder
   - `Prefabs/Enemies` folder
   - `Prefabs/Items` folder
   - `Prefabs/UI` folder
   - `Audio` folder
   - `Audio/Music` folder
   - `Audio/SFX` folder
   - `Materials` folder
   - `Textures` folder

### Step 1.3: Import Required Packages

1. **Go to:** `Window → Package Manager`
2. **In Package Manager:**
   - Click dropdown (top left) → Select `Unity Registry`
   - Search for: `TextMeshPro`
   - Click `Install` if not already installed
   - Search for: `Input System`
   - Click `Install` if not already installed
   - When prompted, click `Yes` to restart Unity

---

## 2. Scene Creation

### Step 2.1: Create All Required Scenes

1. **Create Splash Screen Scene:**

   - `File → New Scene`
   - Select: `Basic (Built-in)`
   - Click `Create`
   - `File → Save As`
   - Navigate to: `Assets/Scenes/`
   - Name: `SplashScreen`
   - Click `Save`
2. **Create Main Menu Scene:**

   - `File → New Scene`
   - Select: `Basic (Built-in)`
   - Click `Create`
   - `File → Save As`
   - Navigate to: `Assets/Scenes/`
   - Name: `MainMenu`
   - Click `Save`
3. **Create Tutorial Scene:**

   - `File → New Scene`
   - Select: `Basic (Built-in)`
   - Click `Create`
   - `File → Save As`
   - Navigate to: `Assets/Scenes/`
   - Name: `Tutorial`
   - Click `Save`
4. **Create Main Game Scene (if not exists):**

   - If `Main.unity` already exists, skip this
   - Otherwise: `File → New Scene` → `Basic (Built-in)` → Save as `Main` in `Assets/Scenes/`
5. **Create Level 2 Scene:**

   - `File → New Scene`
   - Select: `Basic (Built-in)`
   - Click `Create`
   - `File → Save As`
   - Navigate to: `Assets/Scenes/`
   - Name: `Level2`
   - Click `Save`

---

## 3. Splash Screen Setup

### Step 3.1: Create Splash Screen UI

1. **Open `SplashScreen.unity` scene**
2. **Create Canvas:**

   - Right-click in Hierarchy (left panel)
   - `UI → Canvas`
   - Select Canvas in Hierarchy
   - In Inspector (right panel):
     - `Canvas Scaler` component:
       - `UI Scale Mode`: `Scale With Screen Size`
       - `Reference Resolution`: X=`1920`, Y=`1080`
     - `Graphic Raycaster` component: Leave default
3. **Create Background Image:**

   - Right-click `Canvas` in Hierarchy
   - `UI → Image`
   - Name it: `Background`
   - In Inspector:
     - `Rect Transform`:
       - Click anchor preset (top-left of Rect Transform) → Hold `Alt` → Click `Stretch/Stretch`
       - This makes it fill entire screen
     - `Image` component:
       - `Color`: Black (R=0, G=0, B=0, A=255)
       - Or assign a background texture if you have one
4. **Create Logo/Title:**

   - Right-click `Canvas` in Hierarchy
   - `UI → Text - TextMeshPro`
   - If prompted, click `Import TMP Essentials`
   - Name it: `Title`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos X`: 0
       - `Pos Y`: 50
       - `Width`: 800
       - `Height`: 200
     - `TextMeshProUGUI` component:
       - `Text`: `THE LAST OASIS`
       - `Font Size`: 72
       - `Alignment`: Center
       - `Color`: White or your choice
5. **Create Subtitle (Optional):**

   - Right-click `Canvas` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name it: `Subtitle`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos X`: 0
       - `Pos Y`: -50
       - `Width`: 600
       - `Height`: 100
     - `TextMeshProUGUI` component:
       - `Text`: `Loading...`
       - `Font Size`: 36
       - `Alignment`: Center
       - `Color`: Gray

### Step 3.2: Add Splash Screen Controller

1. **Create Empty GameObject:**

   - Right-click in Hierarchy
   - `Create Empty`
   - Name it: `SplashScreenController`
2. **Add Script:**

   - Select `SplashScreenController` in Hierarchy
   - In Inspector, click `Add Component`
   - Type: `SplashScreenController`
   - Click on the script name when it appears
   - OR drag `Assets/Scripts/UI/SplashScreenController.cs` onto the GameObject
3. **Configure Script:**

   - In Inspector, `SplashScreenController` component:
     - `Display Duration`: `3` (seconds)
     - `Next Scene Name`: `MainMenu`
4. **Save Scene:**

   - `File → Save` (Ctrl+S / Cmd+S)

---

## 4. Main Menu Setup

### Step 4.1: Create Main Menu UI

1. **Open `MainMenu.unity` scene**
2. **Create Canvas:**

   - Right-click in Hierarchy
   - `UI → Canvas`
   - Select Canvas
   - In Inspector:
     - `Canvas Scaler`:
       - `UI Scale Mode`: `Scale With Screen Size`
       - `Reference Resolution`: X=`1920`, Y=`1080`
3. **Create Background:**

   - Right-click `Canvas` in Hierarchy
   - `UI → Image`
   - Name: `Background`
   - In Inspector:
     - `Rect Transform`: Anchor to `Stretch/Stretch` (Alt+Click)
     - `Image`:
       - `Color`: Dark color (e.g., R=30, G=30, B=50, A=255)
       - Or assign background texture
4. **Create Title:**

   - Right-click `Canvas` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `Title`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Top/Center
       - `Pos Y`: -100
       - `Width`: 1000
       - `Height`: 150
     - `TextMeshProUGUI`:
       - `Text`: `THE LAST OASIS`
       - `Font Size`: 80
       - `Alignment`: Center
       - `Color`: White
5. **Create Start Button:**

   - Right-click `Canvas` in Hierarchy
   - `UI → Button - TextMeshPro`
   - Name: `StartButton`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos X`: 0
       - `Pos Y`: 50
       - `Width`: 300
       - `Height`: 60
     - `Button` component: Leave default
     - Select child `Text (TMP)`:
       - `Text`: `START GAME`
       - `Font Size`: 36
       - `Alignment`: Center
6. **Create Options Button:**

   - Right-click `Canvas` in Hierarchy
   - `UI → Button - TextMeshPro`
   - Name: `OptionsButton`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos X`: 0
       - `Pos Y`: -30
       - `Width`: 300
       - `Height`: 60
     - Select child `Text (TMP)`:
       - `Text`: `OPTIONS`
       - `Font Size`: 36
7. **Create Quit Button:**

   - Right-click `Canvas` in Hierarchy
   - `UI → Button - TextMeshPro`
   - Name: `QuitButton`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos X`: 0
       - `Pos Y`: -110
       - `Width`: 300
       - `Height`: 60
     - Select child `Text (TMP)`:
       - `Text`: `QUIT`
       - `Font Size`: 36

### Step 4.2: Add Main Menu Controller

1. **Create Empty GameObject:**

   - Right-click in Hierarchy
   - `Create Empty`
   - Name: `MainMenuController`
2. **Add Script:**

   - Select `MainMenuController`
   - In Inspector, `Add Component` → Type: `MainMenuController`
3. **Configure Script:**

   - In Inspector, `MainMenuController` component:
     - `Start Button`: Drag `StartButton` from Hierarchy
     - `Options Button`: Drag `OptionsButton` from Hierarchy
     - `Quit Button`: Drag `QuitButton` from Hierarchy
     - `Game Scene Name`: Type `Main`
     - `Tutorial Scene Name`: Type `Tutorial`
     - `Options Scene Name`: Type `Options` (or leave empty if using panel)
4. **Save Scene:**

   - `File → Save`

---

## 5. Options Menu Setup

### Step 5.1: Create Options Panel (In Main Menu Scene)

1. **Open `MainMenu.unity` scene**
2. **Create Options Panel:**

   - Right-click `Canvas` in Hierarchy
   - `UI → Panel`
   - Name: `OptionsPanel`
   - In Inspector:
     - `Rect Transform`: Anchor to `Stretch/Stretch`
     - `Image` component:
       - `Color`: Semi-transparent black (R=0, G=0, B=0, A=200)
3. **Create Title:**

   - Right-click `OptionsPanel` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `OptionsTitle`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Top/Center
       - `Pos Y`: -50
       - `Width`: 600
       - `Height`: 80
     - `TextMeshProUGUI`:
       - `Text`: `OPTIONS`
       - `Font Size`: 60
       - `Alignment`: Center
4. **Create Music Volume Slider:**

   - Right-click `OptionsPanel` in Hierarchy
   - `UI → Slider`
   - Name: `MusicVolumeSlider`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos X`: 0
       - `Pos Y`: 50
       - `Width`: 400
       - `Height`: 30
     - `Slider` component:
       - `Min Value`: 0
       - `Max Value`: 1
       - `Value`: 0.7
   - Create label:
     - Right-click `MusicVolumeSlider` in Hierarchy
     - `UI → Text - TextMeshPro`
     - Name: `MusicLabel`
     - In Inspector:
       - `Rect Transform`:
         - `Anchor Preset`: Left/Center
         - `Pos X`: -200
         - `Pos Y`: 0
         - `Width`: 150
         - `Height`: 30
       - `TextMeshProUGUI`:
         - `Text`: `Music Volume:`
         - `Font Size`: 24
5. **Create SFX Volume Slider:**

   - Right-click `OptionsPanel` in Hierarchy
   - `UI → Slider`
   - Name: `SFXVolumeSlider`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos X`: 0
       - `Pos Y`: -20
       - `Width`: 400
       - `Height`: 30
     - `Slider` component:
       - `Min Value`: 0
       - `Max Value`: 1
       - `Value`: 0.8
   - Create label:
     - Right-click `SFXVolumeSlider` in Hierarchy
     - `UI → Text - TextMeshPro`
     - Name: `SFXLabel`
     - Configure same as MusicLabel but text: `SFX Volume:`
6. **Create Difficulty Dropdown:**

   - Right-click `OptionsPanel` in Hierarchy
   - `UI → Dropdown - TextMeshPro`
   - Name: `DifficultyDropdown`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos X`: 0
       - `Pos Y`: -90
       - `Width`: 300
       - `Height`: 40
     - `Dropdown` component:
       - Click `+` under `Options` list
       - Add 3 options:
         - Option 0: `Easy`
         - Option 1: `Medium`
         - Option 2: `Hard`
       - `Value`: 0
7. **Create Back Button:**

   - Right-click `OptionsPanel` in Hierarchy
   - `UI → Button - TextMeshPro`
   - Name: `BackButton`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Bottom/Center
       - `Pos Y`: 50
       - `Width`: 200
       - `Height`: 50
     - Select child `Text (TMP)`:
       - `Text`: `BACK`
       - `Font Size`: 30
8. **Set Panel Inactive:**

   - Select `OptionsPanel` in Hierarchy
   - In Inspector, top-left, uncheck the checkbox next to the name
   - This hides it initially

### Step 5.2: Add Options Menu Controller

1. **Create Empty GameObject:**

   - Right-click in Hierarchy
   - `Create Empty`
   - Name: `OptionsMenuController`
2. **Add Script:**

   - Select `OptionsMenuController`
   - `Add Component` → `OptionsMenuController`
3. **Configure Script:**

   - In Inspector, `OptionsMenuController` component:
     - `Options Panel`: Drag `OptionsPanel` from Hierarchy
     - `Music Volume Slider`: Drag `MusicVolumeSlider` from Hierarchy
     - `SFX Volume Slider`: Drag `SFXVolumeSlider` from Hierarchy
     - `Difficulty Dropdown`: Drag `DifficultyDropdown` from Hierarchy
     - `Back Button`: Drag `BackButton` from Hierarchy
4. **Update MainMenuController:**

   - Select `MainMenuController` in Hierarchy
   - In Inspector, find `OnOptionsClicked` method (if exists) or add reference to `OptionsMenuController`
5. **Save Scene:**

   - `File → Save`

---

## 6. Tutorial Scene Setup

### Step 6.1: Create Tutorial Environment

1. **Open `Tutorial.unity` scene**
2. **Create Ground:**

   - Right-click in Hierarchy
   - `3D Object → Plane`
   - Name: `Ground`
   - In Inspector:
     - `Transform`:
       - `Position`: X=0, Y=0, Z=0
       - `Scale`: X=10, Y=1, Z=10
3. **Add Basic Lighting:**

   - In Hierarchy, find `Directional Light`
   - In Inspector:
     - `Rotation`: X=50, Y=-30, Z=0
     - `Color`: Light yellow/white
     - `Intensity`: 1

### Step 6.2: Create Tutorial UI

1. **Create Canvas:**

   - Right-click in Hierarchy
   - `UI → Canvas`
   - Configure same as Main Menu Canvas
2. **Create Tutorial Panel:**

   - Right-click `Canvas` in Hierarchy
   - `UI → Panel`
   - Name: `TutorialPanel`
   - In Inspector:
     - `Rect Transform`: Anchor to `Stretch/Stretch`
     - `Image`:
       - `Color`: Semi-transparent black (A=220)
3. **Create Tutorial Text:**

   - Right-click `TutorialPanel` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `TutorialText`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Width`: 800
       - `Height`: 400
     - `TextMeshProUGUI`:
       - `Text`: `TUTORIAL\n\nRight-click to move\nRight-click enemies to attack\nCollect items for power-ups\nSurvive for 5 minutes!`
       - `Font Size`: 32
       - `Alignment`: Center
       - `Color`: White
4. **Create Skip Button:**

   - Right-click `TutorialPanel` in Hierarchy
   - `UI → Button - TextMeshPro`
   - Name: `SkipButton`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Bottom/Right
       - `Pos X`: -50
       - `Pos Y`: 50
       - `Width`: 150
       - `Height`: 40
     - Select child `Text (TMP)`:
       - `Text`: `SKIP`
       - `Font Size`: 24

### Step 6.3: Add Tutorial Controller

1. **Create Empty GameObject:**

   - Right-click in Hierarchy
   - `Create Empty`
   - Name: `TutorialController`
2. **Add Script:**

   - Select `TutorialController`
   - `Add Component` → `TutorialController`
3. **Configure Script:**

   - In Inspector, `TutorialController` component:
     - `Tutorial Panel`: Drag `TutorialPanel` from Hierarchy
     - `Skip Button`: Drag `SkipButton` from Hierarchy
     - `Next Scene Name`: Type `Main`
4. **Save Scene:**

   - `File → Save`

---

## 7. Main Game Scene Setup

### Step 7.1: Set Up Environment

1. **Open `Main.unity` scene** (or create if doesn't exist)
2. **Create Ground:**

   - Right-click in Hierarchy
   - `3D Object → Plane`
   - Name: `Ground`
   - In Inspector:
     - `Transform`:
       - `Position`: X=0, Y=0, Z=0
       - `Scale`: X=20, Y=1, Z=20
3. **Add Buildings/Obstacles:**

   - Create simple buildings:
     - Right-click in Hierarchy
     - `3D Object → Cube`
     - Name: `Building1`
     - In Inspector:
       - `Transform`:
         - `Position`: X=10, Y=1, Z=10
         - `Scale`: X=4, Y=4, Z=4
   - Repeat to create 5-10 buildings scattered around
   - These act as obstacles and cover
4. **Add Lighting:**

   - Select `Directional Light` in Hierarchy
   - In Inspector:
     - `Rotation`: X=50, Y=-30, Z=0
     - `Intensity`: 1.2
   - Add point lights for atmosphere:
     - Right-click in Hierarchy
     - `Light → Point Light`
     - Name: `Lantern1`
     - In Inspector:
       - `Transform`:
         - `Position`: X=5, Y=3, Z=5
       - `Light` component:
         - `Color`: Orange (R=255, G=150, B=50)
         - `Intensity`: 2
         - `Range`: 10
   - Create 3-5 more point lights around the scene

### Step 7.2: Create GameManager GameObject

1. **Create Empty GameObject:**

   - Right-click in Hierarchy
   - `Create Empty`
   - Name: `GameManager`
2. **Add Script:**

   - Select `GameManager`
   - `Add Component` → `GameManager`
3. **Configure Script:**

   - In Inspector, `GameManager` component:
     - `Game Duration`: `300` (5 minutes in seconds)
     - `Current Difficulty`: `Easy` (dropdown)
     - `Points Per Kill`: `10`
     - `Points Per Second`: `1`
     - `Points Per Pickup`: `5`
4. **This GameObject will persist between scenes** (DontDestroyOnLoad is handled in script)

### Step 7.3: Create SceneTransitionManager GameObject

1. **Create Empty GameObject:**

   - Right-click in Hierarchy
   - `Create Empty`
   - Name: `SceneTransitionManager`
2. **Add Script:**

   - Select `SceneTransitionManager`
   - `Add Component` → `SceneTransitionManager`
3. **Configure Script:**

   - In Inspector, `SceneTransitionManager` component:
     - `Fade Duration`: `1` (second)
     - `Fade Image`: Leave empty (auto-created if null)

### Step 7.4: Create AudioManager GameObject

1. **Create Empty GameObject:**

   - Right-click in Hierarchy
   - `Create Empty`
   - Name: `AudioManager`
2. **Add Script:**

   - Select `AudioManager`
   - `Add Component` → `AudioManager`
3. **Configure Script:**

   - In Inspector, `AudioManager` component:
     - `Music Source`: Leave empty (auto-created)
     - `SFX Source`: Leave empty (auto-created)
     - `Background Music`: Drag audio clip here (when you have one)
     - `Player Attack Sound`: Drag audio clip
     - `Enemy Attack Sound`: Drag audio clip
     - `Item Pickup Sound`: Drag audio clip
     - `Player Hurt Sound`: Drag audio clip
     - `Enemy Death Sound`: Drag audio clip
     - `Footstep Sound`: Drag audio clip
     - `Music Volume`: `0.7`
     - `SFX Volume`: `0.8`

**Note:** Audio clips can be added later. The system will work without them initially.

---

## 8. Second Level Scene Setup

### Step 8.1: Duplicate Main Scene

1. **In Project window:**

   - Navigate to: `Assets/Scenes/`
   - Right-click `Main.unity`
   - `Duplicate`
   - Rename: `Level2`
2. **Open `Level2.unity` scene**

### Step 8.2: Modify Level 2 Environment

1. **Rearrange Buildings:**

   - Select buildings in Hierarchy
   - Move them to different positions
   - Change some scales for variety
2. **Add Different Features:**

   - Add more obstacles
   - Create different building layouts
   - Add unique landmarks
3. **Update Spawn Points:**

   - Create spawn point markers:
     - Right-click in Hierarchy
     - `Create Empty`
     - Name: `SpawnPoint1`
     - In Inspector:
       - `Transform`:
         - `Position`: X=5, Y=0, Z=5
   - Create 5-10 spawn points scattered around
4. **Save Scene:**

   - `File → Save`

---

## 9. UI Setup for Game Scene

### Step 9.1: Create HUD Canvas

1. **Open `Main.unity` scene**
2. **Create Canvas:**

   - Right-click in Hierarchy
   - `UI → Canvas`
   - Name: `HUDCanvas`
   - In Inspector:
     - `Canvas` component:
       - `Render Mode`: `Screen Space - Overlay`
     - `Canvas Scaler`:
       - `UI Scale Mode`: `Scale With Screen Size`
       - `Reference Resolution`: X=1920, Y=1080

### Step 9.2: Create Health Bar

1. **Create Health Bar Background:**

   - Right-click `HUDCanvas` in Hierarchy
   - `UI → Image`
   - Name: `HealthBarBackground`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Top/Left
       - `Pos X`: 50
       - `Pos Y`: -50
       - `Width`: 300
       - `Height`: 30
     - `Image`:
       - `Color`: Dark red (R=100, G=0, B=0, A=255)
2. **Create Health Bar Fill:**

   - Right-click `HealthBarBackground` in Hierarchy
   - `UI → Image`
   - Name: `HealthBarFill`
   - In Inspector:
     - `Rect Transform`:
       - Anchor to `Stretch/Stretch` (Alt+Click)
       - `Left`: 2
       - `Right`: 2
       - `Top`: 2
       - `Bottom`: 2
     - `Image`:
       - `Color`: Red (R=255, G=0, B=0, A=255)
       - `Image Type`: `Filled`
       - `Fill Method`: `Horizontal`
       - `Fill Origin`: `Left`
3. **Create Health Bar Slider (Alternative Method):**

   - Actually, use Slider instead:
   - Delete `HealthBarFill`
   - Right-click `HealthBarBackground` in Hierarchy
   - `UI → Slider`
   - Name: `HealthBar`
   - In Inspector:
     - `Rect Transform`: Anchor to `Stretch/Stretch`
     - `Slider` component:
       - `Min Value`: 0
       - `Max Value`: 100
       - `Value`: 100
     - Delete child `Handle Slide Area` (we don't need handle)
     - Select child `Fill Area → Fill`:
       - `Color`: Red

### Step 9.3: Create Timer Display

1. **Create Timer Text:**
   - Right-click `HUDCanvas` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `TimerText`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Top/Center
       - `Pos Y`: -50
       - `Width`: 200
       - `Height`: 50
     - `TextMeshProUGUI`:
       - `Text`: `05:00`
       - `Font Size`: 48
       - `Alignment`: Center
       - `Color`: White

### Step 9.4: Create Score Display

1. **Create Score Text:**
   - Right-click `HUDCanvas` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `ScoreText`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Top/Right
       - `Pos X`: -50
       - `Pos Y`: -50
       - `Width`: 300
       - `Height`: 50
     - `TextMeshProUGUI`:
       - `Text`: `Score: 0`
       - `Font Size`: 36
       - `Alignment`: Left
       - `Color`: White

### Step 9.5: Create Kills Display

1. **Create Kills Text:**
   - Right-click `HUDCanvas` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `KillsText`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Top/Right
       - `Pos X`: -50
       - `Pos Y`: -100
       - `Width`: 200
       - `Height`: 40
     - `TextMeshProUGUI`:
       - `Text`: `Kills: 0`
       - `Font Size`: 28
       - `Alignment`: Left
       - `Color`: White

### Step 9.6: Create Items Display

1. **Create Items Text:**
   - Right-click `HUDCanvas` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `ItemsText`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Top/Right
       - `Pos X`: -50
       - `Pos Y`: -140
       - `Width`: 200
       - `Height`: 40
     - `TextMeshProUGUI`:
       - `Text`: `Items: 0`
       - `Font Size`: 28
       - `Alignment`: Left
       - `Color`: White

### Step 9.7: Add UIManager

1. **Create Empty GameObject:**

   - Right-click in Hierarchy
   - `Create Empty`
   - Name: `UIManager`
2. **Add Script:**

   - Select `UIManager`
   - `Add Component` → `UIManager`
3. **Configure Script:**

   - In Inspector, `UIManager` component:
     - `Health Bar`: Drag `HealthBar` slider from Hierarchy
     - `Timer Text`: Drag `TimerText` from Hierarchy
     - `Score Text`: Drag `ScoreText` from Hierarchy
     - `Kills Text`: Drag `KillsText` from Hierarchy
     - `Items Text`: Drag `ItemsText` from Hierarchy
     - `Player Actor`: Leave empty (will find automatically) OR drag Player GameObject when created

### Step 9.8: Create Pause Menu

1. **Create Pause Panel:**

   - Right-click `HUDCanvas` in Hierarchy
   - `UI → Panel`
   - Name: `PausePanel`
   - In Inspector:
     - `Rect Transform`: Anchor to `Stretch/Stretch`
     - `Image`:
       - `Color`: Semi-transparent black (A=200)
   - **Set inactive** (uncheck checkbox)
2. **Create Pause Title:**

   - Right-click `PausePanel` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `PauseTitle`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Top/Center
       - `Pos Y`: -100
       - `Width`: 400
       - `Height`: 100
     - `TextMeshProUGUI`:
       - `Text`: `PAUSED`
       - `Font Size`: 60
       - `Alignment`: Center
       - `Color`: White
3. **Create Resume Button:**

   - Right-click `PausePanel` in Hierarchy
   - `UI → Button - TextMeshPro`
   - Name: `ResumeButton`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos Y`: 50
       - `Width`: 250
       - `Height`: 50
     - Select child `Text (TMP)`:
       - `Text`: `RESUME`
       - `Font Size`: 30
4. **Create Restart Button:**

   - Right-click `PausePanel` in Hierarchy
   - `UI → Button - TextMeshPro`
   - Name: `RestartButton`
   - Configure same as ResumeButton but:
     - `Pos Y`: -20
     - `Text`: `RESTART`
5. **Create Main Menu Button:**

   - Right-click `PausePanel` in Hierarchy
   - `UI → Button - TextMeshPro`
   - Name: `MainMenuButton`
   - Configure same as ResumeButton but:
     - `Pos Y`: -90
     - `Text`: `MAIN MENU`
6. **Create Quit Button:**

   - Right-click `PausePanel` in Hierarchy
   - `UI → Button - TextMeshPro`
   - Name: `QuitButton`
   - Configure same as ResumeButton but:
     - `Pos Y`: -160
     - `Text`: `QUIT`
7. **Add PauseMenuController:**

   - Create Empty GameObject: `PauseMenuController`
   - `Add Component` → `PauseMenuController`
   - In Inspector:
     - `Pause Panel`: Drag `PausePanel`
     - `Resume Button`: Drag `ResumeButton`
     - `Restart Button`: Drag `RestartButton`
     - `Main Menu Button`: Drag `MainMenuButton`
     - `Quit Button`: Drag `QuitButton`

### Step 9.9: Create End Menu

1. **Create End Panel:**

   - Right-click `HUDCanvas` in Hierarchy
   - `UI → Panel`
   - Name: `EndPanel`
   - In Inspector:
     - `Rect Transform`: Anchor to `Stretch/Stretch`
     - `Image`: Semi-transparent black
   - **Set inactive**
2. **Create End Title:**

   - Right-click `EndPanel` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `EndTitle`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Top/Center
       - `Pos Y`: -80
       - `Width`: 600
       - `Height`: 100
     - `TextMeshProUGUI`:
       - `Text`: `VICTORY` (will change based on win/lose)
       - `Font Size`: 70
       - `Alignment`: Center
       - `Color`: White
3. **Create Score Text:**

   - Right-click `EndPanel` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `EndScoreText`
   - In Inspector:
     - `Rect Transform`:
       - `Anchor Preset`: Center/Middle
       - `Pos Y`: 50
       - `Width`: 500
       - `Height`: 40
     - `TextMeshProUGUI`:
       - `Text`: `Score: 0`
       - `Font Size`: 32
       - `Alignment`: Center
4. **Create Time Text:**

   - Right-click `EndPanel` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `EndTimeText`
   - Configure same as EndScoreText but:
     - `Pos Y`: 0
     - `Text`: `Time Survived: 05:00`
5. **Create Kills Text:**

   - Right-click `EndPanel` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `EndKillsText`
   - Configure same as EndScoreText but:
     - `Pos Y`: -50
     - `Text`: `Enemies Killed: 0`
6. **Create High Score Text:**

   - Right-click `EndPanel` in Hierarchy
   - `UI → Text - TextMeshPro`
   - Name: `EndHighScoreText`
   - Configure same as EndScoreText but:
     - `Pos Y`: -100
     - `Text`: `High Score: 0`
7. **Create Buttons:**

   - Create `RestartButton`, `MainMenuButton`, `QuitButton` same as Pause Menu
   - Position them at bottom
8. **Add EndMenuController:**

   - Create Empty GameObject: `EndMenuController`
   - `Add Component` → `EndMenuController`
   - In Inspector, assign all UI elements:
     - `End Panel`: Drag `EndPanel`
     - `End Title`: Drag `EndTitle`
     - `End Score Text`: Drag `EndScoreText`
     - `End Time Text`: Drag `EndTimeText`
     - `End Kills Text`: Drag `EndKillsText`
     - `End High Score Text`: Drag `EndHighScoreText`
     - `Restart Button`: Drag `RestartButton`
     - `Main Menu Button`: Drag `MainMenuButton`
     - `Quit Button`: Drag `QuitButton`
9. **Save Scene:**

   - `File → Save`

---

## 10. Player Setup

### Step 10.1: Create Player GameObject

1. **Open `Main.unity` scene**
2. **Create Player:**

   - Right-click in Hierarchy
   - `3D Object → Capsule` (or use your player model)
   - Name: `Player`
   - In Inspector:
     - `Transform`:
       - `Position`: X=0, Y=1, Z=0
       - `Rotation`: X=0, Y=0, Z=0
       - `Scale`: X=1, Y=1, Z=1
3. **Tag Player:**

   - In Inspector, top section:
     - `Tag`: Click dropdown → `Add Tag...`
     - Click `+` button
     - Name: `Player`
     - Click `Save`
     - Select `Player` GameObject
     - Set `Tag` to `Player`
4. **Add NavMesh Agent:**

   - Select `Player` in Hierarchy
   - `Add Component` → Type: `Nav Mesh Agent`
   - In Inspector, `Nav Mesh Agent` component:
     - `Radius`: 0.5
     - `Height`: 2
     - `Speed`: 5
     - `Acceleration`: 8
     - `Stopping Distance`: 0.5
     - `Base Offset`: 1
5. **Add Actor Component:**

   - Select `Player`
   - `Add Component` → `Actor`
   - In Inspector, `Actor` component:
     - `Max Health`: 100
6. **Add PlayerController Component:**

   - Select `Player`
   - `Add Component` → `PlayerController`
   - In Inspector, `PlayerController` component:
     - `Click Effect`: Leave empty (or create particle effect prefab)
     - `Clickable Layers`: Create new layer:
       - `Edit → Project Settings → Tags and Layers`
       - Under `Layers`, find empty slot (e.g., Layer 8)
       - Name it: `Ground`
       - Assign `Ground` layer to ground plane
       - In `PlayerController`, set `Clickable Layers` to include `Ground` layer
     - `Attack Speed`: 1.5
     - `Attack Delay`: 0.3
     - `Attack Distance`: 1.5
     - `Attack Damage`: 1
     - `Hit Effect`: Leave empty (or create particle effect)
7. **Add Animator (if you have animations):**

   - Select `Player`
   - `Add Component` → `Animator`
   - In Inspector, `Animator` component:
     - `Controller`: Assign your player animator controller
     - `Avatar`: Assign if you have one
8. **Add Interactable Component (Optional):**

   - If player needs to be interactable, add `Interactable` component
   - Set `Interaction Type` to appropriate value

### Step 10.2: Set Up Player Camera

1. **Find Main Camera:**

   - In Hierarchy, select `Main Camera`
2. **Add CameraController (if script exists):**

   - Select `Main Camera`
   - `Add Component` → `CameraController` (if you have this script)
   - Configure to follow player
3. **OR Manually Configure Camera:**

   - In Inspector, `Transform`:
     - `Position`: X=0, Y=15, Z=-10
     - `Rotation`: X=60, Y=0, Z=0
   - `Camera` component:
     - `Projection`: `Perspective`
     - `Field of View`: 60
   - Create script to follow player OR use Cinemachine
4. **Save Scene:**

   - `File → Save`

---

## 11. Enemy Setup

### Step 11.1: Create Basic Enemy Prefab

1. **Create Enemy GameObject:**

   - Right-click in Hierarchy
   - `3D Object → Capsule` (or use enemy model)
   - Name: `BasicEnemy`
   - In Inspector:
     - `Transform`:
       - `Position`: X=10, Y=1, Z=10
       - `Scale`: X=1, Y=1, Z=1
2. **Tag Enemy:**

   - In Inspector:
     - `Tag`: Create new tag `Enemy` (same process as Player tag)
     - Set tag to `Enemy`
3. **Add NavMesh Agent:**

   - Select `BasicEnemy`
   - `Add Component` → `Nav Mesh Agent`
   - In Inspector:
     - `Radius`: 0.5
     - `Height`: 2
     - `Speed`: 3.5
     - `Acceleration`: 8
     - `Stopping Distance`: 1.5
     - `Base Offset`: 1
4. **Add Actor Component:**

   - Select `BasicEnemy`
   - `Add Component` → `Actor`
   - In Inspector:
     - `Max Health`: 50
5. **Add EnemyAI Component:**

   - Select `BasicEnemy`
   - `Add Component` → `EnemyAI`
   - In Inspector, `EnemyAI` component:
     - `Detection Range`: 10
     - `Attack Range`: 1.5
     - `Attack Damage`: 1
     - `Attack Speed`: 1.0
     - `Attack Delay`: 0.3
     - `Hit Effect`: Leave empty (or particle effect)
     - `Rotation Speed`: 5
6. **Add Animator:**

   - Select `BasicEnemy`
   - `Add Component` → `Animator`
   - Assign enemy animator controller
7. **Add Interactable Component:**

   - Select `BasicEnemy`
   - `Add Component` → `Interactable`
   - In Inspector, `Interactable` component:
     - `Interaction Type`: `Enemy`
     - `My Actor`: Drag `Actor` component from same GameObject (or it auto-finds)
8. **Create Prefab:**

   - Drag `BasicEnemy` from Hierarchy to `Assets/Prefabs/Enemies/` folder
   - Name: `BasicEnemy`
   - Delete `BasicEnemy` from scene (we'll spawn it)

### Step 11.2: Create Ranged Enemy Prefab

1. **Duplicate BasicEnemy:**

   - In Project window, right-click `BasicEnemy` prefab
   - `Duplicate`
   - Rename: `RangedEnemy`
2. **Modify RangedEnemy:**

   - Double-click `RangedEnemy` prefab to open Prefab Mode
   - Select root GameObject
   - In Inspector:
     - Remove `EnemyAI` component
     - `Add Component` → `RangedEnemyAI`
   - Configure `RangedEnemyAI`:
     - `Detection Range`: 15
     - `Attack Range`: 10
     - `Attack Damage`: 2
     - `Projectile Speed`: 10
     - `Fire Rate`: 1.5
   - Exit Prefab Mode (click arrow at top)

### Step 11.3: Create Fast Enemy Prefab

1. **Duplicate BasicEnemy:**

   - Right-click `BasicEnemy` prefab
   - `Duplicate`
   - Rename: `FastEnemy`
2. **Modify FastEnemy:**

   - Double-click `FastEnemy` prefab
   - Select root GameObject
   - In Inspector:
     - Remove `EnemyAI` component
     - `Add Component` → `FastEnemyAI`
   - Configure `FastEnemyAI`:
     - `Detection Range`: 12
     - `Attack Range`: 1.5
     - `Attack Damage`: 1
     - `Speed`: 6 (faster than basic)
   - In `Nav Mesh Agent`:
     - `Speed`: 6
   - In `Actor`:
     - `Max Health`: 30 (lower health)
   - Exit Prefab Mode

### Step 11.4: Create Boss Enemy Prefab

1. **Duplicate BasicEnemy:**

   - Right-click `BasicEnemy` prefab
   - `Duplicate`
   - Rename: `BossEnemy`
2. **Modify BossEnemy:**

   - Double-click `BossEnemy` prefab
   - Select root GameObject
   - In Inspector:
     - `Transform`:
       - `Scale`: X=1.5, Y=1.5, Z=1.5 (bigger)
     - Remove `EnemyAI` component
     - `Add Component` → `BossAI`
   - Configure `BossAI`:
     - `Detection Range`: 20
     - `Attack Range`: 2
     - `Attack Damage`: 3
     - `Special Attack Damage`: 5
     - `Special Attack Cooldown`: 10
   - In `Actor`:
     - `Max Health`: 200 (much higher)
   - In `Nav Mesh Agent`:
     - `Speed`: 4
   - Exit Prefab Mode

---

## 12. Item Prefabs Setup

### Step 12.1: Create Health Pack Prefab

1. **Create Health Pack GameObject:**

   - Right-click in Hierarchy
   - `3D Object → Cube` (or use item model)
   - Name: `HealthPack`
   - In Inspector:
     - `Transform`:
       - `Scale`: X=0.5, Y=0.5, Z=0.5
     - `Mesh Renderer`:
       - `Material`: Create red material or assign existing
2. **Add ItemPickup Component:**

   - Select `HealthPack`
   - `Add Component` → `ItemPickup`
   - In Inspector, `ItemPickup` component:
     - `Item Type`: `HealthPack` (dropdown)
     - `Heal Amount`: 25
3. **Add Interactable Component:**

   - Select `HealthPack`
   - `Add Component` → `Interactable`
   - In Inspector:
     - `Interaction Type`: `Item`
     - `My Actor`: Leave empty (not needed for items)
4. **Tag Item:**

   - Create tag: `Interactable`
   - Set `HealthPack` tag to `Interactable`
5. **Create Prefab:**

   - Drag `HealthPack` to `Assets/Prefabs/Items/`
   - Delete from scene

### Step 12.2: Create Green Potion Prefab

1. **Create Green Potion:**

   - Right-click in Hierarchy
   - `3D Object → Sphere`
   - Name: `GreenPotion`
   - In Inspector:
     - `Transform`:
       - `Scale`: X=0.3, Y=0.3, Z=0.3
     - `Mesh Renderer`:
       - `Material`: Green material
2. **Add ItemPickup Component:**

   - Select `GreenPotion`
   - `Add Component` → `ItemPickup`
   - In Inspector:
     - `Item Type`: `GreenPotion`
     - `Size Multiplier`: 2
     - `Speed Multiplier`: 2
     - `Duration`: 5
3. **Add Interactable Component:**

   - Same as HealthPack
4. **Tag and Prefab:**

   - Tag: `Interactable`
   - Create prefab in `Assets/Prefabs/Items/`

### Step 12.3: Create Speed Boost Prefab

1. **Create Speed Boost:**

   - Right-click in Hierarchy
   - `3D Object → Cylinder`
   - Name: `SpeedBoost`
   - Scale: 0.4
   - Material: Blue
2. **Add ItemPickup Component:**

   - `Item Type`: `SpeedBoost`
   - `Speed Boost Multiplier`: 1.5
   - `Speed Boost Duration`: 10
3. **Add Interactable, Tag, Create Prefab:**

   - Same process

### Step 12.4: Create Damage Boost Prefab

1. **Create Damage Boost:**

   - Similar to Speed Boost
   - Material: Orange/Red
2. **Add ItemPickup Component:**

   - `Item Type`: `DamageBoost`
   - `Damage Boost Multiplier`: 2
   - `Damage Boost Duration`: 15
3. **Add Interactable, Tag, Create Prefab:**

   - Same process

---

## 13. Enemy Spawner Setup

### Step 13.1: Create Difficulty Settings

1. **Create Easy Difficulty:**

   - In Project window, right-click `Assets/Scripts/Core/`
   - `Create → Game → Difficulty Settings`
   - Name: `EasyDifficulty`
   - In Inspector:
     - `Enemy Spawn Rate`: 0.5 (enemies per second)
     - `Enemy Speed`: 3
     - `Enemy Health`: 50
     - `Enemy Damage`: 1
     - `Max Enemies On Screen`: 10
     - `Item Spawn Rate`: 0.2
     - `Player Health Multiplier`: 1.0
     - `Score Multiplier`: 1.0
2. **Create Medium Difficulty:**

   - Duplicate `EasyDifficulty`
   - Rename: `MediumDifficulty`
   - In Inspector:
     - `Enemy Spawn Rate`: 0.75
     - `Enemy Speed`: 4
     - `Enemy Health`: 75
     - `Enemy Damage`: 2
     - `Max Enemies On Screen`: 15
     - `Item Spawn Rate`: 0.15
     - `Score Multiplier`: 2.0
3. **Create Hard Difficulty:**

   - Duplicate `EasyDifficulty`
   - Rename: `HardDifficulty`
   - In Inspector:
     - `Enemy Spawn Rate`: 1.0
     - `Enemy Speed`: 5
     - `Enemy Health`: 100
     - `Enemy Damage`: 3
     - `Max Enemies On Screen`: 20
     - `Item Spawn Rate`: 0.1
     - `Score Multiplier`: 3.0

### Step 13.2: Create Enemy Spawner GameObject

1. **Open `Main.unity` scene**
2. **Create Spawn Points:**

   - Create 8-10 empty GameObjects
   - Name: `SpawnPoint1`, `SpawnPoint2`, etc.
   - Position them around the edges of your map
   - Example positions:
     - SpawnPoint1: X=20, Y=0, Z=0
     - SpawnPoint2: X=-20, Y=0, Z=0
     - SpawnPoint3: X=0, Y=0, Z=20
     - SpawnPoint4: X=0, Y=0, Z=-20
     - etc.
3. **Create Enemy Spawner GameObject:**

   - Right-click in Hierarchy
   - `Create Empty`
   - Name: `EnemySpawner`
4. **Add EnemySpawner Component:**

   - Select `EnemySpawner`
   - `Add Component` → `EnemySpawner`
5. **Configure EnemySpawner:**

   - In Inspector, `EnemySpawner` component:
     - `Enemy Prefabs`:
       - Set `Size` to 3
       - Element 0: Drag `BasicEnemy` prefab
       - Element 1: Drag `RangedEnemy` prefab
       - Element 2: Drag `FastEnemy` prefab
     - `Spawn Points`:
       - Set `Size` to number of spawn points (e.g., 8)
       - Drag each `SpawnPoint` GameObject into elements
     - `Spawn Interval`: 2 (will be overridden by difficulty)
     - `Max Enemies On Screen`: 20 (will be overridden)
     - `Easy Settings`: Drag `EasyDifficulty` asset
     - `Medium Settings`: Drag `MediumDifficulty` asset
     - `Hard Settings`: Drag `HardDifficulty` asset
6. **Save Scene:**

   - `File → Save`

---

## 14. Audio Setup

### Step 14.1: Import Audio Files

1. **Prepare Audio Files:**

   - Get or create audio files:
     - Background music (loopable)
     - Player attack sound
     - Enemy attack sound
     - Item pickup sound
     - Player hurt sound
     - Enemy death sound
     - Footstep sound
2. **Import to Unity:**

   - Drag audio files into `Assets/Audio/` folder
   - Organize: Music in `Audio/Music/`, SFX in `Audio/SFX/`
3. **Configure Audio Import Settings:**

   - Select audio file in Project
   - In Inspector:
     - For Music:
       - `Load Type`: `Streaming` (for large files)
       - `Compression Format`: `Vorbis`
       - `Quality`: 70
     - For SFX:
       - `Load Type`: `Decompress On Load`
       - `Compression Format`: `PCM` (for quality) or `Vorbis` (for size)
       - `3D Sound`: Check this for spatial sounds
       - `Spatial Blend`: 1.0 (full 3D)

### Step 14.2: Assign Audio to AudioManager

1. **Open `Main.unity` scene**
2. **Select AudioManager GameObject:**

   - In Hierarchy, find `AudioManager`
3. **Assign Audio Clips:**

   - In Inspector, `AudioManager` component:
     - `Background Music`: Drag music file from `Assets/Audio/Music/`
     - `Player Attack Sound`: Drag from `Assets/Audio/SFX/`
     - `Enemy Attack Sound`: Drag from `Assets/Audio/SFX/`
     - `Item Pickup Sound`: Drag from `Assets/Audio/SFX/`
     - `Player Hurt Sound`: Drag from `Assets/Audio/SFX/`
     - `Enemy Death Sound`: Drag from `Assets/Audio/SFX/`
     - `Footstep Sound`: Drag from `Assets/Audio/SFX/`
4. **Save Scene:**

   - `File → Save`

---

## 15. Lighting Setup

### Step 15.1: Set Up Multiple Light Sources

1. **Open `Main.unity` scene**
2. **Configure Directional Light (Sun):**

   - Select `Directional Light` in Hierarchy
   - In Inspector:
     - `Transform`:
       - `Rotation`: X=50, Y=-30, Z=0
     - `Light` component:
       - `Type`: `Directional`
       - `Color`: Light yellow (R=255, G=245, B=220)
       - `Intensity`: 1.2
       - `Shadows`: `Soft Shadows`
3. **Create Point Lights (Lanterns):**

   - Right-click in Hierarchy
   - `Light → Point Light`
   - Name: `Lantern1`
   - In Inspector:
     - `Transform`:
       - `Position`: X=5, Y=3, Z=5
     - `Light` component:
       - `Type`: `Point`
       - `Color`: Orange (R=255, G=150, B=50)
       - `Intensity`: 2
       - `Range`: 10
       - `Shadows`: `Soft Shadows`
4. **Create More Point Lights:**

   - Duplicate `Lantern1` 4-5 times
   - Position them around the scene
   - Vary colors slightly (warm oranges, yellows)
5. **Create Spot Lights (Optional):**

   - Right-click in Hierarchy
   - `Light → Spot Light`
   - Name: `SpotLight1`
   - In Inspector:
     - `Transform`:
       - `Position`: X=0, Y=8, Z=0
       - `Rotation`: X=90, Y=0, Z=0
     - `Light` component:
       - `Type`: `Spot`
       - `Color`: White
       - `Intensity`: 3
       - `Range`: 15
       - `Spot Angle`: 45
       - `Shadows`: `Soft Shadows`
6. **Configure Light Settings:**

   - `Edit → Project Settings → Quality`
   - Under `Rendering`:
     - `Pixel Light Count`: 4 or higher
     - `Shadows`: `All` or `Hard and Soft`
7. **Save Scene:**

   - `File → Save`

---

## 16. Particle Effects Setup

### Step 16.1: Create Click Effect

1. **Create Particle System:**

   - Right-click in Hierarchy
   - `Effects → Particle System`
   - Name: `ClickEffect`
2. **Configure Particle System:**

   - Select `ClickEffect`
   - In Inspector, `Particle System` component:
     - `Duration`: 0.5
     - `Start Lifetime`: 0.5
     - `Start Speed`: 2
     - `Start Size`: 0.2
     - `Start Color`: White or light blue
     - `Max Particles`: 20
     - `Emission`:
       - `Rate over Time`: 0
       - `Bursts`: Click `+`
         - `Count`: 10
     - `Shape`:
       - `Shape`: `Circle`
       - `Radius`: 0.5
     - `Color over Lifetime`:
       - Enable checkbox
       - Set gradient: White → Transparent
     - `Size over Lifetime`:
       - Enable checkbox
       - Set curve: 1 → 0
3. **Create Prefab:**

   - Drag `ClickEffect` to `Assets/Prefabs/`
   - Delete from scene
4. **Assign to PlayerController:**

   - Select `Player` GameObject
   - In Inspector, `PlayerController` component:
     - `Click Effect`: Drag `ClickEffect` prefab

### Step 16.2: Create Hit Effect

1. **Create Hit Particle System:**

   - Right-click in Hierarchy
   - `Effects → Particle System`
   - Name: `HitEffect`
2. **Configure Hit Effect:**

   - In Inspector:
     - `Duration`: 0.3
     - `Start Lifetime`: 0.3
     - `Start Speed`: 3
     - `Start Size`: 0.3
     - `Start Color`: Red
     - `Max Particles`: 15
     - `Emission`:
       - `Bursts`: Count 8
     - `Shape`: `Sphere`, Radius 0.3
     - `Color over Lifetime`: Red → Transparent
3. **Create Prefab:**

   - Drag to `Assets/Prefabs/`
   - Delete from scene
4. **Assign to PlayerController and EnemyAI:**

   - `PlayerController` → `Hit Effect`: Drag prefab
   - `EnemyAI` → `Hit Effect`: Drag prefab

### Step 16.3: Create Item Pickup Effect

1. **Create Pickup Particle System:**

   - Similar process
   - Name: `PickupEffect`
   - Color: Gold/Yellow
   - Shape: `Sphere`
   - Burst: 20 particles
2. **Create Prefab and Assign:**

   - Same process

### Step 16.4: Create Environmental Effects

1. **Create Sand Particles:**

   - Right-click in Hierarchy
   - `Effects → Particle System`
   - Name: `SandParticles`
   - Configure:
     - `Duration`: Infinite (uncheck)
     - `Start Lifetime`: 5
     - `Start Speed`: 1
     - `Start Size`: 0.1
     - `Start Color`: Beige/Tan
     - `Max Particles`: 100
     - `Emission`: `Rate over Time`: 20
     - `Shape`: `Box`
     - `Simulation Space`: `World`
2. **Position in Scene:**

   - Place where you want sand effects
   - Create multiple instances

---

## 17. NavMesh Setup

### Step 17.1: Set Up NavMesh

1. **Open `Main.unity` scene**
2. **Mark Objects as Navigation Static:**

   - Select `Ground` plane
   - In Inspector, top-right:
     - Click `Static` dropdown
     - Check `Navigation Static`
   - Select all buildings
   - Mark as `Navigation Static`
3. **Open Navigation Window:**

   - `Window → AI → Navigation`
4. **Bake NavMesh:**

   - In Navigation window, `Bake` tab:
     - `Agent Radius`: 0.5
     - `Agent Height`: 2
     - `Max Slope`: 45
     - `Step Height`: 0.4
     - `Drop Height`: 0
     - `Jump Distance`: 0
   - Click `Bake` button
   - Wait for blue NavMesh to appear in Scene view
5. **Verify NavMesh:**

   - In Scene view, you should see blue areas
   - These are walkable areas
   - Enemies and player can only move on blue areas
6. **Set Up NavMesh Obstacles (Optional):**

   - If you want moving obstacles:
     - Select obstacle GameObject
     - `Add Component` → `Nav Mesh Obstacle`
     - Configure shape and size
7. **Save Scene:**

   - `File → Save`

---

## 18. Terrain Setup

### Step 18.1: Create Terrain

1. **Open `Main.unity` scene** (or `Level2.unity`)
2. **Create Terrain:**

   - Right-click in Hierarchy
   - `3D Object → Terrain`
   - Name: `Terrain`
3. **Configure Terrain:**

   - Select `Terrain`
   - In Inspector, `Terrain` component:
     - `Terrain Data`: Click `Create New...`
     - Save as: `MainTerrainData` in `Assets/`
     - `Terrain Width`: 200
     - `Terrain Length`: 200
     - `Terrain Height`: 30
4. **Import Terrain Textures:**

   - Get terrain textures (sand, dirt, grass)
   - Import to `Assets/Textures/`
   - Configure textures:
     - Select texture
     - In Inspector:
       - `Texture Type`: `Default`
       - `Max Size`: 512 or 1024
       - Click `Apply`
5. **Paint Terrain:**

   - Select `Terrain` in Hierarchy
   - In Inspector, `Paint Texture` tool:
     - Click `Edit Terrain Textures...`
     - Click `Add Texture...`
     - Assign sand texture
     - Click `Add`
     - Paint entire terrain with sand
     - Add more textures and paint different areas
6. **Sculpt Terrain (Optional):**

   - Use `Raise/Lower Terrain` tool
   - Create hills and valleys
   - Use `Smooth Height` to smooth
7. **Place Objects on Terrain:**

   - Position buildings and objects on terrain
   - Use `Snap to Terrain` (right-click in Scene view)
8. **Rebake NavMesh:**

   - `Window → AI → Navigation`
   - Click `Bake`
   - NavMesh will include terrain
9. **Save Scene:**

   - `File → Save`

---

## 19. Build Settings

### Step 19.1: Add Scenes to Build

1. **Open Build Settings:**

   - `File → Build Settings...`
2. **Add Scenes:**

   - Click `Add Open Scenes` (adds current scene)
   - OR drag scenes from Project window to `Scenes In Build` list
   - Add in this order:
     1. `SplashScreen` (Index 0)
     2. `MainMenu` (Index 1)
     3. `Tutorial` (Index 2)
     4. `Main` (Index 3)
     5. `Level2` (Index 4)
3. **Set Splash Screen as First Scene:**

   - In `Scenes In Build`, ensure `SplashScreen` is at index 0
   - Drag to reorder if needed
4. **Close Build Settings:**

   - Click `X` or press Escape
5. **Save All Scenes:**

   - `File → Save All` (Ctrl+Shift+S / Cmd+Shift+S)

---

## 20. Final Testing

### Step 20.1: Test Each Scene

1. **Test Splash Screen:**

   - Open `SplashScreen.unity`
   - Click Play
   - Verify:
     - Title appears
     - Transitions to MainMenu after 3 seconds
2. **Test Main Menu:**

   - Open `MainMenu.unity`
   - Click Play
   - Verify:
     - Buttons are clickable
     - Start Game loads Main scene
     - Options opens options panel
     - Quit exits (in editor, stops play mode)
3. **Test Options Menu:**

   - In MainMenu, click Options
   - Verify:
     - Sliders work
     - Difficulty dropdown works
     - Settings save (restart game, check if saved)
4. **Test Tutorial:**

   - Open `Tutorial.unity`
   - Click Play
   - Verify:
     - Tutorial text appears
     - Skip button works
     - Transitions to Main scene
5. **Test Main Game Scene:**

   - Open `Main.unity`
   - Click Play
   - Verify:
     - Player spawns
     - Can click to move
     - Enemies spawn
     - Can attack enemies
     - Health bar updates
     - Timer counts down
     - Score updates
     - Items can be picked up
     - Pause menu works (ESC)
     - End menu appears on win/lose
6. **Test Save/Load:**

   - In game, pause
   - Click Save (if button exists)
   - Quit to menu
   - Click Load
   - Verify game state restored

### Step 20.2: Common Issues Checklist

- **Player not moving:**

  - Check NavMesh is baked
  - Check Player has NavMesh Agent
  - Check Clickable Layers includes Ground layer
- **Enemies not spawning:**

  - Check EnemySpawner has prefabs assigned
  - Check spawn points exist
  - Check GameManager state is "Playing"
  - Check difficulty settings assigned
- **UI not updating:**

  - Check UIManager has all references assigned
  - Check GameManager exists in scene
  - Check events are firing (use Debug.Log)
- **Audio not playing:**

  - Check AudioManager has clips assigned
  - Check AudioManager GameObject exists
  - Check volume settings
- **Scene transitions not working:**

  - Check SceneTransitionManager exists
  - Check scene names are correct
  - Check scenes are in Build Settings

### Step 20.3: Performance Check

1. **Check Frame Rate:**

   - `Window → Analysis → Profiler`
   - Click Play
   - Monitor FPS (should be 60+)
2. **Optimize if Needed:**

   - Reduce particle counts
   - Reduce light count
   - Use LOD groups for models
   - Optimize textures

---

## Additional Notes

### Creating Materials

1. **Create Material:**

   - Right-click in `Assets/Materials/`
   - `Create → Material`
   - Name: `SandMaterial`
2. **Configure Material:**

   - In Inspector:
     - `Albedo`: Assign texture or set color
     - `Metallic`: 0
     - `Smoothness`: 0.3
     - `Normal Map`: Assign if you have one
3. **Assign to Objects:**

   - Drag material onto GameObject in Scene or Hierarchy

### Creating Animator Controllers

1. **Create Animator Controller:**

   - Right-click in Project
   - `Create → Animator Controller`
   - Name: `PlayerAnimator`
2. **Set Up States:**

   - Double-click to open Animator window
   - Right-click → `Create State → Empty`
   - Name: `Idle`
   - Assign Idle animation clip
   - Repeat for `Walk`, `Attack`, `Death`
3. **Create Transitions:**

   - Click on state
   - Right-click → `Make Transition`
   - Drag to target state
   - Configure conditions in Inspector
4. **Assign to GameObject:**

   - Select GameObject
   - In Animator component, assign controller

---

## Conclusion

You now have a complete Unity setup for "The Last Oasis"!

**Next Steps:**

1. Test everything thoroughly
2. Add your own art assets
3. Fine-tune gameplay balance
4. Add more polish (animations, effects)
5. Build and test final game

**Remember:**

- Save frequently (Ctrl+S / Cmd+S)
- Test each feature as you add it
- Use the Console window to check for errors
- Refer to Unity documentation for specific features

Good luck with your game!
