using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Editor tool: Tools > Build Level 1
// Places the platforms, enemies, checkpoints, gate, pit and exit as REAL objects
// in the open scene (Level1). Nothing is created while the game runs.
// The scripts (Hazard, PatrolEnemy, Checkpoint, CameraFollow...) only add behavior.
public static class Level1Builder
{
    static readonly Color EnemyRed   = new Color(1.00f, 0.15f, 0.15f);
    static readonly Color GateColor  = new Color(0.35f, 0.45f, 0.70f);
    static readonly Color PlateColor = new Color(1.00f, 0.85f, 0.10f);
    static readonly Color ExitColor  = new Color(0.20f, 1.00f, 0.40f);
    static readonly Color FlagColor  = new Color(0.00f, 0.55f, 0.65f);

    static GameObject groundTemplate;
    static Sprite boxSprite;

    [MenuItem("Tools/Build Level 1")]
    public static void Build()
    {
        GameObject ground = GameObject.Find("Ground");
        GameObject player = GameObject.Find("Player");
        GameObject plate  = GameObject.Find("PressurePlate");
        GameObject door   = GameObject.Find("ExitDoor");
        if (door == null) door = GameObject.Find("Door");
        Camera cam = Camera.main;

        if (ground == null || player == null || plate == null || door == null || cam == null)
        {
            EditorUtility.DisplayDialog("Build Level 1",
                "Open the Level1 scene first. It needs: Ground, Player, PressurePlate, Door and Main Camera.", "OK");
            return;
        }

        // Already built? offer to rebuild
        if (GameObject.Find("Platforms") != null)
        {
            if (!EditorUtility.DisplayDialog("Build Level 1",
                    "The level was already built. Delete it and build again?", "Rebuild", "Cancel"))
                return;
            foreach (string n in new[] { "Platforms", "Enemies", "Checkpoints", "KillZone" })
            {
                GameObject old = GameObject.Find(n);
                if (old != null) Undo.DestroyObjectImmediate(old);
            }
        }

        groundTemplate = ground;
        boxSprite = ground.GetComponent<SpriteRenderer>().sprite;

        Transform platforms   = Container("Platforms");
        Transform enemies     = Container("Enemies");
        Transform checkpoints = Container("Checkpoints");

        // ---------------- Platforms (top surface of the floor is y = -3) ----------------
        Wall("LeftWall", platforms, -8f, 2f, 1f, 16f);
        Block("Step_Intro", platforms, 2.5f, 3.5f, -2f, 1f);          // first little jump

        Block("Ground_B",  platforms, 12f, 20f, -3f, 1f);             // after pit 1
        Block("Island_C",  platforms, 24f, 28f, -3f, 1f);             // after pit 2
        Block("Platform_1", platforms, 31f, 34f, -1.5f, 0.5f);        // stairs over the pit
        Block("Platform_2", platforms, 37f, 40f, 0f, 0.5f);
        Block("Platform_3", platforms, 43f, 46f, -1.5f, 0.5f);

        Block("Ground_D",  platforms, 48f, 70f, -3f, 1f);             // puzzle area
        Block("Platform_Plate", platforms, 56f, 60f, -0.5f, 0.5f);    // the switch is up here
        GameObject gate = Wall("Gate", platforms, 66f, 1.5f, 1f, 9f);
        gate.GetComponent<SpriteRenderer>().color = GateColor;

        Block("Ground_F",  platforms, 74.5f, 92f, -3f, 1f);           // final stretch
        Wall("RightWall", platforms, 93f, 2f, 1f, 16f);

        // ---------------- Enemies (red = danger) ----------------
        Enemy(enemies, "Enemy_1", 16f,   -2.55f, Vector2.right, 2.5f, 2.0f);
        Enemy(enemies, "Enemy_Flyer_1", 41.5f, -1.0f, Vector2.up, 1.8f, 2.2f);
        Enemy(enemies, "Enemy_3", 52.5f, -2.55f, Vector2.right, 2.0f, 2.5f);
        Enemy(enemies, "Enemy_4", 62.5f, -2.55f, Vector2.right, 1.5f, 3.0f);
        Enemy(enemies, "Enemy_5", 78.5f, -2.55f, Vector2.right, 2.0f, 3.5f);
        Enemy(enemies, "Enemy_Flyer_2", 84f, -1.5f, Vector2.up, 1.4f, 2.5f);

        // ---------------- Checkpoints ----------------
        Flag(checkpoints, "Checkpoint_1", 13f);
        Flag(checkpoints, "Checkpoint_2", 49f);
        Flag(checkpoints, "Checkpoint_3", 68.2f);

        // ---------------- Pit (kills the player when falling) ----------------
        GameObject kz = new GameObject("KillZone", typeof(BoxCollider2D));
        Undo.RegisterCreatedObjectUndo(kz, "Build Level 1");
        kz.transform.position = new Vector3(45f, -9f, 0f);
        kz.transform.localScale = new Vector3(160f, 2f, 1f);
        kz.GetComponent<BoxCollider2D>().isTrigger = true;
        kz.AddComponent<Hazard>();

        // ---------------- Existing objects ----------------
        // Pressure plate: now a trigger (PressureSwitch needs OnTriggerEnter2D) that opens the Gate
        plate.transform.position = new Vector3(59f, -0.35f, 0f);
        plate.transform.localScale = new Vector3(1.2f, 0.3f, 1f);
        plate.GetComponent<SpriteRenderer>().color = PlateColor;
        plate.GetComponent<BoxCollider2D>().isTrigger = true;
        PressureSwitch ps = plate.GetComponent<PressureSwitch>();
        ps.doorToOpen = gate;
        EditorUtility.SetDirty(ps);

        // Door (has WinZone): it is the EXIT, so it goes to the end of the level and turns green
        door.name = "ExitDoor";
        door.transform.position = new Vector3(89f, -2f, 0f);
        door.transform.localScale = new Vector3(1.2f, 2f, 1f);
        door.GetComponent<SpriteRenderer>().color = ExitColor;

        // Player: start position + a tighter jump (more gravity, same speed)
        player.transform.position = new Vector3(-5f, -2.4f, 0f);
        PlayerController pc = player.GetComponent<PlayerController>();
        pc.jumpForce = 14f;
        EditorUtility.SetDirty(pc);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sharedMaterial = GetNoFrictionMaterial();
        EditorUtility.SetDirty(rb);

        // Camera follows the player
        cam.transform.position = new Vector3(0f, 0f, -10f);
        CameraFollow cf = cam.GetComponent<CameraFollow>();
        if (cf == null) cf = Undo.AddComponent<CameraFollow>(cam.gameObject);
        cf.target = player.transform;
        cf.offset = new Vector3(0f, 1.5f, -10f);
        cf.smoothSpeed = 6f;
        cf.useLimits = true;
        cf.minPosition = new Vector2(0f, 0f);
        cf.maxPosition = new Vector2(82f, 2f);
        EditorUtility.SetDirty(cf);

        EditorSceneManager.MarkSceneDirty(player.scene);
        Debug.Log("Level 1 built. Press Ctrl+S to save the scene.");
    }

    // ---------------- helpers ----------------
    static Transform Container(string name)
    {
        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Build Level 1");
        return go.transform;
    }

    // A platform between x0 and x1, whose top surface is at topY
    static GameObject Block(string name, Transform parent, float x0, float x1, float topY, float thickness)
    {
        GameObject go = Object.Instantiate(groundTemplate, parent);
        go.name = name;
        go.transform.localPosition = new Vector3((x0 + x1) / 2f, topY - thickness / 2f, 0f);
        go.transform.localScale = new Vector3(x1 - x0, thickness, 1f);
        Undo.RegisterCreatedObjectUndo(go, "Build Level 1");
        return go;
    }

    // A block defined by its center and size
    static GameObject Wall(string name, Transform parent, float x, float yCenter, float w, float h)
    {
        GameObject go = Object.Instantiate(groundTemplate, parent);
        go.name = name;
        go.transform.localPosition = new Vector3(x, yCenter, 0f);
        go.transform.localScale = new Vector3(w, h, 1f);
        Undo.RegisterCreatedObjectUndo(go, "Build Level 1");
        return go;
    }

    static void Enemy(Transform parent, string name, float x, float y, Vector2 dir, float dist, float speed)
    {
        GameObject go = new GameObject(name, typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D));
        Undo.RegisterCreatedObjectUndo(go, "Build Level 1");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(x, y, 0f);
        go.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = boxSprite;
        sr.color = EnemyRed;
        sr.sortingOrder = 1;

        BoxCollider2D col = go.GetComponent<BoxCollider2D>();
        col.size = Vector2.one;
        col.isTrigger = true;

        go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        go.AddComponent<Hazard>();
        PatrolEnemy p = go.AddComponent<PatrolEnemy>();
        p.direction = dir;
        p.distance = dist;
        p.speed = speed;
    }

    static void Flag(Transform parent, string name, float x)
    {
        GameObject go = new GameObject(name, typeof(SpriteRenderer), typeof(BoxCollider2D));
        Undo.RegisterCreatedObjectUndo(go, "Build Level 1");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(x, -2f, 0f);
        go.transform.localScale = new Vector3(0.3f, 2f, 1f);

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = boxSprite;
        sr.color = FlagColor;
        sr.sortingOrder = -1;

        BoxCollider2D col = go.GetComponent<BoxCollider2D>();
        col.size = new Vector2(3f, 1f);   // wider trigger so it is easy to touch
        col.isTrigger = true;

        go.AddComponent<Checkpoint>();
    }

    static PhysicsMaterial2D GetNoFrictionMaterial()
    {
        const string folder = "Assets/Materials";
        const string path = folder + "/NoFriction.physicsMaterial2D";

        PhysicsMaterial2D mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);
        if (mat != null) return mat;

        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets", "Materials");

        mat = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        AssetDatabase.CreateAsset(mat, path);
        AssetDatabase.SaveAssets();
        return mat;
    }
}
