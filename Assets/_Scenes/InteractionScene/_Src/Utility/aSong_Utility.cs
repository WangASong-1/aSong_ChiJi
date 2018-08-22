using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RenderingMode 
{
	Opaque,
	Cutout,
	Fade,
	Transparent,
}

public class aSong_Utility  {
    public const float PassingPicShowTime = 4f;
    public const float PassingPic_Dialog_ShowTime = 6f;

    //手枪震动力度和时间
    public const float PistolTriggerHapticPulseStreath = 0.8f;
    public const float PistolTriggerHapticPulseTime = 0.2f;
    public const float RifleTriggerHapticPulseStreath = 1f;
    public const float RifleTriggerHapticPulseTime = 0.3f;

    //yield静态保存
    #region
    private static List<Vector3> allWayPoints = new List<Vector3> ();

    static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100);

    static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    public static WaitForEndOfFrame EndOfFrame
    {
        get { return _endOfFrame; }
    }

    static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
    public static WaitForFixedUpdate FixedUpdate
    {
        get { return _fixedUpdate; }
    }

    public static WaitForSeconds WaitSeconds(float seconds)
    {
        if (!_timeInterval.ContainsKey(seconds))
            _timeInterval.Add(seconds, new WaitForSeconds(seconds));
        return _timeInterval[seconds];

    }
    #endregion


    //改变材质球的RenderMode
    public static void SetMaterialRenderingMode (Material material, RenderingMode renderingMode)
	{
		switch (renderingMode) {
		case RenderingMode.Opaque:
			material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			material.SetInt ("_ZWrite", 1);
			material.DisableKeyword ("_ALPHATEST_ON");
			material.DisableKeyword ("_ALPHABLEND_ON");
			material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = -1;
			break;
		case RenderingMode.Cutout:
			material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			material.SetInt ("_ZWrite", 1);
			material.EnableKeyword ("_ALPHATEST_ON");
			material.DisableKeyword ("_ALPHABLEND_ON");
			material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 2450;
			break;
		case RenderingMode.Fade:
			material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			material.SetInt ("_ZWrite", 0);
			material.DisableKeyword ("_ALPHATEST_ON");
			material.EnableKeyword ("_ALPHABLEND_ON");
			material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;
			break;
		case RenderingMode.Transparent:
			material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			material.SetInt ("_ZWrite", 0);
			material.DisableKeyword ("_ALPHATEST_ON");
			material.DisableKeyword ("_ALPHABLEND_ON");
			material.EnableKeyword ("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;
			break;
		}
	}

	//计算到目标点的距离,若不能到达，则计算的是两点之间的距离
	public static float CalculatePathLength (Vector3 _position,Vector3 targetPosition, UnityEngine.AI.NavMeshAgent _nav,UnityEngine.AI.NavMeshPath _path)
	{
		_path.ClearCorners ();
		allWayPoints.Clear ();

		UnityEngine.AI.NavMeshPath path = _path;
		if (_nav.enabled) {
			if (_path != null) {
				_nav.CalculatePath (targetPosition, _path);
				path = _path;
			}else{
				path = new UnityEngine.AI.NavMeshPath ();
				_nav.CalculatePath (targetPosition, path);
			}
		}

		allWayPoints.Add (_position);
		for(int i = 0; i < path.corners.Length; i++)
		{
			allWayPoints.Add (path.corners [i]);
		}
			
		allWayPoints.Add (targetPosition);

		float pathLength = 0;

		for(int i = 0; i < allWayPoints.Count -1; i++)
		{
			pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
		}
		return pathLength;
	}

	public static void SetActiveSelf(GameObject obj, bool value){
		if (obj.activeSelf != value)
			obj.SetActive (value);
	}

    public static void SetPlayerEyePosition( Transform _playerArea, Transform _eye, Vector3 _vector)
    {
        _playerArea.position = _vector;
        //Vector3 vec = _eye.localPosition;
        Vector3 vec = _eye.position;
        vec.y = _playerArea.position.y;
        _playerArea.position -= vec - _playerArea.position; // _playerArea.InverseTransformVector(vec)
    }

    public static void SetPlayerToPoint(Transform _playerArea, Transform _eye, Transform _point)
    {
        _playerArea.rotation = Quaternion.LookRotation(_point.forward);
        _playerArea.Rotate(Vector3.up, -_eye.localEulerAngles.y);
        SetPlayerEyePosition(_playerArea, _eye, _point.position);
    }
}
