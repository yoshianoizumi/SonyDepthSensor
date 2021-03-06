﻿//
//  LoaderOutScene
//  Created by Yoshinao Izumi on 2017/04/19.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderOutScene : LoaderBase {

	public GameObject mePrefab;
	public ChonanController chonan;
	public JinanController jinan;
	public SannanController sannnan;
	public GameObject groundPrefab;
	public GameObject cakePrefab;
	public GameObject capPrefab;

	private AutoWalkMeController meObj;
	private SimpleController[] sonObjs;
	private GameObject groundObj;
	private GameObject cakeObj;
	private GameObject capObj;

	private bool loaded=false;

	private Vector3 virtualCameraPos;
	//sencor value estimage 
	private float groundy=-0.6f;
	private float scaleCharacter =2.0f; //1.5f;//may required caluc self in relation with cake scale or some
	//private Vector3 postionMainCamera = new Vector3(0f,0,0f); for debug
	private Vector2 postionGroundCenterXY = new Vector2(0f,0.25f);
	private Vector2 postionCakeCenterXY = new Vector2(-0.165f,0.8f);
	private Vector2 postionCapCenterXY = new Vector2(0.15f,0.597f);

	private Vector3 groundExecuteSize;
	private Vector3 cakeExecuteSize;
	private Vector3 capExecuteSize;

	private Vector3 thirdSonStartPos;

	private enum sceneActionState {
		none,
		wakeUp,
		sceneStart,
		sceneStartWait,
		onEndPoint
	}

	private float groundTop=0;

	void Start () {
	}

	void Update () {

		if (!loaded) {
			loaded = true;
			Vector3 meshSize;
			groundObj = Instantiate (groundPrefab, new Vector3 (1, 0, 0), Quaternion.identity);
			groundObj.transform.localScale = CommonStatic.outCamScaleGround;
			meshSize = groundObj.GetComponent<MeshFilter> ().mesh.bounds.size;
			groundExecuteSize 
				   = new Vector3 (meshSize.x * CommonStatic.outCamScaleGround.x,
				meshSize.y * CommonStatic.outCamScaleGround.y,
				meshSize.z * CommonStatic.outCamScaleGround.z);
			Vector3 postionGround = new Vector3 (postionGroundCenterXY.x, groundy, postionGroundCenterXY.y);
			groundObj.transform.position = postionGround;

			groundTop = groundy + groundExecuteSize.y / 2;


			Quaternion cakeQ = Quaternion.AngleAxis (-90, new Vector3 (1, 0, 0));
			cakeObj = Instantiate (cakePrefab, new Vector3 (0, 0, 0), cakeQ);
			cakeObj.transform.localScale = CommonStatic.outCamScaleCake;
			meshSize = cakeObj.GetComponent<MeshFilter> ().mesh.bounds.size;
			cakeExecuteSize 
					= new Vector3 (
				meshSize.x * CommonStatic.outCamScaleCake.x,
				meshSize.y * CommonStatic.outCamScaleCake.y,
				meshSize.z * CommonStatic.outCamScaleCake.z);
			float cakegroundPos = groundTop + cakeExecuteSize.z / 2.0f;//now -0.03 deffietent
			Vector3 postionCake = new Vector3 (postionCakeCenterXY.x, cakegroundPos-0.03f, postionCakeCenterXY.y);
			cakeObj.transform.position = postionCake;
	

			//double capSizeY=CommonStatic.capRateY * CommonStatic.outCamScaleCap.y;
			capObj = Instantiate (capPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
			capObj.transform.localScale = CommonStatic.outCamScaleCap;

			MeshFilter[] capmeshed = capObj.GetComponentsInChildren<MeshFilter> (true);
			foreach (MeshFilter mf in capmeshed) {
				if (mf.name == "mesh_cup_base") {
					meshSize = mf.mesh.bounds.size;
					break;
				}
			}
			capExecuteSize 
					= new Vector3 (meshSize.x * CommonStatic.outCamScaleCap.x,
				meshSize.y * CommonStatic.outCamScaleCap.y,
				meshSize.z * CommonStatic.outCamScaleCap.z);
			float capGroundPos = groundTop + capExecuteSize.y / 2.0f;//diff -0.0254f
			Vector3 postionCap = new Vector3 (postionCapCenterXY.x, capGroundPos-0.0254f, postionCapCenterXY.y);
			capObj.transform.position = postionCap;


			Vector3 cameraPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
			virtualCameraPos = cameraPos;

			//sons
			Vector3[] edges;
			sonObjs = new SimpleController[3];
	
			//first son 
			Vector3 firstSonStartPos = new Vector3 (
				                            cakeObj.transform.position.x,
				                            cakeObj.transform.position.y + cakeExecuteSize.z / 2.0f + CommonStatic.charaRateY / 2f,
				                            cakeObj.transform.position.z);
				
			ChonanController chonanObj = Instantiate (chonan, firstSonStartPos, SimpleController.identityQue ());
			chonanObj.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
			sonObjs [0] = chonanObj.GetComponentInChildren<SimpleController> (true);
			
			//second son
			edges = CommonStatic.getSightEdgePoint (cakeObj, cakeExecuteSize.x / 2.0f, virtualCameraPos);
			Vector3 secondSonStartPos;
			secondSonStartPos = edges [1];
			secondSonStartPos.y = groundTop + CommonStatic.charaRateY / 2f;
			JinanController jinanObj = Instantiate (jinan, secondSonStartPos, SimpleController.identityQue ());
			jinanObj.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
			sonObjs [1] = jinanObj.GetComponentInChildren<SimpleController> (true);
			float sideSpan = (CommonStatic.charaRateY * CommonStatic.outCamScaleCharacter.y) / 2;
			jinanObj.transform.position += new Vector3 (-sideSpan, 0, 0);

			//third son
			edges = CommonStatic.getSightEdgePoint (capObj, capExecuteSize.x / 2.0f, virtualCameraPos);
			Vector3 edgepos = edges [0];
			float hideback = (CommonStatic.charaRateY * CommonStatic.outCamScaleCharacter.y) * 1.1f;
			thirdSonStartPos = new Vector3 (edgepos.x + 0.02f, 
				groundTop + CommonStatic.charaRateY / 2f, edgepos.z + hideback);
			SannanController sannanObj = Instantiate (sannnan, thirdSonStartPos, SimpleController.identityQue ());
			sannanObj.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
			sonObjs [2] = sannanObj.GetComponentInChildren<SimpleController> (true);
		
			//---init ---
			sonObjs [0].initSon (cakeObj, cakeExecuteSize, virtualCameraPos);
			sonObjs [1].initSon (cakeObj, cakeExecuteSize, virtualCameraPos);
			sonObjs [2].initSon (capObj, capExecuteSize, virtualCameraPos);

		}
	}
		
	//hand event call
	public void startScene(Vector3 palmPoint){
		GameObject me=Instantiate(mePrefab,palmPoint, SimpleController.identityQue());
		me.transform.localScale = new Vector3(scaleCharacter, scaleCharacter, scaleCharacter);
		meObj = me.GetComponentInChildren<AutoWalkMeController> (true);
		AutoWalkMeController rc = meObj.GetComponentInChildren<AutoWalkMeController> (true);
		rc.setLoaderReference(this,virtualCameraPos);
		rc.onShowWithEffect = onShowWithEffect;
		rc.onShow = onShow;
		meObj.setAction (true);
		meObj.startShowWithEffect ();
	}

	private void startMainScene(){
		foreach(SimpleController mb in sonObjs){
			mb.doFadeIn ();
		}
			
		CapController cap = capObj.GetComponentInChildren<CapController> (true);
		cap.showKumamon ();

		meObj.transform.position = new Vector3 (postionGroundCenterXY.x ,
			groundTop + CommonStatic.charaRateY / 2f -0.03f,
			postionCapCenterXY.y+0.04f);

		AutoWalkMeController rc = meObj.GetComponentInChildren<AutoWalkMeController> (true);
		rc.setLoaderReference(this,virtualCameraPos);
		meObj.setAction (true);
		meObj.startShow ();
	}

	private void clean(){
		meObj.clear ();
		meObj = null;

		foreach(SimpleController sn in sonObjs){
			sn.clear();
		}
		sonObjs = null;
		groundObj = null;
		cakeObj = null;
		capObj = null;
	}

	public Transform getJinanTransform(){
		return sonObjs [1].transform;
	}

	public GameObject getCapObj(){
		return capObj;
	}

	public Vector3 getCapExecuteSize(){
		return capExecuteSize;
	}

	public Vector3 getCharaExecuteSize(){
		return scaleCharacter*(new Vector3(CommonStatic.charaRateX,CommonStatic.charaRateY,CommonStatic.charaRateZ));
	}
		
	public void onShowWithEffect(){
		startMainScene ();
	}

	// event visible 
	public void onShow(){
		//member loaded
	}

	// methoad to action start
	public void startMeWalking(){
		foreach(SimpleController mb in sonObjs){
			mb.setAction (true);
		}
		AutoWalkMeController rc = meObj.GetComponentInChildren<AutoWalkMeController> (true);
		rc.startWalking ();
	}

	//methoad to end state
	public void meToEndState() {
		AutoWalkMeController rc = meObj.GetComponentInChildren<AutoWalkMeController> (true);
		rc.toEndState ();
	}
}
