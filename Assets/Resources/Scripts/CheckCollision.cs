using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckCollision : MonoBehaviour
{

    public bool SpawnAgain = false;//再次生成
    public GameObject child;//定义子对象
    public string base64;//base64数据
    public int _device;//设备信息

    private int randomTime = 0;//随机次数
    private int num;//碰撞体个数
    private Collider2D[] results;//碰撞体数组
    private List<bool> isFinish = new List<bool>();//是否可以生成
    // Start is called before the first frame update

    void Start()
    {
        SpawnAgain = true;

        child = transform.GetChild(0).gameObject;
        child.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);


    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnAgain)
        {
            StartCoroutine(RandomPos());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator RandomPos()
    {
        SpawnAgain = false;
        results = new Collider2D[10];
        //将所有碰撞到的物体存入数组
        num = GetComponent<Collider2D>().OverlapCollider((new ContactFilter2D()).NoFilter(), results);

        //Debug.LogFormat("num = {0}", num);
        //满足条件再次随机
        while ((num != 0 || isFinish.Contains(false)) && randomTime < 1)
        {
            //判断碰撞体存在时间
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i])
                {
                    //Debug.Log(111);
                    //Debug.Log(results[i].gameObject.name);
                    if (results[i].gameObject.GetComponent<TimeCount>().isTimeUp == false)
                    {
                        isFinish.Add(false); //Debug.Log(222);

                    }
                    else if (results[i].gameObject.GetComponent<TimeCount>().isTimeUp == true)
                    {
                        isFinish.Add(true); //Debug.Log(333);
                    }
                }
            }

            // 再次随机
            if (isFinish.Contains(false) || num != 0)
            {

                // float x = Random.Range(-6.8f, 6.8f);
                // float y = Random.Range(-5.4f, 5.4f);
                // Vector3 RandomPos = new Vector3(x, y, -1);
                // //Debug.Log(RandomPos);
                Vector2 new_pos = GameObject.Find("StartPoint").GetComponent<ConnectSocket>().ScanScreen();
                if (new_pos.x == 0 && new_pos.y == 0)
                {
                    float w = GameObject.Find("StartPoint").GetComponent<ConnectSocket>().w;
                    float h = GameObject.Find("StartPoint").GetComponent<ConnectSocket>().h;
                    new_pos.x = Random.Range(w + 0.675f, -w - 0.675f);
                    new_pos.y = Random.Range(h + 0.675f, -h - 0.675f);
                }
                gameObject.transform.position = new_pos;
                yield return new WaitForSeconds(0.1f);
                // Debug.Log(RandomPos);
                randomTime++;
            }

            isFinish.Clear();
            results = new Collider2D[10];
            num = GetComponent<Collider2D>().OverlapCollider((new ContactFilter2D()).NoFilter(), results);
            yield return new WaitForSeconds(0.1f);
        }

        //重复次数超出后
        if (randomTime >= 1)
        {
            //判断时间
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i])
                {
                    //Debug.Log(results[i].gameObject.name);
                    if (results[i].gameObject.GetComponent<TimeCount>().isTimeUp == false)
                    {
                        isFinish.Add(false);

                    }
                    else if (results[i].gameObject.GetComponent<TimeCount>().isTimeUp == true)
                    {
                        isFinish.Add(true);
                    }

                }

            }

            //根据时间判断
            if (isFinish.Contains(false))
            {
                //根据标签区分来自服务器或者本地
                if (gameObject.tag == "Father")
                {
                    ConnectSocket.ImgData img = new ConnectSocket.ImgData();
                    img.img_url = base64;
                    //Debug.Log(base64);
                    img.device = _device;
                    GameObject.Find("StartPoint").GetComponent<ConnectSocket>().imgs.Add(img);
                    GameObject.Find("StartPoint").GetComponent<ConnectSocket>().isReCreate = true;
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }

            }
            else
            {
                //淡出动画
                GameObject.Find("StartPoint").GetComponent<ConnectSocket>().isReCreate = false;
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i])
                    {
                        Animation myAnim2 = results[i].gameObject.transform.GetChild(0).gameObject.GetComponent<Animation>();
                        myAnim2.Play("FadeOut");
                        yield return new WaitForSeconds(0.4f);
                        Destroy(results[i].gameObject);

                    }
                }
            }

        }

        //出现动画
        child = transform.GetChild(0).gameObject;
        child.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);

        Animation myAnim = child.GetComponent<Animation>();
        myAnim.Play("rotate");

        //销毁碰撞体和刚体避免干扰
        Destroy(GetComponent<CheckCollision>());
        Destroy(GetComponent<Rigidbody2D>());

    }


}
