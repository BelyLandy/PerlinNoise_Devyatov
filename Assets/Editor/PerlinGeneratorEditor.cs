using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinGenerator))]
public class PerlinGeneratorEditor : Editor
{
    SerializedProperty generatedTexture;
    SerializedProperty perlinTexSizeX;
    SerializedProperty perlinTexSizeY;
    SerializedProperty noiseScale;
    SerializedProperty noiseColorGradient;
    SerializedProperty randomizeNoiseOffset;
    SerializedProperty perlinOffset;

    SerializedProperty heightMultiplier;
    //SerializedProperty updateMeshMaterial;
    SerializedProperty targetMeshFilter;
    SerializedProperty terrainShader;

    SerializedProperty exportPath;
    SerializedProperty exportFileName;

    SerializedProperty prefabExportPath;
    SerializedProperty includeWaterInPrefab;
    SerializedProperty includeTreesInPrefab;
    
    SerializedProperty addWater;
    SerializedProperty waterLevel;
    SerializedProperty waterShader;
    
    SerializedProperty treePrefab;
    SerializedProperty treeSpawnDensity;
    
    SerializedProperty currentPlane;
    SerializedProperty currentWaterPlane;
    SerializedProperty currentTrees;

    protected void OnEnable()
    {
        generatedTexture = serializedObject.FindProperty("generatedTexture");
        perlinTexSizeX = serializedObject.FindProperty("perlinTexSizeX");
        perlinTexSizeY = serializedObject.FindProperty("perlinTexSizeY");
        noiseScale = serializedObject.FindProperty("noiseScale");
        noiseColorGradient = serializedObject.FindProperty("noiseColorGradient");
        randomizeNoiseOffset = serializedObject.FindProperty("randomizeNoiseOffset");
        perlinOffset = serializedObject.FindProperty("perlinOffset");

        heightMultiplier = serializedObject.FindProperty("heightMultiplier");
        //updateMeshMaterial = serializedObject.FindProperty("updateMeshMaterial");
        targetMeshFilter = serializedObject.FindProperty("targetMeshFilter");
        terrainShader = serializedObject.FindProperty("terrainShader");

        exportPath = serializedObject.FindProperty("exportPath");
        exportFileName = serializedObject.FindProperty("exportFileName");

        prefabExportPath = serializedObject.FindProperty("prefabExportPath");
        includeWaterInPrefab = serializedObject.FindProperty("includeWaterInPrefab");
        includeTreesInPrefab = serializedObject.FindProperty("includeTreesInPrefab");

        addWater = serializedObject.FindProperty("addWater");
        waterLevel = serializedObject.FindProperty("waterLevel");
        waterShader = serializedObject.FindProperty("waterShader");
        
        treePrefab = serializedObject.FindProperty("treePrefab");
        treeSpawnDensity = serializedObject.FindProperty("treeSpawnDensity");

        currentPlane = serializedObject.FindProperty("currentPlane");
        currentWaterPlane = serializedObject.FindProperty("currentWaterPlane");
        currentTrees = serializedObject.FindProperty("currentTrees");
    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Режим осмотра ланшафта!", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
            
            //Debug.LogWarning("Функции создания и редактирования мэшей недоступны в Playmode!");
            return;
        }
        
        serializedObject.Update();

        // Блок "Настройки шума Перлина".
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Генерация ландшафта с шумом Перлина.", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(generatedTexture, new GUIContent("Текстура шума"));
        EditorGUILayout.PropertyField(perlinTexSizeX);
        EditorGUILayout.PropertyField(perlinTexSizeY);
        EditorGUILayout.PropertyField(noiseScale);
        EditorGUILayout.PropertyField(randomizeNoiseOffset);
        EditorGUILayout.PropertyField(perlinOffset);
        EditorGUILayout.PropertyField(noiseColorGradient);
        if (GUILayout.Button("Сгенерировать шум"))
        {
            ((PerlinGenerator)target).GenerateNoise();
        }
        if (GUILayout.Button("Сохранить параметры шума"))
        {
            ((PerlinGenerator)target).SaveNoiseParameters();
        }
        if (GUILayout.Button("Загрузить параметры шума"))
        {
            ((PerlinGenerator)target).LoadNoiseParameters();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Блок "Экспорт/Сохранение текстуры шума"
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Экспорт/Сохранение текстуры шума.", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(exportPath, new GUIContent("Путь сохранения текстуры"));
        EditorGUILayout.PropertyField(exportFileName);
        if (GUILayout.Button("Экспортировать текстуру шума"))
        {
            ((PerlinGenerator)target).ExportNoiseTexture();
        }
        if (GUILayout.Button("Загрузить текстуру"))
        {
            ((PerlinGenerator)target).LoadNoiseTexture();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Блок "Настройки меша, генерация плейна и шейдера"
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Настройки меша, генерация плейна и шейдера.", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(heightMultiplier, new GUIContent("Множитель высоты"));
        //EditorGUILayout.PropertyField(updateMeshMaterial);
        EditorGUILayout.PropertyField(addWater, new GUIContent("Генерировать ли воду?"));
        EditorGUILayout.PropertyField(targetMeshFilter);
        EditorGUILayout.PropertyField(terrainShader, new GUIContent("Шейдер плейна"));
        EditorGUILayout.PropertyField(currentPlane, new GUIContent("Объект плейн"));
        if (GUILayout.Button("Сгенерировать новый плейн"))
        {
            ((PerlinGenerator)target).GeneratePlane();
        }
        if (GUILayout.Button("Применить шум к мешу"))
        {
            ((PerlinGenerator)target).ApplyNoiseToMesh();
        }
        if (GUILayout.Button("Применить шейдер к мешу"))
        {
            PerlinGenerator pg = (PerlinGenerator)target;
            if (pg.targetMeshFilter != null)
            {
                MeshRenderer mr = pg.targetMeshFilter.GetComponent<MeshRenderer>();
                if (mr != null && pg.terrainShader != null)
                {
                    mr.sharedMaterial.shader = pg.terrainShader;
                    Debug.Log("Применён шейдер: " + pg.terrainShader.name);
                }
                else
                {
                    Debug.LogWarning("Не найден MeshRenderer или шейдер не задан!");
                }
            }
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();

        // Блок "Сохранение префаба."
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Сохранение префаба. (Не забудьте экспортировать текстуру шума!)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(prefabExportPath, new GUIContent("Путь сохранения префаба"));
        EditorGUILayout.PropertyField(includeWaterInPrefab, new GUIContent("Сохранить воду?"));
        EditorGUILayout.PropertyField(includeTreesInPrefab, new GUIContent("Сохранить деревья?"));
        if (GUILayout.Button("Сохранить мэш как Prefab"))
        {
            ((PerlinGenerator)target).SaveMeshAsPrefab();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        
        // Блок "Настройки воды."
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Настройки воды.", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(waterLevel, new GUIContent("Уровень воды (в единицах мира)"));
        EditorGUILayout.PropertyField(waterShader, new GUIContent("Шейдер воды"));
        EditorGUILayout.PropertyField(currentWaterPlane, new GUIContent("Объект воды"));
        if (GUILayout.Button("Добавить воду"))
        {
            ((PerlinGenerator)target).AddWater();
        }
        if (GUILayout.Button("Удалить воду"))
        {
            ((PerlinGenerator)target).ClearWater();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        
        // Блок "Генерация деревьев."
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Генерация деревьев.", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(treePrefab, new GUIContent("Префаб дерева"));
        EditorGUILayout.PropertyField(treeSpawnDensity, new GUIContent("Плотность деревьев"));
        EditorGUILayout.PropertyField(currentTrees, new GUIContent("Объект деревьев"));
        if (GUILayout.Button("Сгенерировать деревья"))
        {
            ((PerlinGenerator)target).GenerateTrees();
        }
        if (GUILayout.Button("Удалить все деревья"))
        {
            ((PerlinGenerator)target).ClearTrees();
        }
        EditorGUILayout.EndVertical();
        
        GUILayout.Box("Made By Devyatov Denis 238.");

        serializedObject.ApplyModifiedProperties();
    }
}
