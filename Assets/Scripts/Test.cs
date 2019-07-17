using UnityEngine;
using Tangzx.ABSystem;

public class Test : MonoBehaviour
{
    AssetBundleManager manager;

    void Start()
    {
        manager = gameObject.AddComponent<AssetBundleManager>();
        manager.Init(() =>
        {
            LoadObjects();
        });
    }

    void LoadObjects()
    {
        Debug.Log(Time.realtimeSinceStartup + " "+Time.frameCount);
        manager.Load("Assets/Prefabs/Sphere.prefab", (a) =>
        {
            a.Instantiate("Sphere.prefab");
            Debug.Log(Time.realtimeSinceStartup + " "+Time.frameCount);
//            manager.Load("role.spriteatlas", (a2) =>
//            {
//                Debug.Log("sxxx"+a2);
////                GameObject go2 = a2.Instantiate();
//            });

        });
//        manager.Load("Assets.Prefabs.Cube.prefab", (a) =>
//        {
//            GameObject go = a.Instantiate();
//            go.transform.localPosition = new Vector3(6, 3, 3);
//        });
        //manager.Load("Assets.Prefabs.Plane.prefab.ab", (a) =>
        //{
        //    GameObject go = a.Instantiate();
        //    go.transform.localPosition = new Vector3(9, 3, 3);
        //});
        //manager.Load("Assets.Prefabs.Capsule.prefab.ab", (a) =>
        //{
        //    GameObject go = a.Instantiate();
        //    go.transform.localPosition = new Vector3(12, 3, 3);
        //});
    }
}