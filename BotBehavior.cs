using UnityEngine;
using System.Collections;

public class BotBehavior : MonoBehaviour
{
	private const int INTERNEURON_COUNT = 12;
	private const float MOVEMENT_SCALE = 0.2f;
	private const float TURN_SCALE = 180.0f;
	static private int NUMBER_SENSOR_MOTOR_NEURONS = 5;
	private const int LEFT_LEG_INDEX = INTERNEURON_COUNT + 0;//Array indexing is 0-based in unity
	private const int RIGHT_LEG_INDEX = INTERNEURON_COUNT + 1;
	private const int RIGHT_EAR_INDEX = INTERNEURON_COUNT + 2;
	private const int LEFT_EAR_INDEX = INTERNEURON_COUNT + 3;
	private const int TACTILE_NEURON=INTERNEURON_COUNT+4;
	//private const int FORWARD_LEG_INDEX = INTERNEURON_COUNT + 5;
	//private const int GUSTATORY_NEURON_1 = INTERNEURON_COUNT + 6;
	//private const int GUSTATORY_NEURON_2 = INTERNEURON_COUNT + 7;
	//private const int GUSTATORY_NEURON_3 = INTERNEURON_COUNT + 7;
	//private const int PACEMAKER_NEURON_1 = INTERNEURON_COUNT + 6;
	//private const int PACEMAKER_NEURON_2 = INTERNEURON_COUNT + 7;
	public const float MAXIMUM_RATE=1.0f;
	public const float MINIMUM_RATE=0.0f;
	public bool IsDestroyed=false;

	[System.Serializable]
	public class Neuron
	{
		public bool IsLeftEar;
		public bool IsRightEar;
		public bool IsLeftLeg;
		public bool IsRightLeg;
		//public bool IsForwardLeg;
		//public bool IsGust1;
		//public bool IsGust2;
		//public bool IsGust3;
		//public bool IsPace1;
		//public bool IsPace2;
		public float[] InputWeightIndex=new float[INTERNEURON_COUNT+NUMBER_SENSOR_MOTOR_NEURONS];
		public float SpontaneousRate;
		public float LastActivity;
		public float CurrentActivity;
		public bool IsTactile;
	}

	public bool IsCopy = false;
	public Neuron EarLeft = new Neuron();
	public Neuron EarRight = new Neuron();
	public Neuron LegLeft = new Neuron();
	public Neuron LegRight = new Neuron();
	//public Neuron LegForward = new Neuron ();
	//public Neuron Gustatory1 = new Neuron ();
	//public Neuron Gustatory2 = new Neuron ();
	//public Neuron Gustatory3 = new Neuron ();
	//public Neuron PaceMaker1 = new Neuron ();
	//public Neuron PaceMaker2 = new Neuron ();
	public Neuron Tactile=new Neuron();
	public Neuron[] Interneurons = new Neuron[INTERNEURON_COUNT];

	private float GetNewRandomSynapseWeight(float fOldWeight)
	{
		float fNewWeight=(Random.value-0.5f)*2;//Gives a value between -1 and +1
		return (fOldWeight+fNewWeight)/2;//Average of old and new weights
	}

	[System.Serializable]//Serialization is a way of storing information - allows viewing in inspector
	public class Boundary
	{
		public float xMin, xMax, zMin, zMax;
	}

	public bool HasEatenFood1=false;
	public bool HasEatenFood2=false;
	//public bool HasEatenFood3=false;

	void OnTriggerEnter(Collider other)
	{
		//if (other.name==("FoodObject1")|other.name==("FoodObject2")|other.name==("FoodObject3"))
		if(other.name==("FoodObject1"))
		{
			//Tactile.CurrentActivity=1;
		}
		else return;
		if (other.name==("FoodObject1")&&HasEatenFood1==false)
		{
			HasEatenFood1=true;
			GameObject.Find ("MasterController").GetComponent<MasterController>().NumberEaten1++;
		}
		//else if (other.name==("FoodObject2")&&HasEatenFood2==false)//Do not need to eat the foods in order
		//{
			//HasEatenFood2=true;
			//GameObject.Find ("MasterController").GetComponent<MasterController>().NumberEaten2++;
		//}
		//else if (other.name==("FoodObject3")&&HasEatenFood3==false&&HasEatenFood1==true&&HasEatenFood2==true)//Must eat foods in order of frequency
		//{
			//HasEatenFood3=true;
			//GameObject.Find ("MasterController").GetComponent<MasterController>().NumberEaten3++;
		//}
		else return;
		//if(HasEatenFood3==true&HasEatenFood2==true&HasEatenFood1==true)
		//if(HasEatenFood2==true&HasEatenFood1==true)
		if(HasEatenFood1==true)
		{
			GameObject.Find ("MasterController").GetComponent<MasterController>().NumberEatenAll++;	
		}
	}

	public Neuron InitializeNeuron()
	{
		Neuron InitializeTarget = new Neuron ();
		InitializeTarget.InputWeightIndex [LEFT_LEG_INDEX] = GetNewRandomSynapseWeight (0f);
		InitializeTarget.InputWeightIndex [RIGHT_LEG_INDEX] = GetNewRandomSynapseWeight (0f);
		InitializeTarget.InputWeightIndex [RIGHT_EAR_INDEX] = GetNewRandomSynapseWeight (0f);
		InitializeTarget.InputWeightIndex [LEFT_EAR_INDEX] = GetNewRandomSynapseWeight (0f);
		//InitializeTarget.InputWeightIndex [FORWARD_LEG_INDEX] = GetNewRandomSynapseWeight (0f);
		//InitializeTarget.InputWeightIndex [GUSTATORY_NEURON_1] = GetNewRandomSynapseWeight (0f);
		//InitializeTarget.InputWeightIndex [GUSTATORY_NEURON_2] = GetNewRandomSynapseWeight (0f);
		//InitializeTarget.InputWeightIndex [GUSTATORY_NEURON_3] = GetNewRandomSynapseWeight (0f);
		//InitializeTarget.InputWeightIndex [PACEMAKER_NEURON_1] = GetNewRandomSynapseWeight (0f);
		//InitializeTarget.InputWeightIndex [PACEMAKER_NEURON_2] = GetNewRandomSynapseWeight (0f);
		InitializeTarget.InputWeightIndex [TACTILE_NEURON] = GetNewRandomSynapseWeight (0f);
		InitializeTarget.SpontaneousRate = Random.value;
		InitializeTarget.LastActivity = InitializeTarget.SpontaneousRate;
		InitializeTarget.CurrentActivity = InitializeTarget.SpontaneousRate;
		int InterneuronCheck = new int ();
		while (InterneuronCheck<INTERNEURON_COUNT)
		{
			InitializeTarget.InputWeightIndex [InterneuronCheck] = GetNewRandomSynapseWeight (0f);
			InterneuronCheck = InterneuronCheck + 1;
		}
		//Debug.Log (InitializeTarget.InputWeightIndex[LEFT_LEG_INDEX]);
		return InitializeTarget;
	}

	float GetActivityChange(Neuron Postsynaptic, Neuron Presenaptic, int SynapseIndex, float TotalWeights)
	{
		float SynapticWeight = Postsynaptic.InputWeightIndex[SynapseIndex];
		SynapticWeight = (0.25f*SynapticWeight);
		return SynapticWeight * Presenaptic.LastActivity;
	}

	float GetLoudnessAtEar(string FoodName,Vector3 EarPosition)
	{
		Vector3 Loc2=new Vector3();
		Loc2=GameObject.Find(FoodName).transform.position;
		float fDistance=Mathf.Sqrt ((EarPosition.x-Loc2.x)*(EarPosition.x-Loc2.x)+(EarPosition.z-Loc2.z)*(EarPosition.z-Loc2.z));
		int Intensity=new int();
		if(FoodName=="FoodObject1")Intensity=GameObject.Find(FoodName).GetComponent<FoodOsc>().Intensity;
		//if(FoodName=="FoodObject2")Intensity=GameObject.Find(FoodName).GetComponent<FoodOsc2>().Intensity;
		//if(FoodName=="FoodObject3")Intensity=GameObject.Find(FoodName).GetComponent<FoodOsc3>().Intensity;
		if(fDistance<1f) fDistance=1f;
		return (1f*Intensity)/(fDistance);
	}

	//This allows a pacemaker neuron to follow the 'true' frequency of the sound; IE it makes the sound waveform known to the circuit.
	float GetPaceMakerLoudness(string FoodName)
	{
		int Intensity = new int ();
		if(FoodName=="FoodObject1")Intensity=GameObject.Find(FoodName).GetComponent<FoodOsc>().Intensity;
		//if(FoodName=="FoodObject2")Intensity=GameObject.Find(FoodName).GetComponent<FoodOsc2>().Intensity;
		//if(FoodName=="FoodObject3")Intensity=GameObject.Find(FoodName).GetComponent<FoodOsc3>().Intensity;
		return Intensity*1.0f;
	}

	Neuron UpdateNeuron(Neuron UpdateTarget)
	{
		float FireRate = UpdateTarget.SpontaneousRate*1.0f;
		//if(UpdateTarget.IsGust1)
		//{
			//if(HasEatenFood1==true)	UpdateTarget.CurrentActivity=1.0f;
			//else UpdateTarget.CurrentActivity=0.0f;
			//return UpdateTarget;
		//}
		//if(UpdateTarget.IsGust2)
		//{
			//if(HasEatenFood2==true)	UpdateTarget.CurrentActivity=1.0f;
			//else UpdateTarget.CurrentActivity=0.0f;
			//return UpdateTarget;
		//}
		//if(UpdateTarget.IsGust3)
		//{
			//if(HasEatenFood3==true)	UpdateTarget.CurrentActivity=1.0f;
			//else UpdateTarget.CurrentActivity=0.0f;
			//return UpdateTarget;
		//}
		if(UpdateTarget.IsTactile==true)
		{
			UpdateTarget.CurrentActivity=FireRate;
			return UpdateTarget;
		}
		
		Transform EarTransform=null;
		if (UpdateTarget.IsLeftEar == true)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				if(transform.GetChild(i).name=="PositionEarLeft")
				{
					EarTransform=transform.GetChild(i);
				}
			}
		}
		if (UpdateTarget.IsRightEar == true)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				if(transform.GetChild(i).name=="PositionEarRight") 
				{
					EarTransform=transform.GetChild(i);
				}
			}
		}
		if (UpdateTarget.IsRightEar == true|UpdateTarget.IsLeftEar == true)
		{
			Vector3 EarPosition=new Vector3();
			EarPosition=EarTransform.position;
			FireRate=FireRate+GetLoudnessAtEar("FoodObject1",EarPosition);
			//FireRate=FireRate+GetLoudnessAtEar("FoodObject2",EarPosition);
			//FireRate=FireRate+GetLoudnessAtEar("FoodObject3",EarPosition);
			FireRate = Mathf.Clamp (FireRate, MINIMUM_RATE, MAXIMUM_RATE);
			UpdateTarget.CurrentActivity = FireRate;
			return UpdateTarget;//We're done with the ear.
		}
		//if (UpdateTarget.IsPace1 == true)
		//{
			//FireRate=FireRate+GetPaceMakerLoudness("FoodObject1");
			//FireRate = Mathf.Clamp (FireRate, MINIMUM_RATE, MAXIMUM_RATE);
			//UpdateTarget.CurrentActivity = FireRate;
			//return UpdateTarget;//We're done with the pacemaker.
		//}
		//if (UpdateTarget.IsPace2 == true)
		//{
			//FireRate=FireRate+GetPaceMakerLoudness("FoodObject2");
			//FireRate = Mathf.Clamp (FireRate, MINIMUM_RATE, MAXIMUM_RATE);
			//UpdateTarget.CurrentActivity = FireRate;
			//return UpdateTarget;//We're done with the pacemaker.
		//}
		float TotalInputWeights=new float();
		int SynapseCounter=new int();
		while(SynapseCounter<INTERNEURON_COUNT+NUMBER_SENSOR_MOTOR_NEURONS)//Note: TotalInputWeights is deprecated. It can be used to normalize inputs, though.
		{
			TotalInputWeights=TotalInputWeights+Mathf.Abs (UpdateTarget.InputWeightIndex[SynapseCounter]);
			SynapseCounter=SynapseCounter+1;
		}
		SynapseCounter = 0;
		while (SynapseCounter<INTERNEURON_COUNT)//So we can make a weighted average of them
		{
			FireRate=FireRate+GetActivityChange (UpdateTarget, Interneurons [SynapseCounter], SynapseCounter,TotalInputWeights);
			SynapseCounter=SynapseCounter+1;
		}
		FireRate = FireRate + GetActivityChange (UpdateTarget,LegLeft,LEFT_LEG_INDEX,TotalInputWeights);
		FireRate = FireRate + GetActivityChange (UpdateTarget,LegRight,RIGHT_LEG_INDEX,TotalInputWeights);
		//FireRate = FireRate + GetActivityChange (UpdateTarget,LegForward,FORWARD_LEG_INDEX,TotalInputWeights);
		FireRate = FireRate + GetActivityChange (UpdateTarget,EarLeft,LEFT_EAR_INDEX,TotalInputWeights);
		FireRate = FireRate + GetActivityChange (UpdateTarget,EarRight,RIGHT_EAR_INDEX,TotalInputWeights);
		//FireRate = FireRate + GetActivityChange (UpdateTarget,Gustatory1,GUSTATORY_NEURON_1,TotalInputWeights);
		//FireRate = FireRate + GetActivityChange (UpdateTarget,Gustatory2,GUSTATORY_NEURON_2,TotalInputWeights);
		//FireRate = FireRate + GetActivityChange (UpdateTarget,Gustatory3,GUSTATORY_NEURON_3,TotalInputWeights);
		//FireRate = FireRate + GetActivityChange (UpdateTarget,PaceMaker1,PACEMAKER_NEURON_1,TotalInputWeights);
		//FireRate = FireRate + GetActivityChange (UpdateTarget,PaceMaker2,PACEMAKER_NEURON_2,TotalInputWeights);
		FireRate = FireRate + GetActivityChange (UpdateTarget,Tactile,TACTILE_NEURON,TotalInputWeights);
		FireRate = Mathf.Clamp (FireRate, MINIMUM_RATE, MAXIMUM_RATE);
		UpdateTarget.CurrentActivity = FireRate;
		return UpdateTarget;
	}

	Neuron RollOverActivities(Neuron RollTarget)//Vanity function
	{
		RollTarget.LastActivity = RollTarget.CurrentActivity;
		return RollTarget;
	}

	Neuron Mutate(Neuron Mutated)
	{
		int iInputPick = Random.Range (0, NUMBER_SENSOR_MOTOR_NEURONS + INTERNEURON_COUNT);
		//string sInputPick = iInputPick.ToString();
		//Debug.Log(sInputPick);
		if(Random.value>.025f) Mutated.InputWeightIndex[iInputPick]=GetNewRandomSynapseWeight(Mutated.InputWeightIndex[iInputPick]);
		else Mutated.SpontaneousRate=(Mutated.SpontaneousRate+Random.value)/2;//Average of old rate and a random rate
		return Mutated;
	}	

	// Use this for initialization
	void Start ()
	{
		//transform.Rotate (new Vector3(0.0f,Random.value*360f,0.0f));
		IsDestroyed=false;
		HasEatenFood1=false;
		HasEatenFood2=false;
		//HasEatenFood3=false;
		if (IsCopy == false)
		{
			EarLeft = InitializeNeuron ();
			EarRight = InitializeNeuron ();
			LegLeft = InitializeNeuron ();
			LegRight = InitializeNeuron ();
			//LegForward = InitializeNeuron ();
			//Gustatory1 = InitializeNeuron ();
			//Gustatory2 = InitializeNeuron ();
			//PaceMaker1 = InitializeNeuron ();
			//PaceMaker2 = InitializeNeuron ();
			//Gustatory3 = InitializeNeuron ();
			Tactile=InitializeNeuron ();
			int InterneuronCheck = new int ();
			while (InterneuronCheck<INTERNEURON_COUNT)
			{
				Interneurons [InterneuronCheck] = InitializeNeuron ();
				InterneuronCheck = InterneuronCheck + 1;
			}
			IsCopy = true;
		}
		else
		{
			int NumMutations=0;
			while (Random.value < 0.8f)//Yields an average of four mutations, exponentially distributed.
			{
				NumMutations++;
			}
			//Debug.Log ("Mutation happening");
			while(NumMutations>0)
			{
				int iMutateTarget=Random.Range(0,INTERNEURON_COUNT+NUMBER_SENSOR_MOTOR_NEURONS);
				switch(iMutateTarget)
				{
				case LEFT_LEG_INDEX: LegLeft=Mutate (LegLeft); break;
				case RIGHT_LEG_INDEX: LegRight=Mutate (LegRight); break;
				//case FORWARD_LEG_INDEX: LegForward=Mutate (LegForward); break;
				case RIGHT_EAR_INDEX: EarRight=Mutate (EarRight); break;
				case LEFT_EAR_INDEX: EarLeft=Mutate (EarLeft); break;
				//case GUSTATORY_NEURON_1: Gustatory1=Mutate (Gustatory1); break;
				//case GUSTATORY_NEURON_2: Gustatory1=Mutate (Gustatory2); break;
				//case PACEMAKER_NEURON_1: PaceMaker1=Mutate (PaceMaker1); break;
				//case PACEMAKER_NEURON_2: PaceMaker2=Mutate (PaceMaker2); break;
				//case GUSTATORY_NEURON_3: Gustatory1=Mutate (Gustatory3); break;
				case TACTILE_NEURON: Tactile=Mutate (Tactile); break;
				default: Interneurons[iMutateTarget]=Mutate (Interneurons[iMutateTarget]); break;
				}
				NumMutations=NumMutations-1;
			}
		}
		EarLeft.IsLeftEar = true;
		EarRight.IsRightEar = true;
		LegLeft.IsLeftLeg = true;
		LegRight.IsRightLeg = true;
		//LegForward.IsForwardLeg = true;
		//Gustatory1.IsGust1 = true;
		//Gustatory2.IsGust2 = true;
		//PaceMaker1.IsPace1 = true;
		//PaceMaker2.IsPace2 = true;
		//Gustatory3.IsGust3 = true;
		Tactile.IsTactile = true;
	}

	public Boundary boundary;//Boundary is a new data type - capital is the type, lowercase is the reference

	// Update is called once per frame
	void Update () 
	{
		int InterneuronCheck=new int();
		InterneuronCheck=0;
		while (InterneuronCheck<INTERNEURON_COUNT)
		{
			Interneurons[InterneuronCheck]=UpdateNeuron(Interneurons[InterneuronCheck]);
			InterneuronCheck=InterneuronCheck+1;
		}
		//LegForward=UpdateNeuron(LegForward);
		EarLeft=UpdateNeuron(EarLeft);
		EarRight=UpdateNeuron(EarRight);
		LegLeft=UpdateNeuron(LegLeft);
		LegRight=UpdateNeuron(LegRight);
		//Gustatory1=UpdateNeuron(Gustatory1);
		//Gustatory2=UpdateNeuron(Gustatory2);
		//PaceMaker1=UpdateNeuron(PaceMaker1);
		//PaceMaker2=UpdateNeuron(PaceMaker2);
		//Gustatory3=UpdateNeuron(Gustatory3);
		Tactile=UpdateNeuron(Tactile);

		float fTurn = LegLeft.CurrentActivity;
		fTurn = fTurn - LegRight.CurrentActivity;
		Vector3 vTurn = new Vector3 ();
		vTurn.y = fTurn*TURN_SCALE;
		transform.Rotate (vTurn,Space.World);

		float fMove = (LegRight.CurrentActivity+LegLeft.CurrentActivity)/2;
		fMove = 1.0f;//Fixed forward
		Vector3 vMove=new Vector3();
		vMove.z=fMove*MOVEMENT_SCALE;
		transform.Translate (vMove, Space.Self);
		if (transform.position.x < boundary.xMin | transform.position.x > boundary.xMax | transform.position.z < boundary.zMin | transform.position.z > boundary.zMax) {
			Tactile.CurrentActivity = 1.0f;
			transform.position = new Vector3 (
				Mathf.Clamp (transform.position.x, boundary.xMin, boundary.xMax), 
				0.0f, 
				Mathf.Clamp (transform.position.z, boundary.zMin, boundary.zMax)
			);
		}
		else
		{
			Tactile.CurrentActivity = 0.0f;
		}

		InterneuronCheck=0;
		while (InterneuronCheck<INTERNEURON_COUNT)
		{
			RollOverActivities(Interneurons[InterneuronCheck]);
			InterneuronCheck=InterneuronCheck+1;
		}
		EarLeft=RollOverActivities (EarLeft);
		EarRight=RollOverActivities (EarRight);
		LegLeft=RollOverActivities (LegLeft);
		LegRight=RollOverActivities (LegRight);
		//LegForward=RollOverActivities (LegForward);
		//Gustatory1=RollOverActivities (Gustatory1);
		//Gustatory2=RollOverActivities (Gustatory2);
		//PaceMaker1=RollOverActivities (PaceMaker1);
		//PaceMaker2=RollOverActivities (PaceMaker2);
		//Gustatory3=RollOverActivities (Gustatory3);
		Tactile=RollOverActivities(Tactile);
	}
}
