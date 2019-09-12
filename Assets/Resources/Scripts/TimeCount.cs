using UnityEngine;

public class TimeCount : MonoBehaviour
{
    public float StartTime;//生成开始计时
    public float CurTime;//当前时间
    public bool isTimeUp = false;//时间是否到达

    //public float timeUse = 15f;
    // Start is called before the first frame update

    void Start()
    {
        StartTime = Time.time;
        isTimeUp = false;

    }

    // Update is called once per frame
    void Update()
    {
        //判断没个物体的存在时间
        CurTime = Time.time;
        if (gameObject.name == "LOGO")
        {
            //logo永不消失
            isTimeUp = false;
            this.enabled = false;
        }
        else
        {
            if (CurTime - StartTime >= 20)
            {
                isTimeUp = true;
                //Debug.Log(CurTime - StartTime);
                this.enabled = false;
            }
        }

    }


}
