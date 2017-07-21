using UnityEngine;
using System.Collections;

public class Joystick : MonoBehaviour
{
    public TouchDirType touchDirType;

    bool isPress = false;
    bool touchActionActive = false;
    float h, v;
    public float bigCircleRadius = 100;

    Transform bigCircleTrans;
    Transform smallCircleTrans;

    Vector2 bigCircleStartWorldPos = Vector2.zero;
    Vector2 smallCircleStartLocalPos = Vector2.zero;

    Vector2 startPressPos;      // 技能摇杆按下时的初始位置 即第一帧位置

    private Vector2 offset;     // 摇杆的偏移量  -1到1
    public Vector2 GetOffset()
    {
        return new Vector2(h, v);
    }

    void Start()
    {

#if UNITY_ANDROID || UNITY_IPHONE
        TouchManager.Instance.onTouchBegan += OnTouchBegan;
#endif

        bigCircleTrans = transform;
        smallCircleTrans = transform.GetChild(0);
        smallCircleStartLocalPos = smallCircleTrans.localPosition;
    }

    void Destroy()
    {

#if UNITY_ANDROID || UNITY_IPHONE
        TouchManager.Instance.onTouchBegan -= OnTouchBegan;
#endif
    }

    void Update()
    {
        if (isPress)
        {
            PressIsTrue();
        }

        switch (touchDirType)
        {
            case TouchDirType.Empty:
                break;
            case TouchDirType.Left:
                TouchManager.leftOffset = GetOffset().ToString();
                break;
            case TouchDirType.Right:
                TouchManager.rightOffset = GetOffset().ToString();
                break;
            default:
                break;
        }
    }

    public void OnPointDown()
    {
#if UNITY_EDITOR
        this.isPress = true;
        startPressPos = TouchManager.GetPosition(touchDirType);
#endif
        touchActionActive = true;
    }

    public void OnPointUp()
    {
        this.isPress = false;
        smallCircleTrans.localPosition = smallCircleStartLocalPos;

        touchActionActive = false;
        // 鼠标抬起时 将 h,v归零
        h = 0;
        v = 0;
    }

    #region 手机端使用
    public void OnTouchBegan(TouchDirType dirType)
    {
        if (dirType != touchDirType || touchActionActive == false)
            return;

        isPress = true;
        startPressPos = TouchManager.GetPosition(touchDirType);
    }
    #endregion


    // 按下时 触发此方法
    void PressIsTrue()
    {
        // UICamera.lastTouchPosition 为当前鼠标按下时的坐标（Vector2类型）
        if (bigCircleStartWorldPos == Vector2.zero)
        {
            bigCircleStartWorldPos = Camera.main.WorldToScreenPoint(bigCircleTrans.position);
        }
        Vector2 touchPos = TouchManager.GetPosition(touchDirType) - bigCircleStartWorldPos;
        // 当鼠标拖动的位置与中心位置大于bigCircleRadius时，则固定按钮位置不会超过bigCircleRadius。  bigCircleRadius为背景图片半径长度
        if (Vector2.Distance(touchPos, Vector2.zero) > bigCircleRadius)
        {
            // 按钮位置为 鼠标方向单位向量 * bigCircleRadius
            smallCircleTrans.localPosition = touchPos.normalized * bigCircleRadius;
        }
        else
        {
            // 按钮位置为鼠标位置
            smallCircleTrans.localPosition = touchPos;
        }

        // 按钮位置x轴 / 半径 的值为0-1的横向偏移量
        h = smallCircleTrans.localPosition.x / bigCircleRadius;

        // 按钮位置y轴 / 半径 的值为0-1的纵向偏移量
        v = smallCircleTrans.localPosition.y / bigCircleRadius;
    }
}