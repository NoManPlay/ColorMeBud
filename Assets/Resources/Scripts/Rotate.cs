using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 生成前范围内随机角度
        Vector3 RandomRo = new Vector3(0, 0, Random.Range(-20f, 20f));
        this.transform.Rotate(RandomRo);

    }

    // Update is called once per frame
    void Update()
    {

    }

}
