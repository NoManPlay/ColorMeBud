using LitJson;
using WebSocketSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConnectSocket : MonoBehaviour
{
    public struct ImgData
    {
        public string img_url;
        public int device;
    }//创建构造体ImgData存放服务器信息
    public List<ImgData> imgs = new List<ImgData>();//创建构造体数组
    public bool isReCreate = false;//判断物体是否返回队列
    public Sprite[] LocalImgs;//本地图片数组
    public float w;//屏幕width
    public float h;//屏幕height

    private bool isNew = false;//是否为新生成状态
    private List<CircleCollider2D> cc2ds = new List<CircleCollider2D>();//创建碰撞体数组
    private float sendTime;//服务器onmessage时间
    private float nowTime;//当前时间

    void Start()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        w = pos.x;
        h = pos.y;
        Debug.Log(pos.x + " " + pos.y);
        sendTime = Time.time;
        //Debug.Log(sendTime);
        Invoke("CreateAtLocal", 1f);
        Invoke("CreateAtLocal", 2f);
        Invoke("CreateAtLocal", 3f);
        Invoke("CreateAtLocal", 4f);
        Invoke("CreateAtLocal", 5f);
        Invoke("CreateAtLocal", 6f);
        Invoke("CreateAtLocal", 7f);
        Invoke("CreateAtLocal", 8f);
        Invoke("CreateAtLocal", 9f);
        Invoke("CreateAtLocal", 10f);
        WebSocketConnect();
        StartCoroutine(ImgCreateCoroutine());

    }

    void Update()
    {

        if (isNew)
        {
            sendTime = Time.time;
            //Debug.Log(sendTime);
            isNew = false;
        }

        nowTime = Time.time;
        float waitTime = nowTime - sendTime;

        if (waitTime > 30)
        {
            Invoke("CreateAtLocal", 1f);
            Invoke("CreateAtLocal", 2f);
            Invoke("CreateAtLocal", 3f);
            Invoke("CreateAtLocal", 4f);
            Invoke("CreateAtLocal", 5f);
            Invoke("CreateAtLocal", 6f);
            Invoke("CreateAtLocal", 7f);
            Invoke("CreateAtLocal", 8f);
            Invoke("CreateAtLocal", 9f);
            Invoke("CreateAtLocal", 10f);

            sendTime = Time.time;
        }

    }

    /// <summary>
    /// 处理生成流程
    /// </summary>
    /// <returns></returns>
    private IEnumerator ImgCreateCoroutine()
    {

        while (true)
        {
            //执行新生成函数
            CreateNewOne();
            //判断是否为返回队列状态
            if (isReCreate)
            {
                yield return new WaitForSeconds(0.1f);
                isReCreate = false;
            }
            else
            {
                yield return new WaitForSeconds(1.2f);
            }

        }

    }

    /// <summary>
    /// 连接websocket服务器
    /// 使用websocket-sharp插件
    /// </summary>
    void WebSocketConnect()
    {

        using (var ws = new WebSocket("ws://47.101.163.69:8000/ws/fetch/"))
        {

            ws.OnOpen += (sender, e) =>
            {
                Debug.Log("open");
            };

            ws.OnMessage += (sender, e) =>
            {
                //Debug.Log("go");
                string a = e.Data;
                //外部处理data
                DoSomething(a);
            };

            ws.OnClose += (sender, e) =>
            {
                Debug.Log("close" + e.Code + " " + e.Reason);
                if (e.Code == 1006)
                {
                    Invoke("WebSocketConnect", 5f);
                }
            };

            ws.OnError += (sender, e) =>
            {
                // Debug.Log("error");
                Invoke("WebSocketConnect", 5f);
            };

            ws.ConnectAsync();

        }
    }

    /// <summary>
    /// 提取json转换为对象
    /// 将对象加入待生成的list排队
    /// </summary>
    /// <param name="a"></param>
    public void DoSomething(string a)
    {

        JsonData jd = JsonMapper.ToObject(a);
        ImgData img = new ImgData();
        img.img_url = (string)jd["message"]["payload"]["image"];
        img.device = (int)jd["message"]["payload"]["device"];
        imgs.Add(img);
        //确认需要刷新图片
        isNew = true;
        //Debug.Log(isNew);

    }

    /// <summary>
    /// 队列为空不执行
    /// 实例化prefab，随机位置生成
    /// </summary>
    private void CreateNewOne()
    {

        if (imgs.Count == 0) return;

        GameObject Father400 = (GameObject)Instantiate(Resources.Load("Prefabs/Father400"));
        float x = Random.Range(w + 0.675f, -w - 0.675f);
        float y = Random.Range(h + 0.675f, -h - 0.675f);
        Vector3 RandomPos = new Vector3(x, y, -1);
        Father400.transform.position = RandomPos;
        // Vector2 pos = Father400.transform.position = ScanScreen();
        // Debug.Log(pos);

        //将base64实例化为texture2d
        Base64ToTexture2D(Father400);

    }

    /// <summary>
    /// 队列为空不执行
    /// 将base64转化为sprite
    /// </summary>
    /// <param name="obj"></param>
    private void Base64ToTexture2D(GameObject obj)
    {

        if (imgs.Count == 0) return;
        //将list中第一项实例化并移出队列
        ImgData img = imgs[0];
        imgs.RemoveAt(0);
        string img_url = img.img_url;
        int device = img.device;
        obj.GetComponent<CheckCollision>().base64 = img_url;
        obj.GetComponent<CheckCollision>()._device = device;
        GameObject child = obj.transform.GetChild(0).gameObject;
        Texture2D pic = new Texture2D(1, 1);
        byte[] data = Convert.FromBase64String(img_url);
        pic.LoadImage(data);
        pic.Apply();
        //转换为sprite，赋给负责碰撞体的父级和负责动画的子级
        Sprite temp = Sprite.Create(pic, new Rect(0, 0, 900, 900), new Vector2(0.5f, 0.5f));
        obj.GetComponent<SpriteRenderer>().sprite = temp;
        child.GetComponent<SpriteRenderer>().sprite = temp;
        //为父级添加碰撞体circleCollider，设定isTrigger属性
        CircleCollider2D c2d = obj.AddComponent<CircleCollider2D>();
        c2d.isTrigger = true;
        //判断device属性决定缩放比例和碰撞体积
        if (device == 1)
        {
            c2d.radius *= 0.9f;
            //Debug.Log(1);
            obj.transform.localScale = new Vector3(0.24f, 0.24f, 1f);
        }
        else if (device == 2)
        {
            c2d.radius *= 0.6f;
            // Debug.Log(2);
            obj.transform.localScale = new Vector3(0.15f, 0.15f, 1f);
        }

    }

    /// <summary>
    /// 根据缩放比例再次调整碰撞体积
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool ContainsCollider(float x, float y)
    {

        for (int i = 0; i < cc2ds.Count; i++)
        {
            float radius;
            float temp_radius;
            if (cc2ds[i].transform.localScale.x < 0.2f)
            {
                radius = cc2ds[i].radius * 0.15f;
                temp_radius = 0.75f;
            }
            else
            {
                radius = cc2ds[i].radius * 0.24f;
                temp_radius = 1.2f;
            }

            //Debug.Log("radius:" + radius);
            Vector2 origin = cc2ds[i].transform.position;
            //Debug.Log("origin:" + origin);
            float distence = (origin - new Vector2(x, y)).magnitude;
            //Debug.Log("distence:" + distence);
            if (distence < radius + temp_radius)
            {
                return true;
            }
        }
        return false;

    }

    /// <summary>
    /// 随机位置已存在物体时扫描屏幕空位
    /// </summary>
    /// <returns></returns>
    public Vector2 ScanScreen()
    {

        cc2ds.Clear();
        cc2ds = new List<CircleCollider2D>(GameObject.FindObjectsOfType<CircleCollider2D>());

        for (float x = w + 0.675f; x <= -w - 0.675f; x += 0.5f)
        {
            for (float y = h + 0.675f; y <= -h - 0.675f; y += 0.5f)
            {
                if (!ContainsCollider(x, y))
                {
                    //ct.transform.position = new Vector2(0, 0);
                    return new Vector2(x, y);
                }
            }
        }
        //ct.transform.position = new Vector2(0, 0);
        return new Vector2(0, 0);

    }

    /// <summary>
    /// 长时间无人操作时本地提取图片展示
    /// </summary>
    private void CreateAtLocal()
    {

        int index = Random.Range(0, LocalImgs.Length);
        int device = Random.Range(1, 3);
        // Debug.Log(index);

        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/Father400"));
        float x = Random.Range(w + 0.675f, -w - 0.675f);
        float y = Random.Range(h + 0.675f, -h - 0.675f);
        Vector3 RandomPos = new Vector3(x, y, -1);
        obj.transform.position = RandomPos;
        obj.tag = "Local";
        GameObject child = obj.transform.GetChild(0).gameObject;

        Sprite localImg = LocalImgs[index];

        obj.GetComponent<SpriteRenderer>().sprite = localImg;
        child.GetComponent<SpriteRenderer>().sprite = localImg;

        CircleCollider2D c2d = obj.AddComponent<CircleCollider2D>();
        c2d.isTrigger = true;
        if (device == 1)
        {
            c2d.radius *= 0.9f;
            //Debug.Log(1);
            obj.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
        }
        else if (device == 2)
        {
            c2d.radius *= 0.6f;
            // Debug.Log(2);
            obj.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        }
    }

}
