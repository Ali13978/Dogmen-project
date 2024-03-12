/*
 * POLYGON DOG PREFAB SCRIPT
 * DESCRIPTION: This script demonstrates the range of animations and Prefabs included in 
 * Polygon Dog which can be customized for the users preference. Please attach this to the
 * Dog.Prefab asset then customize the keys for the particular animations.
 * 
 * PLEASE NOTE: This script is intended for demonstration purposes and user customization or 
 * third party animation plugins will be required for further animation options
 */
using System.Collections.Generic;
using UnityEngine;
public class POLYGON_DogPrefabs : MonoBehaviour
{
    public bool DisplayPrefabUI = true;
    public GameObject[] Coyote; //0
	public GameObject[] Dalmatian; //1
	public GameObject[] DalmatianCollar; //2
	public GameObject[] Doberman; //3
	public GameObject[] DobermanCollar; //4
	public GameObject[] Fox; //5
	public GameObject[] GermanShepherd; //6
	public GameObject[] GermanShepherdCollar; //7
	public GameObject[] GoldenRetriever;//8
	public GameObject[] GoldenRetrieverCollar; //9
	public GameObject[] Greyhound;//10
	public GameObject[] GreyhoundCollar;//11
	//Hellhound // 12
	public GameObject[] Husky;//13
	public GameObject[] HuskyCollar; //14
	public GameObject[] Labrador;//15
	public GameObject[] LabradorCollar; //16
	public GameObject[] Pointer;//17
	public GameObject[] PointerCollar; // 18
	public GameObject[] Ridgeback; // 19
	public GameObject[] RidgebackCollar; //20
	//Robot //21
	//SciFi //22
	public GameObject[] Shiba; //23
	public GameObject[] ShibaCollar; //24
	public GameObject[] Wolf; //25
	public GameObject[] ZombieDoberman; //26
	public GameObject[] ZombieGermanShepherd; //27
	public List<GameObject[]> AllDogAttach = new List<GameObject[]>(); // All dog attachments
	private Transform[] children; // All Dog Meshes in parent object
	private Transform getDogName; // Dog Parent
	private Transform DogTransform; // Dog Children
	private List<GameObject> NoAttachDogs = new List<GameObject>(); // Dogs without attachments
	static int DogCounter; // dog index no
	static int counter; // current counter for dog attachment
	private GUIStyle guiStyle = new GUIStyle(); // GUI style for overlay
	[Header("Selection Keys")]
	public KeyCode CycleDogUp = KeyCode.UpArrow; // Mouse Left-Click Attack
	public KeyCode CycleDogDown = KeyCode.DownArrow; // Mouse Right-Click Attack
	public KeyCode CycleAttachmentsUp = KeyCode.LeftArrow; // Move forward
	public KeyCode CycleAttachmentsDown = KeyCode.DownArrow; // Move backward
	private void Start()
	{	
		children = this.transform.GetComponentsInChildren<Transform>(true); // Sort out dogs with attachments and those without
		for (int x = 0; x < children.Length; x++)
		{
			Transform child = children[x];
			if (child.name.Contains("Dogs")) // Assign dogs with attachments
			{
				getDogName = child;
			}
			if (child.name.Contains("HellHound") || child.name.Contains("Robot") || child.name.Contains("Scifi")) // Assign dogs with no attachments
			{
				NoAttachDogs.Add(child.gameObject);
			}
		}
		// Add each dog attachment array to list
		AllDogAttach.Add(Coyote); //0
		AllDogAttach.Add(Dalmatian); //1
		AllDogAttach.Add(DalmatianCollar); //2
		AllDogAttach.Add(Doberman); //3
		AllDogAttach.Add(DobermanCollar); //4
		AllDogAttach.Add(Fox); // 5
		AllDogAttach.Add(GermanShepherd); //6
		AllDogAttach.Add(GermanShepherdCollar); // 7
		AllDogAttach.Add(GoldenRetriever); // 8
		AllDogAttach.Add(GoldenRetrieverCollar); // 9
		AllDogAttach.Add(Greyhound); //10
		AllDogAttach.Add(GreyhoundCollar); //11
		AllDogAttach.Add(null); //12
		AllDogAttach.Add(Husky);//13
		AllDogAttach.Add(HuskyCollar); //14
		AllDogAttach.Add(Labrador);//15
		AllDogAttach.Add(LabradorCollar);//16
		AllDogAttach.Add(Pointer);//17
		AllDogAttach.Add(PointerCollar);//18
		AllDogAttach.Add(Ridgeback); //19
		AllDogAttach.Add(RidgebackCollar);//20
		AllDogAttach.Add(null); //21
		AllDogAttach.Add(null); //22
		AllDogAttach.Add(Shiba); //23
		AllDogAttach.Add(ShibaCollar); //24
		AllDogAttach.Add(Wolf); //25
		AllDogAttach.Add(ZombieDoberman); //26
		AllDogAttach.Add(ZombieGermanShepherd);//27
		DogTransform = getDogName; // Set Dog Transform to Dogs
		DogCounter = 6; // Set Dog counter to 0 on startup
		counter = 0; // Set attachment counter to 0 on startup
		setInvisible(1);
		if (CheckValid(DogTransform.GetChild(6).gameObject, AllDogAttach[0][0], 1)) // Check if Dog mesh is valid
		{
			DogTransform.GetChild(6).gameObject.SetActive(true);
		}
		if (CheckValid(DogTransform.GetChild(DogCounter).gameObject, AllDogAttach[0][0], 2)) // Check if Dog attachment is valid
		{
			AllDogAttach[DogCounter][counter].gameObject.SetActive(true);
		}
		guiStyle.fontSize = 18;
		guiStyle.normal.textColor = Color.black;
	}
	void OnGUI()
	{
		if(DisplayPrefabUI)
		{
		GUI.backgroundColor = Color.black;
		GUI.Label(new Rect((Screen.width - 550), 10, 400, 30), "Current Dog: " + DogTransform.GetChild(DogCounter).gameObject.name.ToString(), guiStyle);
		GUI.Label(new Rect((Screen.width - 550), 45, 400, 30), "Cycle Dog Up: " + CycleDogUp.ToString(), guiStyle);
		GUI.Label(new Rect((Screen.width - 550), 70, 400, 30), "Cycle Dog Down: " + CycleDogDown.ToString(), guiStyle);
		GUI.Label(new Rect((Screen.width - 550), 95, 400, 30), "Cycle Attachment Up: " + CycleAttachmentsUp.ToString(), guiStyle);
		GUI.Label(new Rect((Screen.width - 550), 120, 400, 30), "Cycle Attachment Down: " + CycleAttachmentsDown.ToString(), guiStyle);
		}
	}
	void Update()
	{
		if (Input.GetKeyDown(CycleDogUp)) // Cycle through dog descending
		{
			if (DogCounter < DogTransform.childCount - 1)
			{
				DogCounter += 1;
			}
			else
			{
				DogCounter = 0;
			}
			setInvisible(3);
			if (AllDogAttach[DogCounter] != null && AllDogAttach[DogCounter].Length > 0)
			{
				if (counter < AllDogAttach[DogCounter].Length)
				{
					if (CheckValid(DogTransform.GetChild(DogCounter).gameObject, AllDogAttach[DogCounter][counter], 2))
					{
						AllDogAttach[DogCounter][counter].gameObject.SetActive(true);
					}
				}
				else
				{
					counter = 0;
					if (CheckValid(DogTransform.GetChild(DogCounter).gameObject, AllDogAttach[DogCounter][counter], 2))
					{
						AllDogAttach[DogCounter][counter].gameObject.SetActive(true);
					}
				}
			}
			DogTransform.GetChild(DogCounter).gameObject.SetActive(true);
		}
		if (Input.GetKeyDown(CycleDogDown)) // Cycle through dog ascending
		{
			if (DogCounter > 0)
			{
				DogCounter -= 1;
			}
			else
			{
				DogCounter = DogTransform.childCount - 1;
			}
			setInvisible(3);
			if (AllDogAttach[DogCounter] != null && AllDogAttach[DogCounter].Length > 0)
			{
				if (counter < AllDogAttach[DogCounter].Length)
				{
					if (CheckValid(DogTransform.GetChild(DogCounter).gameObject, AllDogAttach[DogCounter][counter], 2))
					{
						AllDogAttach[DogCounter][counter].gameObject.SetActive(true);
					}
				}
				else
				{
					counter = 0;
					if (CheckValid(DogTransform.GetChild(DogCounter).gameObject, AllDogAttach[DogCounter][counter], 2))
					{
						AllDogAttach[DogCounter][counter].gameObject.SetActive(true);
					}
				}
			}
			DogTransform.GetChild(DogCounter).gameObject.SetActive(true);
		}
		if (Input.GetKeyDown(CycleAttachmentsUp)) // Cycle through Dog attachment descending
		{
			setInvisible(2);
			if (AllDogAttach[DogCounter] != null && AllDogAttach[DogCounter].Length > 0)
			{
				if (counter < AllDogAttach[DogCounter].Length - 1)
				{
					counter += 1;
				}
				else
				{
					counter = 0;
				}
				if (counter < AllDogAttach[DogCounter].Length)
				{
					if (CheckValid(DogTransform.GetChild(DogCounter).gameObject, AllDogAttach[DogCounter][counter], 2))
					{
						AllDogAttach[DogCounter][counter].gameObject.SetActive(true);
					}
				}
				else
				{
					counter = 0;
				}
			}
			else
			{
				Debug.Log(DogTransform.GetChild(DogCounter).gameObject.name + " HAS NO ATTACHMENTS!");
			}
		}
		if (Input.GetKeyDown(CycleAttachmentsDown))// Cycle through Dog attachment ascending
		{
			setInvisible(2);
			if (AllDogAttach[DogCounter] != null && AllDogAttach[DogCounter].Length > 0)
			{
				if (counter > 0)
				{
					counter -= 1;
				}
				else
				{
					counter = AllDogAttach[DogCounter].Length - 1;
				}
				if (counter < AllDogAttach[DogCounter].Length)
				{
					if (CheckValid(DogTransform.GetChild(DogCounter).gameObject, AllDogAttach[DogCounter][counter], 2))
					{
						AllDogAttach[DogCounter][counter].gameObject.SetActive(true);
					}
				}
				else
				{
					counter = 0;
				}
			}
			else
			{
				Debug.Log(DogTransform.GetChild(DogCounter).gameObject.name + " HAS NO ATTACHMENTS!");
			}
		}
	}
	private bool CheckValid(GameObject Dog, GameObject DogAttach, int checkMarker) // 1 = Dog, 2 = Attach
	{
		if (checkMarker == 1 && Dog != null) // Check if Dog is valid
		{
			return true;
		}
		else if (checkMarker == 2 && DogAttach != null) // Check if Dog Attachment is valid
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	private void setInvisible(int visibleMarker) // 1 = Dog, 2 = Attach, 3 = Both
	{
		if (visibleMarker == 1 || visibleMarker == 3) // Turn off All Dogs
		{
			for (int y = 0; y < DogTransform.childCount; y++)
			{
				if (DogTransform.GetChild(y).gameObject.activeInHierarchy)
				{
					DogTransform.GetChild(y).gameObject.SetActive(false);
				}
			}
		}
		if (visibleMarker == 2 || visibleMarker == 3) // Turn off All Dog Attachments
		{
			for (int v = 0; v < AllDogAttach.Count; v++)
			{
				if (AllDogAttach[v] != null)
				{
					for (int x = 0; x < AllDogAttach[v].Length; x++)
					{
						if (AllDogAttach[v][x] != null)
						{
							AllDogAttach[v][x].gameObject.SetActive(false);
						}
					}
				}
			}
		}
	}
}
