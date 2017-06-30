using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TouchDirType    // 方向类型
{
    Empty,
    Left,
    Right
}
public class TouchManager : MonoBehaviour {

    private static TouchManager instance;
    public static TouchManager Instance { get { return instance; } }

    float halfWidth;
    static Dictionary<TouchDirType, Vector2> dirTypeTouch;
    Dictionary<int, TouchDirType> touchIndexType;

    public Action<TouchDirType> onTouchBegan;       // 手指按下时 事件
    public Action<TouchDirType> onTouchMoved;       // 手指按住时 事件
    public Action<TouchDirType> onTouchEnded;       // 手指松开时 事件

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // Use this for initialization
    void Start()
    {
        halfWidth = Screen.width * 0.5f;

        dirTypeTouch = new Dictionary<TouchDirType, Vector2>();
        dirTypeTouch.Add(TouchDirType.Left, Vector2.zero);
        dirTypeTouch.Add(TouchDirType.Right, Vector2.zero);

        touchIndexType = new Dictionary<int, TouchDirType>();
    }

    void LateUpdate()
    {
        for (int i = 0, length = Input.touchCount > 2 ? 2 : Input.touchCount; i < length; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < halfWidth)
                {
                    touchIndexType.Add(touch.fingerId, TouchDirType.Left);
                }
                else
				{
					touchIndexType.Add(touch.fingerId, TouchDirType.Right);
                }
				dirTypeTouch[touchIndexType[touch.fingerId]] = touch.position;
                if (onTouchBegan != null)
					onTouchBegan(touchIndexType[touch.fingerId]);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                dirTypeTouch[touchIndexType[touch.fingerId]] = touch.position;
                if (onTouchMoved != null)
					onTouchMoved(touchIndexType[touch.fingerId]);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (onTouchEnded != null)
					onTouchEnded(touchIndexType[touch.fingerId]);
				dirTypeTouch[touchIndexType[touch.fingerId]] = Vector2.zero;
				touchIndexType.Remove(touch.fingerId);
            }
        }
    }

    /// <summary>
    /// 通过方向获取输入的坐标
    /// </summary>
    /// <param name="dirType"></param>
    /// <returns></returns>
    public static Vector2 GetTouchPosition(TouchDirType dirType)
    {
#if UNITY_EDITOR
        return Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IPHONE
        return dirTypeTouch[dirType];
#endif
    }

    public static string leftOffset;
    public static string rightOffset;
    void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;
        bb.normal.textColor = new Color(1, 0, 0);    
        bb.fontSize = 40;
        
		GUI.Label(new Rect(0, 0, 500, 50), "LeftPosition: " + GetTouchPosition(TouchDirType.Left), bb);

		GUI.Label(new Rect(0, 80, 500, 50), "RightPosition: " + GetTouchPosition(TouchDirType.Right), bb);

		GUI.Label(new Rect(0, 160, 500, 50), "LeftOffset: " + leftOffset, bb);

		GUI.Label(new Rect(0, 240, 500, 50), "RightOffset: " + rightOffset, bb);
    }

}
