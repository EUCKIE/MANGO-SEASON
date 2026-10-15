using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class Question3Setup
{
    [MenuItem("Mango Season/Apply Question 3 Setup")]
    public static void Apply()
    {
        var ground = GameObject.Find("Ground");
        var grass = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Ground_Grass.mat");
        grass.SetColor("_BaseColor", new Color(.23f,.46f,.19f));
        // A subtle grid makes ground scale readable without obscuring collectibles.
        var tex = new Texture2D(64,64); tex.wrapMode = TextureWrapMode.Repeat;
        for(int y=0;y<64;y++) for(int x=0;x<64;x++) tex.SetPixel(x,y, x<2 || y<2 ? new Color(.82f,.88f,.75f) : Color.white);
        tex.Apply();
        System.IO.File.WriteAllBytes("Assets/Materials/GroundGrid.png",tex.EncodeToPNG());
        AssetDatabase.ImportAsset("Assets/Materials/GroundGrid.png");
        grass.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/GroundGrid.png"));
        grass.SetTextureScale("_BaseMap",new Vector2(20,13));
        ground.GetComponent<Renderer>().sharedMaterial=grass;
        var wall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Wall_Hazard.mat");
        wall.SetColor("_BaseColor",new Color(.72f,.16f,.10f)); wall.SetFloat("_Smoothness",.12f);
        foreach(var r in GameObject.Find("Walls").GetComponentsInChildren<Renderer>()) r.sharedMaterial=wall;
        var gold = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mango_Golden.mat");
        gold.SetColor("_BaseColor",new Color(1,.67f,.04f)); gold.SetFloat("_Smoothness",.45f);
        gold.EnableKeyword("_EMISSION"); gold.SetColor("_EmissionColor",new Color(.35f,.17f,.01f));
        var sun = GameObject.Find("Directional Light").GetComponent<Light>();
        sun.type=LightType.Directional; sun.color=new Color(1,.90f,.73f); sun.intensity=1.35f;
        sun.transform.rotation=Quaternion.Euler(50,-35,0); sun.shadows=LightShadows.Soft; sun.shadowStrength=.75f;
        RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight=new Color(.48f,.55f,.64f); RenderSettings.ambientIntensity=.8f;
        var lampObj=GameObject.Find("Harvest Point Light") ?? new GameObject("Harvest Point Light");
        lampObj.transform.SetParent(GameObject.Find("Lighting").transform);
        lampObj.transform.position=new Vector3(-9,4,-2);
        var lamp=lampObj.GetComponent<Light>(); if(lamp==null) lamp=lampObj.AddComponent<Light>();
        lamp.type=LightType.Point; lamp.color=new Color(1,.68f,.25f); lamp.intensity=4; lamp.range=12;
        lamp.shadows=LightShadows.Soft; lamp.shadowStrength=.5f;
        var camera=GameObject.Find("Main Camera").GetComponent<Camera>();
        camera.transform.position=new Vector3(0,30,-30); camera.transform.LookAt(new Vector3(0,0,1));
        camera.orthographic=false; camera.fieldOfView=55; camera.nearClipPlane=.3f; camera.farClipPlane=120;
        camera.cullingMask=~(1<<2); camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=new Color(.13f,.21f,.25f);
        var player=GameObject.Find("Player"); player.transform.position=new Vector3(-14,1,-7);
        var rb=player.GetComponent<Rigidbody>(); rb.constraints=RigidbodyConstraints.FreezeRotation; rb.useGravity=true;
        var managerObj=GameObject.Find("Harvest Game") ?? new GameObject("Harvest Game");
        var game=managerObj.GetComponent<MangoSeasonGame>(); if(game==null) game=managerObj.AddComponent<MangoSeasonGame>();
        game.player=player.transform;
        var fruitRoot=GameObject.Find("Collectibles").transform;
        game.Total=fruitRoot.childCount;
        int i=0;
        Vector3[] positions={new Vector3(-11,.65f,-7),new Vector3(-5,.65f,-4),new Vector3(1,.65f,-2),new Vector3(7,.65f,4),new Vector3(14,.65f,6),new Vector3(-14,.65f,7),new Vector3(-4,.65f,8),new Vector3(3,.65f,-9)};
        foreach(Transform fruit in fruitRoot)
        {
            fruit.position=positions[i++ % positions.Length]; fruit.localScale=new Vector3(.75f,.95f,.75f);
            fruit.GetComponent<Renderer>().sharedMaterial=gold;
            fruit.GetComponent<Collider>().isTrigger=true;
            var pickup=fruit.GetComponent<MangoPickup>(); if(pickup==null) pickup=fruit.gameObject.AddComponent<MangoPickup>(); pickup.game=game;
        }
        var canvasObj=GameObject.Find("Harvest HUD");
        if(canvasObj==null || canvasObj.transform.childCount==0)
        {
            if(canvasObj==null) canvasObj=new GameObject("Harvest HUD",typeof(Canvas),typeof(UnityEngine.UI.CanvasScaler),typeof(UnityEngine.UI.GraphicRaycaster));
            var canvas=canvasObj.GetComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scale=canvasObj.GetComponent<UnityEngine.UI.CanvasScaler>(); scale.uiScaleMode=UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scale.referenceResolution=new Vector2(1600,900); scale.matchWidthOrHeight=.5f;
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
            if(font==null) throw new System.Exception("TMP Essentials font is not imported yet.");
            var panel=new GameObject("Header",typeof(RectTransform),typeof(UnityEngine.UI.Image)); panel.transform.SetParent(canvasObj.transform,false);
            var rect=panel.GetComponent<RectTransform>(); rect.anchorMin=new Vector2(0,1);rect.anchorMax=Vector2.one;rect.pivot=new Vector2(.5f,1);rect.sizeDelta=new Vector2(0,104);rect.anchoredPosition=Vector2.zero;
            panel.GetComponent<UnityEngine.UI.Image>().color=new Color(.055f,.12f,.10f,.96f);
            Text(panel.transform,"Title","MANGO SEASON",font,new Vector2(28,-14),new Vector2(650,40),32,new Color(1,.78f,.23f));
            game.scoreText=Text(panel.transform,"Score","MANGOES  0 / 8",font,new Vector2(1050,-18),new Vector2(510,42),32,Color.white);
            Text(panel.transform,"Controls","WASD / ARROWS  •  Walk into mangoes to collect",font,new Vector2(28,-58),new Vector2(1050,32),20,new Color(.83f,.89f,.80f));
            var footer=new GameObject("Feedback Background",typeof(RectTransform),typeof(UnityEngine.UI.Image));footer.transform.SetParent(canvasObj.transform,false);
            var fr=footer.GetComponent<RectTransform>();fr.anchorMin=Vector2.zero;fr.anchorMax=new Vector2(1,0);fr.pivot=new Vector2(.5f,0);fr.sizeDelta=new Vector2(0,62);fr.anchoredPosition=Vector2.zero;
            footer.GetComponent<UnityEngine.UI.Image>().color=new Color(.055f,.12f,.10f,.96f);
            game.messageText=Text(footer.transform,"Feedback","Collect the golden mangoes. Avoid the red boundaries.",font,new Vector2(28,-12),new Vector2(1500,40),25,Color.white);
        }
        EditorUtility.SetDirty(game); EditorUtility.SetDirty(grass); EditorUtility.SetDirty(wall);EditorUtility.SetDirty(gold);
        AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        var view=SceneView.lastActiveSceneView;
        if(view!=null) { view.LookAt(new Vector3(0,0,0),Quaternion.Euler(55,0,0),27); view.Repaint(); }
        Debug.Log("Question 3 setup saved: materials, two lights, camera, score and feedback.");
    }
    static TMP_Text Text(Transform parent,string name,string value,TMP_FontAsset font,Vector2 pos,Vector2 size,int fontSize,Color color)
    {
        var obj=new GameObject(name,typeof(RectTransform),typeof(TextMeshProUGUI));obj.transform.SetParent(parent,false);
        var rect=obj.GetComponent<RectTransform>();rect.anchorMin=new Vector2(0,1);rect.anchorMax=new Vector2(0,1);rect.pivot=new Vector2(0,1);rect.anchoredPosition=pos;rect.sizeDelta=size;
        var text=obj.GetComponent<TextMeshProUGUI>();text.font=font;text.text=value;text.fontSize=fontSize;text.color=color;text.raycastTarget=false;
        return text;
    }
}
