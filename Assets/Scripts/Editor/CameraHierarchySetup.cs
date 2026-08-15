#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CameraHierarchySetup
{
    [MenuItem("Tools/Setup Camera Bobbing Hierarchy")]
    public static void SetupHierarchy()
    {
        FPSController fpsController = Object.FindFirstObjectByType<FPSController>();
        if (fpsController == null)
        {
            Debug.LogError("No FPSController found in active scene!");
            return;
        }

        GameObject playerObj = fpsController.gameObject;
        Undo.RegisterFullObjectHierarchyUndo(playerObj, "Setup Camera Bobbing Hierarchy");

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = Object.FindFirstObjectByType<Camera>();
        }

        if (mainCamera == null)
        {
            Debug.LogError("No Camera found in active scene!");
            return;
        }

        GameObject mainCameraObj = mainCamera.gameObject;

        // 1. Check or create CameraPitchPivot
        Transform pitchPivot = playerObj.transform.Find("CameraPitchPivot");
        if (pitchPivot == null)
        {
            GameObject pitchObj = new GameObject("CameraPitchPivot");
            pitchPivot = pitchObj.transform;
            pitchPivot.SetParent(playerObj.transform, false);
            pitchPivot.localPosition = new Vector3(0f, 0.7f, 0f);
            pitchPivot.localRotation = Quaternion.identity;
            pitchPivot.localScale = Vector3.one;
            Undo.RegisterCreatedObjectUndo(pitchObj, "Create CameraPitchPivot");
        }

        // 2. Check or create CameraMotionPivot
        Transform motionPivot = pitchPivot.Find("CameraMotionPivot");
        if (motionPivot == null)
        {
            GameObject motionObj = new GameObject("CameraMotionPivot");
            motionPivot = motionObj.transform;
            motionPivot.SetParent(pitchPivot, false);
            motionPivot.localPosition = Vector3.zero;
            motionPivot.localRotation = Quaternion.identity;
            motionPivot.localScale = Vector3.one;
            Undo.RegisterCreatedObjectUndo(motionObj, "Create CameraMotionPivot");
        }

        // 3. Parent Main Camera under CameraMotionPivot
        mainCameraObj.transform.SetParent(motionPivot, false);
        mainCameraObj.transform.localPosition = Vector3.zero;
        mainCameraObj.transform.localRotation = Quaternion.identity;
        mainCameraObj.transform.localScale = Vector3.one;

        // 4. Setup CameraBobbingController component
        CameraBobbingController bobbing = motionPivot.GetComponent<CameraBobbingController>();
        if (bobbing == null)
        {
            bobbing = motionPivot.gameObject.AddComponent<CameraBobbingController>();
        }

        // Serialized fields assignment via SerializedObject to persist in scene
        SerializedObject serializedBobbing = new SerializedObject(bobbing);
        SerializedProperty fpsProp = serializedBobbing.FindProperty("_fpsController");
        SerializedProperty motionProp = serializedBobbing.FindProperty("_motionTransform");

        if (fpsProp != null)
        {
            fpsProp.objectReferenceValue = fpsController;
        }

        if (motionProp != null)
        {
            motionProp.objectReferenceValue = motionPivot;
        }

        serializedBobbing.ApplyModifiedProperties();

        // 5. Update FPSController cameraPivot reference to pitchPivot
        SerializedObject serializedFps = new SerializedObject(fpsController);
        SerializedProperty camPivotProp = serializedFps.FindProperty("cameraPivot");
        if (camPivotProp != null)
        {
            camPivotProp.objectReferenceValue = pitchPivot;
        }
        serializedFps.ApplyModifiedProperties();

        // 6. Setup PlayerRecoilController on Main Camera
        PlayerRecoilController recoil = mainCameraObj.GetComponent<PlayerRecoilController>();
        if (recoil == null)
        {
            recoil = mainCameraObj.AddComponent<PlayerRecoilController>();
        }

        SerializedObject serializedRecoil = new SerializedObject(recoil);
        SerializedProperty recoilTransformProp = serializedRecoil.FindProperty("_recoilTransform");
        if (recoilTransformProp != null)
        {
            recoilTransformProp.objectReferenceValue = mainCameraObj.transform;
        }
        serializedRecoil.ApplyModifiedProperties();

        // 7. Update PlayerWeaponManager camera and recoil references if present
        PlayerWeaponManager weaponManager = playerObj.GetComponent<PlayerWeaponManager>();
        if (weaponManager != null)
        {
            SerializedObject serializedWeaponManager = new SerializedObject(weaponManager);
            SerializedProperty camProp = serializedWeaponManager.FindProperty("playerCamera");
            if (camProp != null)
            {
                camProp.objectReferenceValue = mainCamera;
            }

            SerializedProperty recoilProp = serializedWeaponManager.FindProperty("_recoilController");
            if (recoilProp != null)
            {
                recoilProp.objectReferenceValue = recoil;
            }

            serializedWeaponManager.ApplyModifiedProperties();
        }

        EditorSceneManager.MarkSceneDirty(playerObj.scene);
        Debug.Log("<color=green>Successfully configured Camera Bobbing & Recoil Hierarchy!</color> Hierarchy: Player -> CameraPitchPivot -> CameraMotionPivot -> Main Camera.");
    }
}
#endif
