using System.Collections.Generic;
using UnityEngine;
using System;

public enum PatternType {SHELL=1, LOXODROME, SQUID, RING, GRID}; 

public class Spawn : MonoBehaviour {

    [Range(1, 2)]
    public int Shape = 1; //1 Sphere, 2 Cube
    public GameObject SpawnSphere; 
    public GameObject SpawnCube;  
    [Range(1, 10)]
    public int Branches = 6;
    [Range(2, 20)]
    public int Leaves = 15;

    private List<List<FormGameObject>> branches = new List<List<FormGameObject>>(); 
    private bool initMe = true; 
    private PatternType currentPattern;
    private TimerUtils timerUtils = new TimerUtils(27.4285715f); //change scene every 64 beats at 140 BPM

    void Start() {
        initMe = true;
    }

    void Update() {
        if (initMe) {
            InitScene();
            initMe = false;
        } 
        if (timerUtils.ReadyToUpdate()) {
            UpdateScene();
        }
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
        GameObject spawnObject = (Shape == 1) ? SpawnSphere : SpawnCube;
        for (int branch = 0; branch < Branches; branch++) {
            List<FormGameObject> leaves = new List<FormGameObject>();
            for (int leaf = 0; leaf < Leaves; leaf++) {
                GameObject instance = Instantiate(spawnObject, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                FormGameObject FGO = instance.GetComponent<FormGameObject>();
                FGO.InitFormObject(branch, leaf);
                leaves.Add(FGO);
            }
            branches.Add(leaves);
        }
    }

    private void UpdateScene() {
        //NextPatternType();
        currentPattern = PatternType.GRID;
        for (int branch = 0; branch < Branches; branch++) {
            List<FormGameObject> leaves = branches[branch];
            for (int leaf = 0; leaf < Leaves; leaf++) {
                leaves[leaf].UpdatePattern(currentPattern, TargetPosition(branch, leaf), TargetScale(branch, leaf));
            }
        }
    }

    private Vector3 TargetPosition(int branch, int leaf) {
        Vector3 formPosition = Vector3.zero;
        switch (currentPattern) {
            case PatternType.SHELL: {
                formPosition = ShellPosition((float) branch, (float) leaf);
                break;
            }
            case PatternType.LOXODROME: {
                formPosition = LoxodromePosition((float) branch, (float) leaf);
                break;    
            }
            case PatternType.SQUID: {
                formPosition = SquidPosition((float) branch, (float) leaf);
                break;
            }
            case PatternType.RING: {
                formPosition = RingPosition((float) branch, (float) leaf);
                break;
            }
            case PatternType.GRID: {
                formPosition = GridPosition((float) branch, (float) leaf);
                break;
            }
            default: {
                break;
            }
        }
        return formPosition;
    }

    private Vector3 ShellPosition(float fbranch, float fleaf) {
        float xPos = 2.0f + fbranch*1.5f;
        return Quaternion.Euler(0, fleaf/Leaves*360.0f, 0) * new Vector3(xPos, 0.0f, 0.0f); //create ring
    }

    private Vector3 LoxodromePosition(float fbranch, float fleaf) {
        Vector3 pos = new Vector3(0f, 6f, 0f);
        pos = Quaternion.Euler(fleaf/Branches*90.0f, 0, 0) * pos;
        return Quaternion.Euler(0, fbranch/Branches*360.0f, 0) * pos;
    }

    private Vector3 SquidPosition(float fbranch, float fleaf) {
        float a = fbranch / (float) Branches;
        return Quaternion.Euler(0, 0, a*360.0f) * new Vector3(0.0f, fleaf*0.5f, 0.0f);
    }

    private Vector3 RingPosition(float fbranch, float fleaf) {
        float a = (fbranch*(float) Leaves + fleaf) / ((float) Branches * (float) Leaves);
        return Quaternion.Euler(0, 0, a*360.0f) * new Vector3(0.0f, 9.0f, 0.0f);
    }

    private Vector3 GridPosition(float fbranch, float fleaf) {
        return new Vector3(2.0f*(fbranch - (float) Branches*0.5f), 2.0f*(fleaf - (float) Leaves*0.5f), 0.0f);
    }

    private Vector3 TargetScale(float fbranch, float fleaf) {
        Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
        switch (currentPattern) {
            case PatternType.SHELL: {
                scale *= 1.0f - fbranch*0.1f;
                break;
            }
            case PatternType.LOXODROME: {
                break;
            }
            case PatternType.SQUID: {
                scale *= 1.0f - fleaf*0.05f;
                break;
            }
            case PatternType.RING: {
                scale *= 1.0f - fbranch*0.05f;
                break;
            }
            case PatternType.GRID: {
                break;
            }
            default: {
                break;
            }
        }
        return scale;
    }
}
