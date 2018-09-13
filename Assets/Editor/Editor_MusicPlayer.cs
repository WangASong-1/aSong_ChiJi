using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class Editor_MusicPlayer : EditorWindow {
	[MenuItem("GameObject/aSongEditor/MusicPlayer")]
	static void AddWindow(){
		//创建窗口
		Rect wr = new Rect (0, 0, 500, 500);
		Editor_MusicPlayer window = (Editor_MusicPlayer)EditorWindow.GetWindowWithRect(typeof(Editor_MusicPlayer),wr,true,"MusicPlayer");
		window.Show ();
	}

	private float mBeginTime;
	private float mEndTime;
	private AudioSource mAudioSource;
	private string text;
	private int dialogCount = 0;

	private List<float> beginTime;
	private List<string> lyric;
	private List<float> endTime;

	private string musicStr = "播放";
	private bool isPlayMusic = false;


	private string bugReporterName = "aSong_TestFold";

	void OnGUI(){
		mAudioSource = EditorGUILayout.ObjectField ("添加音频", mAudioSource, typeof(AudioSource), true) as AudioSource;
		if(mAudioSource == null ){
			return;
		}
		if(beginTime ==null){
			beginTime = new List<float> ();
			lyric = new List<string> ();
			endTime = new List<float> ();
		}

		mAudioSource.time = EditorGUILayout.Slider (mAudioSource.time, 0, mAudioSource.clip.length);
		if(GUILayout.Button(musicStr, GUILayout.Width(50))){
			isPlayMusic = !isPlayMusic;
			if(isPlayMusic){
				musicStr = "暂停";
				if(mAudioSource.isPlaying)
					mAudioSource.UnPause ();
				else{
					mAudioSource.Play ();
				}
			}else{
				musicStr = "播放";
				mAudioSource.Pause ();
			}
		}


		EditorGUILayout.BeginHorizontal ();
		if(GUILayout.Button("记录开始时间", GUILayout.Width(100))){
			mBeginTime = mAudioSource.time;
		}
		EditorGUILayout.TextArea(mBeginTime.ToString(),GUILayout.Width(100));

		if(GUILayout.Button("记录结束时间", GUILayout.Width(100))){
			mEndTime = mAudioSource.time;
		}
		EditorGUILayout.TextArea(mEndTime.ToString(),GUILayout.Width(100));
		EditorGUILayout.EndHorizontal ();

		if(GUILayout.Button("记录时间", GUILayout.Width(200))){
			dialogCount++;
			beginTime.Add (mBeginTime);
			lyric.Add ("");
			endTime.Add (mEndTime);
			mBeginTime = 0;
			mEndTime = 0;
		}

		if(dialogCount > 0){
			for(int i = 0; i < dialogCount; i ++){
				EditorGUILayout.BeginHorizontal ();
				//text = EditorGUILayout.TextField("输入文字:",text,GUILayout.Width(300));
				beginTime[i] = EditorGUILayout.FloatField(beginTime[i],GUILayout.Width(50));
				lyric[i] = EditorGUILayout.TextArea(lyric[i],GUILayout.Width(300));
				endTime[i] = EditorGUILayout.FloatField(endTime[i],GUILayout.Width(50));
				EditorGUILayout.EndHorizontal ();
			}
		}

		bugReporterName = EditorGUILayout.TextField ("输入文件名", bugReporterName, GUILayout.Width (300));
		if(GUILayout.Button("保存", GUILayout.Width(200))){
			SaveBug();
		}
	}

	void OnInspectorUpdate(){
		this.Repaint();
	}


	//用于保存当前信息
	void SaveBug()
	{
		//Directory.CreateDirectory(Application.dataPath + bugReporterName);
		Directory.CreateDirectory("Assets/Dialog/");
		StreamWriter sw = new StreamWriter("Assets/Dialog/" + bugReporterName + ".txt");
		for(int i = 0; i < beginTime.Count; i ++){
			sw.WriteLine(beginTime[i] + "$" +lyric[i] + "$" + endTime[i]);
		}

		sw.Close();
	}
}
