using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//道具名. 
public enum PropName
{
    pistol, M416 ,other
}

//道具种类:射手步枪,冲锋枪,雷
public enum PropType
{
    bomb, other
}


public class aSongUI_PropData : MonoBehaviour {
    public class Prop
    {
        public int propID;
        public Sprite pic;
        public PropType type;
        public PropName name;
        public static int idCount = 0;
        public Prop(PropName name)
        {
            idCount++;
            propID = idCount;
            pic = Resources.Load<Sprite>(name.ToString());
            type = aSongUI_Controller.Instance.GetPropType(name);
        }

        private Prop() { }
    }

    public List<Prop> props;
}
