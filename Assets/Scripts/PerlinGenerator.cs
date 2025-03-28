using UnityEngine;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class NoiseParameters
{
    public int perlinTexSizeX;
    public int perlinTexSizeY;
    public float noiseScale;
    public bool randomizeNoiseOffset;
    public Vector2 perlinOffset;
}

public class PerlinGenerator : MonoBehaviour
{
    [Header("Размер текстуры.")] 
    public int perlinTexSizeX = 256;
    public int perlinTexSizeY = 256;

    [Header("Настройки шума Перлина.")] 
    public bool randomizeNoiseOffset = true;
    public Vector2 perlinOffset;
    public float noiseScale = 4f;

    [Tooltip("Градиент для выбора цвета в зависимости от значения шума.")]
    public Gradient noiseColorGradient;

    [Tooltip("RawImage, на котором будет отображаться текстура шума.")]
    public RawImage noiseImage;

    [Tooltip("Путь для сохранения экспортированного изображения, параметров и префабов (относительно проекта).")]
    public string exportPath = "Assets/NoisePrefabs/";

    [Tooltip("Имя файла для сохранения изображения.")]
    public string exportFileName = "noise.png";
    
    [Tooltip("MeshFilter, к которому будет применяться шум для создания высот.")]
    public MeshFilter targetMeshFilter;

    [Tooltip("Множитель для высоты.")] 
    public float heightMultiplier = 2.5f;

    //[Tooltip("Если включено, материал MeshRenderer обновится.")]
    public bool updateMeshMaterial = true;

    [Tooltip("Shader для мэша ландшафта.")] 
    public Shader terrainShader;

    [Tooltip("Добавлять воду?")] 
    public bool addWater = true;

    [Tooltip("Уровень моря (в единицах мира)")]
    public float waterLevel = 0.5f;

    [Tooltip("Shader для воды.")]
    public Shader waterShader;

    [Tooltip("Путь для сохранения префаба плейна (относительно проекта)")]
    public string prefabExportPath = "Assets/NoisePrefabs/";

    [Tooltip("Сохранить в префабе также воду как дочерний объект?")]
    public bool includeWaterInPrefab = false;

    [Tooltip("Сохранить в префабе также сгенерированные деревья как дочерний объект?")]
    public bool includeTreesInPrefab = false;
    
    [Header("Настройки деревьев")] 
    public GameObject treePrefab;
    
    [Range(0f, 1f)] public float treeSpawnDensity = 0.1f;

    private Texture2D generatedTexture;
    
    private GameObject currentPlane;
    private GameObject currentWaterPlane;

    /// <summary>
    /// Генерирует текстуру шума Перлина с учетом градиента и назначает её на RawImage.
    /// </summary>
    public void GenerateNoise()
    {
        if (randomizeNoiseOffset)
        {
            perlinOffset = new Vector2(Random.Range(0f, 10000f), Random.Range(0f, 10000f));
        }

        generatedTexture = new Texture2D(perlinTexSizeX, perlinTexSizeY);
        generatedTexture.filterMode = FilterMode.Point;
        for (int y = 0; y < perlinTexSizeY; y++)
        {
            for (int x = 0; x < perlinTexSizeX; x++)
            {
                float xCoord = perlinOffset.x + ((float)x / perlinTexSizeX) * noiseScale;
                float yCoord = perlinOffset.y + ((float)y / perlinTexSizeY) * noiseScale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Color color = noiseColorGradient.Evaluate(sample);
                
                generatedTexture.SetPixel(x, y, color);
            }
        }

        generatedTexture.Apply();
        if (noiseImage != null)
        {
            noiseImage.texture = generatedTexture;
        }
        else
        {
            Debug.LogWarning("RawImage для отображения текстуры не назначен!");
        }
    }

    /// <summary>
    /// Экспортирует сгенерированную текстуру в PNG по указанному пути.
    /// </summary>
    public void ExportNoiseTexture()
    {
        if (generatedTexture == null)
        {
            Debug.LogWarning("Сначала необходимо сгенерировать текстуру!");
            return;
        }

        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
        }

        byte[] bytes = generatedTexture.EncodeToPNG();
        string fullPath = Path.Combine(exportPath, exportFileName);
        File.WriteAllBytes(fullPath, bytes);
        
        Debug.Log("Текстура сохранена по пути: " + fullPath);
    }

    /// <summary>
    /// Загружает текстуру из указанного файла и назначает её на RawImage.
    /// </summary>
    public void LoadNoiseTexture()
    {
    #if UNITY_EDITOR
        string fullPath = Path.Combine(exportPath, exportFileName);
        if (!File.Exists(fullPath))
        {
            Debug.LogWarning("Файл не найден по пути: " + fullPath);
            return;
        }

        byte[] bytes = File.ReadAllBytes(fullPath);
        Texture2D loadedTexture = new Texture2D(2, 2);
        if (loadedTexture.LoadImage(bytes))
        {
            generatedTexture = loadedTexture;
            if (noiseImage != null)
            {
                noiseImage.texture = generatedTexture;
            }

            Debug.Log("Текстура успешно загружена из: " + fullPath);
        }
        else
        {
            Debug.LogWarning("Не удалось загрузить текстуру.");
        }
    #else
        Debug.LogWarning("Загрузка текстуры доступна только в редакторе.");
    #endif
    }

    /// <summary>
    /// Загружает параметры шума из JSON файла и обновляет настройки генератора,
    /// затем перегенерирует текстуру для RawImage.
    /// </summary>
    public void LoadNoiseParameters()
    {
    #if UNITY_EDITOR
        string path = Path.Combine(exportPath, "NoiseParameters.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            NoiseParameters parameters = JsonUtility.FromJson<NoiseParameters>(json);
            if (parameters != null)
            {
                perlinTexSizeX = parameters.perlinTexSizeX;
                perlinTexSizeY = parameters.perlinTexSizeY;
                noiseScale = parameters.noiseScale;
                randomizeNoiseOffset = parameters.randomizeNoiseOffset;
                perlinOffset = parameters.perlinOffset;

                Debug.Log("Параметры шума успешно загружены из: " + path);
    
                GenerateNoise();
            }
            else
            {
                Debug.LogWarning("Не удалось распарсить параметры шума из файла.");
            }
        }
        else
        {
            Debug.LogWarning("Файл параметров не найден по пути: " + path);
        }
    #else
        Debug.LogWarning("Загрузка параметров доступна только в редакторе!");
    #endif
    }

    /// <summary>
    /// Применяет шум для генерации высот меша, используя UV-координаты.
    /// </summary>
    public void ApplyNoiseToMesh()
    {
        ClearTrees();
        
        if (targetMeshFilter == null)
        {
            Debug.LogWarning("MeshFilter не назначен!");
            return;
        }

        if (generatedTexture == null)
        {
            Debug.LogWarning("Сначала необходимо сгенерировать текстуру шума!");
            return;
        }

        Mesh mesh = targetMeshFilter.sharedMesh;
        if (mesh == null)
        {
            Debug.LogWarning("У MeshFilter отсутствует меш!");
            return;
        }

        Vector3[] vertices = mesh.vertices;
        Color[] vertexColors = new Color[vertices.Length];
        Vector2[] uvs = mesh.uv;
        if (uvs.Length != vertices.Length)
        {
            Debug.LogWarning("UV координаты отсутствуют или их количество не соответствует количеству вершин!");
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            float sample = Mathf.PerlinNoise(perlinOffset.x + uvs[i].x * noiseScale,
                perlinOffset.y + uvs[i].y * noiseScale);
            Color color = noiseColorGradient.Evaluate(sample);
            vertices[i].y = sample * heightMultiplier;
            vertexColors[i] = color;
        }

        mesh.vertices = vertices;
        mesh.colors = vertexColors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        Debug.Log("Высоты и цвета вершин обновлены на основе шума.");
        
        if (updateMeshMaterial)
        {
            MeshRenderer renderer = targetMeshFilter.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.SetTexture("_HeightTex", generatedTexture);
                Debug.Log("Материал меша обновлён: _HeightTex установлен.");
            }
            else
            {
                Debug.LogWarning("MeshRenderer или его материал не назначен!");
            }
        }
    }

    /// <summary>
    /// Генерирует процедурный плейн с корректными UV, так чтобы его центр совпадал с (0,0,0).
    /// При генерации нового плейна старый удаляется. Также настраивается камера для обзора.
    /// Если включена опция addWater, то генерируется водная плоскость.
    /// </summary>
    public void GeneratePlane()
    {
        ClearTrees();
        ClearPlanesAndWater();
        
        if (currentPlane != null)
        {
        #if UNITY_EDITOR
            DestroyImmediate(currentPlane);
        #else
            Destroy(currentPlane);
        #endif
            currentPlane = null;
        }

        GameObject planeObject = new GameObject("Procedural Plane");
        MeshFilter mf = planeObject.AddComponent<MeshFilter>();
        MeshRenderer mr = planeObject.AddComponent<MeshRenderer>();
        if (terrainShader != null)
        {
            mr.sharedMaterial = new Material(terrainShader);
        }
        else
        {
            mr.sharedMaterial = new Material(Shader.Find("Standard"));
        }

        int xSize = 10;
        int zSize = 10;
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Plane";

        Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[xSize * zSize * 6];

        int vertIndex = 0;
 
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float xPos = x - xSize / 2f;
                float zPos = z - zSize / 2f;
                vertices[vertIndex] = new Vector3(xPos, 0, zPos);
                uvs[vertIndex] = new Vector2((float)x / xSize, (float)z / zSize);
                vertIndex++;
            }
        }

        int triIndex = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                int a = z * (xSize + 1) + x;
                int b = a + 1;
                int c = a + (xSize + 1);
                int d = c + 1;
                triangles[triIndex++] = a;
                triangles[triIndex++] = c;
                triangles[triIndex++] = b;
                triangles[triIndex++] = b;
                triangles[triIndex++] = c;
                triangles[triIndex++] = d;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mf.mesh = mesh;
        
        planeObject.transform.position = Vector3.zero;

        currentPlane = planeObject;
        targetMeshFilter = mf;
        Debug.Log("Плейн сгенерирован с центром в (0,0,0).");
        
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position =
                new Vector3(0, Mathf.Max(xSize, zSize) * 1.5f, -Mathf.Max(xSize, zSize) * 1.5f);
            mainCam.transform.LookAt(Vector3.zero);
            CameraOrbit orbit = mainCam.GetComponent<CameraOrbit>();
            if (orbit != null)
            {
                orbit.target = planeObject.transform;
                orbit.distance = Mathf.Max(xSize, zSize) * 1.5f;
            }
        }

        if (addWater)
        {
            GenerateWaterPlane(xSize, zSize);
        }
    }

    /// <summary>
    /// Генерирует водную плоскость, совпадающую по размерам с плейном, на заданном уровне моря.
    /// Если ранее существовал объект воды то он удаляется.
    /// </summary>
    public void GenerateWaterPlane(int xSize, int zSize)
    {
        if (currentWaterPlane != null)
        {
        #if UNITY_EDITOR
            DestroyImmediate(currentWaterPlane);
        #else
            Destroy(currentWaterPlane);
        #endif
            currentWaterPlane = null;
        }

        GameObject waterPlane = new GameObject("Water Plane");
        MeshFilter mf = waterPlane.AddComponent<MeshFilter>();
        MeshRenderer mr = waterPlane.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        mesh.name = "Water Plane Mesh";

        Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[xSize * zSize * 6];

        int vertIndex = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float xPos = x - xSize / 2f;
                float zPos = z - zSize / 2f;
                vertices[vertIndex] = new Vector3(xPos, 0, zPos);
                uvs[vertIndex] = new Vector2((float)x / xSize, (float)z / zSize);
                vertIndex++;
            }
        }

        int triIndex = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                int a = z * (xSize + 1) + x;
                int b = a + 1;
                int c = a + (xSize + 1);
                int d = c + 1;
                triangles[triIndex++] = a;
                triangles[triIndex++] = c;
                triangles[triIndex++] = b;
                triangles[triIndex++] = b;
                triangles[triIndex++] = c;
                triangles[triIndex++] = d;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mf.mesh = mesh;
        
        waterPlane.transform.position = new Vector3(0, waterLevel, 0);

        if (waterShader != null)
        {
            mr.sharedMaterial = new Material(waterShader);
        }
        else
        {
            mr.sharedMaterial = new Material(Shader.Find("Standard"));
            mr.sharedMaterial.color = Color.blue;
        }

        currentWaterPlane = waterPlane;
        Debug.Log("Водная плоскость сгенерирована на уровне: " + waterLevel);
    }

    /// <summary>
    /// Отдельный метод для добавления воды через кнопку в инспекторе.
    /// </summary>
    public void AddWater()
    {
        if (targetMeshFilter != null)
        {
            Mesh mesh = targetMeshFilter.sharedMesh;
            if (mesh != null)
            {
                int xSize = Mathf.RoundToInt(mesh.bounds.size.x);
                int zSize = Mathf.RoundToInt(mesh.bounds.size.z);
                GenerateWaterPlane(xSize, zSize);
            }
            else
            {
                Debug.LogWarning("У мэша нет корректных размеров!");
            }
        }
        else
        {
            Debug.LogWarning("Нет целевого мэша для добавления воды!");
        }
    }

    /// <summary>
    /// Сохраняет сгенерированный плейн как префаб.
    /// </summary>
    public void SaveMeshAsPrefab()
    {
        #if UNITY_EDITOR
        if (currentPlane != null)
        {
            if (includeWaterInPrefab)
            {
                if (currentWaterPlane == null)
                {
                    Debug.LogWarning("Галочка 'Сохранить в префабе также воду' установлена, но вода не сгенерирована!");
                    return;
                }
                else
                {
                    currentWaterPlane.transform.SetParent(currentPlane.transform);
                }
            }
            
            if (includeTreesInPrefab)
            {
                GameObject treeParent = GameObject.Find("Tree Clusters");
                if (treeParent == null)
                {
                    Debug.LogWarning("Галочка 'Сохранить деревья' установлена, но контейнер деревьев не найден или деревья не сгенерированы!");
                }
                else
                {
                    treeParent.transform.SetParent(currentPlane.transform);
                }
            }
            
            if (generatedTexture != null)
            {
                ExportNoiseTexture();
            }
            else
            {
                Debug.LogWarning("Текстура шума не сгенерирована, поэтому её экспорт невозможен!");
            }
            
            string textureAssetPath = Path.Combine(exportPath, exportFileName);
            Texture2D heightMapTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureAssetPath);
            if (heightMapTexture == null)
            {
                Debug.LogWarning("Не удалось загрузить текстуру шума из: " + textureAssetPath);
            }
            
            MeshFilter planeMF = currentPlane.GetComponent<MeshFilter>();
            if (planeMF != null && planeMF.sharedMesh != null)
            {
                string planeMeshAssetPath = Path.Combine(prefabExportPath, "GeneratedPlaneMesh.asset");
                Mesh existingPlaneMesh = AssetDatabase.LoadAssetAtPath<Mesh>(planeMeshAssetPath);
                if (existingPlaneMesh == null)
                {
                    AssetDatabase.CreateAsset(planeMF.sharedMesh, planeMeshAssetPath);
                    Debug.Log("Plane mesh сохранен в " + planeMeshAssetPath);
                }
                else
                {
                    planeMF.sharedMesh = existingPlaneMesh;
                    Debug.Log("Plane mesh asset уже существует в " + planeMeshAssetPath);
                }
            }
            else
            {
                Debug.LogWarning("Plane MeshFilter или его меш не назначен!");
            }
            
            MeshRenderer planeMR = currentPlane.GetComponent<MeshRenderer>();
            if (planeMR != null && planeMR.sharedMaterial != null)
            {
                if (heightMapTexture != null)
                {
                    planeMR.sharedMaterial.SetTexture("_HeightTex", heightMapTexture);
                }

                string planeMatAssetPath = Path.Combine(prefabExportPath, "GeneratedPlaneMaterial.asset");
                Material existingPlaneMat = AssetDatabase.LoadAssetAtPath<Material>(planeMatAssetPath);
                if (existingPlaneMat == null)
                {
                    Material newPlaneMat = new Material(planeMR.sharedMaterial);
                    AssetDatabase.CreateAsset(newPlaneMat, planeMatAssetPath);
                    planeMR.sharedMaterial = newPlaneMat;
                    Debug.Log("Plane material asset сохранен в " + planeMatAssetPath);
                }
                else
                {
                    planeMR.sharedMaterial = existingPlaneMat;
                    Debug.Log("Plane material asset уже существует в " + planeMatAssetPath);
                }
            }
            else
            {
                Debug.LogWarning("Plane MeshRenderer или материал не назначен!");
            }
            
            if (includeWaterInPrefab && currentWaterPlane != null)
            {
                MeshFilter waterMF = currentWaterPlane.GetComponent<MeshFilter>();
                if (waterMF != null && waterMF.sharedMesh != null)
                {
                    string waterMeshAssetPath = Path.Combine(prefabExportPath, "GeneratedWaterMesh.asset");
                    Mesh existingWaterMesh = AssetDatabase.LoadAssetAtPath<Mesh>(waterMeshAssetPath);
                    if (existingWaterMesh == null)
                    {
                        AssetDatabase.CreateAsset(waterMF.sharedMesh, waterMeshAssetPath);
                        Debug.Log("Water mesh asset сохранен в " + waterMeshAssetPath);
                    }
                    else
                    {
                        waterMF.sharedMesh = existingWaterMesh;
                        Debug.Log("Water mesh asset уже существует в " + waterMeshAssetPath);
                    }
                }
                else
                {
                    Debug.LogWarning("Water MeshFilter или его меш не назначен!");
                }

                MeshRenderer waterMR = currentWaterPlane.GetComponent<MeshRenderer>();
                if (waterMR != null && waterMR.sharedMaterial != null)
                {
                    string waterMatAssetPath = Path.Combine(prefabExportPath, "GeneratedWaterMaterial.asset");
                    Material existingWaterMat = AssetDatabase.LoadAssetAtPath<Material>(waterMatAssetPath);
                    if (existingWaterMat == null)
                    {
                        Material newWaterMat = new Material(waterMR.sharedMaterial);
                        AssetDatabase.CreateAsset(newWaterMat, waterMatAssetPath);
                        waterMR.sharedMaterial = newWaterMat;
                        Debug.Log("Water material asset сохранен в " + waterMatAssetPath);
                    }
                    else
                    {
                        waterMR.sharedMaterial = existingWaterMat;
                        Debug.Log("Water material asset уже существует в " + waterMatAssetPath);
                    }
                }
                else
                {
                    Debug.LogWarning("Water MeshRenderer или материал не назначен!");
                }
            }

            // Сохранение самого префаба
            if (!Directory.Exists(prefabExportPath))
            {
                Directory.CreateDirectory(prefabExportPath);
            }

            string prefabPath = Path.Combine(prefabExportPath, "GeneratedMesh.prefab");
            PrefabUtility.SaveAsPrefabAssetAndConnect(currentPlane, prefabPath, InteractionMode.UserAction);
            Debug.Log("Mesh prefab сохранен в " + prefabPath);
        }
        else
        {
            Debug.LogWarning("Нет сгенерированного плейна для сохранения!");
        }
        #else
            Debug.LogWarning("Сохранение префаба доступно только в редакторе!");
        #endif
    }

    /// <summary>
    /// Сохраняет параметры генерации шума в виде JSON файла (доступно только в редакторе).
    /// </summary>
    public void SaveNoiseParameters()
    {
    #if UNITY_EDITOR
        NoiseParameters parameters = new NoiseParameters();
        parameters.perlinTexSizeX = perlinTexSizeX;
        parameters.perlinTexSizeY = perlinTexSizeY;
        parameters.noiseScale = noiseScale;
        parameters.randomizeNoiseOffset = randomizeNoiseOffset;
        parameters.perlinOffset = perlinOffset;
        string json = JsonUtility.ToJson(parameters, true);
        string path = Path.Combine(exportPath, "NoiseParameters.json");
        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
        }

        File.WriteAllText(path, json);
        Debug.Log("Noise parameters сохранен в " + path);
    #else
        Debug.LogWarning("Сохранение параметров доступно только в редакторе!");
    #endif
    }

    /// <summary>
    /// Метод для генерации деревьев на плейне в местах с травой.
    /// </summary>
    public void GenerateTrees()
    {
        if (targetMeshFilter == null)
        {
            Debug.LogWarning("MeshFilter не назначен, невозможно генерировать деревья!");
            return;
        }

        if (treePrefab == null)
        {
            Debug.LogWarning("Префаб дерева не назначен!");
            return;
        }

        Mesh mesh = targetMeshFilter.sharedMesh;
        if (mesh == null)
        {
            Debug.LogWarning("У целевого меша отсутствуют данные!");
            return;
        }

        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        
        GameObject treeParent = new GameObject("Tree Clusters");
        treeParent.transform.parent = currentPlane != null ? currentPlane.transform : transform;

        for (int i = 0; i < vertices.Length; i++)
        {
            float sample = Mathf.PerlinNoise(perlinOffset.x + uvs[i].x * noiseScale,
                perlinOffset.y + uvs[i].y * noiseScale);
            
            if (sample > 0.18f && sample < 0.4f)
            {
                if (Random.value < treeSpawnDensity)
                {
                    Vector3 worldPos = targetMeshFilter.transform.TransformPoint(vertices[i]);
                    worldPos.y += 0.1f;
                    GameObject tree = Instantiate(treePrefab, worldPos, Quaternion.Euler(0, Random.Range(0, 360f), 0),
                        treeParent.transform);
                }
            }
        }

        Debug.Log("Деревья сгенерированы.");
    }

    /// <summary>
    /// Удаляет все ранее сгенерированные деревья (по имени).
    /// </summary>
    public void ClearTrees()
    {
        GameObject treeParent = GameObject.Find("Tree Clusters");
        if (treeParent != null)
        {
        #if UNITY_EDITOR
            DestroyImmediate(treeParent);
        #else
            Destroy(treeParent);
        #endif
            Debug.Log("Удалены все ранее сгенерированные деревья.");
        }
    }
    
    /// <summary>
    /// Удаляет все ранее сгенерированные плейны и водные плоскости (по имени).
    /// </summary>
    private void ClearPlanesAndWater()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Procedural Plane" || obj.name == "Water Plane")
            {
            #if UNITY_EDITOR
                DestroyImmediate(obj);
            #else
                Destroy(obj);
            #endif
            }
        }
        Debug.Log("Удалены все ранее сгенерированные плейны и водная плоскость.");
    }
    
    /// <summary>
    /// Удаляет все ранее сгенерированные деревья (объект с именем "Tree Clusters").
    /// </summary>
    public void ClearWater()
    {
        GameObject waterParent = GameObject.Find("Water Plane");
        if (waterParent != null)
        {
        #if UNITY_EDITOR
            DestroyImmediate(waterParent);
        #else
            Destroy(treeParent);
        #endif
            Debug.Log("Удалена вода.");
        }
    }
}