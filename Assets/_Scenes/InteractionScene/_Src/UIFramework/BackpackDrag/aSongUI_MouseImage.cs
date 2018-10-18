using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aSongUI_MouseImage : MonoBehaviour {
    private aSongUI_DragManager gsm;
    private Image mouse_image;
    private int mouse_type = 0;
    public Sprite none;
    public Sprite hair;
    public Sprite weapon;
    public Sprite foot;
    public Color None;
    public Color NotNone;
    public Camera cam;

    void Awake()
    {
        gsm = aSongUI_DragManager.GetInstance();
        gsm.SetMouse(this);
        mouse_image = GetComponent<Image>();
    }

    public int GetMouseType()
    {
        return mouse_type;
    }

    public void SetMouseType(int Mouse_type)
    {
        mouse_type = Mouse_type;
    }

    void Update()
    {
        if (mouse_type == 0)
        {
            mouse_image.sprite = none;
            mouse_image.color = None;
        }
        else
        {
            mouse_image.color = NotNone;
            if (mouse_type == 1) mouse_image.sprite = hair;
            else if (mouse_type == 2) mouse_image.sprite = weapon;
            else if (mouse_type == 3) mouse_image.sprite = foot;
        }
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
    }
}
