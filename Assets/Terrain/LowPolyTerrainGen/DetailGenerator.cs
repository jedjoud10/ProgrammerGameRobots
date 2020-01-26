using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailGenerator : MonoBehaviour
{
    private SphereCollider spcollider;
    [Tooltip("The terrain collider you want the detail to spawn on")]
    public MeshCollider terraincollider;
    [Tooltip("The details")]
    public Detail[] details;
    private RaycastHit hit;
    [System.Serializable]
    public struct Detail
    {
        [Tooltip("Max number of attempts to spawn this detail")]
        public int num;
        [Tooltip("Densirty of the detail with perlin noise, the lower the number the more detail will spawn (Number between 0 - 1)")]
        public float density;
        [Tooltip("The frequency of the perlin noise")]
        public float scale;
        [Tooltip("The world scale of the detail when spawned (Picked randomly between those numbers)")]
        public Vector2 randomrange;
        [Tooltip("The range of the terrain normal slope, if lower or higher than those two numbers then do not spawn")]
        public Vector2 normalrange;
        [Tooltip("The detail model or gameobject")]
        public GameObject foliage;
        [Tooltip("Use a randomly picked rotation ?")]
        public bool randomrotation;
    }
    // Start is called before the first frame update
    void Start()
    {
        Invoke("generate", 1);
    }
    public void generate()
    {
        spcollider = GetComponent<SphereCollider>();
        foreach (var detail in details)
        {
            for (int i = 0; i < detail.num; i++)
            {
                Vector2 pos = new Vector2(Random.Range(spcollider.bounds.min.x, spcollider.bounds.max.x), Random.Range(spcollider.bounds.min.z, spcollider.bounds.max.z));
                if (Mathf.PerlinNoise(pos.x * detail.scale, pos.y * detail.scale) > detail.density)
                {
                    Vector3 pos3 = new Vector3(pos.x, transform.position.y, pos.y);
                    //Debug.DrawLine(pos3, pos3 - Vector3.down * 99999999, Color.red, 999999999);
                    if (Physics.Raycast(pos3, Vector3.down * 9999999999, out hit))
                    {
                        if (Vector3.Dot(hit.normal, Vector3.up) > detail.normalrange.x && Vector3.Dot(hit.normal, Vector3.up) < detail.normalrange.y)
                        {
                            if (hit.collider.gameObject == terraincollider.gameObject)
                            {
                                Quaternion rot = Quaternion.Euler(Quaternion.LookRotation(Vector3.forward, hit.normal).eulerAngles);
                                if (detail.randomrotation)
                                {
                                    rot = Random.rotation;
                                }
                                GameObject objectvar = Instantiate(detail.foliage, hit.point, rot);
                                objectvar.transform.localScale *= Random.Range(detail.randomrange.x, detail.randomrange.y);
                                //Debug.Log(hit.collider);
                                objectvar.transform.SetParent(gameObject.transform);
                                //objectvar.transform.localScale *= Vector3.Dot(hit.normal, Vector3.up);                                
                            }
                        }
                        /*GameObject treedebu = Instantiate(detail.foliage, hit.point, Quaternion.Euler(Quaternion.LookRotation(Vector3.forward, hit.normal).eulerAngles));
                        treedebu.transform.localScale *= Vector3.Dot(hit.normal, Vector3.up) / dotdiv;
                        Debug.Log(Vector3.Dot(hit.normal, Vector3.up) / dotdiv);
                        */
                    }
                }
            }
        }
    }

}
