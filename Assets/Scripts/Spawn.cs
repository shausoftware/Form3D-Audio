using System.Collections.Generic;
using UnityEngine;
using System;

// Created by SHAU - 2019
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

public enum PatternType {SHELL, LOXODROME, SQUID, RING}; 

public class Spawn : MonoBehaviour {

    [Range(1, 2)]
    public int Shape = 1; //1 Cube, 2 Sphere
    [Range(1, 3)]
    public int Quality = 1; //1 High, 2 Low   
    public float BPM = 140;
    public GameObject SpawnCube;  
    public GameObject SpawnSphere; 
    
    private int Branches = 10;
    private int Leaves = 20;
    private List<List<FormGameObject>> branches = new List<List<FormGameObject>>(); 
    private bool initMe = true; 
    private PatternType currentPattern;
    private TimerUtils timerUtils;

    void Awake() {
        //Physics.autoSimulation = false;
        timerUtils = new TimerUtils(BPMUtils.GetDuration(BPM, 64)); //change scene every 64 beats at 140 BPM
        initMe = true;
        Keyboard.reset += Reset;
        Keyboard.changeShape += UpdateShape;
        Keyboard.changeQuality += UpdateQuality;
    }

    void Update() {
        if (initMe) {
            InitScene();
            initMe = false;
        } 
        if (timerUtils.ReadyToUpdate()) {
            UpdateScene(false);
        }
    }

    void OnDisable() {
		Keyboard.reset -= Reset;
        Keyboard.changeShape -= UpdateShape;
        Keyboard.changeQuality -= UpdateQuality;
	} 

    void Reset() {
        UpdateScene(true);
    }

    void UpdateShape(int shapeId) {
        Shape = shapeId;
        ClearScene();
        InitScene();
        UpdateScene(true);
    }

    void UpdateQuality(int qualityId) {
        ClearScene();
        switch (qualityId) {
            case 1: {
                Branches = 10;
                Leaves = 20;
                break;
            }
            case 2: {
                Branches = 6;
                Leaves = 15;
                break;
            }
            default: {
                break;
            }
        }
        InitScene();
        UpdateScene(true);
    }

    private void NextPatternType() {
        Array values = System.Enum.GetValues(typeof(PatternType));
        int currentIndex = (int) currentPattern;
        while (true) {
            int newIndex = UnityEngine.Random.Range(0, values.Length);
            if (newIndex != currentIndex) {
                currentPattern = (PatternType) values.GetValue(newIndex);
                break;
            }
        }
    }

    private void InitScene() {
        branches = new List<List<FormGameObject>>();
        GameObject spawnObject = (Shape == 1) ? SpawnCube : SpawnSphere;
        for (int branch = 0; branch < Branches; branch++) {
            List<FormGameObject> leaves = new List<FormGameObject>();
            for (int leaf = 0; leaf < Leaves; leaf++) {
                GameObject instance = Instantiate(spawnObject, new Vector3(0,0,0), Quaternion.identity);
                FormGameObject FGO = instance.GetComponent<FormGameObject>();
                FGO.InitFormObject(branch, leaf);
                leaves.Add(FGO);
            }
            branches.Add(leaves);
        }
    }

    private void UpdateScene(bool reset) {
        NextPatternType();
        //currentPattern = PatternType.RING;
        for (int branch = 0; branch < Branches; branch++) {
            List<FormGameObject> leaves = branches[branch];
            for (int leaf = 0; leaf < Leaves; leaf++) {
                if (reset) {
                    leaves[leaf].Reset();
                }
                leaves[leaf].UpdatePattern(currentPattern, TargetPosition(branch, leaf), TargetScale(branch, leaf));
            }
        }
    }

    private void ClearScene() {
        for (int branch = 0; branch < Branches; branch++) {
            List<FormGameObject> leaves = branches[branch];
            for (int leaf = 0; leaf < Leaves; leaf++) {
                GameObject.Destroy(leaves[leaf].gameObject);
            }
        }
        branches = new List<List<FormGameObject>>();   
    }

    private Vector3 TargetPosition(int branch, int leaf) {
        Vector3 formPosition = Vector3.zero;
        switch (currentPattern) {
            case PatternType.SHELL: {
                formPosition = ShellPosition(branch, leaf);
                break;
            }
            case PatternType.LOXODROME: {
                formPosition = LoxodromePosition(branch, leaf);
                break;    
            }
            case PatternType.SQUID: {
                formPosition = SquidPosition(branch, leaf);
                break;
            }
            case PatternType.RING: {
                formPosition = RingPosition(branch, leaf);
                break;
            }
            default: {
                break;
            }
        }
        return formPosition;
    }

    private Vector3 ShellPosition(float fbranch, float fleaf) {
        float xPos = 2 + fbranch*1.5f;
        return Quaternion.Euler(0, fleaf/Leaves*360, 0) * new Vector3(xPos, 0,0); //create ring
    }

    private Vector3 LoxodromePosition(float fbranch, float fleaf) {
        Vector3 pos = new Vector3(0,8,0);
        pos = Quaternion.Euler(fleaf/Branches*90, 0, 0) * pos;
        return Quaternion.Euler(0, fbranch/Branches*360, 0) * pos;
    }

    private Vector3 SquidPosition(float fbranch, float fleaf) {
        float a = fbranch / Branches;
        return Quaternion.Euler(0,0, a*360) * new Vector3(0, fleaf*0.5f, 0);
    }

    private Vector3 RingPosition(float fbranch, float fleaf) {
        float a = (fbranch*Leaves + fleaf) / Branches * Leaves;
        return Quaternion.Euler(0,0, a*360) * new Vector3(0,9,0);
    }

    private Vector3 TargetScale(float fbranch, float fleaf) {
        Vector3 scale = new Vector3(1,1,1);
        switch (currentPattern) {
            case PatternType.SHELL: {
                scale *= 1 - fbranch*0.08f;
                break;
            }
            case PatternType.SQUID: {
                scale *= 1 - fleaf*0.03f;
                break;
            }
            default: {
                break;
            }
        }
        return scale;
    }
}