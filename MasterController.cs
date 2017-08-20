using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;

public class MasterController : MonoBehaviour {

	public int NumberEaten1=0;
	public int NumberEaten2=0;
	//public int NumberEaten3=0;
	public int NumberEatenAll=0;
	public static int NumberCreatures=200;
	public int TimeOut=0;
	//public List<int> NumberEaters3;
	public List<int> NumberEaters2;
	public List<int> NumberEaters1;
	public List<int> FramesElapsed;
	public int NumGenerations=0;
	const int NumberOffSpring=5;

	public GameObject NewCreature;

	private int GetNumberEaten(GameObject Eater)
	{
		int iReturn=new int();
		if(Eater.GetComponent<BotBehavior>().HasEatenFood1==true) iReturn++;
		//if(Eater.GetComponent<BotBehavior>().HasEatenFood2==true) iReturn++;
		//if(Eater.GetComponent<BotBehavior>().HasEatenFood3==true) iReturn++;
		return iReturn;
	}

	private void PrintNeuralNetwork(GameObject Eater,string sFileName)
	{
		float[] InputList = Eater.GetComponent<BotBehavior> ().LegLeft.InputWeightIndex;
	}

	bool GetIsTimeForNextGeneration(int ThresholdCount)
	{
		//if(NumGenerations<1000) return (TimeOut<=0)|(NumberEaten1>=ThresholdCount);
		//if(NumGenerations<2000) return (TimeOut<=0)|(NumberEaten2>=ThresholdCount);
		return (TimeOut<=0)|(NumberEatenAll>=ThresholdCount);
	}

	void WriteFloatListToFile(List<float> ListToUse,string FileName,string EntryDelimiter)
	{
		FileStream fs;
		fs=File.Create(FileName);
		int iLoop=0;
		float ThisEntry=0f;
		string sList="";
		while(iLoop<ListToUse.Count)
		{
			ThisEntry=ListToUse[iLoop];
			sList=sList+ThisEntry.ToString();
			sList=sList+EntryDelimiter;
			iLoop++;
		}
		byte[] info = new UTF8Encoding(true).GetBytes(sList);
		fs.Write(info,0,info.Length);
		fs.Close ();
	}

	void WriteIntListToFile(List<int> ListToUse,string FileName,string EntryDelimiter)
	{
		FileStream fs;
		fs=File.Create(FileName);
		int iLoop=0;
		int ThisEntry=0;
		string sList="";
		while(iLoop<ListToUse.Count)
		{
			ThisEntry=ListToUse[iLoop];
			sList=sList+ThisEntry.ToString();
			sList=sList+EntryDelimiter;
			iLoop++;
		}
		byte[] info = new UTF8Encoding(true).GetBytes(sList);
		fs.Write(info,0,info.Length);
		fs.Close ();	
	}

	//Takes two values with a range of -180 to +180 and gets the angular separation
	float GetAngularSeparation(float fAngle1, float fAngle2)
	{
		return Mathf.Abs(Mathf.DeltaAngle(fAngle1,fAngle2));
	}

	// Update is called once per frame
	void Update () {
		TimeOut=TimeOut-1;
		int ThresholdCount=NumberCreatures / NumberOffSpring;
		if (GetIsTimeForNextGeneration(ThresholdCount))
		{
			if(NumGenerations>0)
			{
				FramesElapsed.Add(1000-TimeOut);
				//NumberEaters3.Add(NumberEaten3);
				//NumberEaters2.Add(NumberEatenAll);
				//NumberEaters1.Add(NumberEaten1+NumberEaten2);
				NumberEaters1.Add(NumberEaten1);
				//WriteIntListToFile(NumberEaters2,"Numbereaters2.txt",",");
				WriteIntListToFile(NumberEaters1,"Numbereaters1.txt",",");
				WriteIntListToFile(FramesElapsed,"FramesElapsed.txt",",");
			}
			TimeOut=1000;
			GameObject[] PlayerList=new GameObject[NumberCreatures];
			PlayerList=GameObject.FindGameObjectsWithTag("Player");
			Vector3 StartLoc=new Vector3((UnityEngine.Random.value-0.5f)*30,0.0f,(UnityEngine.Random.value-0.5f)*30);
			Vector3 FoodLoc=new Vector3((UnityEngine.Random.value-0.5f)*30,0.0f,(UnityEngine.Random.value-0.5f)*30);
			//Vector3 FoodLoc2=new Vector3((UnityEngine.Random.value-0.5f)*30,0.0f,(UnityEngine.Random.value-0.5f)*30);
			while (Vector3.Distance(StartLoc,FoodLoc)<15.0f)
			{
				FoodLoc=new Vector3((UnityEngine.Random.value-0.5f)*30,0.0f,(UnityEngine.Random.value-0.5f)*30);
			}
			float fFace = (UnityEngine.Random.value-0.5f) * 360f;
			Debug.Log (fFace + " start");
			Debug.Log (FoodLoc.z - StartLoc.z + " zdist");
			Debug.Log (FoodLoc.x - StartLoc.x + " xdist");
			float angle = Mathf.Atan2 (FoodLoc.x - StartLoc.x,FoodLoc.z - StartLoc.z);
			angle = Mathf.Rad2Deg*angle;
			Debug.Log (angle + " angle");
			while (GetAngularSeparation(angle,fFace)<=15.0f)
			{
				fFace = (UnityEngine.Random.value-0.5f) * 360f;
			}
			Debug.Log (fFace + " final");
			Quaternion StartFace = Quaternion.Euler(0.0f, fFace, 0.0f);
			NumberEaten1=0;
			//NumberEaten2=0;
			//NumberEaten3=0;
			NumberEatenAll=0;
			int[] Fed=new int[4];
			int EatCount;
			foreach (GameObject PlayerListEntry in PlayerList)
			{
				EatCount=GetNumberEaten(PlayerListEntry);
				Fed[EatCount]++;
			}
			int iLoop=1;
			int SurvivorCount=0;
			//int FoodEatenLoop=2;//Start with those that have eaten 2, then those that have eaten 1
			int FoodEatenLoop=1;
			while(FoodEatenLoop>0)
			{	
				foreach (GameObject PlayerListEntry in PlayerList)
				{
					if(SurvivorCount>=ThresholdCount|GetNumberEaten(PlayerListEntry)==0)
					{
						PlayerListEntry.GetComponent<BotBehavior>().IsDestroyed=true;
						Destroy (PlayerListEntry);
					}
					else if(GetNumberEaten(PlayerListEntry)>=FoodEatenLoop)
					{
						if(SurvivorCount<ThresholdCount)
						{
							iLoop=0;
							while(iLoop<NumberOffSpring)
							{
								Instantiate (PlayerListEntry,StartLoc,StartFace);
								iLoop=iLoop+1;
							}
							PlayerListEntry.transform.position = StartLoc;
							PlayerListEntry.transform.rotation = StartFace;
							//(Mathf.Clamp (transform.position.x, 0.0f,0.0f),0.0f,Mathf.Clamp (transform.position.z, 0.0f,0.0f));
							SurvivorCount++;
						}
						Destroy (PlayerListEntry);
						PlayerListEntry.GetComponent<BotBehavior>().IsDestroyed=true;
					}
				}
				FoodEatenLoop=FoodEatenLoop-1;
			}				
			NumGenerations=NumGenerations+1;
			while(SurvivorCount<ThresholdCount)//In case there are too few eaters (eg timeout)
			{
				iLoop=1;
				SurvivorCount=SurvivorCount+1;
				while(iLoop<NumberOffSpring)
				{
					iLoop=iLoop+1;
					Instantiate (NewCreature,StartLoc,StartFace);
					Instantiate (NewCreature,StartLoc,StartFace);
				}
			}
			GameObject[] PlayerList2=new GameObject[NumberCreatures];
			PlayerList2=GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject PlayerListEntry in PlayerList2)
			{		
				PlayerListEntry.GetComponent<BotBehavior>().HasEatenFood1=false;
				//PlayerListEntry.GetComponent<BotBehavior>().HasEatenFood2=false;
				//PlayerListEntry.GetComponent<BotBehavior>().HasEatenFood3=false;
				PlayerListEntry.GetComponent<BotBehavior>().enabled=true;
			}
			GameObject.Find ("FoodObject1").transform.position = FoodLoc;//ReRandomize food
			//GameObject.Find ("FoodObject2").transform.position = FoodLoc2;//ReRandomize food
			//GameObject.Find ("FoodObject3").transform.position = new Vector3 ((Random.value-0.5f)*30,0.0f,(Random.value-0.5f)*30);//ReRandomize food
		}
	}
}
